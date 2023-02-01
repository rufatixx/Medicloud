using System;
namespace FlexitHisMVC.Models
{
    public class Company
    {
        public long id { get; set; }
        public string name { get; set; }
        public int groupID { get; set; }
        public int isActive { get; set; }
        public int cUserID { get; set; }
        public DateTime cdate { get; set; }
    }
}
