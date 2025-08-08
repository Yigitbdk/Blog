using System;
public class CommentResponseDto
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public int UserId { get; set; }
    public string Content { get; set; }
    public DateTime CreateDate { get; set; }
    public string UserName { get; set; }
}