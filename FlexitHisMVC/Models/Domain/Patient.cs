using System;
namespace FlexitHisMVC.Models.Domain
{
	public class Patient
	{
        public long ID { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string father { get; set; }
        public int genderID { get; set; }
        public long phone { get; set; }
        public string fin { get; set; }
        public DateTime bDate { get; set; }
    }
}

