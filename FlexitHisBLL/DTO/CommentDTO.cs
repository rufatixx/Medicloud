
namespace Medicloud.BLL.DTO
{
	public class CommentDTO
	{
		public int Id { get; set; }
		public string Description { get; set; }
		public int UserId { get; set; }
		public string UserName { get; set; }
		public DateTime CDate { get; set; }
		public List<CommentDTO> Replies { get; set; }
		public bool IsAuthor { get; set; }


	}
	public class AddPortfoliioCommentDTO
	{
		public int Id { get; set; }
		public string Description { get; set; }
		public int UserId { get; set; }
		public DateTime CDate { get; set; }
		public int PortfolioId { get; set; }
		public int ParentId { get; set; }
	}

}
