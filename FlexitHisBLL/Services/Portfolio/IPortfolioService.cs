using Medicloud.BLL.DTO;

namespace Medicloud.BLL.Services.Portfolio
{
	public interface IPortfolioService
	{
		Task<int> AddPortfolioAsync(PortfolioDTO dto);
		Task<List<PortfolioDTO>> GetPortfolioByOrganizationIdAsync(int organizationId);
		Task<PortfolioDTO> GetPortfolioByIdAsync(int id);
		Task UpdateAsync(PortfolioDTO dto);
		Task DeleteAsync(int id);
	}
}
