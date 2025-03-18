using Dapper;
using Medicloud.DAL.DAO;
using Medicloud.DAL.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
			string sql = "SELECT * FROM work_hours WHERE staffId = @StaffId";

			var con= _unitOfWork.GetConnection();
			var result = await con.QueryAsync<WorkHourDAO>(sql,new {StaffId=staffId});
			return result.ToList();
		}

        public async Task<StaffWorkHoursDAO> GetStaffWorkHourById(int id)
        {
            string sql = "SELECT * FROM staff_work_hours WHERE id = @Id";

            var con = _unitOfWork.GetConnection();
            var result = await con.QuerySingleOrDefaultAsync<StaffWorkHoursDAO>(sql, new { Id = id });
            return result;
        }

        public async Task<int> AddBreakAsync(StaffBreakDAO dao)
		{
			string AddSql = $@"
			INSERT INTO staff_breaks
            (startTime,endTime,staffWorkHourId)
			VALUES (@{nameof(StaffBreakDAO.startTime)},
            @{nameof(StaffBreakDAO.endTime)},
            @{nameof(StaffBreakDAO.staffWorkHourId)});

			SELECT LAST_INSERT_ID();";
			var con = _unitOfWork.GetConnection();
			var newId = await con.QuerySingleOrDefaultAsync<int>(AddSql, dao);
			return newId;
		}

		public async Task<List<StaffBreakDAO>> GetStaffBreaksWithWorkHourId(int workHourId)
		{
			string sql = "SELECT * FROM staff_breaks WHERE staffWorkHourId = @WorkHourId AND isActive=1";

			var con = _unitOfWork.GetConnection();
			var result = await con.QueryAsync<StaffBreakDAO>(sql, new { WorkHourId = workHourId });
			return result.ToList();
		}


		public async Task<bool> RemoveBreakAsync(int id)
		{
			string sql = "UPDATE staff_breaks SET isActive=0 WHERE id=@Id";
			var con = _unitOfWork.GetConnection();
			int result = await con.ExecuteAsync(sql, new {Id=id });
			return result > 0;
		}

		public async Task<bool> UpdateBreakAsync(StaffBreakDAO dao)
		{
			string sql = $@"
			UPDATE staff_breaks SET
            startTime=@{nameof(StaffBreakDAO.startTime)},
            endTime=@{nameof(StaffBreakDAO.endTime)}

			WHERE id=@{nameof(StaffBreakDAO.id)}";
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
	}
}
