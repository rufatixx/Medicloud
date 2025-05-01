using Medicloud.Models;

namespace Medicloud.ViewModels
{
	public class AddServiceModalViewModel
	{
		public List<DepartmentDAO> Departments { get; set; }
		public List<ServiceGroup> ServiceGroups { get; set; }
		public List<ServiceObj> Services { get; set; }
	}
}
