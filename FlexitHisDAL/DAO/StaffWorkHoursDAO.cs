using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medicloud.DAL.DAO
{
	public class StaffWorkHoursDAO
	{
		public int id { get; set; }
		public int staffId { get; set; }
		public int dayOfWeek { get; set; }
		public string? startTime { get; set; }
		public string? endTime { get; set; }
	}
}
