
using Medicloud.DAL.DAO;

namespace Medicloud.DAL.Repository.OrganizationPhoto
{
	public interface IOrganizationPhotoRepository
	{
		Task<int> AddAsync(int organizationId,int fileId);
		Task<List<FileDAO>> GetByOrganizationId(int organizationId);
	}
}
