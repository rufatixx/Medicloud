
namespace Medicloud.BLL.DTO
{
	public class PortfolioDTO
	{
		public int id { get; set; }
		public string description { get; set; }
		public int fileId { get; set; }
		public int organizationId { get; set; }
		public FileDTO file { get; set; }
		public  List<TempDTO>categories { get; set; }
		public bool isActive { get; set; }
		public string categoryIds  { get; set; }
		public List<CommentDTO> Comments { get; set; }
		public List<TempDTO> Categories { get; set; }
	}
}
