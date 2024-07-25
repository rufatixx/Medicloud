using System.Text;
using Medicloud.Models;
using MySql.Data.MySqlClient;

namespace Medicloud.DAL.Repository
{
    public class UserRepo
    {
        private readonly string _connectionString;

        public UserRepo(string conString)
        {
            _connectionString = conString;
        }

        public bool UserExists(string mobile)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                using (MySqlCommand com = new MySqlCommand("select count(1) from user where mobile=@mobile", connection))
                {
                    com.Parameters.AddWithValue("@mobile", mobile);
                    var count = Convert.ToInt32(com.ExecuteScalar());
                    com.Dispose();
                    return count > 0;
                }
            }
        }


        public List<User> GetUserList(long organizationID = 0)
        {
            List<User> personalList = new List<User>();

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {

                connection.Open();
                string organizationQuery = ""; // Initialize the organization query string as empty

                // Check if organizationID is greater than 0 and prepare the WHERE clause
                if (organizationID > 0)
                {
                    organizationQuery = " WHERE uhr.organizationID = @organizationID";
                }

                // Build the SQL query string dynamically, including the WHERE clause if applicable
                string sqlQuery = @$"SELECT u.*, s.name AS specialityName
                    FROM users u
                    JOIN medicloud.user_organization_rel uhr ON u.id = uhr.userID
                    LEFT JOIN speciality s ON u.specialityID = s.id {organizationQuery}";


                using (MySqlCommand com = new MySqlCommand(sqlQuery, connection))
                {
                    // Add the organizationID parameter only if it's greater than 0
                    if (organizationID > 0)
                    {
                        com.Parameters.AddWithValue("@organizationID", organizationID);
                    }

                    MySqlDataReader reader = com.ExecuteReader();
                    // Process the reader as usual

                    if (reader.HasRows)
                    {


                        while (reader.Read())
                        {

                            User personal = new User();
                            personal.ID = Convert.ToInt32(reader["id"]);
                            personal.depID = reader["departmentID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["departmentID"]);
                            personal.name = reader["name"] == DBNull.Value ? "" : reader["name"].ToString();
                            personal.surname = reader["surname"] == DBNull.Value ? "" : reader["surname"].ToString();
                            personal.father = reader["father"] == DBNull.Value ? "" : reader["father"].ToString();
                            personal.mobile = reader["mobile"] == DBNull.Value ? "" : reader["mobile"].ToString();
                            personal.username = reader["username"] == DBNull.Value ? "" : reader["username"].ToString();
                            personal.email = reader["email"] == DBNull.Value ? "" : reader["email"].ToString();
                            personal.passportSerialNum = reader["passportSerialNum"] == DBNull.Value ? "" : reader["passportSerialNum"].ToString();
                            personal.fin = reader["fin"] == DBNull.Value ? "" : reader["fin"].ToString();

                            personal.bDate = reader["bDate"] == DBNull.Value ? DateTime.Now.Date.ToString("yyyy-MM-dd") : Convert.ToDateTime(reader["bDate"]).Date.ToString("yyyy-MM-dd");
                            personal.speciality = new Speciality
                            {
                                id = Convert.ToInt64(reader["specialityID"]),
                                name = reader["specialityName"] == DBNull.Value ? "" : reader["specialityName"].ToString()
                            };
                            personal.isActive = reader["isActive"] == DBNull.Value ? false : Convert.ToBoolean(reader["isActive"]);
                            personal.isUser = reader["isUser"] == DBNull.Value ? false : Convert.ToBoolean(reader["isUser"]);
                            personal.isDr = reader["isDr"] == DBNull.Value ? false : Convert.ToBoolean(reader["isDr"]);
                            personal.isAdmin = reader["isAdmin"] == DBNull.Value ? false : Convert.ToBoolean(reader["isAdmin"]);

                            personalList.Add(personal);


                        }



                    }

                }
                connection.Close();
            }
            return personalList;
        }

        public List<User> GetRefererList()
        {
            List<User> refererList = new List<User>();

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {

                connection.Open();
                using (MySqlCommand com = new MySqlCommand("SELECT * FROM users where referral = 1;", connection))
                {

                    MySqlDataReader reader = com.ExecuteReader();
                    if (reader.HasRows)
                    {


                        while (reader.Read())
                        {

                            User referralPersonal = new User();
                            referralPersonal.ID = Convert.ToInt32(reader["id"]);
                            referralPersonal.name = reader["name"].ToString();
                            referralPersonal.surname = reader["surname"].ToString();
                            referralPersonal.father = reader["father"].ToString();
                            refererList.Add(referralPersonal);


                        }



                    }

                }

                connection.Close();
            }
            return refererList;
        }

        public User GetUser(string mobileNumber, string pass)
        {
            User user = new User();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {

                    connection.Open();
                    using (MySqlCommand com = new MySqlCommand(@"SELECT 
    u.*, 
    up.expire_date as subscription_expire_date
FROM 
    users u
left JOIN 
    user_plans up ON u.id = up.user_id AND up.isActive = 1
WHERE 
    u.pwd = SHA2(@pass, 256) 
    AND u.mobile = @mobileNumber 
    AND u.isActive = 1 
    AND u.isUser = 1 
    AND u.isRegistered = 1;
", connection))
                    {

                        com.Parameters.AddWithValue("@pass", pass);
                        com.Parameters.AddWithValue("@mobileNumber", mobileNumber);
                        using (MySqlDataReader reader = com.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {


                                while (reader.Read())
                                {
                                    user.ID = Convert.ToInt32(reader["id"]);
                                    user.isActive = Convert.ToBoolean(reader["isActive"]);

                                    user.name = reader["name"].ToString();
                                    user.surname = reader["surname"].ToString();
                                    user.isAdmin = reader["isAdmin"] == DBNull.Value ? false : Convert.ToBoolean(reader["isAdmin"]);
                                    user.isUser = reader["isUser"] == DBNull.Value ? false : Convert.ToBoolean(reader["isUser"]);
                                    user.isDr = reader["isDr"] == DBNull.Value ? false : Convert.ToBoolean(reader["isDr"]);
                                    user.subscription_expire_date = reader["subscription_expire_date"] == DBNull.Value ? null : Convert.ToDateTime(reader["subscription_expire_date"]);
                                }

                                connection.Close();





                            }

                        }

                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
          
            return user;
        }

        public User GetUserByID(int id)
        {
            User user = new User();

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {

                connection.Open();
                using (MySqlCommand com = new MySqlCommand(@"SELECT 
    a.*, 
    s.name AS speciality, 
    p.expire_date as plan_expire_date
FROM 
    users a
JOIN 
    speciality s ON a.specialityID = s.id
LEFT JOIN 
    user_plans p ON a.id = p.user_id AND p.isActive = 1
WHERE 
    a.id = @id
    AND a.isActive = 1;

", connection))
                {

                    com.Parameters.AddWithValue("@id", id);

                    using (MySqlDataReader reader = com.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {


                            while (reader.Read())
                            {

                                user.ID = Convert.ToInt32(reader["id"]);
                                user.isActive = Convert.ToBoolean(reader["isActive"]);
                                user.specialityID = Convert.ToInt64(reader["specialityID"]);
                                user.speciality = new Speciality
                                {
                                    id = Convert.ToInt64(reader["specialityID"]),
                                    name = reader.GetString(reader.GetOrdinal("speciality"))
                                };
                                user.name = reader["name"].ToString();
                                user.surname = reader["surname"].ToString();
                                user.father = reader["father"] == DBNull.Value ? "" : reader["father"].ToString();
                                user.mobile = reader["mobile"] == DBNull.Value ? "" : reader["mobile"].ToString();
                                user.username = reader["username"] == DBNull.Value ? "" : reader["username"].ToString();
                                user.email = reader["email"] == DBNull.Value ? "" : reader["email"].ToString();
                                user.passportSerialNum = reader["passportSerialNum"] == DBNull.Value ? "" : reader["passportSerialNum"].ToString();
                                user.fin = reader["fin"] == DBNull.Value ? "" : reader["fin"].ToString();
                                user.bDate = reader["bDate"] == DBNull.Value ? DateTime.Now.Date.ToString("yyyy-MM-dd") : Convert.ToDateTime(reader["bDate"]).Date.ToString("yyyy-MM-dd");
                                user.isAdmin = reader["isAdmin"] == DBNull.Value ? false : Convert.ToBoolean(reader["isAdmin"]);
                                user.isUser = reader["isUser"] == DBNull.Value ? false : Convert.ToBoolean(reader["isUser"]);
                                user.isDr = reader["isDr"] == DBNull.Value ? false : Convert.ToBoolean(reader["isDr"]);
                                user.subscription_expire_date = reader["plan_expire_date"] == DBNull.Value ? null : Convert.ToDateTime(reader["plan_expire_date"]);
                                user.cDate = reader["cDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["cDate"]);



                            }

                            connection.Close();





                        }

                    }

                }
                connection.Close();
            }
            return user;
        }

        public User? GetUserByPhone(string mobileNumber)
        {


            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {

                connection.Open();
                using (MySqlCommand com = new MySqlCommand("select * from users where mobile=@mobile", connection))
                {

                    com.Parameters.AddWithValue("@mobile", mobileNumber);

                    using (MySqlDataReader reader = com.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            User personal = new User();


                            while (reader.Read())
                            {
                                personal.ID = Convert.ToInt32(reader["id"]);
                                personal.isActive = Convert.ToBoolean(reader["isActive"]);
                                personal.otpSentDate = reader["otp_sent_date"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["otp_sent_date"]);
                                personal.recovery_otp_send_date = reader["recovery_otp_send_date"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["recovery_otp_send_date"]);
                                personal.name = reader["name"].ToString();
                                personal.surname = reader["surname"].ToString();
                                personal.isAdmin = reader["isAdmin"] == DBNull.Value ? false : Convert.ToBoolean(reader["isAdmin"]);
                                personal.isRegistered = reader["isRegistered"] == DBNull.Value ? false : Convert.ToBoolean(reader["isRegistered"]);
                                personal.isUser = reader["isUser"] == DBNull.Value ? false : Convert.ToBoolean(reader["isUser"]);
                                personal.isDr = reader["isDr"] == DBNull.Value ? false : Convert.ToBoolean(reader["isDr"]);
                            }

                            connection.Close();


                            return personal;


                        }


                    }

                }

            }
            return null;
        }

        public long InsertUser(string name = "", string surname = "",
            string father = "", int specialityID = 0, string passportSerialNum = "",
            string fin = "", string phone = "", string email = "",
            string bDate = "", string username = "", string pwd = "",
            int isUser = 0, int isDr = 0, int isAdmin = 0,
            int isActive = 0, string otp = "")
        {
            try
            {


                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    var query = @"INSERT INTO users (name, surname, father, specialityID, mobile, email, bDate, username, pwd, 
                          isUser, isDr, isAdmin, isActive, otp_code)
                          SELECT @name, @surname, @father, @specialityID, @mobile, @email, @bDate, @username, SHA2(@pwd, 256), 
                          @isUser, @isDr, @isAdmin, @isActive, @otp_code
                          FROM DUAL
                          WHERE NOT EXISTS (
                            SELECT 1 FROM users 
                            WHERE mobile = @mobile)";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", name ?? "");
                        command.Parameters.AddWithValue("@surname", surname ?? "");
                        command.Parameters.AddWithValue("@father", father ?? "");
                        command.Parameters.AddWithValue("@specialityID", specialityID);
                        command.Parameters.AddWithValue("@fin", fin);
                        command.Parameters.AddWithValue("@passportSerialNum", passportSerialNum);
                        command.Parameters.AddWithValue("@mobile", phone ?? "");
                        command.Parameters.AddWithValue("@email", email ?? "");
                        command.Parameters.AddWithValue("@bDate", string.IsNullOrEmpty(bDate) ? (object)DBNull.Value : DateTime.Parse(bDate));
                        command.Parameters.AddWithValue("@username", username ?? "");
                        command.Parameters.AddWithValue("@pwd", pwd ?? "");
                        command.Parameters.AddWithValue("@isUser", isUser);
                        command.Parameters.AddWithValue("@isActive", isActive);
                        command.Parameters.AddWithValue("@isDr", isDr);
                        command.Parameters.AddWithValue("@isAdmin", isAdmin);
                        command.Parameters.AddWithValue("@otp_code", otp ?? "");


                        command.ExecuteNonQuery();
                        return command.LastInsertedId;
                    }
                }
            }
            catch (Exception ex)
            {

                return 0;
            }
        }

        public int UpdateUser(int userID = 0, string name = "", string surname = "", string father = "",
            int specialityID = 0, string passportSerialNum = "", string fin = "",
            string mobile = "", string email = "", string bDate = "",
            string username = "", int isUser = 0, int isDr = 0, int isActive = 0,
            int isAdmin = 0, string otp = "", string recoveryOtp = "", DateTime? recoveryOtpSendDate = null, string password = "", int isRegistered = 0)
        {
            int updated = 0;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    var query = new StringBuilder("UPDATE users SET ");
                    var parameters = new List<MySqlParameter>();

                    if (!string.IsNullOrEmpty(name))
                    {
                        query.Append("name = @name, ");
                        parameters.Add(new MySqlParameter("@name", name));
                    }
                    if (!string.IsNullOrEmpty(surname))
                    {
                        query.Append("surname = @surname, ");
                        parameters.Add(new MySqlParameter("@surname", surname));
                    }
                    if (!string.IsNullOrEmpty(father))
                    {
                        query.Append("father = @father, ");
                        parameters.Add(new MySqlParameter("@father", father));
                    }
                    if (!string.IsNullOrEmpty(mobile))
                    {
                        query.Append("mobile = @mobile, ");
                        parameters.Add(new MySqlParameter("@mobile", mobile));
                    }
                    if (!string.IsNullOrEmpty(password))
                    {
                        query.Append("pwd = @pwd, ");
                        parameters.Add(new MySqlParameter("@pwd", password));
                    }
                    if (!string.IsNullOrEmpty(email))
                    {
                        query.Append("email = @email, ");
                        parameters.Add(new MySqlParameter("@email", email));
                    }
                    if (!string.IsNullOrEmpty(bDate))
                    {
                        query.Append("bDate = @bDate, ");
                        parameters.Add(new MySqlParameter("@bDate", string.IsNullOrEmpty(bDate) ? (object)DBNull.Value : DateTime.Parse(bDate)));
                    }
                    if (!string.IsNullOrEmpty(username))
                    {
                        query.Append("username = @username, ");
                        parameters.Add(new MySqlParameter("@username", username));
                    }
                    if (!string.IsNullOrEmpty(passportSerialNum))
                    {
                        query.Append("passportSerialNum = @passportSerialNum, ");
                        parameters.Add(new MySqlParameter("@passportSerialNum", passportSerialNum));
                    }
                    if (!string.IsNullOrEmpty(fin))
                    {
                        query.Append("fin = @fin, ");
                        parameters.Add(new MySqlParameter("@fin", fin));
                    }
                    if (specialityID != 0)
                    {
                        query.Append("specialityID = @specialityID, ");
                        parameters.Add(new MySqlParameter("@specialityID", specialityID));
                    }
                    if (isUser != 0)
                    {
                        query.Append("isUser = @isUser, ");
                        parameters.Add(new MySqlParameter("@isUser", isUser));
                    }
                    if (isActive != 0)
                    {
                        query.Append("isActive = @isActive, ");
                        parameters.Add(new MySqlParameter("@isActive", isActive));
                    }
                    if (isDr != 0)
                    {
                        query.Append("isDr = @isDr, ");
                        parameters.Add(new MySqlParameter("@isDr", isDr));
                    }
                    if (isAdmin != 0)
                    {
                        query.Append("isAdmin = @isAdmin, ");
                        parameters.Add(new MySqlParameter("@isAdmin", isAdmin));
                    }
                    if (!string.IsNullOrEmpty(otp))
                    {
                        query.Append("otp_code = @otp_code, ");
                        parameters.Add(new MySqlParameter("@otp_code", otp));
                    }
                    if (!string.IsNullOrEmpty(recoveryOtp))
                    {
                        query.Append("recovery_otp = @recovery_otp, ");
                        parameters.Add(new MySqlParameter("@recovery_otp", recoveryOtp));
                    }
                   
                    if (recoveryOtpSendDate != null)
                    {
                        query.Append("recovery_otp_send_date = @recovery_otp_send_date, ");
                        parameters.Add(new MySqlParameter("@recovery_otp_send_date", recoveryOtpSendDate));
                    }
                    if (isRegistered > 0)
                    {
                        query.Append("isRegistered = @isRegistered, ");
                        parameters.Add(new MySqlParameter("@isRegistered", isRegistered));
                    }
                    query.Append("otp_sent_date = @otp_sent_date, ");
                    parameters.Add(new MySqlParameter("@otp_sent_date", DateTime.Now));


                    // Remove the last comma and space
                    if (parameters.Count > 0)
                    {
                        query.Length -= 2;
                    }

                    query.Append(" WHERE id = @userID");
                    parameters.Add(new MySqlParameter("@userID", userID));

                    using (MySqlCommand com = new MySqlCommand(query.ToString(), connection))
                    {
                        com.Parameters.AddRange(parameters.ToArray());

                        updated = com.ExecuteNonQuery();
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return updated;
        }

        public int UpdateUserExpireDate(int userId, DateTime expireDate)
        {
            using MySqlConnection connection = new MySqlConnection(_connectionString);
            connection.Open();
            int res = 0;

            var query = $@"UPDATE users SET subscription_expire_date = @expireDate WHERE id=@id";

            using (var com = new MySqlCommand(query, connection))
            {
                com.Parameters.AddWithValue("@expireDate", expireDate);
                com.Parameters.AddWithValue("@id", userId);

                res = com.ExecuteNonQuery();
            }
                
            connection.Close();
            return res;
        }
        
        public string GetOtpData(string phone)
        {
            string otpHash = string.Empty;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    var query = "SELECT otp_code, otp_sent_date FROM users WHERE mobile = @mobile";
                    using (MySqlCommand com = new MySqlCommand(query, connection))
                    {
                        com.Parameters.AddWithValue("@mobile", phone);

                        using (var reader = com.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                otpHash = reader["otp_code"] == DBNull.Value ? null : reader["otp_code"].ToString();



                            }

                        }
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // Log the exception
                // elangoAPI.StandardMessages.CallSerilog(ex);
                //result.Success = false;
                //result.Message = "Server error: " + ex.Message;
            }

            return otpHash;
        }

        public string GetRecoveryOtpData(string phone)
        {
            string otpHash = string.Empty;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    var query = "SELECT recovery_otp, recovery_otp_send_date FROM users WHERE mobile = @mobile";
                    using (MySqlCommand com = new MySqlCommand(query, connection))
                    {
                        com.Parameters.AddWithValue("@mobile", phone);

                        using (var reader = com.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                otpHash = reader["recovery_otp"] == DBNull.Value ? null : reader["recovery_otp"].ToString();



                            }

                        }
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // Log the exception
                // elangoAPI.StandardMessages.CallSerilog(ex);
                //result.Success = false;
                //result.Message = "Server error: " + ex.Message;
            }

            return otpHash;
        }

        public int UpdateUserPwd(int userID, string pwd)
        {
            int updated = 0;
            List<User> personalList = new List<User>();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {

                    connection.Open();
                    using (MySqlCommand com = new MySqlCommand("update users set pwd = SHA2(@pwd,256) where id = @userID", connection))
                    {
                        com.Parameters.AddWithValue("@userID", userID);
                        com.Parameters.AddWithValue("@pwd", pwd ?? "");


                        updated = com.ExecuteNonQuery();

                    }
                    connection.Close();


                }


            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);


            }
            return updated;
        }
    }
}

