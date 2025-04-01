using Medicloud.DAL.Entities;

namespace Medicloud.DAL.Repository.PatientCard
{
	public interface IPatientCardServiceRelRepository
	{
		Task<bool> InsertServiceToPatientCard(long patientCardID, int serviceID, int? depID, int? senderDocID, int? docID);
		Task<bool> RemoveServiceFromPatientCard(long patientCardID, int serviceID);
		Task<bool> RemovePatientServiceById(int id);
		Task<List<PatientCardServiceDAO>> GetServicesFromPatientCard(int patientCardID, int organizationID);
		Task<List<StaffDAO>> GetDoctorsFromPatientCard(int patientCardID);
	}
}
