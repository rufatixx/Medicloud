using Medicloud.DAL.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medicloud.DAL.Repository.OrganizationServiceRel
{
	public interface IOrganizationServiceRelRepository
	{
		Task<int> AddAsync(TempRelDAO dao);
		Task<bool> RemoveAsync(int organizationId, int serviceId);
	}
}
