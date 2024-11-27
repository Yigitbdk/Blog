namespace BlogApp.Dto
{
    public class UpdateProfileRequestDto
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Bio { get; set; }
        public string ProfilePicture { get; set; }
    }
}
