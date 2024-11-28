using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Dto
{
    public class LoginRequestDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
