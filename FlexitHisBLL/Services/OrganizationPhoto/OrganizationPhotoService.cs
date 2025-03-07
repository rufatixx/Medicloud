using Medicloud.BLL.DTO;
using Medicloud.DAL.Infrastructure.UnitOfWork;
using Medicloud.DAL.Repository.File;
using Medicloud.DAL.Repository.OrganizationPhoto;

using System.Threading.Tasks;

namespace Medicloud.BLL.Services.OrganizationPhoto
{
	public class OrganizationPhotoService:IOrganizationPhotoService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IOrganizationPhotoRepository _organizationPhotoRepository;
		private readonly IFileRepository _fileRepository;
		public OrganizationPhotoService(IOrganizationPhotoRepository organizationPhotoRepository, IUnitOfWork unitOfWork, IFileRepository fileRepository)
		{
			_organizationPhotoRepository = organizationPhotoRepository;
			_unitOfWork = unitOfWork;
			_fileRepository = fileRepository;
		}

		public async Task<int> AddAsync(int organizationId, FileDTO dto)
		{
			using var con=_unitOfWork.BeginConnection();
			int fileId=await _fileRepository.AddFileAsync(new()
			{
				fileName = dto.fileName,
				filePath = dto.filePath
			});
			return await _organizationPhotoRepository.AddAsync(organizationId, fileId);
		}

		public async Task<List<FileDTO>> GetByOrganizationId(int organizationId)
		{
			using var con = _unitOfWork.BeginConnection();

			var data =await _organizationPhotoRepository.GetByOrganizationId(organizationId);

			if (data == null) return null;
			var result = new List<FileDTO>();
			foreach ( var item in data)
			{
				result.Add(new FileDTO()
				{
					fileName = item.fileName,
					filePath = item.filePath,
					isActive = item.isActive,
					id = item.id
				});
			}
			return result;
		}

		public Task RemoveAsync(int id)
		{
			throw new NotImplementedException();
		}
	}
}
