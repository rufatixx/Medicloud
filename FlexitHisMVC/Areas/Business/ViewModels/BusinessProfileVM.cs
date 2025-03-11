using Medicloud.BLL.DTO;

namespace Medicloud.WebUI.Areas.Business.ViewModels
{
	public class BusinessProfileVM
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string LogoPath { get; set; }
		public string CoverPath { get; set; }
		public string LogoSrc { get; set; }
		public string CoverSrc { get; set; }
		public FileDTO Logo { get; set; }
		public FileDTO Cover { get; set; }
		public List<FileDTO> WorkImages { get; set; }
		public List<PortfolioDTO> portfolios { get; set; }
	}
}
