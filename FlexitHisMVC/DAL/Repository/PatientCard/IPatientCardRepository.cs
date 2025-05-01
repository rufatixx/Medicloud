using Medicloud.DAL.Entities;
using Medicloud.Models;

namespace Medicloud.DAL.Repository.PatientCard
{
	public interface IPatientCardRepository
	{
		Task<List<PatientDocDTO>> GetAllPatientsCards(long organizationID, long patientID, int doctorID = 0);
		Task<List<PatientCardDAO>> GetPatientsCardsByDate(DateTime date, long organizationID, int doctorID = 0);
		Task<int> AddAsync(PatientCardDAO dao);
	}
}
