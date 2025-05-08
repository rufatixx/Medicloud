using Dapper;
using Medicloud.BLL.Models;
using Medicloud.DAL.Entities;
using Medicloud.DAL.Infrastructure.Abstract;
using Medicloud.Models;
using Medicloud.Models.Domain;
using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
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
            (requestTypeID,userID,patientID,organizationID,serviceID,docID,priceGroupID,note,referDocID,startDate,endDate,isOnline,companyID)
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
            @{nameof(PatientCardDAO.endDate)},
            @{nameof(PatientCardDAO.isOnline)},
            @{nameof(PatientCardDAO.companyID)});

			SELECT LAST_INSERT_ID();";
			var con = _unitOfWork.BeginConnection();
			var newId = await con.QuerySingleOrDefaultAsync<int>(AddSql, dao);
			return newId;
		}

		public async Task<List<PatientCardDAO>> GetPatientsCardsByDate(DateTime date, long organizationID, int doctorID = 0)
		{
			var queryBuilder = new StringBuilder($@"
        SELECT a.*
        FROM patient_card a
        WHERE a.organizationID = @organizationID AND DATE(a.startDate) = DATE(@SelectedDate)");

			// Dynamically add patient condition if patientID is greater than 0
			if (doctorID > 0)
			{
				queryBuilder.Append(" AND a.docID = @UserID");
			}
			queryBuilder.Append(" ORDER BY a.cDate DESC");

			var parameters = new { organizationID, UserID = doctorID > 0 ? doctorID : (object)null, SelectedDate = date };
			try
			{
				var con = _unitOfWork.GetConnection();

				var patientList = await con.QueryAsync<PatientCardDAO>(queryBuilder.ToString(), parameters);
				return patientList.ToList();
			}
			catch (Exception ex)
			{
				StandardMessages.CallSerilog(ex);
				Console.WriteLine(ex.Message);
				return new();
			}
		}

		public async Task<PatientCardDAO> GetPatientCardById(int id)
		{
			var queryBuilder = new StringBuilder($@"
        SELECT a.*
        FROM patient_card a
        WHERE a.id=@Id");

			// Dynamically add patient condition if patientID is greater than 0
			try
			{
				var con = _unitOfWork.GetConnection();

				var patientCard = await con.QuerySingleOrDefaultAsync<PatientCardDAO>(queryBuilder.ToString(), new { Id = id });
				return patientCard;
			}
			catch (Exception ex)
			{
				StandardMessages.CallSerilog(ex);
				Console.WriteLine(ex.Message);
				return new();
			}
		}



		public async Task<List<AppointmentViewModel>> GetCardsByRange(DateTime startDate, DateTime endDate, int organizationID, int userID)
		{

			string userFilter = userID > 0 ? "AND pc.docID = @userID" : string.Empty;
			string query = @$"
        SELECT 
            pc.id,
            pc.patientID AS patient_id,
            pc.docID AS user_id,
            pc.serviceID AS service_id,
            pc.organizationID AS organization_id,
            pc.startDate AS start_date,
            pc.endDate AS end_date,
            pc.isActive AS is_active,
            u.name AS user_name,
            u.surname AS user_surname,
            u.specialityID AS user_speciality_id,
            sp.name AS user_speciality_name
        FROM medicloud.patient_card pc
        LEFT JOIN medicloud.users u ON u.id = pc.docID
        LEFT JOIN medicloud.speciality sp ON sp.id = u.specialityID
        WHERE pc.startDate BETWEEN @startDate AND @endDate
          
          AND pc.organizationID = @organizationID
          {userFilter}
        ORDER BY pc.startDate ASC;";



			try
			{
				var con = _unitOfWork.GetConnection();
				var result = await con.QueryAsync<AppointmentViewModel>(query, new
				{
					startDate = startDate.Date,
					endDate = endDate.Date.AddDays(1).AddSeconds(-1),
					organizationID = organizationID,
					userID = userID,
				});
				return result.ToList();
			}
			catch (Exception ex)
			{
				Medicloud.StandardMessages.CallSerilog(ex);
				Console.WriteLine(ex.Message);
				return new List<AppointmentViewModel>();
			}
		}


		public async Task<List<AppointmentViewModel>> GetCardsByDate(DateTime date, long organizationID, int doctorID = 0)
		{

			string query = @"
        SELECT
				pc.id,
				pc.patientID AS patient_id,
				pc.docID AS user_id,
				pc.serviceID AS service_id,
				pc.organizationID AS organization_id,
				pc.startDate AS start_date,
				pc.endDate AS end_date,
				pc.isActive AS is_active,
               p.name AS patient_name,
               p.surname AS patient_surname,
               p.clientPhone AS patient_phone,
               s.name AS service_name,
               u.name AS user_name,
               u.surname AS user_surname,
               u.specialityID AS user_speciality_id,
               sp.name AS user_speciality_name
        FROM medicloud.patient_card pc
        LEFT JOIN medicloud.patients p ON p.id = pc.patientID
        LEFT JOIN medicloud.services s ON s.id = pc.serviceID
        LEFT JOIN medicloud.users u ON u.id = pc.docID
        LEFT JOIN medicloud.speciality sp ON sp.id = u.specialityID
        WHERE DATE(pc.startDate) = DATE(@date)
         
          AND pc.organizationID = @organizationID
          AND (@userID = 0 OR pc.docID = @userID)
        ORDER BY pc.startDate ASC;";

			try
			{
				var con = _unitOfWork.GetConnection();

				var patientList = await con.QueryAsync<AppointmentViewModel>(query, new
				{
					date=date,
					organizationID = organizationID,
					userID=doctorID
				});
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


