using Medicloud.DAL.DAO;

namespace Medicloud.DAL.Repository.WorkHour
{
	public interface IWorkHoursRepository
	{
		Task<int> AddAsync(WorkHourDAO dao);
		Task<bool> UpdateAsync(WorkHourDAO dao);
		Task<List<WorkHourDAO>> GetStaffWorkHours(int staffId);
		Task<List<WorkHourDAO>> GetOrganizationWorkHours(int organizationId);
		Task<int> AddBreakAsync(BreakDAO dao);
		Task<bool> RemoveBreakAsync(int id);
		Task<bool> RemoveAllBreaksByWorkHourIdAsync(int id);
		Task<bool> UpdateBreakAsync(BreakDAO dao);
		Task<List<BreakDAO>> GetBreaksWithWorkHourId(int workHourId);
		Task<WorkHourDAO> GetWorkHourById(int id);
		Task<int>AddOrganizationWorkHourAsync(int organizationId,int workHourId);
		Task<int>AddStaffWorkHourAsync(int staffId,int workHourId);
    }
}
