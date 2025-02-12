using Medicloud.DAL.DAO;

namespace Medicloud.DAL.Repository.Staff
{
	public interface IStaffWorkHoursRepository
	{
		Task<int> AddAsync(StaffWorkHoursDAO dao);
		Task<bool> UpdateAsync(StaffWorkHoursDAO dao);
		Task<List<StaffWorkHoursDAO>> GetStaffWorkHours(int staffId);
		Task<int> AddBreakAsync(StaffBreakDAO dao);
		Task<bool> RemoveBreakAsync(int id);
		Task<bool> UpdateBreakAsync(StaffBreakDAO dao);
		Task<List<StaffBreakDAO>> GetStaffBreaksWithWorkHourId(int workHourId);
		Task<StaffWorkHoursDAO> GetStaffWorkHourById(int id);

    }
}
