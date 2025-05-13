using Dapper;
using MailKit.Search;
using Medicloud.DAL.Entities;
using Medicloud.DAL.Infrastructure.Abstract;
using static Mysqlx.Expect.Open.Types;

namespace Medicloud.DAL.Repository.Patient
{
	public class PatientRepository : IPatientRepository
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

		public async Task<List<PatientDAO>> GetPatientsWithCards(int orgID ,int docID=0,string search=null,bool onlyActiveCards=true)
		{
			//Console.WriteLine(docID);
			//Console.WriteLine(orgID);
			var parameters = new DynamicParameters();
			parameters.Add("orgID", orgID);
			string Condition = "";
			if (docID > 0)
			{
				parameters.Add("docID", docID);

				Condition = " AND a.docID = @docID";
			}

			if (!string.IsNullOrWhiteSpace(search))
			{
				search = "%" + search.Trim().ToLower() + "%";
				Condition += " AND LOWER(CONCAT(p.name, ' ', p.surname ,' ',p.father)) LIKE LOWER(@SearchTerm) OR LOWER(p.clientPhone) LIKE LOWER(@SearchTerm)";
				parameters.Add("@SearchTerm", search);
			}

			if (onlyActiveCards)
			{
				Condition = " AND a.isActive=1";
			}
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
    a.docID,
	a.finished,
	a.isActive
FROM 
    patients p
INNER JOIN 
    patient_card a ON p.id = a.patientID
WHERE   
    a.organizationID = @orgID 
    {Condition}
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
				parameters,
				splitOn: "PatientCardID"
			);

			return patients.ToList();
		}

		public async Task<int> UpdateAsync(PatientDAO patientDAO)
		{
			string sql = $@"
			UPDATE patients SET 
            name = @{nameof(PatientDAO.name)},
            surname=@{nameof(PatientDAO.surname)},
            father=@{nameof(PatientDAO.father)},
            clientPhone=@{nameof(PatientDAO.clientPhone)},
            bDate=@{nameof(PatientDAO.bDate)},
            genderID=@{nameof(PatientDAO.genderID)},
            fin=@{nameof(PatientDAO.fin)},
            email=@{nameof(PatientDAO.clientEmail)},
            orgReasonId=@{nameof(PatientDAO.orgReasonId)} 
			WHERE id=@{nameof(PatientDAO.id)};";
			var con = _unitOfWork.BeginConnection();

			var newId = await con.ExecuteAsync(sql, patientDAO);
			return newId;
		}

	}
}
