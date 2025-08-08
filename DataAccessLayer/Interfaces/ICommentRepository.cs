
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.Data;
using DataAccessLayer.Entities;

namespace DataAccessLayer.Interfaces
{
    public class CommentRepository : ICommentRepository
    {
        private readonly BlogIdentityDbContext _context;

        public CommentRepository(BlogIdentityDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Comment>> GetCommentsByPostIdAsync(int postId)
        {
            return await _context.Comments
                                .Where(c => c.PostId == postId)
                                .Include(c => c.User)
                                .Include(c => c.Post)
                                .OrderByDescending(c => c.CreateDate)
                                .ToListAsync();
        }

        public async Task<Comment> AddCommentAsync(Comment comment)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<Comment?> GetCommentByIdAsync(int commentId)
        {
            return await _context.Comments
                                 .Include(c => c.User)
                                 .Include(c => c.Post)
                                 .FirstOrDefaultAsync(c => c.CommentId == commentId); // ← BURASI DÜZELDİ
        }


        public async Task<bool> UpdateCommentAsync(Comment comment)
        {
            try
            {
                _context.Comments.Update(comment);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteCommentAsync(int commentId)
        {
            try
            {
                var comment = await _context.Comments.FindAsync(commentId);
                if (comment == null) return false;

                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<Comment>> GetCommentsByUserIdAsync(int userId)
        {
            return await _context.Comments
                                 .Where(c => c.UserId == userId)
                                 .Include(c => c.User)
                                 .Include(c => c.Post)
                                 .OrderByDescending(c => c.CreateDate)
                                 .ToListAsync();
        }

    }
}