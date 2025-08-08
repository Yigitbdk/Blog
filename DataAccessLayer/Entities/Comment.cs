using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DataAccessLayer.Entities
{
    public class Comment
    {
        public int CommentId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreateDate { get; set; } = DateTime.Now;

        // Post ile ilişki
        public int PostId { get; set; }
        public virtual Post Post { get; set; } = null!;

        // Identity User ile ilişki - int tipinde olmalı çünkü ApplicationUser<int> kullanıyorsunuz
        public int UserId { get; set; } // FK - Identity User'ın Id'si (int tipinde)
        public virtual ApplicationUser User { get; set; } = null!;
    }
}