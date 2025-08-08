using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using Microsoft.Extensions.Logging;
using System.Data;

namespace BusinessLogicLayer
{
    //Mevcut User tablosundaki kullanıcıları Identity'ye migrate eden servis
    public class UserMigrationService
    {
        private readonly BlogIdentityDbContext _identityContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<UserMigrationService> _logger;


        public UserMigrationService(
            BlogIdentityDbContext identityContext,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ILogger<UserMigrationService> logger)
        {
            _identityContext = identityContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        //Mevcut kullanıcıları Identity'ye migrate eder
        public async Task<MigrationResult> MigrateUsersAsync()
        {
            var result = new MigrationResult();

            try
            {
                //Mevcut kullanıcıları al
                var existingUsers = await GetExistingUsersAsync();
                _logger.LogInformation($"Found {existingUsers.Count} existing users to migrate");

                //Her kullanıcıyı migrate et
                foreach (var oldUser in existingUsers)
                {
                    try
                    {
                        await MigrateUserAsync(oldUser);
                        result.SuccessCount++;
                        _logger.LogInformation($"Successfully migrated user: {oldUser.Email}");
                    }
                    catch (Exception ex)
                    {
                        result.FailedUsers.Add(new FailedMigration
                        {
                            Email = oldUser.Email,
                            Error = ex.Message
                        });
                        result.FailureCount++;
                        _logger.LogError(ex, $"Failed to migrate user: {oldUser.Email}");
                    }
                }

                //Foreign key ilişkileri 
                await UpdateForeignKeyReferencesAsync();

                result.IsSuccess = result.FailureCount == 0;
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Migration process failed");
                result.IsSuccess = false;
                result.GeneralError = ex.Message;
                return result;
            }
        }


        //Mevcut User tablosundan kullanıcıları getirir
        private async Task<List<LegacyUser>> GetExistingUsersAsync()
        {
            // Mevcut User modelinize göre uyarlanmış query
            var query = @"
                SELECT 
                    UserId,
                    Username,
                    Email,
                    Password,     -- Plain text veya hash (hangi formatta ise)
                    ProfilePicture,
                    Bio,
                    CreateDate
                FROM Users";

            var legacyUsers = new List<LegacyUser>();

            using var connection = _identityContext.Database.GetDbConnection();
            await connection.OpenAsync();
            using var command = connection.CreateCommand();
            command.CommandText = query;

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                legacyUsers.Add(new LegacyUser
                {
                    Id = reader.GetInt32("UserId"),
                    Username = reader.GetString("Username"),
                    Email = reader.GetString("Email"),
                    OldPassword = reader.IsDBNull("Password") ? null : reader.GetString("Password"),
                    ProfilePicture = reader.IsDBNull("ProfilePicture") ? null : reader.GetString("ProfilePicture"),
                    Bio = reader.IsDBNull("Bio") ? null : reader.GetString("Bio"),
                    CreateDate = reader.GetDateTime("CreateDate")
                });
            }

            return legacyUsers;
        }


        //Tek bir kullanıcıyı migrate eder
        private async Task MigrateUserAsync(LegacyUser oldUser)
        {
            // Identity user'da zaten var mı kontrol et
            var existingUser = await _userManager.FindByEmailAsync(oldUser.Email);
            if (existingUser != null)
            {
                _logger.LogWarning($"User already exists in Identity: {oldUser.Email}");
                return;
            }

            // Yeni Identity user oluştur
            var newUser = new ApplicationUser
            {
                UserName = oldUser.Username,
                Email = oldUser.Email,
                ProfilePicture = oldUser.ProfilePicture,
                Bio = oldUser.Bio,
                CreateDate = oldUser.CreateDate,
                EmailConfirmed = true,
                LegacyUserId = oldUser.Id
            };

            IdentityResult result;

            if (!string.IsNullOrEmpty(oldUser.OldPassword))
            {
                // Mevcut şifreleri nasıl saklamışsınız?
                if (IsPlainTextPassword(oldUser.OldPassword))
                {
                    result = await _userManager.CreateAsync(newUser, oldUser.OldPassword);
                }
                else
                {
                    result = await _userManager.CreateAsync(newUser, "TempResetPassword123!");
                    if (result.Succeeded)
                    {
                        // Şifre resetleme token'ı oluştur
                        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(newUser);
                        // Email gönderme servisi burada çağrılabilir
                        _logger.LogInformation($"Password reset token generated for {oldUser.Email}");
                    }
                }
            }
            else
            {
                // Şifresiz kullanıcı - geçici şifre ver
                result = await _userManager.CreateAsync(newUser, "TempPassword123!");
            }

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to create user: {errors}");
            }

            // Default role ata
            await _userManager.AddToRoleAsync(newUser, "User");

            _logger.LogInformation($"Successfully created Identity user for: {oldUser.Email}");
        }

        /// <summary>
        /// Şifrenin plain text olup olmadığını kontrol eder
        /// </summary>
        private bool IsPlainTextPassword(string password)
        {
            // Basit kontrol - hash'ler genelde uzun olur
            // Bu kontrolü kendi şifre formatınıza göre uyarlayın
            return password.Length < 50 && !password.Contains("$") && !password.StartsWith("{");
        }

