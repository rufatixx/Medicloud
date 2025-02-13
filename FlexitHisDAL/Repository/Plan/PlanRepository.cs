using Medicloud.DAL.DAO;
using Medicloud.DAL.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medicloud.DAL.Repository.Plan
{
	public class PlanRepository:IPlanRepository
	{
		private readonly IUnitOfWork _unitOfWork;

		public PlanRepository(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public Task<int> AddAsync(PlanDAO dao)
		{
			throw new NotImplementedException();
		}

		public Task<bool> RemoveAsync()
		{
			throw new NotImplementedException();
		}
	}
}
