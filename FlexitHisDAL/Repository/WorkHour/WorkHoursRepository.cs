using Dapper;
using Medicloud.DAL.DAO;
using Medicloud.DAL.Infrastructure.UnitOfWork;

namespace Medicloud.DAL.Repository.WorkHour
{
	public class WorkHoursRepository:IWorkHoursRepository
	{
		private readonly IUnitOfWork _unitOfWork;

		public WorkHoursRepository(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<int> AddAsync(WorkHourDAO dao)
		{
			string AddSql = $@"
			INSERT INTO work_hours
            (dayOfWeek,startTime,endTime)
			VALUES (@{nameof(WorkHourDAO.dayOfWeek)},
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
			UPDATE work_hours SET
            startTime=@{nameof(WorkHourDAO.startTime)},
            endTime=@{nameof(WorkHourDAO.endTime)}

			WHERE id=@{nameof(WorkHourDAO.id)}";
			var con = _unitOfWork.GetConnection();
			int result = await con.ExecuteAsync(sql, dao);
			return result > 0;
		}

		public async Task<List<WorkHourDAO>> GetStaffWorkHours(int staffId)
		{
			string sql = @"SELECT wh.* FROM work_hours wh
							LEFT JOIN staff_work_hourss swh ON wh.id = swh.workHourId
							WHERE swh.staffId = @StaffId";

			var con= _unitOfWork.GetConnection();
			var result = await con.QueryAsync<WorkHourDAO>(sql,new {StaffId=staffId});
			return result.ToList();
		}


		public async Task<List<WorkHourDAO>> GetOrganizationWorkHours(int organizationId)
		{
			string sql = @"SELECT wh.* FROM work_hours wh
							LEFT JOIN organization_work_hours owh ON wh.id = owh.workHourId
							WHERE owh.organizationId = @OrganizationId";

			var con = _unitOfWork.GetConnection();
			var result = await con.QueryAsync<WorkHourDAO>(sql, new { OrganizationId = organizationId });
			return result.ToList();
		}

		public async Task<WorkHourDAO> GetWorkHourById(int id)
        {
            string sql = "SELECT * FROM work_hours WHERE id = @Id";

            var con = _unitOfWork.GetConnection();
            var result = await con.QuerySingleOrDefaultAsync<WorkHourDAO>(sql, new { Id = id });
            return result;
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
			int result = await con.ExecuteAsync(sql, new {Id=id });
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

		public async Task<bool> RemoveAllBreaksByWorkHourIdAsync(int id)
		{
			string sql = "UPDATE staff_breaks SET isActive=0 WHERE staffWorkHourId=@Id";
			var con = _unitOfWork.GetConnection();
			int result = await con.ExecuteAsync(sql, new { Id = id });
			return result > 0;
		}

		public async Task<int> AddOrganizationWorkHourAsync(int organizationId, int workHourId)
		{
			string AddSql = $@"
			INSERT INTO organization_work_hours
            (organizationId,workHourId)
			VALUES (@OrganizationId,@WorkHourId);

			SELECT LAST_INSERT_ID();";
			var con = _unitOfWork.GetConnection();
			var newId = await con.QuerySingleOrDefaultAsync<int>(AddSql, new { OrganizationId =organizationId, WorkHourId = workHourId});
			return newId;
		}

		public async Task<int> AddStaffWorkHourAsync(int staffId, int workHourId)
		{
			string AddSql = $@"
			INSERT INTO staff_work_hourss
            (staffId,workHourId)
			VALUES (@StaffId,@WorkHourId);

			SELECT LAST_INSERT_ID();";
			var con = _unitOfWork.GetConnection();
			var newId = await con.QuerySingleOrDefaultAsync<int>(AddSql, new { StaffId = staffId, WorkHourId = workHourId });
			return newId;
		}
	}
}
