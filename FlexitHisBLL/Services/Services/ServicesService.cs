
using Medicloud.DAL.DAO;
using Medicloud.DAL.Infrastructure.UnitOfWork;
using Medicloud.DAL.Repository.Services;

namespace Medicloud.BLL.Services.Services
{
    public class ServicesService : IServicesService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServicesRepository _servicesRepository;

        public ServicesService(IUnitOfWork unitOfWork, IServicesRepository servicesRepository)
        {
            _unitOfWork=unitOfWork;
            _servicesRepository=servicesRepository;
        }

        public async Task<List<ServiceDAO>> GetServicesByOrganizationAsync(int organizationID)
        {
            
            using var con= _unitOfWork.BeginConnection();
            var result=await _servicesRepository.GetServicesByOrganizationAsync(organizationID);
            return result;
        }
    }
}
