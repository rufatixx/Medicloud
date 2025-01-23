

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

		public async Task<List<CategoryDAO>> GetAll()
		{
			using var con =  _unitOfWork.BeginConnection();
			return await _categoryRepository.GetAll();
		}
	}
}
