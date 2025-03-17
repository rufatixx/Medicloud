
using Medicloud.BLL.DTO;

namespace Medicloud.BLL.Services.Comment
{
	public interface ICommentService
	{
		Task<int> AddPortfolioCommentAsync(AddPortfoliioCommentDTO dto);
		Task<List<CommentDTO>> GetPortfolioCommentAsync(int id);
	}
}
