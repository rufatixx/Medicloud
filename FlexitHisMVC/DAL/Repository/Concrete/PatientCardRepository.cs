using Dapper;
using Medicloud.DAL.Infrastructure.Abstract;
using Medicloud.DAL.Repository.Abstract;
using Medicloud.Models;
using System.Dynamic;
using System.Text;

namespace Medicloud.DAL.Repository.Concrete
{
    public class PatientCardRepository : IPatientCardRepository
    {
        private readonly IUnitOfWork _unitOfWork;

        public PatientCardRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork=unitOfWork;
        }

        public async Task<List<PatientDocDTO>> GetAllPatientsCards(long organizationID, long patientID)
        {
            var queryBuilder = new StringBuilder($@"
           SELECT a.id as patientCardID,
       a.cDate,
       a.patientID as ID, 
       a.serviceID, 
       a.note, 
       p.name, 
       p.surname, 
       p.father,
       p.clientPhone as phone,
       p.bDate,
       p.genderID,
       p.fin
FROM patient_card a
INNER JOIN patients p ON a.patientID = p.id
WHERE a.organizationID = @organizationID
ORDER BY a.cDate DESC;");

            // Dynamically add patient condition if patientID is greater than 0
            if (patientID > 0)
            {
                queryBuilder.Append(" AND a.patientID = @patientID");
            }

            var parameters = new { organizationID, patientID = patientID > 0 ? patientID : (object)null };
            try
            {
                var con = _unitOfWork.GetConnection();

                var patientList = await con.QueryAsync<PatientDocDTO>(queryBuilder.ToString(), parameters);
                return patientList.ToList();
            }
            catch (Exception ex)
            {
                StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);
                return new();
            }

        }
    }
}
