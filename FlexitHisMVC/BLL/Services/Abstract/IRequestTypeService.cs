using Medicloud.Models;

namespace Medicloud.BLL.Services.Abstract
{
    public interface IRequestTypeService
    {
        Task<List<RequestType>> GetRequestTypesAsync();
    }
}
