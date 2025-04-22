using Dapper;
using Medicloud.DAL.Entities;
using Medicloud.DAL.Infrastructure.Abstract;

namespace Medicloud.DAL.Repository.WorkHour
{
	public class WorkHourRepository:IWorkHourRepository
	{
		private readonly IUnitOfWork _unitOfWork;

		public WorkHourRepository(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<int> AddOrganizationUserWorkHourAsync(WorkHourDAO dao)
		{
			string AddSql = $@"
			INSERT INTO organization_user_work_hours
            (userId,organizationId,dayOfWeek,startTime,endTime)
			VALUES (@{nameof(WorkHourDAO.userId)},
            @{nameof(WorkHourDAO.organizationId)},
            @{nameof(WorkHourDAO.dayOfWeek)},
            @{nameof(WorkHourDAO.startTime)},
            @{nameof(WorkHourDAO.endTime)});

			SELECT LAST_INSERT_ID();";
			var con = _unitOfWork.GetConnection();
			var newId = await con.QuerySingleOrDefaultAsync<int>(AddSql, dao);
			return newId;
		}

		public async Task<bool> UpdateAsync(WorkHourDAO dao)
		{
			string sql = $@"
			UPDATE organization_user_work_hours SET
            startTime=@{nameof(WorkHourDAO.startTime)},
            endTime=@{nameof(WorkHourDAO.endTime)}

			WHERE id=@{nameof(WorkHourDAO.id)}";
			var con = _unitOfWork.GetConnection();
			int result = await con.ExecuteAsync(sql, dao);
			return result > 0;
		}

		public async Task<List<WorkHourDAO>> GetOrganizationUserWorkHours(int userId, int organizationId)
		{
			string sql = @"SELECT wh.* FROM organization_user_work_hours wh
							WHERE wh.userId = @UserId AND wh.organizationId=@OrganizationId";

			var con = _unitOfWork.GetConnection();
			var result = await con.QueryAsync<WorkHourDAO>(sql, new { UserId = userId, OrganizationId=organizationId });
			return result.ToList();
		}

		public async Task<int> AddBreakAsync(BreakDAO dao)
		{
			string AddSql = $@"
			INSERT INTO work_hour_breaks
            (startTime,endTime,workHourId)
			VALUES (@{nameof(BreakDAO.startTime)},
            @{nameof(BreakDAO.endTime)},
            @{nameof(BreakDAO.workHourId)});

			SELECT LAST_INSERT_ID();";
			var con = _unitOfWork.GetConnection();
			var newId = await con.QuerySingleOrDefaultAsync<int>(AddSql, dao);
			return newId;
		}
		public async Task<List<BreakDAO>> GetBreaksWithWorkHourId(int workHourId)
		{
			string sql = "SELECT * FROM work_hour_breaks WHERE workHourId = @WorkHourId AND isActive=1";

			var con = _unitOfWork.GetConnection();
			var result = await con.QueryAsync<BreakDAO>(sql, new { WorkHourId = workHourId });
			return result.ToList();
		}

		public async Task<bool> RemoveBreakAsync(int id)
		{
			string sql = "UPDATE work_hour_breaks SET isActive=0 WHERE id=@Id";
			var con = _unitOfWork.GetConnection();
			int result = await con.ExecuteAsync(sql, new { Id = id });
			return result > 0;
		}

		public async Task<bool> RemoveAllBreaksByWorkHourIdAsync(int id)
		{
			string sql = "UPDATE work_hour_breaks SET isActive=0 WHERE workHourId=@Id";
			var con = _unitOfWork.GetConnection();
			int result = await con.ExecuteAsync(sql, new { Id = id });
			return result > 0;
		}

		public async Task<bool> UpdateBreakAsync(BreakDAO dao)
		{
			string sql = $@"
			UPDATE work_hour_breaks SET
            startTime=@{nameof(BreakDAO.startTime)},
            endTime=@{nameof(BreakDAO.endTime)}

			WHERE id=@{nameof(BreakDAO.id)}";
			var con = _unitOfWork.GetConnection();
			int result = await con.ExecuteAsync(sql, dao);
			return result > 0;
		}

		public async Task<WorkHourDAO> GetWorkHourById(int id)
		{
			string sql = "SELECT * FROM organization_user_work_hours WHERE id = @Id";

			var con = _unitOfWork.GetConnection();
			var result = await con.QuerySingleOrDefaultAsync<WorkHourDAO>(sql, new { Id = id });
			return result;
		}
	}
}
