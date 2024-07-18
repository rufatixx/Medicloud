namespace Medicloud.BLL.Models
{
	public class PaymentViewModel
	{
		public int transaction_id { get; set; }
        public string client_rrn { get; set; }
		public string psp_rrn { get; set; }
		public string client_ip_addr { get; set; }
		public string payment_reason { get; set; }
		public int payment_reason_id { get; set; }
		public int user_id { get; set; }
		public int status { get; set; }
	}
}
