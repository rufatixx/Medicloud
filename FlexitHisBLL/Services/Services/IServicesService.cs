using Medicloud.DAL.DAO;

namespace Medicloud.BLL.Services.Services
{
    public interface IServicesService
    {
        Task<List<ServiceDAO>> GetServicesByOrganizationAsync(int organizationID);
    }
}
