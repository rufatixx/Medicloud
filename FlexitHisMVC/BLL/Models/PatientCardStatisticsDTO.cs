using System;
namespace Medicloud.Models.DTO
{
	public class PatientCardStatisticsDTO
	{
		public string visitsThisMonth { get; set; }
		public string visitsLastMonth { get; set; }
		public int isGrow { get; set; }
		public string percentage { get; set; }
	}
}

