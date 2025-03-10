using Medicloud.BLL.DTO;
using Medicloud.DAL.Infrastructure.UnitOfWork;
using Medicloud.DAL.Repository.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medicloud.BLL.Services.File
{
	public class FileService:IFileService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IFileRepository _fileRepository;

		public FileService(IUnitOfWork unitOfWork, IFileRepository fileRepository)
		{
			_unitOfWork = unitOfWork;
			_fileRepository = fileRepository;
		}

		public async Task<FileDTO> GetById(int id)
		{
			using var con=_unitOfWork.BeginConnection();
			var result=await _fileRepository.GetFileByIdAsync(id);
			return new()
			{
				id = result.id,
				fileName = result.fileName,
				filePath = result.filePath,
				isActive = result.isActive
			};
		}

		public async Task DeleteById(int id)
		{
			using var con= _unitOfWork.BeginConnection();
			await _fileRepository.DeleteFileAsync(id);
		}
	}
}
