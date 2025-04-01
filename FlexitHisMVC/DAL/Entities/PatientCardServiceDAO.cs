using System.Dynamic;

namespace Medicloud.DAL.Entities
{
	public class PatientCardServiceDAO
	{

		public int id { get; set; }
		public int PatientCardID { get; set; }
		public int ServiceID { get; set; }
		public int SenderDocID { get; set; }
		public int patientID { get; set; }
		public string patientName { get; set; }
		public string patientSurname { get; set; }
		public string ServiceName { get; set; }
		public string ServicePrice { get; set; }
		public string ServiceCode { get; set; }
		public string ServiceGroup { get; set; }
		public string DocName { get; set; }
		public string DocSurname { get; set; }
		public string SenderDocName { get; set; }
		public string SenderDocSurname { get; set; }
		public DateTime cDate { get; set; }
		public int card_id { get; set; }


	}
}
