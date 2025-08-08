using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace BusinessLogicLayer
{
    public interface IUserService
    {
        Task<ApplicationUser> RegisterUserAsync(string username, string email, string password, string profilePicture = null);
        Task<ApplicationUser> LoginUserAsync(string email, string password);
        Task<ApplicationUser> UpdateUserProfileAsync(int userId, string username, string bio, string profilePicture);
        Task<ApplicationUser> GetUserProfileAsync(int userId);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> UserExistsAsync(int userId);
        Task<bool> UserNameExistsAsync(string username);


        
    }
}
