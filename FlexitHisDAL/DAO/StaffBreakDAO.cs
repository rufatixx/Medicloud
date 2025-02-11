namespace Medicloud.DAL.DAO
{
	public class StaffBreakDAO
	{
		public int id { get; set; }
		public int staffWorkHourId { get; set; }
		public TimeSpan? startTime { get; set; }
		public TimeSpan? endTime { get; set; }
		public bool isActive { get; set; }
	}
}
