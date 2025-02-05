
using Medicloud.DAL.DAO;

namespace Medicloud.BLL.Services.Category
{
    public interface ICategoryService
    {
		Task<List<CategoryDAO>> GetAll();
		Task<List<CategoryDAO>> GetByOrganizationId(int organizationId);

	}
}
