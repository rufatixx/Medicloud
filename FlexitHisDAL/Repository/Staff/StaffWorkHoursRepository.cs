using Dapper;
using Medicloud.DAL.DAO;
using Medicloud.DAL.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medicloud.DAL.Repository.Staff
{
	public class StaffWorkHoursRepository:IStaffWorkHoursRepository
	{
		private readonly IUnitOfWork _unitOfWork;

		public StaffWorkHoursRepository(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<int> AddAsync(StaffWorkHoursDAO dao)
		{
			string AddSql = $@"
			INSERT INTO staff_work_hours
            (staffId,dayOfWeek,startTime,endTime)
			VALUES (@{nameof(StaffWorkHoursDAO.staffId)},
            @{nameof(StaffWorkHoursDAO.dayOfWeek)},
            @{nameof(StaffWorkHoursDAO.startTime)},
            @{nameof(StaffWorkHoursDAO.endTime)});

			SELECT LAST_INSERT_ID();";
			var con = _unitOfWork.GetConnection();
			var newId = await con.QuerySingleOrDefaultAsync<int>(AddSql, dao);
			return newId;
		}

		public async Task<bool> UpdateAsync(StaffWorkHoursDAO dao)
		{
			string sql = $@"
			UPDATE staff_work_hours SET
            startTime=@{nameof(StaffWorkHoursDAO.startTime)},
            endTime=@{nameof(StaffWorkHoursDAO.endTime)}

			WHERE id=@{nameof(StaffWorkHoursDAO.id)}";
			var con = _unitOfWork.GetConnection();
			int result = await con.ExecuteAsync(sql, dao);
			return result > 0;
		}

		public async Task<List<StaffWorkHoursDAO>> GetStaffWorkHours(int staffId)
		{
			string sql = "SELECT * FROM staff_work_hours WHERE staffId = @StaffId";

			var con= _unitOfWork.GetConnection();
			var result = await con.QueryAsync<StaffWorkHoursDAO>(sql,new {StaffId=staffId});
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
	}
}
