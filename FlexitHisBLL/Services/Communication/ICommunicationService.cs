using System.Net.Mail;

namespace Medicloud.BLL.Services.Communication
{
	public interface ICommunicationService
	{
		Task SendSMSAsync(string smsText, string smsTel);
		Task SendMailAsync(string body, string to);

	}
}
