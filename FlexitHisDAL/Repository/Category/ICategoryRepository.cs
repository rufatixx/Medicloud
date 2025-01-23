using Medicloud.DAL.DAO;

namespace Medicloud.DAL.Repository.Category
{
    public interface ICategoryRepository
    {
		Task<List<CategoryDAO>> GetAll();
    }
}
