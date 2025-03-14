using Medicloud.BLL.DTO;

namespace Medicloud.WebUI.Areas.Business.ViewModels
{
	public class SidebarViewModel
	{
		public string OrganizationName { get; set; }
		public int LogoId { get; set; }
		public string StaffName { get; set; }

		public List<OrganizationDTO> Organizations { get; set; }
		public int ActiveId { get; set; }
		//public List<OrganizationDTO> Organizations { get; set; }
	}
}
