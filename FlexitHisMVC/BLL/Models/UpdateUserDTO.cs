using Medicloud.DAL.Entities;
using Medicloud.Models;

namespace Medicloud.BLL.Models
{
	public class UpdateUserDTO
	{
		public string name { get; set; }
		public string surname { get; set; }
		public string father { get; set; }
		public string fin { get; set; }
		public string bDate { get; set; }
		public string pwd { get; set; }
		public int ID { get; set; }
		public int depID { get; set; }
		public string username { get; set; }
		public long specialityID { get; set; }
		public string mobile { get; set; }
		public string passportSerialNum { get; set; }
		public string email { get; set; }
		public DateTime? recovery_otp_send_date { get; set; }
		public DateTime? otpSentDate { get; set; }
		public DateTime? cDate { get; set; }
		public bool isRegistered { get; set; }
		public int status { get; set; }
		public int userType { get; set; }
		public string imagePath { get; set; }
	}
}
