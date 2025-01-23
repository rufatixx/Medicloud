using Medicloud.DAL.DAO;

namespace Medicloud.DAL.Repository.RequestType
{
    public interface IRequestTypeRepository
    {
        Task<List<RequestTypeDAO>> GetRequestTypesAsync();
    }
}
