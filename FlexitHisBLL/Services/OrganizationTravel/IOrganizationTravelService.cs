using Medicloud.DAL.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medicloud.BLL.Services.OrganizationTravel
{
	public  interface IOrganizationTravelService
	{
		Task<OrganizationTravelDAO> GetOrganizationTravelByOrganizationId(int orgId);
	}
}
