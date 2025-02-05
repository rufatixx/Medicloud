
using Medicloud.BLL.DTO;
using Medicloud.DAL.DAO;
using Medicloud.DAL.Infrastructure.UnitOfWork;
using Medicloud.DAL.Repository.OrganizationServiceRel;
using Medicloud.DAL.Repository.Services;

namespace Medicloud.BLL.Services.Services
{
    public class ServicesService : IServicesService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServicesRepository _servicesRepository;
		private readonly IServiceTypeRepository _serviceTypeRepository;
		private readonly IOrganizationServiceRelRepository _organizationServiceRelRepository;
		public ServicesService(IUnitOfWork unitOfWork, IServicesRepository servicesRepository, IServiceTypeRepository serviceTypeRepository, IOrganizationServiceRelRepository organizationServiceRelRepository)
		{
			_unitOfWork = unitOfWork;
			_servicesRepository = servicesRepository;
			_serviceTypeRepository = serviceTypeRepository;
			_organizationServiceRelRepository = organizationServiceRelRepository;
		}

		public async Task<List<ServiceDAO>> GetServicesByOrganizationAsync(int organizationID)
        {
            
            using var con= _unitOfWork.BeginConnection();
            var result=await _servicesRepository.GetServicesByOrganizationAsync(organizationID);
            return result;
        }

		public async Task<GetServiceTypesDTO> GetServiceTypes()
		{
			using var con = _unitOfWork.BeginConnection();
			var serviceTypes = await _serviceTypeRepository.GetAllTypes();
			var serviceTypeCategories=await _serviceTypeRepository.GetTypeCategories();
			var result = new GetServiceTypesDTO
			{
				ServiceTypeCategories = serviceTypeCategories,
				ServiceTypes = serviceTypes
			};
			return result;
		}

		public async Task<int> AddServiceAsync(AddServiceDTO dto)
		{
			var service = new ServiceDAO
			{
				name = dto.name,
				price = dto.price,
				time = dto.time,
				typeId = dto.typeId,
				isMobile = dto.isMobile,
				isPriceStart=dto.isPriceStart,
			};
			using var con=_unitOfWork.BeginConnection();
			int newServiceId=await _servicesRepository.AddServiceAsync(service);
			int newRelId = await _organizationServiceRelRepository.AddAsync(new()
			{
				FirstModelId = dto.organizationId,
				SecondModelId = newServiceId,
			});
			return newServiceId;
		}

		public async Task<bool> RemoveServiceFromOrg(int organizationId, int serviceId)
		{
			using var con = _unitOfWork.BeginConnection();
			bool deleted=await _organizationServiceRelRepository.RemoveAsync(organizationId, serviceId);
			return deleted;
		}

		public async Task<ServiceDAO> GetServiceById(int serviceId)
		{
			using var con =_unitOfWork.BeginConnection();

			var result = await _servicesRepository.GetServiceByIdAsync(serviceId);
			return result;
		}

		public async Task<bool> UpdateService(AddServiceDTO dto)
		{
			var service = new ServiceDAO
			{
				id= dto.id,
				name = dto.name,
				price = dto.price,
				time = dto.time,
				typeId = dto.typeId,
				isMobile = dto.isMobile,
				isPriceStart = dto.isPriceStart,
			};
			using var con = _unitOfWork.BeginConnection();
			bool updated = await _servicesRepository.UpdateServiceAsync(service);
			return updated;
		}

	}
}
