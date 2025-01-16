using Dapper;
using Medicloud.DAL.Infrastructure.Abstract;
using Medicloud.Models;
using MySql.Data.MySqlClient;

namespace Medicloud.DAL.Repository.Userr
{
	public class UserRepository:IUserRepository
	{
		private readonly IUnitOfWork _unitOfWork;

		public UserRepository(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<User> GetUser(string mobileNumber, string pass)
		{
			string query = @"SELECT 
		    u.*, 
		    up.expire_date as subscription_expire_date
			FROM 
			    users u
			WHERE 
			    u.pwd = SHA2(@Pass, 256) 
				AND u.mobile = @MobileNumber 
				AND u.isActive = 1 
				AND u.isRegistered = 1;
		";
			using var con=_unitOfWork.BeginConnection();
			var result=await con.QuerySingleOrDefaultAsync<User>(query, new { MobileNumber =mobileNumber,Pass=pass});
			return result;
		}
		//		public User GetUser(string mobileNumber, string pass)
		//		{
		//			User user = new User();
		//			try
		//			{
		//				using (MySqlConnection connection = new MySqlConnection(_connectionString))
		//				{

		//					connection.Open();
		//					using (MySqlCommand com = new MySqlCommand(@"SELECT 
		//    u.*, 
		//    up.expire_date as subscription_expire_date
		//FROM 
		//    users u
		//left JOIN 
		//    user_plans up ON u.id = up.user_id AND up.isActive = 1
		//WHERE 
		//    u.pwd = SHA2(@pass, 256) 
		//    AND u.mobile = @mobileNumber 
		//    AND u.isActive = 1 
		//    AND u.isUser = 1 
		//    AND u.isRegistered = 1;
		//", connection))
		//					{

		//						com.Parameters.AddWithValue("@pass", pass);
		//						com.Parameters.AddWithValue("@mobileNumber", mobileNumber);
		//						using (MySqlDataReader reader = com.ExecuteReader())
		//						{
		//							if (reader.HasRows)
		//							{


		//								while (reader.Read())
		//								{
		//									user.ID = Convert.ToInt32(reader["id"]);
		//									user.isActive = Convert.ToBoolean(reader["isActive"]);

		//									user.name = reader["name"].ToString();
		//									user.surname = reader["surname"].ToString();
		//									user.isAdmin = reader["isAdmin"] == DBNull.Value ? false : Convert.ToBoolean(reader["isAdmin"]);
		//									user.isManager = reader["isManager"] == DBNull.Value ? false : Convert.ToBoolean(reader["isManager"]);
		//									user.isUser = reader["isUser"] == DBNull.Value ? false : Convert.ToBoolean(reader["isUser"]);
		//									user.isDr = reader["isDr"] == DBNull.Value ? false : Convert.ToBoolean(reader["isDr"]);
		//									user.subscription_expire_date = reader["subscription_expire_date"] == DBNull.Value ? null : Convert.ToDateTime(reader["subscription_expire_date"]);
		//								}

		//								connection.Close();





		//							}

		//						}

		//					}
		//					connection.Close();
		//				}
		//			}
		//			catch (Exception ex)
		//			{

		//				Console.WriteLine(ex.Message);
		//			}

		//			return user;
		//		}

	}
}
