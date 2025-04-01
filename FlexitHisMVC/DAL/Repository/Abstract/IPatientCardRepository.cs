using Medicloud.Models;

namespace Medicloud.DAL.Repository.Abstract
{
    public interface IPatientCardRepository
    {
        Task<List<PatientDocDTO>> GetAllPatientsCards(long organizationID, long patientID, int doctorID = 0);
    }
}
