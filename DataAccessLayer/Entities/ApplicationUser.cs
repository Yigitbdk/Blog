using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Entities
{
    public class ApplicationUser : IdentityUser<int>
    {
        // Identity'nin default alanları:
        // Id, UserName, Email, EmailConfirmed, PasswordHash, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, etc.

        [StringLength(500)]
        public string? ProfilePicture { get; set; }

        [StringLength(1000)]
        public string? Bio { get; set; }

        public DateTime CreateDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;

        // Migration için
        public int? LegacyUserId { get; set; }

        // Navigations
        public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }

    public class ApplicationRole : IdentityRole<int>
    {
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
    }
}