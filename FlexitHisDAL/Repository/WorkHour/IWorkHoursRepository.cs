using Medicloud.DAL.DAO;

namespace Medicloud.DAL.Repository.WorkHour
{
	public interface IWorkHoursRepository
	{
		Task<int> AddAsync(WorkHourDAO dao);
		Task<bool> UpdateAsync(WorkHourDAO dao);
		Task<List<WorkHourDAO>> GetStaffWorkHours(int staffId);
		Task<int> AddBreakAsync(StaffBreakDAO dao);
		Task<bool> RemoveBreakAsync(int id);
		Task<bool> RemoveAllBreaksByWorkHourIdAsync(int id);
		Task<bool> UpdateBreakAsync(StaffBreakDAO dao);
		Task<List<StaffBreakDAO>> GetStaffBreaksWithWorkHourId(int workHourId);
		Task<StaffWorkHoursDAO> GetStaffWorkHourById(int id);

    }
}
