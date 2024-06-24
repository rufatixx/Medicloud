using System;
namespace Medicloud.Models
{
    public class User

    {
        public int ID { get; set; }
        public int depID { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string father { get; set; }
        public string username { get; set; }
        public string speciality { get; set; }
        public string mobile { get; set; }
        public string passportSerialNum { get; set; }
        public string fin { get; set; }
        public string email { get; set; }
        public string bDate { get; set; }
        public DateTime? otpSentDate { get; set; }
        public bool isActive { get; set; }
        public bool isUser { get; set; }
        public bool isDr { get; set; }
        public bool isAdmin { get; set; }
        public bool isRegistered { get; set; }
        public int status { get; set; }
        public int userType { get; set; }
  

    }
}

