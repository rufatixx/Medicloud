using Dapper;
using Medicloud.DAL.DAO;
using Medicloud.DAL.Infrastructure.UnitOfWork;


namespace Medicloud.DAL.Repository.File
{
	public class FileRepository:IFileRepository
	{
		private readonly IUnitOfWork _unitOfWork;

		public FileRepository(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<int> AddFileAsync(FileDAO fileDao)
		{
			string AddSql = $@"
			INSERT INTO files
            (fileName,filePath)
			VALUES (@{nameof(FileDAO.fileName)},
            @{nameof(FileDAO.filePath)});

			SELECT LAST_INSERT_ID();";
			var con = _unitOfWork.GetConnection();
			var transaction = _unitOfWork.GetTransaction();
			int newFileId;
			if (transaction != null)
			{
				newFileId = await con.QuerySingleAsync<int>(AddSql, fileDao, transaction);
			}
			else
			{
				newFileId = await con.QuerySingleAsync<int>(AddSql, fileDao);
			}
			return newFileId;

		}

		public async Task DeleteFileAsync(int id)
		{
			string _removeFileSql = $@"UPDATE files SET isActive=0 WHERE id=@Id;";
			var con = _unitOfWork.GetConnection();
			var transaction = _unitOfWork.GetTransaction();
			if (transaction != null)
			{
				await con.ExecuteAsync(_removeFileSql, new { Id = id }, transaction);
			}
			else
			{
				await con.ExecuteAsync(_removeFileSql, new { Id = id });
			}
		}

		public async Task<FileDAO> GetFileByIdAsync(int id)
		{
			string sql = "SELECT * FROM files WHERE id=@Id";
			var con = _unitOfWork.GetConnection();
			var transaction = _unitOfWork.GetTransaction();
			FileDAO fileDAO = null;
			if (transaction != null)
			{
				fileDAO = await con.QuerySingleOrDefaultAsync<FileDAO>(sql, new { Id = id }, transaction);
			}
			else
			{
				fileDAO = await con.QuerySingleOrDefaultAsync<FileDAO>(sql, new { Id = id });
			}

			return fileDAO;
		}
	}
}
