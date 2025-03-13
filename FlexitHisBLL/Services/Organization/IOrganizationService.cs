using Medicloud.BLL.DTO;
using Medicloud.DAL.DAO;

namespace Medicloud.BLL.Services.Organization
{
	public interface IOrganizationService
	{
		Task<int> AddAsync(AddOrganizationDTO dto);
		Task<OrganizationDAO?> GetByIdAsync(int id);
		Task<bool> UpdateAsync(UpdateOrganizationDTO dto);
		Task<int>AddOrganizationTravel(OrganizationTravelDAO dao);
		Task<int> UpdateOrganizationCategories(int organizationId, List<int> selectedCategories);
		Task<OrganizationTravelDAO?> GetOrganizationTravel(int organizationId);
		Task<bool> UpdateOrganizationTravel(OrganizationTravelDAO dao);
		Task<int> AddOrganizationPlanAsync(OrganizationPlanDAO dao);
		Task<List<OrganizationPlanDAO>> GetPlansByOrganizationId(int organizationId);
		Task UpdateLogo(FileDTO file, int organizationId);
		Task UpdateCover(FileDTO file, int organizationId);
		Task UpdateCoverId(int fileId, int organizationId);
	}
}
