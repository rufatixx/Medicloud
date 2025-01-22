using Medicloud.DAL.DAO;

namespace Medicloud.DAL.Repository.Organizationn
{
    public interface IOrganizationRepository
    {
		Task<int> AddAsync(OrganizationDAO dAO);
    }
}
