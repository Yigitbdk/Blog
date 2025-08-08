using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Dto
{
    public class AddUserRequestDto
    {

        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? ProfilePicture { get; set; }
        public string? Bio { get; set; }
    }
}
