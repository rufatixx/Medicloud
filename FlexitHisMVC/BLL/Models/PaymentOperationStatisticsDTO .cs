using System;
namespace Medicloud.Models.DTO
{
	public class PaymentOperationStatisticsDTO
    {
		public string incomeThisMonth { get; set; }
		public string incomeLasMonth { get; set; }
		public int isGrow { get; set; }
		public string percentage { get; set; }
	}
}

