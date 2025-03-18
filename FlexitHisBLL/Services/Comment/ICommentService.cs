
using Medicloud.BLL.DTO;
using Medicloud.DAL.DAO;

namespace Medicloud.BLL.Services.Comment
{
	public interface ICommentService
	{
		Task<int> AddPortfolioCommentAsync(AddPortfoliioCommentDTO dto);
		Task<List<CommentDTO>> GetPortfolioCommentAsync(int id);
		Task DeleteCommentAsync(int id);
		Task UpdateCommentAsync(AddPortfoliioCommentDTO dto);

	}
}
