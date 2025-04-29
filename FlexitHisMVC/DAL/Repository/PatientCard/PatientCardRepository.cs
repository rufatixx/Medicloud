using Dapper;
using Medicloud.DAL.Entities;
using Medicloud.DAL.Infrastructure.Abstract;
using Medicloud.Models;
using System.Dynamic;
using System.Text;

namespace Medicloud.DAL.Repository.PatientCard
{
	public class PatientCardRepository : IPatientCardRepository
	{
		private readonly IUnitOfWork _unitOfWork;

		public PatientCardRepository(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<List<PatientDocDTO>> GetAllPatientsCards(long organizationID, long patientID, int doctorID = 0)
		{
			var queryBuilder = new StringBuilder($@"
        SELECT a.id as patientCardID,
               a.cDate,
               a.patientID as ID, 
               a.serviceID, 
               a.note,
               a.finished,
               p.name, 
               p.surname, 
               p.father,
               p.clientPhone as phone,
               p.bDate,
               p.genderID,
               p.fin
        FROM patient_card a
        LEFT JOIN patients p ON a.patientID = p.id
        WHERE a.organizationID = @organizationID");

			// Dynamically add patient condition if patientID is greater than 0
			if (patientID > 0)
			{
				queryBuilder.Append(" AND a.patientID = @patientID");
			}
			if (doctorID > 0)
			{
				queryBuilder.Append(" AND a.userID = @UserID");
			}
			queryBuilder.Append(" ORDER BY a.cDate DESC");

			var parameters = new { organizationID, patientID = patientID > 0 ? patientID : (object)null, UserID = doctorID > 0 ? doctorID : (object)null };
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

		public async Task<int> AddAsync(PatientCardDAO dao)
		{
			string AddSql = $@"
			INSERT INTO patient_card
            (requestTypeID,userID,patientID,organizationID,serviceID,docID,priceGroupID,note,referDocID,startDate,endDate)
			VALUES (@{nameof(PatientCardDAO.requestTypeID)},
            @{nameof(PatientCardDAO.userID)},
            @{nameof(PatientCardDAO.patientID)},
            @{nameof(PatientCardDAO.organizationID)},
            @{nameof(PatientCardDAO.serviceID)},
            @{nameof(PatientCardDAO.docID)},
            @{nameof(PatientCardDAO.priceGroupID)},
            @{nameof(PatientCardDAO.note)},
            @{nameof(PatientCardDAO.referDocID)},
            @{nameof(PatientCardDAO.startDate)},
            @{nameof(PatientCardDAO.endDate)});

			SELECT LAST_INSERT_ID();";
			var con = _unitOfWork.BeginConnection();
			var newId = await con.QuerySingleOrDefaultAsync<int>(AddSql, dao);
			return newId;
		}
	}
}
