using Medicloud.DAL.DAO;

namespace Medicloud.DAL.Repository.Services
{
    public interface IServicesRepository
    {
        Task<List<ServiceDAO>> GetServicesByOrganizationAsync(int organizationID);
    }
}
