using Medicloud.BLL.DTO;
using Medicloud.DAL.DAO;

namespace Medicloud.BLL.Services.Services
{
    public interface IServicesService
    {
        Task<List<ServiceDAO>> GetServicesByOrganizationAsync(int organizationID);
		Task<GetServiceTypesDTO> GetServiceTypes();
		Task<int> AddServiceAsync(AddServiceDTO dto);
		Task<bool> RemoveServiceFromOrg(int organizationId,int serviceId);
		Task<ServiceDAO> GetServiceById(int serviceId);
		Task<bool> UpdateService(AddServiceDTO dto);

	}
}
