using System;
using Medicloud.Models;

namespace Medicloud.Areas.Admin.Model
{
    public class PersonalPageDTO
    {
        public List<User> personalList { get; set; }
        public List<Speciality> specialityList { get; set; }
        public List<Organization> organizationList { get; set; }
      
    }
}

