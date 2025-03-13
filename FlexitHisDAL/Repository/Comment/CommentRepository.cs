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
            (userId,description,cDate)
			VALUES (@{nameof(CommentDAO.userId)},
            @{nameof(CommentDAO.description)},
            @{nameof(CommentDAO.cDate)});

			SELECT LAST_INSERT_ID();";
			var con = _unitOfWork.GetConnection();
			var newId = await con.QuerySingleOrDefaultAsync<int>(AddSql, dao);
			return newId;
		}
	}
}
