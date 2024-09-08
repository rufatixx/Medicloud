using Medicloud.BLL.Services.Abstract;
using Medicloud.DAL.Infrastructure.Abstract;
using Medicloud.DAL.Repository.Abstract;
using Medicloud.Models;

namespace Medicloud.BLL.Services.Concrete
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

        public async Task<List<ServiceObj>> GetServicesByOrganizationAsync(int organizationID)
        {
            
            using var con= _unitOfWork.BeginConnection();
            var result=await _servicesRepository.GetServicesByOrganizationAsync(organizationID);
            return result;
        }
    }
}
