using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using DataAccessLayer.Entities;
using BlogApp.DTOs;

namespace BlogApp.Controllers
{

    // Authentication Controller

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager,
            ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        // Register
        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingUser = await _userManager.FindByEmailAsync(dto.Email);
                if (existingUser != null)
                {
                    return BadRequest("Email already registered");
                }

                var existingUsername = await _userManager.FindByNameAsync(dto.Username);
                if (existingUsername != null)
                {
                    return BadRequest("Username already taken");
                }

                var user = new ApplicationUser
                {
                    UserName = dto.Username,
                    Email = dto.Email,
                    Bio = dto.Bio,
                    CreateDate = DateTime.Now
                };

                var result = await _userManager.CreateAsync(user, dto.Password);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);
                    return BadRequest(new { Errors = errors });
                }

                await _userManager.AddToRoleAsync(user, "User");

                _logger.LogInformation($"New user registered: {dto.Email}");

                return Ok(new
                {
                    Message = "User registered successfully",
                    UserId = user.Id,
                    Username = user.UserName,
                    Email = user.Email
                });
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
                {
                    return BadRequest(ModelState);
                }

                var user = await _userManager.FindByEmailAsync(dto.EmailOrUsername) ??
                          await _userManager.FindByNameAsync(dto.EmailOrUsername);

                if (user == null)
                {
                    return BadRequest("Invalid credentials");
                }

                var result = await _signInManager.PasswordSignInAsync(
                    user.UserName!,
                    dto.Password,
                    dto.RememberMe,
                    lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    _logger.LogInformation($"User logged in: {user.Email}");

                    // Role kontrol
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

                if (result.IsLockedOut)
                {
                    return BadRequest("Account is locked out");
                }

                return BadRequest("Invalid credentials");
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

        // ChangePassword
        [HttpPost("change-password")]
        [Authorize]
        public async Task<ActionResult> ChangePassword(ChangePasswordDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManager.FindByIdAsync(userId!);

                if (user == null)
                {
                    return BadRequest("User not found");
                }

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

        // GetProfile
        [HttpGet("profile")]
        [Authorize]
        public async Task<ActionResult> GetProfile()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManager.FindByIdAsync(userId!);

                if (user == null)
                {
                    return BadRequest("User not found");
                }

                var roles = await _userManager.GetRolesAsync(user);

                return Ok(new
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    Bio = user.Bio,
                    ProfilePicture = user.ProfilePicture,
                    CreateDate = user.CreateDate,
                    Roles = roles
                });
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
                {
                    return BadRequest(ModelState);
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManager.FindByIdAsync(userId!);

                if (user == null)
                {
                    return BadRequest("User not found");
                }

                user.Bio = dto.Bio;
                user.ProfilePicture = dto.ProfilePicture;

                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);
                    return BadRequest(new { Errors = errors });
                }

                _logger.LogInformation($"Profile updated for user: {user.Email}");
                return Ok(new { Message = "Profile updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Profile update failed");
                return StatusCode(500, "Profile update failed");
            }
        }
    }
}