using Medicloud.Models;

namespace Medicloud.DAL.Repository.Abstract
{
    public interface IServicesRepository
    {
        Task<List<ServiceObj>> GetServicesByOrganizationAsync(int organizationID);
    }
}
