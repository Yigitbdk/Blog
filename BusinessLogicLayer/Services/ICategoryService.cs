using DataAccessLayer.Entities;

namespace BusinessLogicLayer.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
    }
}
