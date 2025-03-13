using Medicloud.DAL.DAO;

namespace Medicloud.DAL.Repository.Category
{
    public interface ICategoryRepository
    {
		Task<List<CategoryDAO>> GetAll();
		Task<List<CategoryDAO>> GetByOrganizationId(int organizationId);
		Task<CategoryDAO> GetById(int id);
    }
}
