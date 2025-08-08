using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Entities;
using DataAccessLayer.Data;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.Interfaces;


namespace DataAccessLayer.Data
{
    public class PostRepository : IPostRepository
    {
        private readonly BlogIdentityDbContext _context;

        public PostRepository(BlogIdentityDbContext context)
        {
            _context = context;
        }

        public async Task<List<Post>> GetAllPostsAsync()
        {
            return await _context.Posts
                .Include(p => p.User)
               // .Include(p => p.Category)
                .Include(p => p.Categories)
                .ToListAsync();
        }

        public async Task<Post> GetPostByIdAsync(int postId)
        {
            return await _context.Posts
                .Include(p => p.User)
            //  .Include(p => p.Category)
                .Include(p => p.Categories)
                .FirstOrDefaultAsync(p => p.PostId == postId);
        }

        public async Task<List<Post>> GetPostsByUserIdAsync(int Id)
        {
            return await _context.Posts
                .Where(p => p.UserId == Id)
                .ToListAsync();
        }

        public async Task<List<Post>> GetPostsByCategoryIdAsync(int categoryId)
        {
            return await _context.Posts
            //  .Where(p => p.CategoryId == categoryId)
                .Where(p => p.Categories.Any(c => c.CategoryId == categoryId))
                .ToListAsync();
        }

        public async Task<Post> CreatePostAsync(Post post)
        {
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task<bool> DeletePostAsync(int postId)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post != null)
            {
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> PostExistsAsync(int postId)
        {
            return await _context.Posts.AnyAsync(p => p.PostId == postId);
        }
    }
}
