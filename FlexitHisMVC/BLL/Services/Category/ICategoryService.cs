using Medicloud.DAL.DAO;

namespace Medicloud.BLL.Services.Category
{
    public interface ICategoryService
    {
		Task<List<CategoryDAO>> GetAll();

	}
}
