
using Medicloud.BLL.DTO;

namespace Medicloud.BLL.Services.WorkHours
{
	public interface IWorkHourService
	{
		Task<List<WorkHourDTO>> GetStaffWorkHours(int staffId);
		Task<List<WorkHourDTO>> GetOrganizationWorkHours(int organizationId);
		Task<bool> UpdateWorkHours(UpdateWorkHourDTO dto);
	}
}
