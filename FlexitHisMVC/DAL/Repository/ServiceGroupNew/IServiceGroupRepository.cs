using Medicloud.Models;

namespace Medicloud.DAL.Repository.ServiceGroupNew
{
	public interface IServiceGroupRepository
	{
		Task<List<ServiceGroup>>GetServiceGroupsByOrganizationAndPriceGroup(int organizationId, int priceGroupId);
	}
}
