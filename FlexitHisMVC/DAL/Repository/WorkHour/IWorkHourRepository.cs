using Medicloud.DAL.Entities;

namespace Medicloud.DAL.Repository.WorkHour
{
	public interface IWorkHourRepository
	{
		Task<int> AddOrganizationUserWorkHourAsync(WorkHourDAO dao);
		Task<bool> UpdateAsync(WorkHourDAO dao);
		Task<List<WorkHourDAO>> GetOrganizationUserWorkHours(int userId,int organizationId);
		Task<int> AddBreakAsync(BreakDAO dao);
		Task<bool> RemoveBreakAsync(int id);
		Task<bool> RemoveAllBreaksByWorkHourIdAsync(int id);
		Task<bool> UpdateBreakAsync(BreakDAO dao);
		Task<List<BreakDAO>> GetBreaksWithWorkHourId(int workHourId);
		Task<WorkHourDAO> GetWorkHourById(int id);
	}
}
