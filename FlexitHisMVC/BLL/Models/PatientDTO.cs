using System;
namespace Medicloud.Models
{
    public class PatientDTO
    {
       //public string userToken { get; set; }
       // public string requestToken { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string father { get; set; }
        public string clientPhone { get; set; }
        public string clientEmail { get; set; }
        public int genderID { get; set; }
        public string fin { get; set; }
        public int requestTypeID { get; set; }
        public int priceGroupID { get; set; }
        public int serviceID { get; set; }
        public int depID { get; set; }
        public int docID { get; set; }
        public int referDocID { get; set; }
        public string birthDate { get; set; }
        public string note { get; set; }
        public DateTime selectedDate { get; set; }
        public TimeSpan startTime { get; set; }
        public TimeSpan endTime { get; set; }
		public bool isOnline { get; set; }
		public int orgReasonId { get; set; }
		public int companyID { get; set; }
	}
}
