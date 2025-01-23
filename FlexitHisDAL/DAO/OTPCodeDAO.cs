namespace Medicloud.DAL.DAO
{
	public class OTPCodeDAO
	{
		public int id { get; set; }
		public int user_id { get; set; }
		public string otp_code { get; set; }
		public int otp_type { get; set; }
		public DateTime created_date { get; set; }
		public DateTime expiration_date { get; set; }
		public bool is_used { get; set; }
	}
}
