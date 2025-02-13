using Medicloud.DAL.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medicloud.DAL.Repository.OrganizationPlan
{
	public interface IOrganizationPlanRepository
	{
		Task<int> AddAsync(OrganizationPlanDAO dao);
		Task<bool> UpdateAsync(OrganizationPlanDAO dao);
		Task DeleteAsync(int id);
		Task<List<OrganizationPlanDAO>>GetPlansByOrganizationId(int organizationId);
	}
}
