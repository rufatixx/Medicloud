using Medicloud.BLL.DTO;
using Medicloud.BLL.Services.FileUpload;
using Medicloud.DAL.DAO;
using Medicloud.DAL.Infrastructure.UnitOfWork;
using Medicloud.DAL.Repository.Category;
using Medicloud.DAL.Repository.File;
using Medicloud.DAL.Repository.Portfolio;

namespace Medicloud.BLL.Services.Portfolio
{
	public class PortfolioService:IPortfolioService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IPortfolioRepository _portfolioRepository;
		private readonly IFileUploadService _fileUploadService;
		private readonly IFileRepository _fileRepository;
		private readonly ICategoryRepository _categoryRepository;
		public PortfolioService(IUnitOfWork unitOfWork, IPortfolioRepository portfolioRepository, IFileUploadService fileUploadService, IFileRepository fileRepository, ICategoryRepository categoryRepository)
		{
			_unitOfWork = unitOfWork;
			_portfolioRepository = portfolioRepository;
			_fileUploadService = fileUploadService;
			_fileRepository = fileRepository;
			_categoryRepository = categoryRepository;
		}

		public async Task<int> AddPortfolioAsync(PortfolioDTO dto)
		{
			using var con= _unitOfWork.BeginConnection();
			int newFileId = await _fileRepository.AddFileAsync(new()
			{
				fileName = dto.file.fileName,
				filePath = dto.file.filePath,
			});
			var dao = new PortfolioDAO
			{
				description = dto.description,
				fileId = newFileId,
				organizationId = dto.organizationId,
				categoryIds = dto.categoryIds,
			};
			return await _portfolioRepository.AddAsync(dao);
		}

		public async Task<List<PortfolioDTO>> GetPortfolioByOrganizationIdAsync(int organizationId)
		{
			using var con= _unitOfWork.BeginConnection();
			var data = await _portfolioRepository.GetByOrganizationIdAsync(organizationId);

			var result= new List<PortfolioDTO>();
			if(data != null)
			{
				foreach( var item in data)
				{
					var file=await _fileRepository.GetFileByIdAsync(item.fileId);
					//var fileData =await _fileUploadService.DownloadFile(file.filePath);
					//var fileExtension=Path.GetExtension(file.filePath);
					var fileDTO = new FileDTO() {
						fileName = file.fileName,
						filePath = file.filePath,
						id = file.id,
						//Src = $"data:image/{fileExtension};base64,{Convert.ToBase64String(fileData)}"
					};

					var dto = new PortfolioDTO
					{
						fileId = item.fileId,
						file=fileDTO,
						description = item.description,
						id = item.id,
						
					};
					result.Add(dto);
				}
			}
			return result;
		}

		public async Task UpdateAsync(PortfolioDTO dto)
		{
			using var con = _unitOfWork.BeginConnection();
			var exist = await _portfolioRepository.GetByIdAsync(dto.id);
			if (dto.description != exist.description)
			{
				await _portfolioRepository.UpdateAsync(new()
				{
					id = dto.id,
					description = dto.description,
				});
			}
			if(dto.categoryIds != exist.categoryIds)
			{
				var existingCategoryIds = exist.categoryIds.Split(',').Select(int.Parse).ToList(); 
				var newCategoryIds = dto.categoryIds.Split(',').Select(int.Parse).ToList();

				var categoriesToRemove = existingCategoryIds.Except(newCategoryIds).ToList(); 
				var categoriesToAdd = newCategoryIds.Except(existingCategoryIds).ToList();
				await _portfolioRepository.RemoveCategoriesFromPortfolio(dto.id, categoriesToRemove);

				foreach (var categoryId in categoriesToAdd)
				{
					await _portfolioRepository.AddCategoryToPortfolio(dto.id, categoryId); 
				}
			}

		}

		public async Task<PortfolioDTO> GetPortfolioByIdAsync(int id)
		{
			using var con=_unitOfWork.BeginConnection();
			var data = await _portfolioRepository.GetByIdAsync(id);
			var result = new PortfolioDTO();
			if(data != null)
			{
				result.description = data.description;
				result.categoryIds = data.categoryIds;
				result.organizationId = data.organizationId;
				result.id = data.id;
				result.fileId = data.fileId;
			}
			return result;
		}

		public async Task DeleteAsync(int id)
		{
			using var con = _unitOfWork.BeginConnection();

			await _portfolioRepository.DeleteAsync(id);
		}
	}
}
