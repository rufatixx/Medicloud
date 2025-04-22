namespace Medicloud.DAL.Entities
{
	public class WorkHourDAO
	{
		public int id { get; set; }
		public int userId { get; set; }
		public int organizationId { get; set; }
		public int dayOfWeek { get; set; }
		public TimeSpan? startTime { get; set; }
		public TimeSpan? endTime { get; set; }
		public List<BreakDAO> Breaks { get; set; }
	}
}
