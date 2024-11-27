namespace BlogApp.Dto
{
    public class CommentDto
    {
        public int PostId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; }
    }

}
