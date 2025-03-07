using Medicloud.BLL.DTO;

namespace Medicloud.BLL.Services.OrganizationPhoto
{
	public interface IOrganizationPhotoService
	{
		Task<int> AddAsync(int organizationId, FileDTO dto);
		Task<List<FileDTO>> GetByOrganizationId(int organizationId);
		Task RemoveAsync(int id);
	}
}
