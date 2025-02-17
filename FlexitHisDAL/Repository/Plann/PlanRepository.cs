using Medicloud.DAL.DAO;
using Medicloud.DAL.Infrastructure.UnitOfWork;

namespace Medicloud.DAL.Repository.Plann
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
