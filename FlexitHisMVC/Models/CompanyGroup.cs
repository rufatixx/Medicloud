using System;
namespace FlexitHisMVC.Models
{
    public class CompanyGroup
    {
        public long id { get; set; }
        public string name { get; set; }
        public int type { get; set; }
        public int isActive { get; set; }
        public DateTime cdate { get; set; }
        
    }
}
