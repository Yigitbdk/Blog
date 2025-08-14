using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccessLayer.Entities;

namespace BusinessLogicLayer.Services
{
    public interface ICommentService
    {
        Task<IEnumerable<Comment>> GetCommentsByPostIdAsync(int postId);
        Task<Comment> AddCommentAsync(int postId, int userId, string content);
        Task<Comment?> GetCommentByIdAsync(int commentId);
        Task<bool> UpdateCommentAsync(int commentId, string content);
        Task<bool> DeleteCommentAsync(int commentId);

        // Identity için eklenen method
        Task<IEnumerable<Comment>> GetCommentsByUserIdAsync(int userId);
    }
}