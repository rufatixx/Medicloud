

using Medicloud.BLL.DTO;

namespace Medicloud.BLL.Services.AmenityService
{
	public interface IAmenityService
	{
		Task<List<TempDTO>> GetAll();
		Task<OrganizationAmenityDTO> GetOrganizationAmenitiesAsync(int organizationId);
		Task UpdateOrganizationAmenitiesAsync(OrganizationAmenityDTO dto);
	}
}
