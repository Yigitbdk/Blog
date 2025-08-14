namespace BlogApp.Dto
{
    public class ProfileDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string? Bio { get; set; }
        public string? ProfilePicture { get; set; }
        public DateTime CreateDate { get; set; }
        public IList<string> Roles { get; set; }
    }

}
