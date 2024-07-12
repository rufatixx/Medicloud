using System.Text;
using Medicloud.BLL.DTO;
using Medicloud.DAL.Repository;
using Medicloud.Data;
using Medicloud.Models;
using Medicloud.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Medicloud.BLL.Service
{
    public class SpecialityService
    { 

        private readonly SpecialityRepo _specialityRepo;

        public SpecialityService(SpecialityRepo specialityRepo)
        {
            _specialityRepo = specialityRepo;
        }

        public List<Speciality> GetSpecialities()
        {
            return _specialityRepo.GetSpecialities();
        }

    }


}




