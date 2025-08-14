using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using PresentationLayer.DTOs;
using DataAccessLayer.Entities;
using System.ComponentModel.DataAnnotations;
using BusinessLogicLayer.Services;

namespace BlogApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        /// <summary>
        /// Belirli bir post'un yorumlarını getirir
        /// </summary>
        [HttpGet("getComments/{postId}")]
        public async Task<ActionResult<IEnumerable<Comment>>> GetCommentsByPostId(int postId)
        {
            try
            {
                if (postId <= 0)
                {
                    return BadRequest("Invalid post ID");
                }

                var comments = await _commentService.GetCommentsByPostIdAsync(postId);
                return Ok(comments);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Yeni yorum ekler - Kimlik doğrulaması gerekli
        /// </summary>
        [HttpPost("addComment")]
        [Authorize] // Identity ile kimlik doğrulaması
        public async Task<ActionResult> AddComment(CommentCreateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Identity'den kullanıcı ID'sini al
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return BadRequest("Invalid user ID");
                }

                // DTO'dan primitive parametrelere mapping
                var comment = await _commentService.AddCommentAsync(
                    dto.PostId,
                    userId,  // Identity'den alınan userId
                    dto.Content);

                return CreatedAtAction(
                    nameof(GetCommentsByPostId),
                    new { postId = dto.PostId },
                    comment);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// ID ile belirli bir yorumu getirir
        /// </summary>
        [HttpGet("{commentId}")]
        public async Task<ActionResult<Comment>> GetCommentById(int commentId)
        {
            try
            {
                if (commentId <= 0)
                {
                    return BadRequest("Invalid comment ID");
                }

                var comment = await _commentService.GetCommentByIdAsync(commentId);
                if (comment == null)
                {
                    return NotFound("Comment not found");
                }

                return Ok(comment);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Yorumu siler - Sadece yorum sahibi veya admin
        /// </summary>
        [HttpDelete("{commentId}")]
        [Authorize]
        public async Task<ActionResult> DeleteComment(int commentId)
        {
            try
            {
                if (commentId <= 0)
                {
                    return BadRequest("Invalid comment ID");
                }

                // Yorum sahibi kontrolü
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var isAdmin = User.IsInRole("Admin");

                var comment = await _commentService.GetCommentByIdAsync(commentId);
                if (comment == null)
                {
                    return NotFound("Comment not found");
                }

                // Sadece yorum sahibi veya admin silebilir
                if (comment.UserId.ToString() != currentUserId && !isAdmin)
                {
                    return Forbid("You can only delete your own comments");
                }

                var result = await _commentService.DeleteCommentAsync(commentId);
                if (!result)
                {
                    return NotFound("Comment not found");
                }

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Kullanıcının kendi yorumlarını getirir
        /// </summary>
        [HttpGet("my-comments")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Comment>>> GetMyComments()
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return BadRequest("Invalid user ID");
                }

                var comments = await _commentService.GetCommentsByUserIdAsync(userId);
                return Ok(comments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

// Güncellenmiş DTO'lar
namespace PresentationLayer.DTOs
{
    /// <summary>
    /// Yorum ekleme için DTO - UserId otomatik alınır
    /// </summary>
    public class CommentCreateDto
    {
        [Required(ErrorMessage = "Post ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Post ID must be greater than 0")]
        public int PostId { get; set; }

        [Required(ErrorMessage = "Content is required")]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = "Content must be between 1 and 1000 characters")]
        public string Content { get; set; } = string.Empty;
    }

    // UpdateCommentDto aynı kalır - sadece Content gerekli
}