
namespace Medicloud.DAL.DAO
{
	public class WorkHourDAO
	{
		public int id { get; set; }
		public int dayOfWeek { get; set; }
		public TimeSpan? startTime { get; set; }
		public TimeSpan? endTime { get; set; }
	}
}
