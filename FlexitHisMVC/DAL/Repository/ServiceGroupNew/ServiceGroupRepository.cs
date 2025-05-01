using Dapper;
using Medicloud.DAL.Infrastructure.Abstract;
using Medicloud.Models;

namespace Medicloud.DAL.Repository.ServiceGroupNew
{
	public class ServiceGroupRepository:IServiceGroupRepository
	{
		private readonly IUnitOfWork _unitOfWork;

		public ServiceGroupRepository(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<List<ServiceGroup>> GetServiceGroupsByOrganizationAndPriceGroup(int organizationId, int priceGroupId)
		{
			var sql = @"SELECT DISTINCT sg.*
FROM service_group sg
JOIN services s ON sg.id = s.serviceGroupID
JOIN service_pricegroup spg ON s.id = spg.serviceID
WHERE spg.priceGroupID = @PriceGroupId
  AND sg.organizationID = @OrganizationId
UNION
SELECT DISTINCT parent.*
FROM service_group sg
JOIN services s ON sg.id = s.serviceGroupID
JOIN service_pricegroup spg ON s.id = spg.serviceID
JOIN service_group parent ON sg.parent = parent.id
WHERE spg.priceGroupID = @PriceGroupId
  AND sg.organizationID = @OrganizationId;";
			var con= _unitOfWork.BeginConnection();
			var result=await con.QueryAsync<ServiceGroup>(sql, new { OrganizationId =organizationId, PriceGroupId =priceGroupId});
			return result.ToList();
		}
	}
}
