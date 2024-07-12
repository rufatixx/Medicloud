using System;
namespace Medicloud.Models.DTO
{
	public class ServiceStatisticsDTO
	{
		public long ID { get; set; }
		public string name { get; set; }
		public DateTime lastPurchaseDate { get; set; }
		public string price { get; set; }
		public string quantity { get; set; }
		public string amount { get; set; }
	}
}

