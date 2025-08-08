using BlogApp.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using DataAccessLayer.Data;
using BusinessLogicLayer;
using DataAccessLayer.Entities;
using PostEntity = DataAccessLayer.Entities.Post;

namespace BlogApp.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        

        [HttpDelete("deletePost/{postId}")]
        public async Task<IActionResult> DeletePost(int postId)
        {
            try
            {
                var result = await _postService.DeletePostAsync(postId);

                if (!result)
                    return NotFound(new { message = "Post bulunamadı." });

                return Ok(new { message = "Post başarıyla silindi." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the post." });
            }
        }

        [HttpGet(Name = "GetPosts")]
        public async Task<IActionResult> GetPosts()
        {
            try
            {
                var posts = await _postService.GetAllPostsAsync();

                var postDtos = posts.Select(p => new PostDto
                {
                    PostId = p.PostId,
                    Title = p.Title,
                    Content = p.Content,
                    UserId = p.UserId,  // modelde User FK alanı Id
                    User = p.User != null ? new UserProfileDto
                    {
                        Username = p.User.UserName,  // Identity’de UserName büyük harf Dikkat!
                        Bio = p.User.Bio,
                        ProfilePicture = p.User.ProfilePicture
                    } : null,
                    Categories = p.Categories?.Select(c => new CategoryDto
                    {
                        CategoryId = c.CategoryId,
                        Name = c.Name
                    }).ToList()
                }).ToList();

                return Ok(postDtos);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while fetching posts." });
            }
        }

        [HttpGet("{postId}")]
        public async Task<IActionResult> GetPost(int postId)
        {
            try
            {
                var post = await _postService.GetPostByIdAsync(postId);

                if (post == null)
                    return NotFound(new { message = "Post bulunamadı." });

                var result = new
                {
                    post.PostId,
                    post.Title,
                    post.Content,
                    UserId = post.UserId,
                    Username = post.User?.UserName,
                    Categories = post.Categories?.Select(c => new
                    {
                        c.CategoryId,
                        c.Name
                    }).ToList()
                };

                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while fetching the post." });
            }
        }

        [HttpGet(Name = "GetUserPosts")]
        public async Task<IActionResult> GetUserPosts([FromQuery] int userId)
        {
            try
            {
                var posts = await _postService.GetPostsByUserIdAsync(userId);

                var postDtos = posts.Select(p => new PostDto
                {
                    PostId = p.PostId,
                    Title = p.Title,
                    Content = p.Content,
                    UserId = p.UserId,
                    Categories = p.Categories?.Select(c => new CategoryDto
                    {
                        CategoryId = c.CategoryId,
                        Name = c.Name
                    }).ToList()
                }).ToList();

                return Ok(postDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching user posts." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> CategoryFilter([FromQuery] int categoryId)
        {
            try
            {
                var posts = await _postService.GetAllPostsAsync();

                // Çoklu kategoride filtreleme:
                var filteredPosts = posts.Where(p => p.Categories != null && p.Categories.Any(c => c.CategoryId == categoryId)).ToList();

                var postDtos = filteredPosts.Select(p => new PostDto
                {
                    PostId = p.PostId,
                    Title = p.Title,
                    Content = p.Content,
                    UserId = p.UserId,
                    Categories = p.Categories?.Select(c => new CategoryDto
                    {
                        CategoryId = c.CategoryId,
                        Name = c.Name
                    }).ToList()
                }).ToList();

                return Ok(postDtos);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while filtering posts by category." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddPost([FromBody] PostDto dto)
        {
            try
            {
                var newPost = new PostEntity
                {
                    Title = dto.Title,
                    Content = dto.Content,
                    UserId = dto.UserId,
                    Categories = dto.Categories?.Select(c => new Category
                    {
                        CategoryId = c.CategoryId,
                        Name = c.Name
                    }).ToList() ?? new List<Category>()
                };

                var createdPost = await _postService.CreatePostAsync(newPost);
                return Ok(new { Message = "Post successfully added", PostId = createdPost.PostId });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while creating the post." });
            }
        }

    }
}
