using Medicloud.BLL.DTO;
using Medicloud.DAL.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medicloud.BLL.Services.Staff
{
	public interface IStaffService
	{
		Task<List<StaffWorkHoursDTO>> GetWorkHours(int staffId);
		Task<StaffDAO> GetOwnerStaffByOrganizationId(int organizationId);
		Task<bool> UpdateStaffAsync(StaffDAO dao);
	}
}
