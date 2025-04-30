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
		WHERE name = @name 
			AND surname = @surname 
			AND father = @father 
			AND bDate = @bDate 
			AND organizationID = @organizationID";

			string AddSql = $@"
			INSERT INTO patients
            (userID,name,surname,father,clientPhone,organizationId,bDate,genderID,fin,email)
			VALUES (@{nameof(PatientDAO.userID)},
            @{nameof(PatientDAO.name)},
            @{nameof(PatientDAO.surname)},
            @{nameof(PatientDAO.father)},
            @{nameof(PatientDAO.clientPhone)},
            @{nameof(PatientDAO.organizationID)},
            @{nameof(PatientDAO.bDate)},
            @{nameof(PatientDAO.genderID)},
            @{nameof(PatientDAO.fin)},
            @{nameof(PatientDAO.clientEmail)});

			SELECT LAST_INSERT_ID();";
			var con = _unitOfWork.BeginConnection();

			int existId = await con.QuerySingleOrDefaultAsync<int>(checkSql, new
			{
				name = patientDAO.name,
				surname = patientDAO.surname,
				father = patientDAO.father,
				bDate = patientDAO.bDate,
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
	}
}
