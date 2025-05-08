using Medicloud.DAL.Entities;

namespace Medicloud.BLL.Models
{
	public class AddAnamnesisDTO
	{
		public List<AnamnesisAnswerDAO> Fields {  get; set; }
		public int CardId { get; set; }
		public int DoctorId { get; set; }
	}
}
