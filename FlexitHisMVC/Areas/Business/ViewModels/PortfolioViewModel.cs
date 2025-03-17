using Medicloud.BLL.DTO;

namespace Medicloud.WebUI.Areas.Business.ViewModels
{
	public class PortfolioViewModel
	{
		public int organizationId { get; set; }
		public List<PortfolioDTO> portfolios { get; set; }
		public int UserId { get; set; }
	}
}
