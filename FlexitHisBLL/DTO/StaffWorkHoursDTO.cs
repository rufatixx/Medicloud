

using Medicloud.DAL.DAO;

namespace Medicloud.BLL.DTO
{
	public class StaffWorkHoursDTO
	{
		public int id { get; set; }
		public TimeSpan? startTime { get; set; }
		public TimeSpan? endTime { get; set; }
		public int dayOfWeek { get; set; }
		public List<StaffBreakDAO> Breaks { get; set; }
	}

	public class StaffBreakDTO
	{
		public int id { get; set; }
		public TimeSpan? start{ get; set; }
		public TimeSpan? end { get; set; }
	}
}