        /// <summary>
        /// Mevcut şifre hash'ini Identity formatına çevirir
        /// </summary>
        private async Task<IdentityResult> MigrateUserWithPasswordAsync(ApplicationUser user, string oldPasswordHash)
        {
            // Bu kısım mevcut şifre hashleme yönteminize bağlı
            // Örnek: MD5, SHA256, BCrypt vs.

            // Seçenek 1: Şifreyi geçici bir şifre ile oluştur, sonra hash'i manuel güncelle
            var result = await _userManager.CreateAsync(user, "TempPassword123!");

            if (result.Succeeded)
            {
                // Manuel olarak eski hash'i koru (güvenlik riski olabilir)
                // Bu yaklaşım önerilmez, kullanıcılara şifre sıfırlama linki gönder

                // Alternatif: Kullanıcıya şifre sıfırlama maili gönder
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                // Email sending service burada çağrılır
            }

            return result;
        }

        /// <summary>
        /// Foreign key referanslarını günceller
        /// </summary>
        private async Task UpdateForeignKeyReferencesAsync()
        {
            _logger.LogInformation("Starting foreign key references update...");

            // 1. Posts tablosundaki UserId'leri güncelle
            var updatePostsQuery = @"
                UPDATE Posts 
                SET UserId = au.Id 
                FROM Posts p
                INNER JOIN AspNetUsers au ON p.UserId = au.LegacyUserId
                WHERE au.LegacyUserId IS NOT NULL";

            await _identityContext.Database.ExecuteSqlRawAsync(updatePostsQuery);
            _logger.LogInformation("Posts table UserId references updated");

            // 2. Comments tablosundaki UserId'leri güncelle
            var updateCommentsQuery = @"
                UPDATE Comments 
                SET UserId = au.Id 
                FROM Comments c
                INNER JOIN AspNetUsers au ON c.UserId = au.LegacyUserId
                WHERE au.LegacyUserId IS NOT NULL";

            await _identityContext.Database.ExecuteSqlRawAsync(updateCommentsQuery);
            _logger.LogInformation("Comments table UserId references updated");

            // 3. Migration tamamlandıktan sonra LegacyUserId alanını temizle (opsiyonel)
            var clearLegacyIdsQuery = @"UPDATE AspNetUsers SET LegacyUserId = NULL";
            await _identityContext.Database.ExecuteSqlRawAsync(clearLegacyIdsQuery);
            _logger.LogInformation("Legacy UserId references cleared");

            // 4. Eski User tablosunu yedek olarak sakla ve rename et
            var renameOldTableQuery = @"
                IF OBJECT_ID('Users_Backup', 'U') IS NULL
                    EXEC sp_rename 'Users', 'Users_Backup'";

            await _identityContext.Database.ExecuteSqlRawAsync(renameOldTableQuery);
            _logger.LogInformation("Old Users table renamed to Users_Backup");
        }

        public async Task<int> GetLegacyUserCountAsync()
        {
            try
            {
                var query = "SELECT COUNT(*) FROM Users";
                using var connection = _identityContext.Database.GetDbConnection();
                await connection.OpenAsync();
                using var command = connection.CreateCommand();
                command.CommandText = query;

                var result = await command.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
            catch
            {
                // Users tablosu yoksa (zaten migrate edilmiş)
                return 0;
            }
        }

        public async Task<int> GetIdentityUserCountAsync()
        {
            return await _userManager.Users.CountAsync();
        }

        public async Task<bool> RollbackMigrationAsync()
        {
            try
            {
                _logger.LogWarning("ROLLBACK: Starting migration rollback process");

                // 1. Identity kullanıcılarını sil (sadece migrate edilmiş kullanıcılar)
                var identityUsers = await _userManager.Users.ToListAsync();
                foreach (var user in identityUsers)
                {
                    if (user.LegacyUserId.HasValue) // Sadece migrate edilen kullanıcılar
                    {
                        await _userManager.DeleteAsync(user);
                    }
                }

                // 2. Eski Users tablosunu geri getir
                var restoreTableQuery = @"
            IF OBJECT_ID('Users', 'U') IS NULL AND OBJECT_ID('Users_Backup', 'U') IS NOT NULL
                EXEC sp_rename 'Users_Backup', 'Users'";

                await _identityContext.Database.ExecuteSqlRawAsync(restoreTableQuery);

                _logger.LogWarning("ROLLBACK: Migration rollback completed");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ROLLBACK: Migration rollback failed");
                return false;
            }
        }


    }

    public class LegacyUser
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? OldPassword { get; set; }
        public string? ProfilePicture { get; set; }
        public string? Bio { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public class MigrationResult
    {
        public bool IsSuccess { get; set; }
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public List<FailedMigration> FailedUsers { get; set; } = new();
        public string? GeneralError { get; set; }
    }

    public class FailedMigration
    {
        public string Email { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
    }
       
}