using Medicloud.DAL.Entities;

namespace Medicloud.BLL.Services.Patient
{
	public interface IPatientService
	{
		Task<List<PatientDAO>> GetPatientsWithCardsByDr(int docID, int orgID);
	}
}
