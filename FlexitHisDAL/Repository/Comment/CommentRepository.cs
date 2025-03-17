using Dapper;
using Medicloud.DAL.DAO;
using Medicloud.DAL.Infrastructure.UnitOfWork;

namespace Medicloud.DAL.Repository.Comment
{
	public class CommentRepository:ICommentRepository
	{
		private readonly IUnitOfWork _unitOfWork;

		public CommentRepository(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<int> AddAsync(CommentDAO dao)
		{
			
			string AddSql = $@"
			INSERT INTO comments
            (userId,description,cDate,parentId)
			VALUES (@{nameof(CommentDAO.userId)},
            @{nameof(CommentDAO.description)},
            @{nameof(CommentDAO.cDate)},
            @{nameof(CommentDAO.parentId)});

			SELECT LAST_INSERT_ID();";
			var con = _unitOfWork.GetConnection();
			var transaction=_unitOfWork.GetTransaction();
			var newId = await con.QuerySingleOrDefaultAsync<int>(AddSql, dao,transaction);
			return newId;
		}
		public async Task<int> AddPortfolioCommentAsync(TempRelDAO tempRelDAO)
		{
			string AddSql = $@"
			INSERT INTO portfolio_comments
            (portfolioId,commentId)
			VALUES (@{nameof(TempRelDAO.FirstModelId)},
            @{nameof(TempRelDAO.SecondModelId)});

			SELECT LAST_INSERT_ID();";
			var con = _unitOfWork.GetConnection();
			var transaction = _unitOfWork.GetTransaction();
			var newId = await con.QuerySingleOrDefaultAsync<int>(AddSql, tempRelDAO,transaction);
			return newId;
		}

		public async Task<List<CommentDAO>> GetPortfolioCommentAsync(int id)
		{
			string sql = @"SELECT c.*,u.name as Username 
					FROM comments c
					LEFT JOIN portfolio_comments pc ON c.id = pc.commentId 
					LEFT JOIN users u ON c.userId = u.id 
					WHERE pc.portfolioId=@PortfolioId  AND c.isActive = 1;";

			var con = _unitOfWork.GetConnection();
			var data= await con.QueryAsync<CommentDAO>(sql, new { PortfolioId =id});
			return data.ToList();
		}

		public async Task<List<CommentDAO>> GetCommentReplies(int id)
		{
			string sql = @"SELECT c.*,u.name as Username 
					FROM comments c
					LEFT JOIN users u ON c.userId = u.id 
					WHERE c.parentId=@Id AND c.isActive = 1;";

			var con = _unitOfWork.GetConnection();
			var data = await con.QueryAsync<CommentDAO>(sql, new { Id = id });
			return data.ToList();
		}
	}
}
