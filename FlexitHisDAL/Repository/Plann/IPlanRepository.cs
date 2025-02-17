using Medicloud.DAL.DAO;

namespace Medicloud.DAL.Repository.Plann
{
	public interface IPlanRepository
	{
		Task<int>AddAsync(PlanDAO dao);
		Task<bool>RemoveAsync();
	}
}
