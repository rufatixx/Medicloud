

using Medicloud.BLL.DTO;
using Medicloud.DAL.DAO;
using Medicloud.DAL.Infrastructure.UnitOfWork;
using Medicloud.DAL.Repository.Category;

namespace Medicloud.BLL.Services.Category
{
    public class CategoryService:ICategoryService
    {
		private readonly IUnitOfWork _unitOfWork;
		private readonly ICategoryRepository _categoryRepository;

		public CategoryService(IUnitOfWork unitOfWork, ICategoryRepository categoryRepository)
		{
			_unitOfWork = unitOfWork;
			_categoryRepository = categoryRepository;
		}

		public async Task<List<TempDTO>> GetAll()
		{
			using var con =  _unitOfWork.BeginConnection();
			var data =await _categoryRepository.GetAll();
			var result=new List<TempDTO>();
			if(data != null)
			{
				foreach(var item in data)
				{
					result.Add(new () 
					{ 
						id = item.id,
						name = item.name 
					});
				}
			}
			return result;
		}

		public async Task<List<TempDTO>> GetByOrganizationId(int organizationId)
		{
			using var con = _unitOfWork.BeginConnection();
			var data= await _categoryRepository.GetByOrganizationId(organizationId);
			var result = new List<TempDTO>();
			if (data != null)
			{
				foreach (var item in data)
				{
					result.Add(new()
					{
						id = item.id,
						name = item.name
					});
				}
			}
			return result;
		}
	}
}
