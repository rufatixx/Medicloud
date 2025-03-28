using System;
using System.Text;
using System.Net.Http;
using MailKit;
using MailKit.Security;
using MimeKit;
using MailKit.Net.Smtp;
using MySql.Data.MySqlClient;

namespace Medicloud.BLL.Service
{
    public class CommunicationService
    {
        private readonly string _connectionString;
        public IConfiguration Configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public CommunicationService(string connectionString)
        {
          
            _connectionString = connectionString;
            

        }
        //public async Task sendMailAsync(string body, string to)
        //{


        //    try
        //    {
        //        await Task.Run(() => sendMail(body, to));

        //    }
        //    catch (Exception ex)
        //    {
        //        //elangoAPI.StandardMessages.CallSerilog(ex);
        //    }
        //}
        public async Task sendSmsAsync(string smsText, string smsTel)
        {


            try
            {
                await Task.Run(() => sendSMS(smsText, smsTel));
            }
            catch (Exception ex)
            {
                //elangoAPI.StandardMessages.CallSerilog(ex);
            }
        }
        public async Task sendNotificationAsync(string title, string body, long userID)
        {


            try
            {
                await Task.Run(() => sendNotification(title, body, userID));
            }
            catch (Exception ex)
            {
                //elangoAPI.StandardMessages.CallSerilog(ex);
            }
        }


        public void sendNotification(string title, string body, long userID)
        {
            try
            {
                //string mail = "asadzade99@gmail.com";
                //string pass = "123456";
                ////string json = "{ \"method\" : \"guru.test\", \"params\" : [ \"Guru\" ], \"id\" : 123 }";
                ////string json = "{ \"email\" : \"" + mail + "\", \"password\" : \"" + pass + "\", \" returnSecureToken\" :\"true\"}";
                //var json = Newtonsoft.Json.JsonConvert.SerializeObject(new
                //{
                //    email = mail,
                //    password = pass,
                //    returnSecureToken = true
                //});

                //var content = new StringContent(json, Encoding.UTF8, "application/json");
                //string url = "https://www.googleapis.com/identitytoolkit/v3/relyingparty/verifyPassword?key=AIzaSyCwEuju_UmuNNPrYtxEhsuddOfCzqZQ8nI";
                //HttpClient client = new HttpClient();
                //var rslt = client.PostAsync(url, content);
                //var resp = rslt.Result.Content.ReadAsStringAsync().Result;



                //FirebaseUser deserializedUser = JsonConvert.DeserializeObject<FirebaseUser>(resp);


                //string notifyJson = "{ \"title\" : \"" + title + "\",\"body\" : \"" + body + "\", \"userID\" : \"" + userID + "\", \"seen\" : false }";

                //var notifyContent = new StringContent(notifyJson, Encoding.UTF8, "application/json");
                //string notifyUrl = $"https://pullu-2e3bb.firebaseio.com/users/{deserializedUser.localId}/notifications/{userID}.json?auth={deserializedUser.idToken}";
                //HttpClient notifyClient = new HttpClient();
                //var notifyRslt = notifyClient.PostAsync(notifyUrl, notifyContent);
                //var notifyResp = notifyRslt.Result.RequestMessage;

                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();


                    using (MySqlCommand com = new MySqlCommand(@$"insert into notification (title,description,userid) values (@title,@description,@uID)", connection))
                    {
                        com.Parameters.AddWithValue("title", title);
                        com.Parameters.AddWithValue("description", body);
                        com.Parameters.AddWithValue("uID", userID);



                        com.ExecuteNonQuery();

                    }




                    connection.Close();




                }
            }
            catch
            {


            }

        }

