using System;
namespace Medicloud.Models.DTO
{
	public class PatientStatisticsDTO
	{
		public string newCustomersThisMonth { get; set; }
		public string newCustomersLasMonth { get; set; }
		public int isGrow { get; set; }
		public string percentage { get; set; }
	}
}

