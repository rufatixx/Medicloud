using Medicloud.Models;

namespace Medicloud.BLL.Services.Abstract
{
    public interface IPatientCardService
    {
        Task<List<PatientDocDTO>> GetAllPatientsCards(long organizationID, long patientID,int doctorID=0);
    }
}
