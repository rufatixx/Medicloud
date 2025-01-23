
using System.Net.Mail;
namespace Medicloud.BLL.Services.Communication
{
	public class CommunicationService : ICommunicationService
	{
		public async Task SendSMSAsync(string smsText, string smsTel)
		{
			try
			{
				string apiUrl = $"sendsms.asp?user=mediclound_s&password=Medicloud2025&gsm={smsTel}&text={smsText}";
				var httpClient = new HttpClient();
				httpClient.BaseAddress = new Uri("https://www.poctgoyercini.com/api_http/");
				var response =await httpClient.GetAsync($"{apiUrl}");

			}
			catch (Exception ex)
			{

				Console.WriteLine(ex.Message);


			}
		}

		public async Task SendMailAsync(string body, string to)
		{
			throw new NotImplementedException();
			//try
			//{

			//	//var forwardMessage = new MimeKit.MimeMessage();

			//	//forwardMessage.From.Add(new MimeKit.MailboxAddress("Medicloud", "office@flexit.az"));
			//	//forwardMessage.To.Add(new MailboxAddress("İstifadəçi", $"{to}"));

			//	//forwardMessage.Subject = "Səhiyyə Nazirliyi";
			//	//forwardMessage.Body = new TextPart("plain")
			//	//{
			//	//	Text = message
			//	//};

			//	//using var smtpClient = new SmtpClient();
			//	//await smtpClient.ConnectAsync(_emailSettings.ImapServer, 465, true);
			//	//await smtpClient.AuthenticateAsync(_emailSettings.ImapUsername, _emailSettings.ImapPassword);
			//	//await smtpClient.SendAsync(forwardMessage);
			//	//await smtpClient.DisconnectAsync(true);

			//}
			//catch (Exception ex)
			//{

			//	Console.WriteLine($"error send mail {ex.Message}");
			//}

			//try
			//{
			//	if (!string.IsNullOrEmpty(to))
			//	{
			//		var emailMessage = new MimeKit.MimeMessage();
			//		emailMessage.From.Add(new MailboxAddress("Medicloud", "office@flexit.az"));
			//		emailMessage.To.Add(new MimeKit.MailboxAddress("İstifadəçi", $"{to}"));
			//		emailMessage.Subject = "Qeydiyyat";

			//		var bodyBuilder = new BodyBuilder
			//		{
			//			TextBody = $"{body}",
			//		};

			//		emailMessage.Body = bodyBuilder.ToMessageBody();



			//		using (var smtpClient = new SmtpClient())
			//		{
			//			smtpClient.Connect("mail.hmc.az", 587, SecureSocketOptions.StartTls);
			//			smtpClient.Authenticate("office@flexit.az", "b78053Sa7");
			//			smtpClient.Send(emailMessage);
			//			smtpClient.Disconnect(true);
			//		}
			//	}

			//}
			//catch (Exception ex)
			//{

			//	Console.WriteLine($"error send mail {ex.Message}");
			//}

		}
	}
}
