
using Medicloud.BLL.DTO;

namespace Medicloud.BLL.Services.Category
{
    public interface ICategoryService
    {
		Task<List<TempDTO>> GetAll();
		Task<List<TempDTO>> GetByOrganizationId(int organizationId);

	}
}
