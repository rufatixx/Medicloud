using Medicloud.BLL.Services.Abstract;
using Medicloud.DAL.Infrastructure.Abstract;
using Medicloud.DAL.Repository.Abstract;
using Medicloud.Models;

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

        public Task<List<PatientDocDTO>> GetAllPatientsCards(long organizationID, long patientID)
        {
            using var con = _unitOfWork.BeginConnection();
            var result=_repository.GetAllPatientsCards(organizationID, patientID);
            return result;
        }
    }
}
