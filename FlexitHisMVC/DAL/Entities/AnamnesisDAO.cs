namespace Medicloud.DAL.Entities
{
	public class AnamnesisDAO
	{
		public int id { get; set; }
		public int doctorId { get; set; }
		public int patientCardId { get; set; }
		public DateTime createDate { get; set; }
		public bool isActive { get; set; }
		public List<AnamnesisAnswerDAO> AnamnesisAnswers { get; set; }
	}
}
