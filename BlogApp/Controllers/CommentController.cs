using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogApp.Dto;
using BlogApp.Models;
using BlogApp.Data;

namespace BlogApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly BlogDbContext _context;

        public CommentController(BlogDbContext context)
        {
            _context = context;
        }

        // Yorumları alma
        [HttpGet("getComments/{postId}")]
        public async Task<ActionResult<IEnumerable<Comment>>> GetCommentsByPostId(int postId)
        {
            var comments = await _context.Comments
                                          .Where(c => c.PostId == postId)
                                          .Include(c => c.User) 
                                          .Include(c => c.Post) 
                                          .OrderByDescending(c => c.CreateDate)  
                                          .ToListAsync();

            if (comments == null || comments.Count == 0)
            {
                return NotFound("No comments found for this post.");
            }

            return Ok(comments);
        }

        // Yorum ekleme
        [HttpPost("addComment")]
        public async Task<ActionResult> AddComment(CommentDto dto)
        {
   
            var comment = new Comment
            {
                PostId = dto.PostId,
                UserId = dto.UserId,
                Content = dto.Content,
                CreateDate = DateTime.Now
            };


            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCommentsByPostId), new { postId = dto.PostId }, comment);
        }
    }
}
