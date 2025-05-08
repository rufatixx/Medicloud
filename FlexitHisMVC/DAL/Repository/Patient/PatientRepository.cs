using Dapper;
using Medicloud.DAL.Entities;
using Medicloud.DAL.Infrastructure.Abstract;

namespace Medicloud.DAL.Repository.Patient
{
	public class PatientRepository:IPatientRepository
	{
		private readonly IUnitOfWork _unitOfWork;

		public PatientRepository(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<int> AddAsync(PatientDAO patientDAO)
		{
			string checkSql = @$"SELECT id FROM patients 
		WHERE clientPhone = @ClientPhone 
			AND organizationID = @organizationID";

			string AddSql = $@"
			INSERT INTO patients
            (userID,name,surname,father,clientPhone,organizationId,bDate,genderID,fin,email,orgReasonId)
			VALUES (@{nameof(PatientDAO.userID)},
            @{nameof(PatientDAO.name)},
            @{nameof(PatientDAO.surname)},
            @{nameof(PatientDAO.father)},
            @{nameof(PatientDAO.clientPhone)},
            @{nameof(PatientDAO.organizationID)},
            @{nameof(PatientDAO.bDate)},
            @{nameof(PatientDAO.genderID)},
            @{nameof(PatientDAO.fin)},
            @{nameof(PatientDAO.clientEmail)},
            @{nameof(PatientDAO.orgReasonId)});

			SELECT LAST_INSERT_ID();";
			var con = _unitOfWork.BeginConnection();

			int existId = await con.QuerySingleOrDefaultAsync<int>(checkSql, new
			{
				ClientPhone = patientDAO.clientPhone,
				organizationID = patientDAO.organizationID,
			});
			if (existId == 0)
			{
				var newId = await con.QuerySingleOrDefaultAsync<int>(AddSql, patientDAO);
				return newId;
			}
			return existId;

		}

		public Task<PatientDAO> GetByIdAsync(int id)
		{
			throw new NotImplementedException();
		}

		public async Task<List<PatientDAO>> GetPatientsWithCardsByDr(int docID, int orgID)
		{
			//Console.WriteLine(docID);
			//Console.WriteLine(orgID);
			string query = $@"SELECT 
    p.id,
    p.name,
    p.surname,
    p.father,
    p.clientPhone,
    p.bDate,
    p.genderID,
    p.fin,
    a.id AS PatientCardID,
	a.id,
    a.serviceID,
    a.startDate,
    a.docID
FROM 
    patients p
INNER JOIN 
    patient_card a ON p.id = a.patientID
WHERE 
    a.docID = @docID 
    AND a.organizationID = @orgID 
    AND a.finished = 0
ORDER BY 
     a.startDate DESC;
 ;
            ";

			var con = _unitOfWork.GetConnection();
			//var result =  con.Query<PatientDocDTO>(query, new { docID = docID, orgID = orgID });
			//return result.ToList();

			var patients = await con.QueryAsync<PatientDAO, PatientCardDAO, PatientDAO>(
				query,
				(patient, card) =>
				{
					patient.Cards = new();
					patient.Cards.Add(card);
					return patient;
				},
				new { docID, orgID },
				splitOn: "PatientCardID"
			);

			return patients.ToList();
		}
	}
}
