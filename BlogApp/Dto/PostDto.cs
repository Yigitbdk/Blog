namespace BlogApp.Dto
{
    public class PostDto
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int UserId { get; set; }
        public UserProfileDto? User { get; set; }
        public int? CategoryId { get; set; }
        public CategoryDto? Category { get; set; }
        public List<CategoryDto>? Categories { get; set; }
    }
}

