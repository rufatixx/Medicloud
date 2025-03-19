using Medicloud.DAL.DAO;

namespace Medicloud.DAL.Repository.Amenity
{
	public interface IAmenityRepository
	{
		Task<IEnumerable<TempDAO>> GetAll();
		Task<int> AddOrganizationAmenityAsync(int organizationId, int amenityId);
		Task<IEnumerable<int>> GetOrganizationAmenityIds(int organizationId);
		Task RemoveOrganizationAmenities(int organizationId,IEnumerable<int> removedIds);
	}
}
