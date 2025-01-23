using Medicloud.DAL.DAO;

namespace Medicloud.DAL.Repository.PatientCard
{
    public interface IPatientCardRepository
    {
        Task<List<PatientCardDAO>> GetAllPatientsCards(long organizationID, long patientID);
    }
}
