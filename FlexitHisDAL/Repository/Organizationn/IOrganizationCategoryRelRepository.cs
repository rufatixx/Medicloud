using Medicloud.DAL.DAO;

namespace Medicloud.DAL.Repository.Organizationn
{
    public interface IOrganizationCategoryRelRepository
    {
		Task<int> AddAsync(int organizationId, int categoryId);
		Task<List<TempRelDAO>> GetByOrganizationId(int organizationId);
		Task RemoveAsync(int id);
	}
}
