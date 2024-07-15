using Medicloud.DAL.Repository;
using Medicloud.Models.Domain;
using Medicloud.Models.Repository;

namespace Medicloud.BLL.Service
{
    public class PatientService
    {
        private readonly string _connectionString;
        readonly PatientRepo _patientRepo;

        public PatientService(string conString)
        {
            _connectionString = conString;
            _patientRepo = new PatientRepo(_connectionString);
        }
        public IEnumerable<Patient> GetPatientByName(string keyword)
        {
            var result = _patientRepo.GetPatientByName(keyword);
            return result;
        }

        public Patient GetPatientById(string id)
        {
            var result = _patientRepo.GetPatientById(id);
            return result;
        }
    }
}
