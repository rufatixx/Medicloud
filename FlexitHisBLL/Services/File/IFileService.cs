using Medicloud.BLL.DTO;

namespace Medicloud.BLL.Services.File
{
	public interface IFileService
	{
		Task<FileDTO> GetById(int id);
		Task<int> AddAsync(FileDTO dto);
		Task DeleteById(int id);
	}
}
