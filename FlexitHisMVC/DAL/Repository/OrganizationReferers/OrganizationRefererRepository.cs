using Medicloud.DAL.Entities;

namespace Medicloud.DAL.Repository.OrganizationReferers
{
	public class OrganizationRefererRepository:IOrganizationRefererRepository
	{
		public Task<int> AddAsync(OrganizationRefererDAO dao)
		{
			throw new NotImplementedException();
		}

		public Task<OrganizationRefererDAO> GetByOrganizationId(int organizationId)
		{
			throw new NotImplementedException();
		}
	}
}
