using Medicloud.DAL.DAO;

namespace Medicloud.DAL.Repository.Comment
{
	public interface ICommentRepository
	{
		Task<int> AddAsync(CommentDAO dao);
	}
}
