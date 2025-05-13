
using Medicloud.DAL.Entities;

namespace Medicloud.DAL.Repository.Patient
{
	public interface IPatientRepository
	{
		Task<int>AddAsync(PatientDAO patientDAO);
		Task<int>UpdateAsync(PatientDAO patientDAO);
		Task<PatientDAO> GetByIdAsync(int id);
		Task<List<PatientDAO>> GetPatientsWithCards(int orgID, int docID = 0,string search=null,bool onlyActiveCards = true);

	}
}
