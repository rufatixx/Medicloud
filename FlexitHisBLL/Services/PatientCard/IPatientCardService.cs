
using Medicloud.DAL.DAO;

namespace Medicloud.BLL.Services.PatientCard
{
    public interface IPatientCardService
    {
        Task<List<PatientCardDAO>> GetAllPatientsCards(long organizationID, long patientID);
    }
}
