using Medicloud.BLL.DTO;

namespace Medicloud.WebUI.Areas.Business.ViewModels
{
	public class AddPortfolioViewModel
	{
		public int id { get; set; }
		public string photo { get; set; }
		public string photoSrc { get; set; }
		public string extension { get; set; }
		public int organizationId { get; set; }
		public string description { get; set; }
		public bool isEdit { get; set; }
		public List<int>? selectedCategoryIds { get; set; }
		public List<TempDTO> Categories { get; set; }
	}
}
