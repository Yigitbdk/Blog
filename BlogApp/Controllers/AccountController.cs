
using BlogApp.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using BusinessLogicLayer;
using DataAccessLayer.Data;
namespace BlogApp.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost(Name = "AddUser")]
        public async Task<IActionResult> AddUser(AddUserRequestDto dto)
        {
            try
            {
                var user = await _userService.RegisterUserAsync(
                    dto.Username,
                    dto.Email,
                    dto.Password,
                    dto.ProfilePicture
                );

                return Ok("Added Successfully");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

       /* 
        [HttpPost(Name = "LoginUser")]
        public async Task<IActionResult> LoginUser([FromBody] LoginRequestDto dto)
        {
            try
            {
                var user = await _userService.LoginUserAsync(dto.Email, dto.Password);

                return Ok(new LoginResponseDto
                {
                    UserId = user.Id,
                    Username = user.UserName
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("LOGIN ERROR: " + ex.Message);
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }
       */

        [HttpPost(Name = "UpdateUserProfile")]
        public async Task<IActionResult> UpdateUserProfile(UpdateProfileRequestDto dto)
        {
            try
            {
                await _userService.UpdateUserProfileAsync(
                    dto.UserId,
                    dto.Username,
                    dto.Bio,
                    dto.ProfilePicture
                );

                return Ok(new { message = "Profile updated successfully" });
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
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        [HttpGet(Name = "GetUserProfile")]
        public async Task<IActionResult> GetUserProfile(int userId)
        {
            try
            {
                var user = await _userService.GetUserProfileAsync(userId);

                var result = new UserProfileDto
                {
                    Username = user.UserName,
                    Bio=user.Bio,
                    ProfilePicture=user.ProfilePicture,
                };

                return Ok(result);
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
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }
    }
}
