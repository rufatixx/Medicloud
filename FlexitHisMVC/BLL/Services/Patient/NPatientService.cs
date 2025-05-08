using Medicloud.DAL.Entities;
using Medicloud.DAL.Infrastructure.Abstract;
using Medicloud.DAL.Repository.Patient;

namespace Medicloud.BLL.Services.Patient
{
	public class NPatientService:IPatientService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IPatientRepository _patientRepository;

		public NPatientService(IUnitOfWork unitOfWork, IPatientRepository patientRepository)
		{
			_unitOfWork = unitOfWork;
			_patientRepository = patientRepository;
		}

		public async Task<List<PatientDAO>> GetPatientsWithCardsByDr(int docID, int orgID)
		{
			using var con= _unitOfWork.BeginConnection();
			var result=await _patientRepository.GetPatientsWithCardsByDr(docID, orgID);
			return result;
		}
	}
}
