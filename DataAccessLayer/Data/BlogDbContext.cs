using Microsoft.EntityFrameworkCore;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace DataAccessLayer.Data
{
    public class BlogIdentityDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public BlogIdentityDbContext(DbContextOptions<BlogIdentityDbContext> options)
            : base(options)
        {
        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Identity konfigürasyonu
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>().ToTable("Users");
            builder.Entity<ApplicationRole>().ToTable("Roles");
            builder.Entity<IdentityUserRole<int>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
            builder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");
            builder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");

            ConfigureBlogEntities(builder);
            ConfigureIdentityRelationships(builder);
            SeedIdentityData(builder);
        }

        private void ConfigureBlogEntities(ModelBuilder builder)
        {
            // Post konfigürasyonu
            builder.Entity<Post>(entity =>
            {
                entity.HasKey(p => p.PostId);
                entity.Property(p => p.Title).IsRequired().HasMaxLength(200);
                entity.Property(p => p.Content).IsRequired();

                entity.HasOne(p => p.User)
                      .WithMany(u => u.Posts)
                      .HasForeignKey(p => p.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(p => p.Categories)
                      .WithMany(c => c.Posts)
                      .UsingEntity(j => j.ToTable("PostCategories"));
            });

            // Comment konfigürasyonu
            builder.Entity<Comment>(entity =>
            {
                entity.HasKey(c => c.CommentId);
                entity.Property(c => c.Content).IsRequired().HasMaxLength(1000);

                entity.HasOne(c => c.User)
                      .WithMany(u => u.Comments)
                      .HasForeignKey(c => c.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
                
                entity.HasOne(c => c.Post)
                      .WithMany(p => p.Comments)
                      .HasForeignKey(c => c.PostId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Category konfigürasyonu
            builder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.CategoryId);
            });
        }

        private void ConfigureIdentityRelationships(ModelBuilder builder)
        {
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.UserName).IsUnique();
                entity.Property(u => u.ProfilePicture).HasMaxLength(500);
                entity.Property(u => u.Bio).HasMaxLength(1000);
            });
        }

        private void SeedIdentityData(ModelBuilder builder)
        {
            builder.Entity<ApplicationRole>().HasData(
                new ApplicationRole
                {
                    Id = 1,
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    Description = "System Administrator",
                    CreatedDate = DateTime.Now
                },
                new ApplicationRole
                {
                    Id = 2,
                    Name = "User",
                    NormalizedName = "USER",
                    Description = "Regular User",
                    CreatedDate = DateTime.Now
                }
            );

            // Default admin user
            var hasher = new PasswordHasher<ApplicationUser>();
            builder.Entity<ApplicationUser>().HasData(
                new ApplicationUser
                {
                    Id = 1,
                    UserName = "admin@blog.com",
                    NormalizedUserName = "ADMIN@BLOG.COM",
                    Email = "admin@blog.com",
                    NormalizedEmail = "ADMIN@BLOG.COM",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, "Admin123!"),
                    IsActive = true,
                    CreateDate = DateTime.Now,
                    SecurityStamp = Guid.NewGuid().ToString()
                }
            );

            // Admin'i atama
            builder.Entity<IdentityUserRole<int>>().HasData(
                new IdentityUserRole<int> { UserId = 1, RoleId = 1 }
            );
        }
    }
}