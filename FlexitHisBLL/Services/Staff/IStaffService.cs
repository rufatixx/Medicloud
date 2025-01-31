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
		Task<List<StaffWorkHoursDAO>> GetWorkHours(int staffId);
		Task<StaffDAO> GetOwnerStaffByOrganizationId(int organizationId);
	}
}
