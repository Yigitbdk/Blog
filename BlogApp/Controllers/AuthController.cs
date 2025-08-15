using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using DataAccessLayer.Entities;
using BusinessLogicLayer.Services;
using BlogApp.Dto;

namespace BlogApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IUserService userService,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<AuthController> logger)
        {
            _userService = userService;
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        // Register
        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _userService.RegisterUserAsync(
                    dto.Username, 
                    dto.Email, 
                    dto.Password,null);

                _logger.LogInformation($"New user registered: {dto.Email}");

                return Ok(new
                {
                    Message = "User registered successfully",
                    UserId = user.Id,
                    Username = user.UserName,
                    Email = user.Email
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed for {Email}", dto.Email);
                return StatusCode(500, "Registration failed");
            }
        }

        // Login
        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _userService.LoginUserAsync(dto.EmailOrUsername, dto.Password);

                var result = await _signInManager.PasswordSignInAsync(
                    user.UserName!,
                    dto.Password,
                    dto.RememberMe,
                    lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    _logger.LogInformation($"User logged in: {user.Email}");

                    var roles = await _userManager.GetRolesAsync(user);

                    return Ok(new
                    {
                        Message = "Login successful",
                        User = new
                        {
                            Id = user.Id,
                            Username = user.UserName,
                            Email = user.Email,
                            Bio = user.Bio,
                            ProfilePicture = user.ProfilePicture,
                            Roles = roles
                        }
                    });
                }

                return BadRequest("Login failed");
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed for {EmailOrUsername}", dto.EmailOrUsername);
                return StatusCode(500, "Login failed");
            }
        }

        // Logout
        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();
                _logger.LogInformation("User logged out");
                return Ok(new { Message = "Logged out successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logout failed");
                return StatusCode(500, "Logout failed");
            }
        }

        // GetProfile
        [HttpGet("profile")]
        [Authorize]
        public async Task<ActionResult> GetProfile()
        {
            try
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                
                if (!int.TryParse(userIdString, out int userId))
                    return BadRequest("Invalid user ID");

                var user = await _userService.GetUserProfileAsync(userId);
                var roles = await _userManager.GetRolesAsync(user);

                var profileDto = new ProfileDto
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    Bio = user.Bio,
                    ProfilePicture = user.ProfilePicture,
                    CreateDate = user.CreateDate,
                    Roles = roles
                };

                return Ok(profileDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get profile failed");
                return StatusCode(500, "Failed to get profile");
            }
        }

        // UpdateProfile
        [HttpPut("profile")]
        [Authorize]
        public async Task<ActionResult> UpdateProfile(UpdateProfileDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                
                if (!int.TryParse(userIdString, out int userId))
                    return BadRequest("Invalid user ID");


                var currentUser = await _userService.GetUserProfileAsync(userId);

                await _userService.UpdateUserProfileAsync(
                    userId, 
                    currentUser.UserName,
                    dto.Bio, 
                    dto.ProfilePicture);

                _logger.LogInformation($"Profile updated for user ID: {userId}");
                return Ok(new { Message = "Profile updated successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Profile update failed");
                return StatusCode(500, "Profile update failed");
            }
        }

        // ChangePassword 
        [HttpPost("change-password")]
        [Authorize]
        public async Task<ActionResult> ChangePassword(ChangePasswordDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManager.FindByIdAsync(userId!);

                if (user == null)
                    return BadRequest("User not found");

                var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);
                    return BadRequest(new { Errors = errors });
                }

                _logger.LogInformation($"Password changed for user: {user.Email}");
                return Ok(new { Message = "Password changed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Password change failed");
                return StatusCode(500, "Password change failed");
            }
        }

        [HttpGet("user")]
        [Authorize]
        public async Task<ActionResult> GetCurrentUser()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Unauthorized();

                var roles = await _userService.GetUserRolesAsync(user.Id);

                return Ok(new
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    Bio = user.Bio,
                    ProfilePicture = user.ProfilePicture,
                    Roles = roles
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get current user failed");
                return StatusCode(500, "Failed to get user information");
            }
        }


        [HttpGet("admin/users")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersWithRolesAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get all users failed");
                return StatusCode(500, "Failed to get users");
            }
        }

        // Kullanıcıya rol atama
        [HttpPut("admin/users/{id}/roles")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateUserRole(int id, [FromBody] UpdateRoleRequest request)
        {
            try
            {
                if (!await _userService.UserExistsAsync(id))
                    return NotFound("User not found");

                bool success = false;
                string message = "";

                if (request.Action == "add" && request.Role == "Admin")
                {
                    var isAlreadyAdmin = await _userService.IsUserAdminAsync(id);
                    if (isAlreadyAdmin)
                        return BadRequest("User is already an admin");

                    success = await _userService.AddAdminRoleAsync(id);
                    message = success ? "Admin role added successfully" : "Failed to add admin role";
                }
                else if (request.Action == "remove" && request.Role == "Admin")
                {
                    var isAdmin = await _userService.IsUserAdminAsync(id);
                    if (!isAdmin)
                        return BadRequest("User is not an admin");

                    success = await _userService.RemoveAdminRoleAsync(id);
                    message = success ? "Admin role removed successfully" : "Failed to remove admin role";
                }
                else
                {
                    return BadRequest("Invalid role operation");
                }

                if (success)
                {
                    _logger.LogInformation($"Admin role {request.Action} operation successful for user {id}");
                    return Ok(new { Message = message });
                }
                else
                {
                    return BadRequest(message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update user role failed for user {UserId}", id);
                return StatusCode(500, "Failed to update user role");
            }
        }
    }

    // DTO sınıfı
    public class UpdateRoleRequest
    {
        public string Action { get; set; } = string.Empty; // "add" veya "remove"
        public string Role { get; set; } = string.Empty;   // "Admin"
    }
}
