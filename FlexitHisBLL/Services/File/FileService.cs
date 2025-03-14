using Medicloud.BLL.DTO;
using Medicloud.BLL.Services.FileUpload;
using Medicloud.DAL.DAO;
using Medicloud.DAL.Infrastructure.UnitOfWork;
using Medicloud.DAL.Repository.File;
namespace Medicloud.BLL.Services.File
{
	public class FileService:IFileService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IFileRepository _fileRepository;
		private readonly IFileUploadService _fileUploadService;
		public FileService(IUnitOfWork unitOfWork, IFileRepository fileRepository, IFileUploadService fileUploadService)
		{
			_unitOfWork = unitOfWork;
			_fileRepository = fileRepository;
			_fileUploadService = fileUploadService;
		}

		public async Task<FileDTO> GetById(int id)
		{
			using var con=_unitOfWork.BeginConnection();
			var result=await _fileRepository.GetFileByIdAsync(id);
			return new()
			{
				id = result?.id??0,
				fileName = result?.fileName,
				filePath = result?.filePath,
				isActive = result?.isActive??false
			};
		}

		public async Task DeleteById(int id)
		{
			using var con= _unitOfWork.BeginConnection();
			var file=await _fileRepository.GetFileByIdAsync(id);
			if (file!=null)
			{
				bool isDeleted=_fileUploadService.DeleteFile(file.filePath);
				if (isDeleted)
				{
					await _fileRepository.DeleteFileAsync(id);

				}

			}
		}

		public async Task<int> AddAsync(FileDTO dto)
		{
			var dao = new FileDAO
			{
				fileName = dto.fileName,
				filePath = dto.filePath
			};
			using var con = _unitOfWork.BeginConnection();
			return await _fileRepository.AddFileAsync(dao);
		}
	}
}
