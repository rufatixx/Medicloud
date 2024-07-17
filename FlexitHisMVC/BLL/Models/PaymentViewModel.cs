namespace Medicloud.BLL.Models
{
	public class PaymentViewModel
	{
		public int ServiceId { get; set; } = 1394;
        public string ClientRrn { get; set; } = "123456789";
        public decimal Amount { get; set; }
		public string ClientIp { get; set; }
		public string Hash { get; set; }
	}
}
