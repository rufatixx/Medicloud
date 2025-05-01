using Medicloud.BLL.Services.Abstract;
using Medicloud.DAL.Entities;
using Medicloud.DAL.Infrastructure.Abstract;
using Medicloud.DAL.Repository.PatientCard;
using Medicloud.Models;
using Medicloud.Models.Domain;

namespace Medicloud.BLL.Services.Concrete
{
	public class PatientCardService : IPatientCardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPatientCardRepository _repository;

        public PatientCardService(IUnitOfWork unitOfWork, IPatientCardRepository repository)
        {
            _unitOfWork=unitOfWork;
            _repository=repository;
        }

        public async Task<List<PatientDocDTO>> GetAllPatientsCards(long organizationID, long patientID,int doctorID)
        {
            using var con = _unitOfWork.BeginConnection();
            var result=await _repository.GetAllPatientsCards(organizationID, patientID,doctorID);
            return result;
        }



        public async Task<List<PatientDocDTO>> GetAllCardsByStaffID(long organizationID, long patientID)
        {
            using var con = _unitOfWork.BeginConnection();
            var result = await _repository.GetAllPatientsCards(organizationID, patientID);

            //foreach (var item in result)
            //{
            //    _repository.
            //}
            return result;
        }

        public async Task<List<PatientCardDAO>> GetPatientsCardsByDate(DateTime date, long organizationID, int doctorID = 0)
        {
            using var con = _unitOfWork.BeginConnection();
            var result = await _repository.GetPatientsCardsByDate(date,organizationID, doctorID);

            //foreach (var item in result)
            //{
            //    _repository.
            //}
            return result;
        }
    }
}
