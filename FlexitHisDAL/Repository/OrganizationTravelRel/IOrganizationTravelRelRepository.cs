using Medicloud.DAL.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medicloud.DAL.Repository.OrganizationTravelRel
{
	public interface IOrganizationTravelRelRepository
	{
		Task<int> AddAsync(OrganizationTravelDAO dao);
	}
}
