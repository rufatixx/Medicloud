using System;
namespace FlexitHisMVC.Models
{
    public class ServiceGroup
    {
        public int ID { get; set; }
        public string name { get; set; }
        public bool isHeading { get; set; }
        public int hospitalID { get; set; }
        public int parent { get; set; }

    }
}

