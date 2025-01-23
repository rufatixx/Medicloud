using MySql.Data.MySqlClient;

namespace Medicloud.DAL.Infrastructure.UnitOfWork
{
	public class UnitOfWork : IUnitOfWork
	{
		private bool disposed = false;

		private MySqlTransaction sqlTransaction;
		private MySqlConnection sqlConnection;
		private readonly string _connectionString;
		public UnitOfWork(string connectionString)
		{
			_connectionString = connectionString;
		}

		public MySqlTransaction BeginTransaction()
		{
			EnsureConnectionOpen();
			sqlTransaction = sqlConnection.BeginTransaction();
			return sqlTransaction;
		}
		public MySqlConnection BeginConnection()
		{
			EnsureConnectionOpen();
			return sqlConnection;
		}

		private void EnsureConnectionOpen()
		{
			if (sqlConnection == null || sqlConnection.State == System.Data.ConnectionState.Closed)
			{
				sqlConnection = new MySqlConnection(_connectionString);
				sqlConnection.Open();
			}
			else if (sqlConnection.State != System.Data.ConnectionState.Open)
			{
				sqlConnection.Open();
			}
		}

		public MySqlConnection GetConnection() => sqlConnection;
		public MySqlTransaction GetTransaction() => sqlTransaction;

		public void Dispose()
		{
			Dispose(true);
			//GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					sqlTransaction?.Dispose();
					sqlTransaction = null;
				}

				// Release unmanaged resources.
				if (sqlConnection?.State == System.Data.ConnectionState.Open)
				{
					sqlConnection?.Close();
				}
				sqlConnection?.Dispose();
				disposed = true;
			}
		}
		public void SaveChanges()
		{
			sqlTransaction.Commit();
			sqlConnection.Close();
			sqlTransaction = null;
		}
		~UnitOfWork() { Dispose(false); }
	}
}
