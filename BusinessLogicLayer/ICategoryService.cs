using DataAccessLayer.Entities;

namespace BusinessLogicLayer
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
    }
}
