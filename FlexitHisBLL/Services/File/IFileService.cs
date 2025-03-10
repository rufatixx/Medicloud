using Medicloud.BLL.DTO;

namespace Medicloud.BLL.Services.File
{
	public interface IFileService
	{
		Task<FileDTO> GetById(int id);
		Task DeleteById(int id);
	}
}
