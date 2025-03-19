using Medicloud.BLL.DTO;
using Medicloud.DAL.Infrastructure.UnitOfWork;
using Medicloud.DAL.Repository.Amenity;

namespace Medicloud.BLL.Services.AmenityService
{
	public class AmenityService : IAmenityService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IAmenityRepository _amenityRepository;

		public AmenityService(IUnitOfWork unitOfWork, IAmenityRepository amenityRepository)
		{
			_unitOfWork = unitOfWork;
			_amenityRepository = amenityRepository;
		}

		public async Task<List<TempDTO>> GetAll()
		{
			using var con = _unitOfWork.BeginConnection();
			var data = await _amenityRepository.GetAll();
			return data.Select(d => new TempDTO { id = d.id, name = d.name, }).ToList();
		}

		public async Task<OrganizationAmenityDTO> GetOrganizationAmenitiesAsync(int organizationId)
		{
			using var con = _unitOfWork.BeginConnection();
			var amenityData = await _amenityRepository.GetAll();
			var relData = await _amenityRepository.GetOrganizationAmenityIds(organizationId);
			return new()
			{
				Amenities = amenityData.Select(d => new TempDTO { id = d.id, name = d.name, }).ToList(),
				OrganizationAmenities = relData.ToList(),
				OrganizationId = organizationId
			};
		}

		public async Task UpdateOrganizationAmenitiesAsync(OrganizationAmenityDTO dto)
		{
			using var con = _unitOfWork.BeginConnection();

			var currentAmenityIds = await _amenityRepository.GetOrganizationAmenityIds(dto.OrganizationId);

			var amenityIdsToRemove = currentAmenityIds
				.Where(current => !dto.OrganizationAmenities.Any(newAmenity => newAmenity == current));

			// Find the amenities to add (new ones that are not currently associated with the organization)
			var amenityIdsToAdd = dto.OrganizationAmenities
				.Where(newAmenity => !currentAmenityIds.Contains(newAmenity))
				.Select(newAmenity => newAmenity)
				.ToList();

			// Remove amenities that should no longer be associated

			await _amenityRepository.RemoveOrganizationAmenities(dto.OrganizationId, amenityIdsToRemove);


			// Add amenities that are not yet associated with the organization
			foreach (var amenityId in amenityIdsToAdd)
			{
				await _amenityRepository.AddOrganizationAmenityAsync(dto.OrganizationId, amenityId);
			}
		}
	}
}
