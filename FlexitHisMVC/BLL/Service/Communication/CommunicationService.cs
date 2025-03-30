using System;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Dapper;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Medicloud.DAL.Infrastructure.Abstract;
using Medicloud.BLL.Service.Communication;

namespace Medicloud.BLL.Service.Communication
{
    public class CommunicationService: ICommunicationService
    {
        private readonly IUnitOfWork _unitOfWork;
        public IConfiguration Configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public CommunicationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task sendSmsAsync(string smsText, string smsTel)
        {
            try
            {
                await Task.Run(() => sendSMS(smsText, smsTel));
            }
            catch (Exception ex)
            {
                // Log error if needed
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
                // Log error if needed
            }
        }

        public void sendNotification(string title, string body, long userID)
        {
            try
            {
                _unitOfWork.BeginConnection();
                var con = _unitOfWork.GetConnection();

                string query = @"INSERT INTO notification (title, description, userid) 
                                 VALUES (@title, @description, @userID)";
                con.Execute(query, new { title, description = body, userID });
            }
            catch
            {
                // Log error if needed
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

                    string topic = userID == 0 ? "admin" : userID.ToString();
                    pushNotifyJson = pushNotifyJson.Replace("/topics/", $"/topics/{topic}")
                                                   .Replace("/title/", title)
                                                   .Replace("/body/", body);

                    var content = new StringContent(pushNotifyJson, Encoding.UTF8, "application/json");

                    using var client = new HttpClient();
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"key={appKey}");

                    await client.PostAsync("https://fcm.googleapis.com/fcm/send", content);
                }
                catch
                {
                    // Log error if needed
                }
            }
        }

        public async Task sendMail(string body, string to)
        {
            try
            {
                if (!string.IsNullOrEmpty(to))
                {
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("Medicloud", "office@flexit.az"));
                    message.To.Add(new MailboxAddress("İstifadəçi", to));
                    message.Subject = "Qeydiyyat";
                    message.Body = new BodyBuilder { TextBody = body }.ToMessageBody();

                    using var smtp = new SmtpClient();
                    await smtp.ConnectAsync("mail.hmc.az", 587, SecureSocketOptions.StartTls);
                    await smtp.AuthenticateAsync("office@flexit.az", "b78053Sa7");
                    await smtp.SendAsync(message);
                    await smtp.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending mail: {ex.Message}");
            }
        }

        public void sendSMS(string smsText, string smsTel)
        {
            try
            {
                string apiUrl = $"sendsms.asp?user=mediclound_s&password=Medicloud2025&gsm={smsTel}&text={smsText}";
                using var httpClient = new HttpClient { BaseAddress = new Uri("https://www.poctgoyercini.com/api_http/") };
                httpClient.GetAsync(apiUrl).Wait();
            }
            catch
            {
                // Log error if needed
            }
        }

        public async Task log(string log, string function_name, string ip)
        {
            try
            {
                _unitOfWork.BeginConnection();
                var con = _unitOfWork.GetConnection();

                string query = @"INSERT INTO api_log (ip_adress, log, function_name, cdate) 
                                 VALUES (@ip, @log, @function_name, @cdate)";

                await con.ExecuteAsync(query, new
                {
                    ip,
                    log,
                    function_name,
                    cdate = DateTime.Now
                });
            }
            catch
            {
                // Log error if needed
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
