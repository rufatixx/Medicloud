
using Medicloud.DAL.DAO;
using Medicloud.DAL.Infrastructure.UnitOfWork;
using Medicloud.DAL.Repository.PatientCard;


namespace Medicloud.BLL.Services.PatientCard
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

        public Task<List<PatientCardDAO>> GetAllPatientsCards(long organizationID, long patientID)
        {
            using var con = _unitOfWork.BeginConnection();
            var result=_repository.GetAllPatientsCards(organizationID, patientID);
            return result;
        }
    }
}
