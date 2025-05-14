using Medicloud.DAL.Entities;
using Medicloud.DAL.Infrastructure.Abstract;
using Medicloud.DAL.Repository.Patient;
using Medicloud.Models;

namespace Medicloud.BLL.Services.Patient
{
	public class NPatientService : IPatientService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IPatientRepository _patientRepository;

		public NPatientService(IUnitOfWork unitOfWork, IPatientRepository patientRepository)
		{
			_unitOfWork = unitOfWork;
			_patientRepository = patientRepository;
		}

		public async Task<List<PatientDAO>> GetPatientsWithCardsByDr(int orgID, int docID = 0)
		{
			using var con = _unitOfWork.BeginConnection();
			var result = await _patientRepository.GetPatientsWithCards(orgID, docID);
			return result;
		}

		public async Task<List<PatientDocDTO>> GetPatients(int orgID, int docID = 0, string search = null)
		{
			using var con = _unitOfWork.BeginConnection();
			var data = await _patientRepository.GetPatientsWithCards(orgID, docID, search,false);
			var result = new List<PatientDocDTO>();
			if (data != null)
			{
				foreach (var item in data)
				{
					var index = result.FindIndex(p => p.ID == item.id);
					if (index == -1)
					{
						result.Add(new()
						{
							bDate = DateTime.Parse(item?.bDate??"01.01.0001"),
							email = item.clientEmail,
							phone = item.clientPhone,
							totalCardNumbers = item.Cards?.Count ?? 0,
							patientCardID = item.Cards?.LastOrDefault()?.id ?? 0,
							genderID = item.genderID,
							ID = item.id,
							name = item.name,
							surname = item.surname,
							father=item.father,

						});
					}
					else
					{
						result[index].totalCardNumbers += 1;
					}

				}
			}

			return result;
		}
	}
}
