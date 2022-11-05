using System;
using System.Text;
using System.Text.RegularExpressions;
using crypto;
using FlexitHisCore.Models;
using MySql.Data.MySqlClient;

namespace FlexitHisCore.Repositories
{
    public class Insert
    {
        private readonly string ConnectionString;

        public Insert(string conString)
        {
            ConnectionString = conString;
        }
        static string sha256(string randomString)
        {
            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(randomString));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }
        public long regexPhone(long phone)
        {
            long formattedPhone = 0;


            if (Regex.Match(phone.ToString(), @"[0-9]{12}").Success)
            {
                formattedPhone = Convert.ToInt64(phone.ToString().Substring(3));
            }

            return formattedPhone;
        }
        public bool IsValidPhone(long phone)
        {

            bool isValid = false;



            MySqlConnection connection = new MySqlConnection(ConnectionString);
            try
            {
                //  MailAddress m = new MailAddress(emailaddress);
                connection.Open();

                MySqlCommand com = new MySqlCommand("select * from user where mobile=@phone", connection);
                com.Parameters.AddWithValue("@phone", phone);
                MySqlDataReader reader = com.ExecuteReader();

                bool except = reader.HasRows;
                connection.Close();
                if (except)
                {

                    isValid = true;
                }
                else
                {
                    isValid = false;

                }



            }
            catch (FormatException)
            {
                connection.Close();

            }


            return isValid;

        }
        public string createCode(int length)
        {
            // const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            const string valid = "1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }


       

        public ResponseStruct<int> InsertPayment(string userToken, string requestToken,long kassaID, long payment_typeID,long patientID)
        {
            ResponseStruct<int> response = new ResponseStruct<int>();
            try
            {
                CustomSecurity security = new CustomSecurity(ConnectionString);
                int userID1 = security.selectUserToken(userToken);
                int userID2 = security.selectRequestToken(requestToken);
                DateTime now = DateTime.Now;
                if (userID1 == userID2 && userID1 > 0 && userID2 > 0)
                {
                    long lastID = 0;
                    using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                    {

                        connection.Open();





                        using (MySqlCommand com = new MySqlCommand(@"
Insert INTO payment_operations (kassaID,paymentOperationType,userID,payment_typeID,patientID,price) values (@kassaID,@payOperType,@userID,@payTypeID,@patientID,(SELECT  sum((select price from services where id = a.serviceID ))as serviceSum
FROM patient_request a where hospitalID =(select hospitalID from patients where id = @patientID)
and finished=0 and patientID = @patientID  group by patientID order by serviceSum))", connection))

                        {
                            com.Parameters.AddWithValue("@userID", userID1);
                            com.Parameters.AddWithValue("@payOperType", 1);
                            com.Parameters.AddWithValue("@kassaID", kassaID);
                            com.Parameters.AddWithValue("@payTypeID", payment_typeID);
                            com.Parameters.AddWithValue("@patientID", patientID);
                           
                            com.ExecuteNonQuery();
                            lastID = com.LastInsertedId;
                        }


                       
                        if (lastID > 0)
                        {

                            using (MySqlCommand com = new MySqlCommand(@"UPDATE patient_request SET finished=1 WHERE patientID=@patientID", connection))

                            {
                               
                                com.Parameters.AddWithValue("@patientID", patientID);

                                com.ExecuteNonQuery();
                              
                            }


                            response.status = 1; //inserted
                        }
                        else
                        {
                            response.status = 2; // not inserted (Duplicate)
                        }
                        response.requestToken = security.requestTokenGenerator(userToken, userID1);
                        connection.Close();
                    }




                }
                else
                {
                    response.status = 3; //access danied
                }
            }
            catch (Exception ex)
            {
                //FlexitHisMVC.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);
                response.status = 4; // not inserted (error)
            }


            return response;
        }




        //public StatusStruct sendSMS(long phone)

        //{
        //    StatusStruct statusCode = new StatusStruct();
        //    Security security = new Security(Configuration, _hostingEnvironment);
        //    string smsCode = createCode(4);
        //    long userID = 0;
        //    long formattedPhone = regexPhone(phone);
        //    if (formattedPhone > 0)
        //    {


        //        if (!IsValidPhone(phone)) // Проверка существования юзера
        //        {


        //            try
        //            {



        //                DateTime now = DateTime.Now;

        //                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        //                {

        //                    connection.Open();

        //                    using (MySqlCommand com = new MySqlCommand("INSERT INTO user (mobile,isactive,isblocked,resetrequested,cdate,smsCode)" +
        //                           " Values (@mobile" +
        //                           ",0,0,0,NOW()," +
        //                           "SHA2(@smsCode,512))", connection))
        //                    {


        //                        //com.Parameters.AddWithValue("@name", newUser.name);
        //                        //com.Parameters.AddWithValue("@surname", newUser.surname);

        //                        //com.Parameters.AddWithValue("@mail", newUser.email);
        //                        com.Parameters.AddWithValue("@mobile", phone);
        //                        // com.Parameters.AddWithValue("@userID", userID);
        //                        //com.Parameters.AddWithValue("@pass", newUser.pass);
        //                        ////  com.Parameters.AddWithValue("@birthDate", DateTime.Parse(bDate));
        //                        ////com.Parameters.AddWithValue("@genderName", gender);
        //                        //com.Parameters.AddWithValue("@dateTimeNow", now);
        //                        ////  com.Parameters.AddWithValue("@countryName", country);
        //                        //com.Parameters.AddWithValue("@cityId", newUser.cityId);
        //                        ////com.Parameters.AddWithValue("@professionName", profession);
        //                        com.Parameters.AddWithValue("@smsCode", smsCode);
        //                        com.ExecuteNonQuery();

        //                        long lastId = com.LastInsertedId;
        //                        userID = lastId;
        //                        connection.Close();

        //                        connection.Open();
        //                        com.CommandText = "insert into users_balance (userId,cdate) values (@userId,@cDate)";
        //                        com.Parameters.AddWithValue("@userId", lastId);
        //                        com.Parameters.AddWithValue("@cDate", now);


        //                        com.ExecuteNonQuery();
        //                        connection.Close();

        //                    }

        //                }




        //                //communication.sendMail($"Qeydiyyatı tamamlamaq üçün, zəhmət olmasa <a href=\'https://pullu.az/api/androidmobileapp/verify?code={userToken}'>linkə</a> daxil olun", mail);
        //                //Communication.sendMail($"Qeydiyyati tamamlamaq ucun shifre: {userToken}", newUser.email);
        //                communications.sendSmsAsync($"Qeydiyyati tamamlamaq ucun shifre: {smsCode}", formattedPhone);


        //                //MailMessage mailMsg = new MailMessage();
        //                //SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
        //                //mailMsg.IsBodyHtml = true;
        //                //mailMsg.From = new MailAddress("asadzade99@gmail.com");
        //                //mailMsg.To.Add($"{mail}");
        //                //mailMsg.Subject = "Pullu (Dəstək)";
        //                //mailMsg.Body = $"Qeydiyyatı tamamlamaq üçün, zəhmət olmasa <a href=\'https://pullu.az/api/androidmobileapp/verify?code={userToken}'>linkə</a> daxil olun";

        //                //SmtpServer.Port = 587;
        //                //SmtpServer.Credentials = new System.Net.NetworkCredential("asadzade99@gmail.com", "odqnmjiogipltmwi");
        //                //SmtpServer.EnableSsl = true;

        //                //SmtpServer.Send(mailMsg);



        //                //string json = "{ \"email\" : \"" + mail + "\", \"password\" : \"" + pass + "\", \" returnSecureToken\" : true }";

        //                //var content = new StringContent(json, Encoding.UTF8, "application/json");
        //                //string url = "https://www.googleapis.com/identitytoolkit/v3/relyingparty/signupNewUser?key=AIzaSyCwEuju_UmuNNPrYtxEhsuddOfCzqZQ8nI";
        //                //HttpClient client = new HttpClient();
        //                //var rslt = client.PostAsync(url, content);
        //                //var resp = rslt.Result.RequestMessage;

        //                statusCode.response = 0; // Все ок
        //                statusCode.responseString = "sms sent for registration"; // Все ок



        //            }
        //            catch (Exception ex)
        //            {
        //                statusCode.response = 1;//Ошибка сервера
        //                statusCode.responseString = $"Exception message: {ex.Message}";

        //            }

        //            //http://127.0.0.1:44301/api/androidmobileapp/user/signUp?mail=asadzade99@gmail.com&username=asa&name=asa&surname=asa&pass=1&phone=1&bdate=1987-08-23&gender=Ki%C5%9Fi&country=Az%C9%99rbaycan&city=Bak%C4%B1&profession=Texnologiya%20sektoru

        //        }
        //        else
        //        {
        //            DbSelect select = new DbSelect(Configuration, _hostingEnvironment);
        //            userID = select.getUserID(phone);
        //            if (userID > 0)
        //            {


        //                int affected = 0;
        //                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        //                {

        //                    connection.Open();

        //                    using (MySqlCommand com = new MySqlCommand("update user set smsCode = SHA2(@smsCode,512) where userID=@userID", connection))
        //                    {



        //                        com.Parameters.AddWithValue("@smsCode", smsCode);
        //                        com.Parameters.AddWithValue("@userID", userID);
        //                        affected = com.ExecuteNonQuery();
        //                        connection.Close();

        //                    }

        //                }

        //                communications.sendSmsAsync($"Daxil olmaq üçün şifrə: {smsCode}", formattedPhone);
        //                //statusCode.userToken = security.userTokenGenerator(userID);
        //                //statusCode.requestToken = security.requestTokenGenerator(statusCode.userToken, userID);

        //                statusCode.response = 0;
        //                statusCode.responseString = "SMS sent";


        //            }

        //        }
        //    }
        //    else
        //    {
        //        statusCode.response = 2;
        //        statusCode.responseString = "wrong format";
        //    }
        //    return statusCode;
        //}
        //public VerifyStruct verifySMS(long phone, string code)
        //{
        //    long formattedPhone = regexPhone(phone);
        //    VerifyStruct status = new VerifyStruct();
        //    if (formattedPhone > 0 && !string.IsNullOrEmpty(code) && code.Length == 4)
        //    {
        //        Security security = new Security(Configuration, _hostingEnvironment);
        //        long userID = 0;
        //        int isActive = 0;

        //        try
        //        {

        //            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        //            {
        //                connection.Open();

        //                using (MySqlCommand com = new MySqlCommand("select * from user where smsCode=SHA2(@smsCode,512) and mobile = @phone order by userID desc limit 1", connection))
        //                {

        //                    com.Parameters.AddWithValue("@smsCode", code);
        //                    com.Parameters.AddWithValue("@phone", phone);
        //                    using (MySqlDataReader reader = com.ExecuteReader())
        //                    {
        //                        if (reader.HasRows)
        //                        {
        //                            while (reader.Read())
        //                            {
        //                                userID = Convert.ToInt32(reader["userID"]);
        //                                isActive = Convert.ToInt32(reader["isActive"]);
        //                            }


        //                        }
        //                        else
        //                        {
        //                            status.response = 2;
        //                            status.requestToken = security.requestTokenGenerator(userToken, userID);

        //                            status.responseString = "Access danied";
        //                        }
        //                    }

        //                }
        //                if (userID > 0)
        //                {
        //                    using (MySqlCommand com = new MySqlCommand("update user set smsCode = NULL where userID = @userID", connection))
        //                    {

        //                        com.Parameters.AddWithValue("@userID", userID);
        //                        com.ExecuteNonQuery();
        //                        status.userToken = security.userTokenGenerator(userID);
        //                        status.requestToken = security.requestTokenGenerator(status.userToken, userID);




        //                    }
        //                    switch (isActive)
        //                    {
        //                        case 1:
        //                            status.response = 0;
        //                            status.responseString = "welcome";
        //                            break;
        //                        case 0:
        //                            status.response = 3;
        //                            status.responseString = "continue registration";
        //                            break;

        //                    }




        //                }


        //                connection.Close();

        //            }

        //            int userID1 = security.selectUserToken(userToken);
        //            int userID2 = security.selectRequestToken(requestToken);

        //            if (userID1 == userID2)
        //            {

        //                userID = userID1;


        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            status.response = 1;


        //            status.responseString = $"Exception: {ex.Message}";
        //        }
        //    }
        //    else
        //    {
        //        status.response = 4;
        //        status.responseString = "wrong param format";
        //    }
        //    return status;

        //}
        //public StatusStruct signUp(NewUserStruct newUser)
        //{

        //    StatusStruct status = new StatusStruct();
        //    Security security = new Security(Configuration, _hostingEnvironment);
        //    if (newUser.userType > 0 && !string.IsNullOrEmpty(newUser.name) && !string.IsNullOrEmpty(newUser.surname) && !string.IsNullOrEmpty(newUser.userToken) && !string.IsNullOrEmpty(newUser.requestToken))
        //    {

        //        int userID1 = security.selectUserToken(newUser.userToken);
        //        int userID2 = security.selectRequestToken(newUser.requestToken);

        //        if (userID1 == userID2)
        //        {







        //            try
        //            {
        //                int affected = 0;

        //                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        //                {
        //                    connection.Open();

        //                    using (MySqlCommand com = new MySqlCommand("update user set name = @name,surname=@surname,userType=@userType,isActive=1 where userID=@userID and isActive=0", connection))
        //                    {

        //                        com.Parameters.AddWithValue("@name", newUser.name);
        //                        com.Parameters.AddWithValue("@surname", newUser.surname);
        //                        com.Parameters.AddWithValue("@userType", newUser.userType);
        //                        com.Parameters.AddWithValue("@userID", userID1);
        //                        affected = com.ExecuteNonQuery();

        //                    }



        //                    connection.Close();


        //                }
        //                if (affected > 0)
        //                {
        //                    status.requestToken = security.requestTokenGenerator(newUser.userToken, userID1);
        //                    status.response = 0;
        //                    status.responseString = "OK";
        //                }
        //                else
        //                {

        //                    status.response = 2;
        //                    status.responseString = "User already registered";
        //                }


        //            }
        //            catch (Exception ex)
        //            {
        //                status.response = 1;


        //                status.responseString = $"Exception: {ex.Message}";
        //            }

        //        }
        //        else
        //        {
        //            status.response = 3;
        //            status.responseString = "wrong Token";
        //        }

        //    }
        //    else
        //    {
        //        status.response = 4;
        //        status.responseString = "wrong params";
        //    }


        //    return status;

        //}

        //public StatusStruct SignUp(NewUserStruct newUser)

        //{
        //    StatusStruct statusCode = new StatusStruct();

        //    if (!IsValidEmail(newUser.email)) // Проверка существования юзера
        //    {


        //        try
        //        {
        //            string userToken = createCode(4);


        //            DateTime now = DateTime.Now;

        //            MySqlConnection connection = new MySqlConnection(ConnectionString);


        //            connection.Open();

        //            MySqlCommand com = new MySqlCommand("INSERT INTO user (name,surname,email,mobile,passwd,isactive,isblocked,resetrequested,cdate, countryId,cityId,userToken)" +
        //                " Values (@name,@surname,@mail,@mobile,SHA2(@pass,512)" +
        //                ",0,0,0,@dateTimeNow,1," +
        //                "@cityId,SHA2(@userToken,512))", connection);


        //            com.Parameters.AddWithValue("@name", newUser.name);
        //            com.Parameters.AddWithValue("@surname", newUser.surname);

        //            com.Parameters.AddWithValue("@mail", newUser.email);
        //            com.Parameters.AddWithValue("@mobile", newUser.phone);
        //            com.Parameters.AddWithValue("@pass", newUser.pass);
        //            //  com.Parameters.AddWithValue("@birthDate", DateTime.Parse(bDate));
        //            //com.Parameters.AddWithValue("@genderName", gender);
        //            com.Parameters.AddWithValue("@dateTimeNow", now);
        //            //  com.Parameters.AddWithValue("@countryName", country);
        //            com.Parameters.AddWithValue("@cityId", newUser.cityId);
        //            //com.Parameters.AddWithValue("@professionName", profession);
        //            com.Parameters.AddWithValue("@userToken", userToken);
        //            com.ExecuteNonQuery();

        //            long lastId = com.LastInsertedId;
        //            connection.Close();

        //            connection.Open();
        //            com.CommandText = "insert into users_balance (userId,cdate) values (@userId,@cDate)";
        //            com.Parameters.AddWithValue("@userId", lastId);
        //            com.Parameters.AddWithValue("@cDate", now);


        //            com.ExecuteNonQuery();
        //            connection.Close();


        //            //communication.sendMail($"Qeydiyyatı tamamlamaq üçün, zəhmət olmasa <a href=\'https://pullu.az/api/androidmobileapp/verify?code={userToken}'>linkə</a> daxil olun", mail);
        //            //Communication.sendMail($"Qeydiyyati tamamlamaq ucun shifre: {userToken}", newUser.email);
        //            communications.sendSmsAsync($"Qeydiyyati tamamlamaq ucun shifre: {userToken}", newUser.phone);


        //            //MailMessage mailMsg = new MailMessage();
        //            //SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
        //            //mailMsg.IsBodyHtml = true;
        //            //mailMsg.From = new MailAddress("asadzade99@gmail.com");
        //            //mailMsg.To.Add($"{mail}");
        //            //mailMsg.Subject = "Pullu (Dəstək)";
        //            //mailMsg.Body = $"Qeydiyyatı tamamlamaq üçün, zəhmət olmasa <a href=\'https://pullu.az/api/androidmobileapp/verify?code={userToken}'>linkə</a> daxil olun";

        //            //SmtpServer.Port = 587;
        //            //SmtpServer.Credentials = new System.Net.NetworkCredential("asadzade99@gmail.com", "odqnmjiogipltmwi");
        //            //SmtpServer.EnableSsl = true;

        //            //SmtpServer.Send(mailMsg);



        //            //string json = "{ \"email\" : \"" + mail + "\", \"password\" : \"" + pass + "\", \" returnSecureToken\" : true }";

        //            //var content = new StringContent(json, Encoding.UTF8, "application/json");
        //            //string url = "https://www.googleapis.com/identitytoolkit/v3/relyingparty/signupNewUser?key=AIzaSyCwEuju_UmuNNPrYtxEhsuddOfCzqZQ8nI";
        //            //HttpClient client = new HttpClient();
        //            //var rslt = client.PostAsync(url, content);
        //            //var resp = rslt.Result.RequestMessage;

        //            statusCode.response = 0; // Все ок
        //            statusCode.responseString = "user inserted and code sent"; // Все ок
        //            return statusCode;


        //        }
        //        catch (Exception ex)
        //        {
        //            statusCode.response = 1;//Ошибка сервера
        //            statusCode.responseString = $"Exception message: {ex.Message}";
        //            return statusCode;
        //        }

        //        //http://127.0.0.1:44301/api/androidmobileapp/user/signUp?mail=asadzade99@gmail.com&username=asa&name=asa&surname=asa&pass=1&phone=1&bdate=1987-08-23&gender=Ki%C5%9Fi&country=Az%C9%99rbaycan&city=Bak%C4%B1&profession=Texnologiya%20sektoru

        //    }
        //    else
        //    {

        //        statusCode.response = 2;//Юзер существует
        //        statusCode.responseString = "user is already exist";
        //        return statusCode;
        //    }


        //}
        //public StatusStruct activateUser(int code)
        //{
        //    StatusStruct status = new StatusStruct();
        //    if (code > 0)
        //    {


        //        try
        //        {


        //            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        //            {




        //                connection.Open();

        //                using (MySqlCommand com = new MySqlCommand("update user set isActive=1,userToken=null where userToken=SHA2(@userToken,512)", connection))
        //                {

        //                    com.Parameters.AddWithValue("@userToken", code);
        //                    int exist = com.ExecuteNonQuery();
        //                    if (exist > 0)
        //                    {
        //                        status.response = 0;
        //                        status.responseString = "user activated";
        //                    }
        //                    else
        //                    {
        //                        status.response = 2;
        //                        status.responseString = "code is wrong";

        //                    }

        //                }
        //                connection.Close();


        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            status.response = 1;
        //            status.responseString = $"Server error. Exception Message: {ex.Message}";

        //        }
        //    }
        //    else
        //    {
        //        status.response = 4;
        //        status.responseString = "code is empty";
        //    }
        //    return status;
        //}
    }
}
