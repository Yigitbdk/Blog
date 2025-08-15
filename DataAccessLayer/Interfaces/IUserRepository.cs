using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;


namespace DataAccessLayer.Interfaces
{
    public interface IUserRepository
    {
        Task<ApplicationUser> GetUserByIdAsync(int userId);
        Task<ApplicationUser> GetUserByEmailAsync(string email);
        Task<ApplicationUser> CreateUserAsync(ApplicationUser user);
        Task<ApplicationUser> UpdateUserAsync(ApplicationUser user);
        //
        Task<List<ApplicationUser>> GetAllUsersAsync();
        Task<bool> UserExistsAsync(int userId);
        Task<bool> UserNameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);
    }
}
