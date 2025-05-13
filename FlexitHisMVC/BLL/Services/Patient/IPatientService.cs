using Medicloud.DAL.Entities;
using Medicloud.Models;

namespace Medicloud.BLL.Services.Patient
{
	public interface IPatientService
	{
		Task<List<PatientDAO>> GetPatientsWithCardsByDr(int orgID, int docID=0);
		Task<List<PatientDocDTO>> GetPatients(int orgID, int docID=0,string search=null);
	}
}
