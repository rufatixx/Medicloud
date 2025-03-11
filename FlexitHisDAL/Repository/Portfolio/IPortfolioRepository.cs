using Medicloud.DAL.DAO;

namespace Medicloud.DAL.Repository.Portfolio
{
	public interface IPortfolioRepository
	{
		Task<int> AddAsync(PortfolioDAO dao);
		Task<List<PortfolioDAO>> GetByOrganizationIdAsync(int organizationId);
	}
}
