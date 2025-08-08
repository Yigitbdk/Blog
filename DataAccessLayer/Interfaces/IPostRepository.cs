using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Entities;
using DataAccessLayer.Data;
using Microsoft.EntityFrameworkCore;


namespace DataAccessLayer.Interfaces
{
    public interface IPostRepository
    {
        Task<List<Post>> GetAllPostsAsync();
        Task<Post> GetPostByIdAsync(int postId);
        Task<List<Post>> GetPostsByUserIdAsync(int userId);
        Task<List<Post>> GetPostsByCategoryIdAsync(int categoryId);
        Task<Post> CreatePostAsync(Post post);
        Task<bool> DeletePostAsync(int postId);
        Task<bool> PostExistsAsync(int postId);
    }
}
