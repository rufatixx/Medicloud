using System;
namespace FlexitHisMVC.Areas.Admin.Models.Admin
{
    public struct PersonalDataStruct
    {
        public int ID { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string father { get; set; }
        public string speciality { get; set; }
        public bool isActive { get; set; }
        public bool isUser { get; set; }
       
    }
}

