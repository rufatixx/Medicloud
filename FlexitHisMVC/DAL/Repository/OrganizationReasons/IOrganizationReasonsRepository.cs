using Medicloud.DAL.Entities;

namespace Medicloud.DAL.Repository.OrganizationReasons
{
	public interface IOrganizationReasonsRepository
	{
		Task<int> AddAsync(OrganizationReasonDAO dao);
		Task<List<OrganizationReasonDAO>> GetByOrganizationId(int organizationId);

    }
}