        public async Task sendPushNotificationAsync(string title, string body, long userID = 0)
        {
            if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(body))
            {
                string appKey = "AAAA_D_IDDA:APA91bG5T8YbSwxyLo-q5lJ4BtAJxPexzKjFdZF3--56koZTVqWXR_C_raeGXOTXE9HgKAHmSECOh_sixFlZ65uxtJJfqkMulz3_GoXaU-AYLDT8noldworke8cKLRdXqRLx7tG2HXxr";
                try
                {


                    string pushNotifyJson = @"{
    ""to"": ""/topics/"",
    ""notification"": {
                    ""title"": ""/title/"",
      ""body"": ""/body/"",
      ""mutable_content"": true,
      ""sound"": ""Tri-tone""
      },

   ""data"": {
                    ""url"": ""<url of media image>"",
    ""dl"": ""<deeplink action on tap of notification>""
      }
            }";
                    switch (userID)
                    {
                        case 0:
                            pushNotifyJson = pushNotifyJson.Replace("/topics/", "/topics/admin");
                            break;
                        default:
                            pushNotifyJson = pushNotifyJson.Replace("/topics/", $"/topics/{userID.ToString()}");
                            break;
                    }

                    pushNotifyJson = pushNotifyJson.Replace("/title/", $"{title}");
                    pushNotifyJson = pushNotifyJson.Replace("/body/", $"{body.ToString()}");

                    var pushNotifyContent = new StringContent(pushNotifyJson, Encoding.UTF8, "application/json");
                    string pushNotifyUrl = $"https://fcm.googleapis.com/fcm/send";
                    HttpClient notifyClient = new HttpClient();
                    notifyClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"key={appKey}");
                    var notifyRslt = notifyClient.PostAsync(pushNotifyUrl, pushNotifyContent);
                    var notifyResp = notifyRslt.Result.RequestMessage;


                }
                catch
                {


                }

            }

        }



        //public async Task sendMail(string body, string to)
        //{
        //    try
        //    {
        //        DbSelect select = new DbSelect(Configuration, _hostingEnvironment);
        //        //string email = select.getUserMail(to);
        //        if (!string.IsNullOrEmpty(to))
        //        {
        //            MailMessage mailMsg = new MailMessage();
        //            using (SmtpClient SmtpServer = new SmtpClient("mail.hmc.com"))
        //            {
        //                mailMsg.IsBodyHtml = true;
        //                mailMsg.From = new MailAddress("bingoo@flexit.az");
        //                mailMsg.To.Add($"{to}");
        //                mailMsg.Subject = "Bingoo (Dəstək)";
        //                mailMsg.Body = body;

        //                SmtpServer.Port = 587;

        //                SmtpServer.Credentials = new System.Net.NetworkCredential("bingoo@flexit.az", "c54617Cg8");
        //                SmtpServer.EnableSsl = true;

        //                await SmtpServer.SendMailAsync(mailMsg);
        //                //SmtpServer.Dispose();
        //            }
        //        }

        //    }
        //    catch { }

        //}

        public async Task sendMail(string body, string to)
        {
            try
            {
                //string email = select.getUserMail(to);
                if (!string.IsNullOrEmpty(to))
                {
                    var emailMessage = new MimeKit.MimeMessage();
                    emailMessage.From.Add(new MailboxAddress("Medicloud", "office@flexit.az"));
                    emailMessage.To.Add(new MailboxAddress("İstifadəçi", $"{to}"));
                    emailMessage.Subject = "Qeydiyyat";

                    var bodyBuilder = new BodyBuilder
                    {
                        TextBody =$"{body}",
                    };

                    emailMessage.Body = bodyBuilder.ToMessageBody();



                    using (var smtpClient = new SmtpClient())
                    {
                        smtpClient.Connect("mail.hmc.az", 587, SecureSocketOptions.StartTls);
                        smtpClient.Authenticate("office@flexit.az", "b78053Sa7");
                        smtpClient.Send(emailMessage);
                        smtpClient.Disconnect(true);
                    }
                }

            }
            catch (Exception ex){

                Console.WriteLine($"error send mail {ex.Message}");
            }

        }

        public void sendSMS(string smsText, string smsTel)
        {
            try
            {
                string apiUrl = $"sendsms.asp?user=mediclound_s&password=Medicloud2025&gsm={smsTel}&text={smsText}";
                var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri("https://www.poctgoyercini.com/api_http/");
                var response = httpClient.GetAsync($"{apiUrl}").Result;

            }
            catch
            {

            }

        }

        public async Task log(string log, string function_name, string ip)
        {

            DateTime now = DateTime.Now;

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                using (MySqlCommand com = new MySqlCommand("Insert into api_log (ip_adress,log,function_name,cdate) values (@ipAdress,@log,@function_name,@cdate)", connection))
                {
                    com.Parameters.AddWithValue("@ipAdress", ip);
                    com.Parameters.AddWithValue("@log", log);
                    com.Parameters.AddWithValue("@function_name", function_name);
                    com.Parameters.AddWithValue("@cdate", now);
                    await com.ExecuteNonQueryAsync();
                    com.Dispose();

                }

                connection.Close();

            }


        }

    }
    public class SMSResponse
    {
        public int errno { get; set; }
        public string errtext { get; set; }
        public string message_id { get; set; }
        public string charge { get; set; }
        public string balance { get; set; }

    }
}

