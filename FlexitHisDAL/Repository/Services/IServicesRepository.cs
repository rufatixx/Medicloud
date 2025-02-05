using Medicloud.DAL.DAO;

namespace Medicloud.DAL.Repository.Services
{
    public interface IServicesRepository
    {
        Task<List<ServiceDAO>> GetServicesByOrganizationAsync(int organizationID);
		Task<int> AddServiceAsync(ServiceDAO dao);
		Task<bool> UpdateServiceAsync(ServiceDAO dao);
		Task<ServiceDAO> GetServiceByIdAsync(int serviceId);

	}
}
