using Medicloud.DAL.Entities;

namespace Medicloud.BLL.Models
{
	public class WorkHourViewModel
	{
		public List<WorkHourDTO> WorkHours { get; set; }
		public int UserId { get; set; }
		public int OrganizationId { get; set; }
		public int id { get; set; }
		public string ClosedDays { get; set; }
		public string OpenedDays { get; set; }
	}
}
