using Medicloud.DAL.DAO;
using Medicloud.DAL.Infrastructure.UnitOfWork;
using Medicloud.DAL.Repository.RequestType;

namespace Medicloud.BLL.Services.RequestType
{
    public class RequestTypeService : IRequestTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRequestTypeRepository _typeRepository;

        public RequestTypeService(IUnitOfWork unitOfWork, IRequestTypeRepository typeRepository)
        {
            _unitOfWork=unitOfWork;
            _typeRepository=typeRepository;
        }

        public async Task<List<RequestTypeDAO>> GetRequestTypesAsync()
        {
            using var con = _unitOfWork.BeginConnection();
            var result=await _typeRepository.GetRequestTypesAsync();
            return result;
        }
    }
}
