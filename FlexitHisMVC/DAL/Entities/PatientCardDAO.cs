using System;

namespace Medicloud.DAL.Entities
{
	public class PatientCardDAO
	{
		public int id { get; set; }
		public int requestTypeID { get; set; }
		public int userID { get; set; }
		public int patientID { get; set; }
		public int departmentID { get; set; }
		public int serviceID { get; set; }
		public int docID { get; set; }
		public int priceGroupID { get; set; }
		public string note { get; set; }
		public bool finished { get; set; }
		public int referDocID { get; set; }
		public DateTime cdate { get; set; }
		public int organizationID { get; set; }
		public DateTime? startDate { get; set; }
		public DateTime? endDate { get; set; }
		public bool isOnline { get; set; }
		public int companyID { get; set; }
		public bool isActive { get; set; }
	}
}
