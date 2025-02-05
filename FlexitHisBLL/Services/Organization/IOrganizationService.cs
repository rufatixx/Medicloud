using Medicloud.BLL.DTO;
using Medicloud.DAL.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medicloud.BLL.Services.Organization
{
	public interface IOrganizationService
	{
		Task<int> AddAsync(AddOrganizationDTO dto);
		Task<OrganizationDAO?> GetByIdAsync(int id);
		Task<bool> UpdateAsync(OrganizationDAO dao);
		Task<int>AddOrganizationTravel(OrganizationTravelDAO dao);
		Task<int> UpdateOrganizationCategories(int organizationId, List<int> selectedCategories);
	}
}
