using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using DataAccessLayer.Data;

namespace DataAccessLayer.Factories
{
    public class BlogIdentityDbContextFactory : IDesignTimeDbContextFactory<BlogIdentityDbContext>
    {
        public BlogIdentityDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BlogIdentityDbContext>();

            // Migration işlemi sırasında kullanılacak connection string
            optionsBuilder.UseSqlServer("Server=localhost;Database=BlogDB;Trusted_Connection=True;TrustServerCertificate=True;");

            return new BlogIdentityDbContext(optionsBuilder.Options);
        }
    }
}
