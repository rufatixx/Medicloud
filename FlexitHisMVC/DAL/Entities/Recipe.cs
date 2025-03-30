using System;
namespace Medicloud.Models
{
    public class Recipe
    {
        public long id { get; set; }
        public long paymentOperationType { get; set; }
        public long patientID { get; set; }
        public string patientFullName { get; set; }
        public long userID { get; set; }
        public double price { get; set; }
        public string pTypeName { get; set; }
        public string cashier { get; set; }
        public DateTime cdate { get; set; }
    }
}

