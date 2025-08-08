using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace BlogApp.Dto
{
    public class LoginRequestDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

}
