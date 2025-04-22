using Medicloud.DAL.Entities;

namespace Medicloud.BLL.Models
{

	public class WorkHourDTO
	{
		public int id { get; set; }
		public TimeSpan? startTime { get; set; }
		public TimeSpan? endTime { get; set; }
		public int dayOfWeek { get; set; }
		public List<BreakDTO> Breaks { get; set; }
	}

	public class UpdateWorkHourDTO
	{
		public TimeSpan? startTime { get; set; }
		public TimeSpan? endTime { get; set; }
		public List<BreakDTO> Breaks { get; set; }
		public List<int> OpenedDays { get; set; }
		public List<int> ClosedDays { get; set; }
		public List<int> SelectedDays { get; set; }

	}

}
