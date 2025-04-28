using Medicloud.Models;

namespace Medicloud.BLL.Services.Abstract
{
    public interface IRequestTypeService
    {
        Task<List<RequestTypeDAO>> GetRequestTypesAsync(int organizationId=0);
    }
}
