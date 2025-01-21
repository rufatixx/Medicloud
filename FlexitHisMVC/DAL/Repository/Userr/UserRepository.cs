using Dapper;
using Medicloud.BLL.Models;
using Medicloud.DAL.Infrastructure.Abstract;
using Medicloud.Models;
using Microsoft.Extensions.FileSystemGlobbing;
using MySql.Data.MySqlClient;
using System.Reflection;
using System;
using System.Text;
using static System.Net.WebRequestMethods;
using System.Xml.Linq;
using Medicloud.DAL.DAO;

namespace Medicloud.DAL.Repository.Userr
{
	public class UserRepository:IUserRepository
	{
		private readonly IUnitOfWork _unitOfWork;

		public UserRepository(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

        public async Task<int> AddUser(UserDAO dao)
        {
            string AddSql = $@"
			INSERT INTO users
            (name,surname,father,username,bDate,mobile,email,passportSerialNum,fin,pwd,image_path)
			VALUES (@{nameof(UserDAO.name)},
            @{nameof(UserDAO.surname)},
            @{nameof(UserDAO.father)},
            @{nameof(UserDAO.username)},
            @{nameof(UserDAO.bDate)},
            @{nameof(UserDAO.mobile)},
            @{nameof(UserDAO.email)},
            @{nameof(UserDAO.passportSerialNum)},
            @{nameof(UserDAO.fin)},
            @{nameof(UserDAO.pwd)},
            @{nameof(UserDAO.imagePath)});

			SELECT LAST_INSERT_ID();";
            var con = _unitOfWork.GetConnection();
            var newUserId = await con.QuerySingleOrDefaultAsync<int>(AddSql, dao);
            return newUserId;
        }

		public async Task<User> GetUser(string mobileNumber, string pass)
		{
			string query = @"SELECT 
		    u.*
			FROM 
			    users u
			WHERE 
			    u.pwd = SHA2(@Pass, 256) 
				AND u.mobile = @MobileNumber 
				AND u.isActive = 1 
				AND u.isRegistered = 1;
		";
			using var con = _unitOfWork.BeginConnection();
			var result = await con.QuerySingleOrDefaultAsync<User>(query, new { MobileNumber = mobileNumber, Pass = pass });
			return result;

		}

		public async Task<User> GetUserById(string mobileNumber, string pass)
		{
			string query = @"SELECT 
		    u.*
			FROM 
			    users u
			WHERE 
			    u.pwd = SHA2(@Pass, 256) 
				AND u.mobile = @MobileNumber 
				AND u.isActive = 1 
				AND u.isRegistered = 1;
		";
			using var con = _unitOfWork.BeginConnection();
			var result = await con.QuerySingleOrDefaultAsync<User>(query, new { MobileNumber = mobileNumber, Pass = pass });
			return result;
		}

        public async Task<UserDAO> GetUserById(int id)
        {
            string query = @"SELECT 
		    u.*
			FROM 
			    users u
			WHERE 
				u.id = @Id 
				AND u.isActive = 1 
				AND u.isRegistered = 1;
		";
            var con = _unitOfWork.GetConnection();
            var result = await con.QuerySingleOrDefaultAsync<UserDAO>(query, new { Id=id});
            return result;
        }

        public async Task<int> GetUserIdByPhoneNumber(string mobileNumber)
		{
			string query = @"SELECT 
		    u.id
			FROM 
			    users u
			WHERE 
				u.mobile = @MobileNumber;
		";
			var con = _unitOfWork.GetConnection();
			var result = await con.QuerySingleOrDefaultAsync<int>(query, new { MobileNumber = mobileNumber });
			return result;
		}

        public async Task<int> GetUserIdByEmail(string email)
        {
            string query = @"SELECT 
		    u.id
			FROM 
			    users u
			WHERE 
				u.email = @Email;
		";
            var con = _unitOfWork.GetConnection();
            var result = await con.QuerySingleOrDefaultAsync<int>(query, new { Email = email });
            return result;
        }

        public async Task<int> UpdateUserAsync(UpdateUserDTO userDTO)
		{

			var query = new StringBuilder("UPDATE users SET ");
			var parameters = new DynamicParameters();

			if (!string.IsNullOrEmpty(userDTO.name))
			{
				query.Append("name = @name, ");
				parameters.Add("@name", userDTO.name);
			}
			if (!string.IsNullOrEmpty(userDTO.surname))
			{
				query.Append("surname = @surname, ");
				parameters.Add("@surname", userDTO.surname);
			}
			if (!string.IsNullOrEmpty(userDTO.father))
			{
				query.Append("father = @father, ");
				parameters.Add("@father", userDTO.father);
			}
			if (!string.IsNullOrEmpty(userDTO.mobile))
			{
				query.Append("mobile = @mobile, ");
				parameters.Add("@mobile", userDTO.mobile);
			}
			if (!string.IsNullOrEmpty(userDTO.pwd))
			{
				query.Append("pwd = @pwd, ");
				parameters.Add("@pwd", userDTO.pwd);
			}
			if (!string.IsNullOrEmpty(userDTO.email))
			{
				query.Append("email = @email, ");
				parameters.Add("@email", userDTO.email);
			}
			if (!string.IsNullOrEmpty(userDTO.bDate))
			{
				query.Append("bDate = @bDate, ");
				parameters.Add("@bDate", string.IsNullOrEmpty(userDTO.bDate) ? (object)DBNull.Value : DateTime.Parse(userDTO.bDate));
			}
			if (!string.IsNullOrEmpty(userDTO.username))
			{
				query.Append("username = @username, ");
				parameters.Add("@username", userDTO.username);
			}
			if (!string.IsNullOrEmpty(userDTO.passportSerialNum))
			{
				query.Append("passportSerialNum = @passportSerialNum, ");
				parameters.Add("@passportSerialNum", userDTO.passportSerialNum);
			}
			if (!string.IsNullOrEmpty(userDTO.fin))
			{
				query.Append("fin = @fin, ");
				parameters.Add("@fin", userDTO.fin);
			}
			if (userDTO.isRegistered)
			{
				query.Append("isRegistered = @isRegistered, ");
				parameters.Add("@isRegistered", userDTO.isRegistered);
			}
			if (!string.IsNullOrEmpty(userDTO.imagePath))
			{
				query.Append("image_path = @imagePath, ");
				parameters.Add("@imagePath", userDTO.imagePath);
			}
			query.Append("otp_sent_date = @otp_sent_date, ");
			parameters.Add("@otp_sent_date", DateTime.Now);


			// Remove the last comma and space
			if (parameters.ParameterNames.Count() > 0)
			{
				query.Length -= 2;
			}

			query.Append(" WHERE id = @userID");
			parameters.Add("@userID", userDTO.ID);
			using var con = _unitOfWork.BeginConnection();
			await con.ExecuteAsync(query.ToString(),parameters);
			return userDTO.ID;
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
