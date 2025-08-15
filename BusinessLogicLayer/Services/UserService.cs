using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces;

namespace BusinessLogicLayer.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(IUserRepository userRepository, IPasswordHasher<ApplicationUser> passwordHasher, UserManager<ApplicationUser> userManager)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _userManager = userManager;
        }

        public async Task<ApplicationUser> RegisterUserAsync(string username, string email, string password, string profilePicture = null)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username is required");

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required");

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password is required");

            if (!IsValidEmail(email))
                throw new ArgumentException("Invalid email format");

            if (password.Length < 6)
                throw new ArgumentException("Password must be at least 6 characters long");

            if (await _userRepository.EmailExistsAsync(email))
                throw new InvalidOperationException("Email already exists");

            if (await _userRepository.UserNameExistsAsync(username))
                throw new InvalidOperationException("Username already exists");



            var user = new ApplicationUser
            {
                UserName = username,
                NormalizedUserName = username.ToUpperInvariant(),
                Email = email,
                NormalizedEmail = email.ToUpperInvariant(),
                ProfilePicture = string.IsNullOrEmpty(profilePicture) ? null : profilePicture,
                IsActive = true,
                CreateDate = DateTime.Now
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, password);

            return await _userRepository.CreateUserAsync(user);
        }


        public async Task<ApplicationUser> LoginUserAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required");

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password is required");

            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid email or password");

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (result != PasswordVerificationResult.Success)
                throw new UnauthorizedAccessException("Invalid email or password");

            return user;
        }

        public async Task<ApplicationUser> UpdateUserProfileAsync(int userId, string username, string bio, string profilePicture)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid user ID");

            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username is required");

            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User not found");

            user.UserName = username;
            user.NormalizedUserName = username.ToUpperInvariant();
            user.Bio = bio;
            user.ProfilePicture = string.IsNullOrEmpty(profilePicture) ? null : profilePicture;

            return await _userRepository.UpdateUserAsync(user);
        }

        public async Task<ApplicationUser> GetUserProfileAsync(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid user ID");

            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User not found");

            return user;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return await _userRepository.EmailExistsAsync(email);
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UserNameExistsAsync(string username)
        {
            return await _userRepository.UserNameExistsAsync(username);
        }

        public async Task<bool> UserExistsAsync(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            return user != null;
        }

        // ADMIN

        public async Task<List<UserWithRolesDto>> GetAllUsersWithRolesAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            var result = new List<UserWithRolesDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                result.Add(new UserWithRolesDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    ProfilePicture = user.ProfilePicture,
                    Bio = user.Bio,
                    IsActive = user.IsActive,
                    CreateDate = user.CreateDate,
                    Roles = roles.ToList()
                });
            }

            return result;
        }


        public async Task<bool> AddAdminRoleAsync(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User not found");

            var result = await _userManager.AddToRoleAsync(user, "Admin");
            return result.Succeeded;
        }


        public async Task<bool> RemoveAdminRoleAsync(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User not found");

            var result = await _userManager.RemoveFromRoleAsync(user, "Admin");
            return result.Succeeded;
        }


        public async Task<bool> IsUserAdminAsync(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                return false;

            return await _userManager.IsInRoleAsync(user, "Admin");
        }

        public async Task<List<string>> GetUserRolesAsync(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User not found");

            var roles = await _userManager.GetRolesAsync(user);
            return roles.ToList();
        }
    }

    public class UserWithRolesDto
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? ProfilePicture { get; set; }
        public string? Bio { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }
}

