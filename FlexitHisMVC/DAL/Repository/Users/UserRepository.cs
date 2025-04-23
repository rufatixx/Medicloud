using System.Text;
using Dapper;
using Medicloud.DAL.Entities;
using Medicloud.DAL.Infrastructure.Abstract;

namespace Medicloud.DAL.Repository.Users
{
    public class UserRepository : IUserRepository
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public UserDAO GetUser(string content, string pass, int type)
        {
            _unitOfWork.BeginConnection();
            var con = _unitOfWork.GetConnection();

            string column = type == 1 ? "u.mobile" : type == 2 ? "u.email" : null;
            if (string.IsNullOrEmpty(column)) return new UserDAO();

            string query = $@"
                SELECT u.*, up.expire_date AS subscription_expire_date
                FROM users u
                LEFT JOIN user_plans up ON u.id = up.user_id AND up.isActive = 1
                WHERE u.pwd = SHA2(@pass, 256) AND {column} = @content AND u.isActive = 1 AND u.isRegistered = 1";

            return con.Query<UserDAO>(query, new { pass, content }).FirstOrDefault() ?? new UserDAO();
        }

        public async Task<UserDAO> GetUser(string mobileNumber, string pass)
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
            using var con = _unitOfWork.BeginConnection();
            var result = await con.QuerySingleOrDefaultAsync<UserDAO>(query, new { MobileNumber = mobileNumber, Pass = pass });
            return result;
        }



        public bool UserExists(string mobile)
        {
            _unitOfWork.BeginConnection();
            var con = _unitOfWork.GetConnection();

            string query = "SELECT COUNT(1) FROM users WHERE mobile = @mobile";
            int count = con.ExecuteScalar<int>(query, new { mobile });

            return count > 0;
        }

        public List<UserDAO> GetUserList(long organizationID = 0)
        {
            _unitOfWork.BeginConnection();
            var con = _unitOfWork.GetConnection();

            string organizationFilter = organizationID > 0 ? "WHERE uhr.organizationID = @organizationID" : "";
            string query = $@"
                SELECT u.*, s.name AS specialityName
                FROM users u
                LEFT JOIN user_organization_rel uhr ON u.id = uhr.userID
                LEFT JOIN speciality s ON u.specialityID = s.id
                {organizationFilter}
                GROUP BY u.id";

            var result = con.Query<UserDAO, string, UserDAO>(query, (user, specialityName) =>
            {
                user.speciality = new Speciality
                {
                    id = user.specialityID,
                    name = specialityName
                };
                return user;
            }, new { organizationID }, splitOn: "specialityName").ToList();

            return result;
        }

        public List<UserDAO> GetRefererList()
        {
            _unitOfWork.BeginConnection();
            var con = _unitOfWork.GetConnection();

            string query = "SELECT * FROM users WHERE referral = 1";
            return con.Query<UserDAO>(query).ToList();
        }

   

        public UserDAO GetUserByID(int id)
        {
            _unitOfWork.BeginConnection();
            var con = _unitOfWork.GetConnection();

            string query = @"
                SELECT a.*, s.name AS speciality, p.expire_date AS plan_expire_date
                FROM users a
                JOIN speciality s ON a.specialityID = s.id
                LEFT JOIN user_plans p ON a.id = p.user_id AND p.isActive = 1
                WHERE a.id = @id AND a.isActive = 1";

            var result = con.Query<UserDAO, string, DateTime?, UserDAO>(query, (user, speciality, planExpire) =>
            {
                user.speciality = new Speciality { id = user.specialityID, name = speciality };
                user.subscription_expire_date = planExpire;
                return user;
            }, new { id }, splitOn: "speciality,plan_expire_date").FirstOrDefault();

            return result ?? new UserDAO();
        }

        public UserDAO? GetUserByPhone(string mobileNumber)
        {
            _unitOfWork.BeginConnection();
            var con = _unitOfWork.GetConnection();

            string query = "SELECT * FROM users WHERE mobile = @mobile";
            return con.QueryFirstOrDefault<UserDAO>(query, new { mobile = mobileNumber });
        }

        public UserDAO? GetUserByEmail(string email)
        {
            _unitOfWork.BeginConnection();
            var con = _unitOfWork.GetConnection();

            string query = "SELECT * FROM users WHERE email = @email";
            return con.QueryFirstOrDefault<UserDAO>(query, new { email });
        }

        public long InsertUser(string name = "", string surname = "", string father = "", int specialityID = 0,
            string passportSerialNum = "", string fin = "", string phone = "", string email = "", string bDate = "",
            string username = "", string pwd = "", int isUser = 0, int isDr = 0, int isAdmin = 0, int isActive = 0,
            string otp = "", string imagePath = "", int isRegistered = 0)
        {
            try
            {
                _unitOfWork.BeginConnection();
                var con = _unitOfWork.GetConnection();

                string column = !string.IsNullOrEmpty(phone) ? "mobile = @mobile"
                               : !string.IsNullOrEmpty(email) ? "email = @email" : "";

                if (string.IsNullOrEmpty(column)) return 0;

                string query = $@"
                    INSERT INTO users (name, surname, father, specialityID, mobile, email, bDate, username, pwd, 
                                       isUser, isDr, isAdmin, isActive, otp_code, isRegistered, image_path)
                    SELECT @name, @surname, @father, @specialityID, @mobile, @email, @bDate, @username, SHA2(@pwd, 256),
                           @isUser, @isDr, @isAdmin, @isActive, @otp_code, @isRegistered, @image_path
                    FROM DUAL
                    WHERE NOT EXISTS (SELECT 1 FROM users WHERE {column})";

                con.Execute(query, new
                {
                    name,
                    surname,
                    father,
                    specialityID,
                    mobile = phone,
                    email,
                    bDate = string.IsNullOrEmpty(bDate) ? (DateTime?)null : DateTime.Parse(bDate),
                    username,
                    pwd,
                    isUser,
                    isDr,
                    isAdmin,
                    isActive,
                    otp_code = otp,
                    isRegistered,
                    image_path = imagePath
                });

                return con.QuerySingle<long>("SELECT LAST_INSERT_ID();");
            }
            catch
            {
                return 0;
            }
        }
        public int UpdateUser(int userID = 0, string name = "", string surname = "", string father = "",
    int specialityID = 0, string passportSerialNum = "", string fin = "",
    string mobile = "", string email = "", string bDate = "", string username = "",
    int isUser = 0, int isDr = 0, int isActive = 0, int isAdmin = 0, string otp = "",
    string recoveryOtp = "", DateTime? recoveryOtpSendDate = null,
    string password = "", int isRegistered = 0, string imagePath = "")
        {
            _unitOfWork.BeginConnection();
            var con = _unitOfWork.GetConnection();

            var query = new StringBuilder("UPDATE users SET ");
            var param = new DynamicParameters();

            void AddField(string field, object value)
            {
                if (value != null)
                {
                    query.Append($"{field} = @{field}, ");
                    param.Add($"@{field}", value);
                }
            }

            AddField("name", string.IsNullOrEmpty(name) ? null : name);
            AddField("surname", string.IsNullOrEmpty(surname) ? null : surname);
            AddField("father", string.IsNullOrEmpty(father) ? null : father);
            AddField("mobile", string.IsNullOrEmpty(mobile) ? null : mobile);
            AddField("pwd", string.IsNullOrEmpty(password) ? null : password);
            AddField("email", string.IsNullOrEmpty(email) ? null : email);
            AddField("bDate", string.IsNullOrEmpty(bDate) ? null : DateTime.Parse(bDate));
            AddField("username", string.IsNullOrEmpty(username) ? null : username);
            AddField("passportSerialNum", string.IsNullOrEmpty(passportSerialNum) ? null : passportSerialNum);
            AddField("fin", string.IsNullOrEmpty(fin) ? null : fin);
            AddField("specialityID", specialityID != 0 ? specialityID : null);
            AddField("isUser", isUser != 0 ? isUser : null);
            AddField("isActive", isActive != 0 ? isActive : null);
            AddField("isDr", isDr != 0 ? isDr : null);
            AddField("isAdmin", isAdmin != 0 ? isAdmin : null);
            AddField("otp_code", string.IsNullOrEmpty(otp) ? null : otp);
            AddField("recovery_otp", string.IsNullOrEmpty(recoveryOtp) ? null : recoveryOtp);
            AddField("recovery_otp_send_date", recoveryOtpSendDate);
            AddField("isRegistered", isRegistered > 0 ? isRegistered : null);
            AddField("image_path", string.IsNullOrEmpty(imagePath) ? null : imagePath);
            AddField("otp_sent_date", DateTime.Now);

            // Remove last comma and space
            if (param.ParameterNames.Any())
            {
                query.Length -= 2;
            }

            query.Append(" WHERE id = @userID");
            param.Add("@userID", userID);

            return con.Execute(query.ToString(), param);
        }


        public int UpdateUserPwd(int userID, string pwd)
        {
            _unitOfWork.BeginConnection();
            var con = _unitOfWork.GetConnection();

            string query = "UPDATE users SET pwd = SHA2(@pwd, 256) WHERE id = @userID";
            return con.Execute(query, new { userID, pwd });
        }

        public int UpdateUserExpireDate(int userId, DateTime expireDate)
        {
            _unitOfWork.BeginConnection();
            var con = _unitOfWork.GetConnection();

            string query = "UPDATE users SET subscription_expire_date = @expireDate WHERE id = @id";
            return con.Execute(query, new { expireDate, id = userId });
        }

        public string GetOtpData(string content, int type)
        {
            _unitOfWork.BeginConnection();
            var con = _unitOfWork.GetConnection();

            string column = type == 1 ? "mobile" : type == 2 ? "email" : null;
            if (string.IsNullOrEmpty(column)) return "";

            string query = $"SELECT otp_code FROM users WHERE {column} = @content";
            return con.QueryFirstOrDefault<string>(query, new { content }) ?? "";
        }

        public string GetRecoveryOtpData(string content, int type)
        {
            _unitOfWork.BeginConnection();
            var con = _unitOfWork.GetConnection();

            string column = type == 1 ? "mobile" : type == 2 ? "email" : null;
            if (string.IsNullOrEmpty(column)) return "";

            string query = $"SELECT recovery_otp FROM users WHERE {column} = @content";
            return con.QueryFirstOrDefault<string>(query, new { content }) ?? "";
        }

		public async Task<UserDAO> GetOnlyUserById(int id)
		{
			var con = _unitOfWork.GetConnection();

			string query = @"
                SELECT a.*
                FROM users a
                WHERE a.id = @id AND a.isActive = 1";

			var result = await con.QuerySingleOrDefaultAsync<UserDAO>(query, new { id });

			return result ?? new UserDAO();
		}
	}
}
