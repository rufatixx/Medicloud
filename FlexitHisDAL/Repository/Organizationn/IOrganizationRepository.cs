using Medicloud.DAL.DAO;

namespace Medicloud.DAL.Repository.Organizationn
{
    public interface IOrganizationRepository
    {
		Task<int> AddAsync(OrganizationDAO dAO);
		Task<OrganizationDAO?> GetByIdAsync(int id);
		Task<bool>UpdateAsync(OrganizationDAO dao);
		Task RegisterOrganization(int id);
	}
}
