using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccessLayer.Interfaces;
using BusinessLogicLayer;


namespace BusinessLogicLayer
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;

        public CommentService(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }


        public async Task<IEnumerable<Comment>> GetCommentsByPostIdAsync(int postId)
        {
            if (postId <= 0)
                throw new ArgumentException("Invalid PostId", nameof(postId));

            return await _commentRepository.GetCommentsByPostIdAsync(postId);
        }

        public async Task<Comment> AddCommentAsync(int postId, int userId, string content)
        {
            // Validation
            ValidateCommentInput(postId, userId, content);

            var comment = new Comment
            {
                PostId = postId,
                UserId = userId,
                Content = content,
                CreateDate = DateTime.Now
            };

            return await _commentRepository.AddCommentAsync(comment);
        }

        public async Task<Comment?> GetCommentByIdAsync(int commentId)
        {
            if (commentId <= 0)
                throw new ArgumentException("Invalid CommentId", nameof(commentId));

            return await _commentRepository.GetCommentByIdAsync(commentId);
        }

        public async Task<bool> UpdateCommentAsync(int commentId, string content)
        {
            if (commentId <= 0)
                throw new ArgumentException("Invalid CommentId", nameof(commentId));

            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Content cannot be empty", nameof(content));

            if (content.Length > 1000)
                throw new ArgumentException("Content too long (max 1000 characters)", nameof(content));

            var existingComment = await _commentRepository.GetCommentByIdAsync(commentId);
            if (existingComment == null) return false;

            existingComment.Content = content;
            return await _commentRepository.UpdateCommentAsync(existingComment);
        }

        public async Task<bool> DeleteCommentAsync(int commentId)
        {
            if (commentId <= 0)
                throw new ArgumentException("Invalid CommentId", nameof(commentId));

            return await _commentRepository.DeleteCommentAsync(commentId);
        }

        private void ValidateCommentInput(int postId, int userId, string content)
        {
            if (postId <= 0)
                throw new ArgumentException("Invalid PostId", nameof(postId));

            if (userId <= 0)
                throw new ArgumentException("Invalid UserId", nameof(userId));

            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Content cannot be empty", nameof(content));

            if (content.Length > 1000)
                throw new ArgumentException("Content too long (max 1000 characters)", nameof(content));
        }

        public async Task<IEnumerable<Comment>> GetCommentsByUserIdAsync(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid UserId", nameof(userId));

            return await _commentRepository.GetCommentsByUserIdAsync(userId);
        }

    }
}
