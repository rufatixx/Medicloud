using System;
namespace Medicloud.Models.DTO
{
	public class ServiceStatisticsDTO
	{
		public long ID { get; set; }
		public string service_name { get; set; }
		public DateTime last_purchase_date { get; set; }
		public string price { get; set; }
		public string quantity { get; set; }
		public string amount { get; set; }
	}
}

