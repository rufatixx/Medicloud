using System;
using FlexitHisMVC.Models;

namespace FlexitHisMVC.Areas.Admin.Model
{
    public class PersonalPageDTO
    {
        public List<User> personalList { get; set; }
        public List<Speciality> specialityList { get; set; }
        public List<Hospital> hospitalList { get; set; }
      
    }
}

