using System;
namespace Medicloud.BLL.Service.Communication
{
    public interface ICommunicationService
    {
        Task sendSmsAsync(string smsText, string smsTel);
        Task sendNotificationAsync(string title, string body, long userID);
        void sendNotification(string title, string body, long userID);
        Task sendPushNotificationAsync(string title, string body, long userID = 0);
        Task sendMail(string body, string to);
        void sendSMS(string smsText, string smsTel);
        Task log(string log, string function_name, string ip);
    }
}

