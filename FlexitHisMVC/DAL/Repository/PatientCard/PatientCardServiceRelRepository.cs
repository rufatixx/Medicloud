
using Dapper;
using Medicloud.DAL.Entities;
using Medicloud.DAL.Infrastructure.Abstract;
using MySql.Data.MySqlClient;
using System.Data;
using System.Dynamic;

namespace Medicloud.DAL.Repository.PatientCard
{
	public class PatientCardServiceRelRepository : IPatientCardServiceRelRepository
	{
		private readonly IUnitOfWork _unitOfWork;

		public PatientCardServiceRelRepository(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<bool> InsertServiceToPatientCard(long patientCardID, int serviceID, int? depID, int? senderDocID, int? docID)
		{
			try
			{
				string sql = "INSERT INTO patient_card_service_rel (patientCardID, serviceID";
				var parameters = new DynamicParameters();
				parameters.Add("patientCardID", patientCardID);
				parameters.Add("serviceID", serviceID);

				if (depID.HasValue)
				{
					sql += ", depID";
					parameters.Add("depID", depID.Value);
				}

				if (senderDocID.HasValue)
				{
					sql += ", senderDocID";
					parameters.Add("senderDocID", senderDocID.Value);
				}

				if (docID.HasValue)
				{
					sql += ", docID";
					parameters.Add("docID", docID.Value);
				}

				sql += ") VALUES (@patientCardID, @serviceID";

				if (depID.HasValue)
					sql += ", @depID";
				if (senderDocID.HasValue)
					sql += ", @senderDocID";
				if (docID.HasValue)
					sql += ", @docID";

				sql += ")";

				using var con = _unitOfWork.BeginConnection();
				await con.ExecuteAsync(sql, parameters);
				return true;
			}
			catch (Exception ex)
			{
				Medicloud.StandardMessages.CallSerilog(ex);
				Console.WriteLine(ex.Message);
				return false;
			}

		}

		public async Task<bool> RemoveServiceFromPatientCard(long patientCardID, int serviceID)
		{
			try
			{

				string sql = @"
                UPDATE patient_card_service_rel 
                SET is_removed = 1 
                WHERE patientCardID = @patientCardID 
                  AND serviceID = @serviceID
                  AND is_removed = 0;";  // Ensures that only non-removed services are marked as removed

				using var con = _unitOfWork.BeginConnection();

				int rowsAffected = await con.ExecuteAsync(sql, new { patientCardID, serviceID });
				if (rowsAffected > 0)
				{
					Console.WriteLine("Service successfully marked as removed.");
					return true; // The service was successfully marked as removed
				}
				else
				{
					Console.WriteLine("No service was marked as removed.");
					return false; // No rows were updated, possibly because no matching record was found or it was already removed
				}

			}
			catch (Exception ex)
			{
				Medicloud.StandardMessages.CallSerilog(ex);
				Console.WriteLine(ex.Message);
				return false; // Return false if an exception occurred
			}
		}

		public async Task<bool> RemovePatientServiceById(int id)
		{
			string sql = @"
                UPDATE patient_card_service_rel 
                SET is_removed = 1 
                WHERE id=@id";
			using var con = _unitOfWork.BeginConnection();

			int rowsAffected = await con.ExecuteAsync(sql, new { id });

			if (rowsAffected > 0)
			{
				Console.WriteLine("Service successfully marked as removed.");
				return true; // The service was successfully marked as removed
			}
			else
			{
				Console.WriteLine("No service was marked as removed.");
				return false; // No rows were updated, possibly because no matching record was found or it was already removed
			}
		}

		public async Task<List<PatientCardServiceDAO>> GetServicesFromPatientCard(int patientCardID, int organizationID)
		{

			try
			{
				string query = @"
           SELECT pcsr.*, 
				  p.name AS patientName,
				  p.surname AS patientSurname,
                  p.id as patientID,
                   s.name AS ServiceName, 
                   s.price as ServicePrice,
                   s.code as ServiceCode,
                   sg.name AS ServiceGroup, 
                   doc.name AS DocName, 
                   doc.surname AS DocSurname, 
                   senderDoc.name AS SenderDocName, 
                   senderDoc.surname AS SenderDocSurname,
                   pc.ID as card_id
            FROM patient_card_service_rel pcsr
            left JOIN services s ON pcsr.serviceID = s.id
           left JOIN service_group sg ON s.serviceGroupID = sg.id
            left JOIN users doc ON pcsr.docID = doc.id
           left JOIN users senderDoc ON pcsr.senderDocID = senderDoc.id
           left JOIN patient_card pc ON pcsr.patientCardID = pc.id
           left Join patients p on pc.patientID = p.id
            WHERE pc.organizationID = @organizationID AND pcsr.is_removed = 0" +
				(!patientCardID.Equals(0) ? " AND pcsr.patientCardID = @patientCardID" : "");

				using var con = _unitOfWork.BeginConnection();

				var result =await con.QueryAsync<PatientCardServiceDAO>(query, new { organizationID, patientCardID });
				return result.ToList();
			}
			catch (Exception ex)
			{
				// Обработка ошибок
				Console.WriteLine(ex.Message);
				return null;
			}
		}

		public async Task<List<StaffDAO>> GetDoctorsFromPatientCard(int patientCardID)
		{
			string query = @"
            SELECT pcsr.docID as id, 
                   doc.name, 
                   doc.surname 
            FROM patient_card_service_rel pcsr
            JOIN users doc ON pcsr.docID = doc.id
            WHERE pcsr.patientCardID = @patientCardID
            GROUP BY pcsr.docID";
			using var con=_unitOfWork.BeginConnection();
			var result=await con.QueryAsync<StaffDAO>(query, new { patientCardID });
			return result.ToList();
		}
	}
}
