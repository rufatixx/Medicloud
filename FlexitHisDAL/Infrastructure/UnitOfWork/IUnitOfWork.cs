using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medicloud.DAL.Infrastructure.UnitOfWork
{
	public interface IUnitOfWork : IDisposable
	{
		MySqlTransaction BeginTransaction();
		MySqlConnection BeginConnection();
		MySqlConnection GetConnection();
		MySqlTransaction GetTransaction();
		void SaveChanges();
	}
}
