using Microsoft.EntityFrameworkCore;
using BlogApp.Models;

namespace BlogApp.Data
{
    public class BlogDbContext : DbContext
    {
        public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options) { }

        public DbSet<Comment> Comments { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
