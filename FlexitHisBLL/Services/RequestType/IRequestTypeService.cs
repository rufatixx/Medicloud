
using Medicloud.DAL.DAO;

namespace Medicloud.BLL.Services.RequestType
{
    public interface IRequestTypeService
    {
        Task<List<RequestTypeDAO>> GetRequestTypesAsync();
    }
}
