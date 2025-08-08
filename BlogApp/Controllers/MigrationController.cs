using Microsoft.AspNetCore.Mvc;
using BusinessLogicLayer;

namespace PresentationLayer.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class MigrationController : ControllerBase
    {
        private readonly UserMigrationService _migrationService;
        private readonly ILogger<MigrationController> _logger;

        public MigrationController(UserMigrationService migrationService, ILogger<MigrationController> logger)
        {
            _migrationService = migrationService;
            _logger = logger;
        }


        // Mevcut User'ları Identity'ye migrate eder bir kez çalıştır sonra kaldir
        [HttpPost("migrate-users")]
        public async Task<ActionResult<MigrationResult>> MigrateUsers()
        {
            try
            {
                _logger.LogWarning("User migration process started. This should only be run ONCE!");

                var result = await _migrationService.MigrateUsersAsync();

                if (result.IsSuccess)
                {
                    _logger.LogInformation($"Migration completed successfully. {result.SuccessCount} users migrated.");
                    return Ok(new
                    {
                        Success = true,
                        Message = $"Successfully migrated {result.SuccessCount} users to Identity system",
                        MigratedCount = result.SuccessCount,
                        FailedCount = result.FailureCount,
                        FailedUsers = result.FailedUsers
                    });
                }
                else
                {
                    _logger.LogError($"Migration failed. Success: {result.SuccessCount}, Failed: {result.FailureCount}");
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Migration completed with errors",
                        MigratedCount = result.SuccessCount,
                        FailedCount = result.FailureCount,
                        FailedUsers = result.FailedUsers,
                        GeneralError = result.GeneralError
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Migration process failed with exception");
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Migration failed with critical error",
                    Error = ex.Message
                });
            }
        }

        //Migration durumunu kontrol eder
        [HttpGet("migration-status")]
        public async Task<ActionResult> GetMigrationStatus()
        {
            try
            {
                // Eski Users tablosundaki kullanıcı sayısı
                var legacyUserCount = await _migrationService.GetLegacyUserCountAsync();

                // Identity'deki kullanıcı sayısı  
                var identityUserCount = await _migrationService.GetIdentityUserCountAsync();

                return Ok(new
                {
                    LegacyUsersCount = legacyUserCount,
                    IdentityUsersCount = identityUserCount,
                    MigrationNeeded = legacyUserCount > 0,
                    Status = legacyUserCount > 0 ? "Migration Required" : "Migration Completed"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get migration status");
                return StatusCode(500, "Failed to retrieve migration status");
            }
        }


        //Migration'ı rollback eder
        [HttpPost("rollback-migration")]
        public async Task<ActionResult> RollbackMigration()
        {
            try
            {
                _logger.LogWarning("ROLLBACK: Migration rollback process started!");

                var result = await _migrationService.RollbackMigrationAsync();

                if (result)
                {
                    return Ok(new { Success = true, Message = "Migration successfully rolled back" });
                }
                else
                {
                    return BadRequest(new { Success = false, Message = "Rollback failed" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Rollback process failed");
                return StatusCode(500, new { Success = false, Message = "Rollback failed", Error = ex.Message });
            }
        }
    }
}
