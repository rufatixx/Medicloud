using System;
namespace FlexitHisMVC.Models
{
    public class ServiceObj
    {
        public int ID { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public double price { get; set; }
        public double newPrice { get; set; }
        public int hospitalID { get; set; }
        public int serviceGroupID { get; set; }
        public int serviceTypeID { get; set; }
        public int servicePriceID { get; set; }
        public string serviceGroup { get; set; }
        public bool isActive { get; set; }

    }
}

