using Medicloud.DAL.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medicloud.DAL.Repository.File
{
	public interface IFileRepository
	{
		Task<int> AddFileAsync(FileDAO fileDao);
		Task DeleteFileAsync(int id);
		Task<FileDAO> GetFileByIdAsync(int id);
	}
}
