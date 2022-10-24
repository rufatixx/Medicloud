using System;
namespace FlexitHis_API.Models.Structs.Admin
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
