namespace Medicloud.DAL.DAO
{
	public class BreakDAO
	{
		public int id { get; set; }
		public int workHourId { get; set; }
		public TimeSpan? startTime { get; set; }
		public TimeSpan? endTime { get; set; }
		public bool isActive { get; set; }
	}
}
