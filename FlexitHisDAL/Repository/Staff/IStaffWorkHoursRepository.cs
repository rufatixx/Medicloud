using Medicloud.DAL.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medicloud.DAL.Repository.Staff
{
	public interface IStaffWorkHoursRepository
	{
		Task<int> AddAsync(StaffWorkHoursDAO dao);
		Task<int> UpdateAsync(StaffWorkHoursDAO dao);
		Task<List<StaffWorkHoursDAO>> GetStaffWorkHours(int staffId);
	}
}
