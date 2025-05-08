namespace Medicloud.DAL.Entities
{
	public class AnamnesisAnswerDAO
	{
		public int id { get; set; }
		public int anamnesisId { get; set; }
		public int anamnesisFieldId { get; set; }
		public string answerText { get; set; }
	}
}
