
using Medicloud.DAL.Entities;

namespace Medicloud.DAL.Repository.Patient
{
	public interface IPatientRepository
	{
		Task<int>AddAsync(PatientDAO patientDAO);
		Task<PatientDAO> GetByIdAsync(int id);

	}
}
