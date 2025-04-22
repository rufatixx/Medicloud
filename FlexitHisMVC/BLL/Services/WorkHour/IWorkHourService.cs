using Medicloud.BLL.Models;
using Medicloud.DAL.Entities;

namespace Medicloud.BLL.Services.WorkHour
{
	public interface IWorkHourService
	{
		Task<int> AddOrganizationUserWorkHourAsync(int organizationId, int userId);
		Task<bool> UpdateAsync(WorkHourDAO dao);
		Task<List<WorkHourDTO>> GetOrganizationUserWorkHours(int userId, int organizationId);
		Task<int> AddBreakAsync(BreakDAO dao);
		Task<bool> RemoveBreakAsync(int id);
		Task<bool> RemoveAllBreaksByWorkHourIdAsync(int id);
		Task<bool> UpdateBreakAsync(BreakDAO dao);
		Task<List<BreakDAO>> GetBreaksWithWorkHourId(int workHourId);
		Task<WorkHourDAO> GetWorkHourById(int id);
		Task<bool> UpdateWorkHours(UpdateWorkHourDTO dto);
	}
}
