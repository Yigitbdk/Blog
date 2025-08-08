using BusinessLogicLayer;
using DataAccessLayer.Entities;
using DataAccessLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Data;
using Microsoft.EntityFrameworkCore;


namespace BusinessLogicLayer
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly IUserRepository _userRepository;

        public PostService(IPostRepository postRepository, IUserRepository userRepository)
        {
            _postRepository = postRepository;
            _userRepository = userRepository;
        }

        public async Task<List<Post>> GetAllPostsAsync()
        {
            return await _postRepository.GetAllPostsAsync();
        }

        public async Task<Post> GetPostByIdAsync(int postId)
        {
            if (postId <= 0)
                throw new ArgumentException("Post ID must be greater than 0");

            var post = await _postRepository.GetPostByIdAsync(postId);
            if (post == null)
                throw new InvalidOperationException("Post not found");

            return post;
        }

        public async Task<List<Post>> GetPostsByUserIdAsync(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("User ID must be greater than 0");

            // User'ın var olup olmadığını kontrol et
            if (!await _userRepository.UserExistsAsync(userId))
                throw new InvalidOperationException("User not found");

            return await _postRepository.GetPostsByUserIdAsync(userId);
        }

        public async Task<List<Post>> GetPostsByCategoryIdAsync(int categoryId)
        {
            return await _postRepository.GetPostsByCategoryIdAsync(categoryId);
        }

        public async Task<Post> CreatePostAsync(Post post)
        {
            // Business validations
            if (post == null)
                throw new ArgumentNullException(nameof(post));

            if (string.IsNullOrWhiteSpace(post.Title))
                throw new ArgumentException("Title is required");

            if (string.IsNullOrWhiteSpace(post.Content))
                throw new ArgumentException("Content is required");

            if (post.UserId <= 0)
                throw new ArgumentException("Valid User ID is required");

            // User'ın var olup olmadığını kontrol et
            if (!await _userRepository.UserExistsAsync(post.UserId))
                throw new InvalidOperationException("User not found");

            return await _postRepository.CreatePostAsync(post);
        }

        public async Task<bool> DeletePostAsync(int postId)
        {
            if (postId <= 0)
                throw new ArgumentException("Post ID must be greater than 0");

            if (!await _postRepository.PostExistsAsync(postId))
                return false;

            return await _postRepository.DeletePostAsync(postId);
        }

        public async Task<bool> PostExistsAsync(int postId)
        {
            if (postId <= 0)
                return false;

            return await _postRepository.PostExistsAsync(postId);
        }
    }
}