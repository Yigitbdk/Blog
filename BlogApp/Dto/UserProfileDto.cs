using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Dto
{
    public class UserProfileDto
    {
        public string Username { get; set; }
        public string? Bio { get; set; }
        public string? ProfilePicture { get; set; }
    }

}
