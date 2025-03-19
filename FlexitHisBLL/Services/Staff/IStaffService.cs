
using Medicloud.DAL.DAO;


namespace Medicloud.BLL.Services.Staff
{
	public interface IStaffService
	{
		Task<StaffDAO> GetOwnerStaffByOrganizationId(int organizationId);
		Task<bool> UpdateStaffAsync(StaffDAO dao);
	}
}
