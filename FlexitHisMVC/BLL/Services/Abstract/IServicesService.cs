using Medicloud.Models;

namespace Medicloud.BLL.Services.Abstract
{
    public interface IServicesService
    {
        Task<List<ServiceObj>> GetServicesByOrganizationAsync(int organizationID);
    }
}
