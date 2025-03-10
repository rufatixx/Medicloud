using Dapper;
using Medicloud.DAL.Infrastructure.UnitOfWork;

namespace Medicloud.DAL.Repository.Workspace
{
	public class WorkSpaceNameRepository : IWorkSpaceNameRepository
	{
		private readonly IUnitOfWork _unitOfWork;

		public WorkSpaceNameRepository(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<int> AddAsync(string name, int typeId)
		{
			var sql = @"INSERT INTO workspace_name(name,type_id)
						VALUES(@Name,@TypeId)";

			var con = _unitOfWork.GetConnection();
			int newId = await con.QuerySingleOrDefaultAsync<int>(sql, new { Name = name, TypeId = typeId });
			return newId;
		}
	}
}
