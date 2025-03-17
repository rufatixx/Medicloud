using Medicloud.DAL.DAO;

namespace Medicloud.DAL.Repository.Comment
{
	public interface ICommentRepository
	{
		Task<int> AddAsync(CommentDAO dao);
		Task<int> AddPortfolioCommentAsync(TempRelDAO tempRelDAO);
		Task<List<CommentDAO>> GetPortfolioCommentAsync(int id);
		Task<List<CommentDAO>>GetCommentReplies(int id);
	}
}
