using Medicloud.Models;

namespace Medicloud.DAL.Repository.Abstract
{
    public interface IRequestTypeRepository
    {
        Task<List<RequestType>> GetRequestTypesAsync();
    }
}
