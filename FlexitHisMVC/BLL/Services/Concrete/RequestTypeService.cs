using Medicloud.BLL.Services.Abstract;
using Medicloud.DAL.Infrastructure.Abstract;
using Medicloud.DAL.Repository.Abstract;
using Medicloud.DAL.Repository.RequestType;
using Medicloud.Models;

namespace Medicloud.BLL.Services.Concrete
{
	public class RequestTypeService : IRequestTypeService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IRequestTypeRepository _typeRepository;

		public RequestTypeService(IUnitOfWork unitOfWork, IRequestTypeRepository typeRepository)
		{
			_unitOfWork = unitOfWork;
			_typeRepository = typeRepository;
		}


		public async Task<List<RequestTypeDAO>> GetRequestTypesAsync(int organizationId = 0)
		{
			using var con = _unitOfWork.BeginConnection();
			var result = await _typeRepository.GetRequestTypes(organizationId==35?organizationId:0);
			return result;
		}

	}
}
