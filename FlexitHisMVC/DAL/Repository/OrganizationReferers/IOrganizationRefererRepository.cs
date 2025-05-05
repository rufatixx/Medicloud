using Medicloud.DAL.Entities;

namespace Medicloud.DAL.Repository.OrganizationReferers
{
	public interface IOrganizationRefererRepository
	{
		Task<int> AddAsync(OrganizationRefererDAO dao);
		Task<OrganizationRefererDAO> GetByOrganizationId(int organizationId);
	}
}
