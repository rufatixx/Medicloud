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

		public Task<int> UpdateAsync(StaffWorkHoursDAO dao)
		{
			throw new NotImplementedException();
		}

		public async Task<List<StaffWorkHoursDAO>> GetStaffWorkHours(int staffId)
		{
			string sql = "SELECT * FROM staff_work_hours WHERE staffId = @StaffId";

			var con= _unitOfWork.GetConnection();
			var result = await con.QueryAsync<StaffWorkHoursDAO>(sql,new {StaffId=staffId});
			return result.ToList();
		}
	}
}
