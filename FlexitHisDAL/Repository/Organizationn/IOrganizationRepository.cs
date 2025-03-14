using Medicloud.DAL.DAO;

namespace Medicloud.DAL.Repository.Organizationn
{
    public interface IOrganizationRepository
    {
		Task<int> AddAsync(OrganizationDAO dAO);
		Task<OrganizationDAO?> GetByIdAsync(int id);
		Task<bool>UpdateAsync(OrganizationDAO dao);
		Task RegisterOrganization(int id);
		Task UpdateLogoId(int organizationId, int logoId);
		Task UpdateCoverId(int organizationId, int coverId);
		Task<List<OrganizationDAO>> GetUserOrganizations(int userId);
	}
}
