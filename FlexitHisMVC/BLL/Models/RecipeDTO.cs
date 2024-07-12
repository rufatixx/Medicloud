using System;
using Medicloud.Models.Domain;

namespace Medicloud.Models
{
    public class RecipeDTO
    {
       
        public long serviceID { get; set; }
        public long patientCardID { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string father { get; set; }
        public string phone { get; set; }
        public double price { get; set; }
        public string serviceName { get; set; }
        public string organizationName { get; set; }
        public int isPaid { get; set; }
        public int finished { get; set; }
        public int quantity { get; set; }
    }
}
