using Microsoft.Extensions.Hosting;

namespace BlogApp.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }

        // Navigation property
        public ICollection<Post> Posts { get; set; }
    }
}
