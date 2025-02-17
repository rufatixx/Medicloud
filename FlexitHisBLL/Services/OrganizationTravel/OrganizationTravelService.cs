using Medicloud.DAL.DAO;
using Medicloud.DAL.Infrastructure.UnitOfWork;
using Medicloud.DAL.Repository.OrganizationTravelRel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medicloud.BLL.Services.OrganizationTravel
{
	public class OrganizationTravelService:IOrganizationTravelService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IOrganizationTravelRelRepository _organizationTravelRelRepository;
		public OrganizationTravelService(IUnitOfWork unitOfWork, IOrganizationTravelRelRepository organizationTravelRelRepository)
		{
			_unitOfWork = unitOfWork;
			_organizationTravelRelRepository = organizationTravelRelRepository;
		}

		public async Task<OrganizationTravelDAO> GetOrganizationTravelByOrganizationId(int orgId)
		{

			using var con= _unitOfWork.BeginConnection();
			var result =await _organizationTravelRelRepository.GetByOrganizationIdAsync(orgId);
			return result;
		}
	}
}
