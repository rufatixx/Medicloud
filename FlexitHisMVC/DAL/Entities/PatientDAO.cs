namespace Medicloud.DAL.Entities
{
	public class PatientDAO
	{
		public int id { get; set; }
		public int userID { get; set; }
		public int organizationID { get; set; }
		public string name { get; set; }
		public string surname { get; set; }
		public string father { get; set; }
		public string clientPhone { get; set; }
		public string clientEmail { get; set; }
		public int genderID { get; set; }
		public string fin { get; set; }
		public string bDate { get; set; }
		public int orgReasonId { get; set; }
		public List<PatientCardDAO> Cards { get; set; }
	}
}
