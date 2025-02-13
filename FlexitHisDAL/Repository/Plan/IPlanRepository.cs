using Medicloud.DAL.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medicloud.DAL.Repository.Plan
{
	public interface IPlanRepository
	{
		Task<int>AddAsync(PlanDAO dao);
		Task<bool>RemoveAsync();
	}
}
