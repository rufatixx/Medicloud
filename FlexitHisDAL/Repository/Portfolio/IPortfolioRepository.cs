using Medicloud.DAL.DAO;

namespace Medicloud.DAL.Repository.Portfolio
{
	public interface IPortfolioRepository
	{
		Task<int> AddAsync(PortfolioDAO dao);
		Task<PortfolioDAO> GetByIdAsync(int id);
		Task<List<PortfolioDAO>> GetByOrganizationIdAsync(int organizationId);
		Task UpdateAsync(PortfolioDAO dao);
		Task RemoveCategoriesFromPortfolio(int portfolioId, List<int> categoryIds);
		Task<int> AddCategoryToPortfolio(int portfolioId, int categoryId);
		Task DeleteAsync(int id);
	}
}
