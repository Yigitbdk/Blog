using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DataAccessLayer.Entities
{
    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public DateTime? UpdateDate { get; set; }

        // Identity User ile ilişki - int tipinde olmalı
        public int UserId { get; set; } // FK - Identity User'ın Id'si (int tipinde)
        public virtual ApplicationUser User { get; set; } = null!;

        // Diğer navigation properties
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<Category>? Categories { get; set; }
    }
}