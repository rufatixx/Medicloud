using Medicloud.BLL.Models;
using Medicloud.DAL.Entities;
using MySql.Data.MySqlClient;

namespace Medicloud.DAL.Repository
{
    public class AppointmentRepo
	{
		private readonly string ConnectionString;

		public AppointmentRepo(string conString)
		{
			ConnectionString = conString;
		}

		public bool InsertAppointment(AppointmentDAO appointment)
		{
			MySqlConnection con = new(ConnectionString);
			string query = @"INSERT INTO appointments (patient_id, service_id, organization_id, start_date, end_date, user_id, patient_phone)
                VALUES (@patient_id, @service_id, @orgID, @start_date, @end_date, @user_id, @patient_phone)";

			MySqlCommand cmd = new(query, con);
			cmd.Parameters.AddWithValue("@patient_id", appointment.patient_id);
			cmd.Parameters.AddWithValue("@service_id", appointment.service_id);
			cmd.Parameters.AddWithValue("@orgID", appointment.organization_id);
			cmd.Parameters.AddWithValue("@start_date", appointment.start_date);
			cmd.Parameters.AddWithValue("@end_date", appointment.end_date);
			cmd.Parameters.AddWithValue("@user_id", appointment.user_id);
			cmd.Parameters.AddWithValue("@patient_phone", appointment.patient_phone);

			try
			{
				con.Open();
				cmd.ExecuteNonQuery();
			}
			catch (MySqlException ex)
			{
				Medicloud.StandardMessages.CallSerilog(ex);
				Console.WriteLine(ex.Message);
				return false;
			}
			finally
			{
				con.Close();
			}
			return true;
		}

		public IEnumerable<AppointmentViewModel> GetAllAppointment(long organizationID)
		{
			List<AppointmentViewModel> appointments = new List<AppointmentViewModel>();
			MySqlConnection con = new(ConnectionString);

			string query = @"SELECT a.*,
 p.name patient_name,
 p.surname patient_surname,
 p.id patient_id,
 s.name service_name,
 s.id service_id
FROM appointments a
LEFT JOIN patients p on p.id=a.patient_id
LEFT JOIN services s on s.id=a.service_id
WHERE organization_id = @orgID and is_active = 1";

			MySqlCommand cmd = new(query, con);

            cmd.Parameters.AddWithValue("@orgID", organizationID);
            try
			{
				con.Open();
				MySqlDataReader reader = cmd.ExecuteReader();

				while (reader.Read())
				{
					AppointmentViewModel appointment = new AppointmentViewModel
					{
						id = Convert.ToInt32(reader["id"]),
						patient_id = Convert.ToInt32(reader["patient_id"]),
						service_id = Convert.ToInt32(reader["service_id"]),
						user_id = Convert.ToInt32(reader["user_id"]),
						organization_id = reader["organization_id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["organization_id"]),
						start_date = reader["start_date"] == DBNull.Value ? Convert.ToDateTime("0001-01-01T00:00:00") : Convert.ToDateTime(reader["start_date"]),
						end_date = reader["end_date"] == DBNull.Value ? Convert.ToDateTime("0001-01-01T00:00:00") : Convert.ToDateTime(reader["end_date"]),
						is_active = Convert.ToBoolean(reader["is_active"]),
						patient_name = reader["patient_name"] == DBNull.Value ? "" : reader["patient_name"].ToString(),
						patient_surname = reader["patient_surname"] == DBNull.Value ? "" : reader["patient_surname"].ToString(),
						service_name = reader["service_name"] == DBNull.Value ? "" : reader["service_name"].ToString()
					};

					appointments.Add(appointment);
				}
				appointments.Reverse();
				con.Close();
			}
			catch (Exception ex)
			{
				Medicloud.StandardMessages.CallSerilog(ex);
				Console.WriteLine(ex.Message);
			}

			return appointments;
		}

		public AppointmentViewModel GetAppointmentById(string id)
		{
			AppointmentViewModel appointmentViewModel = new();

			MySqlConnection con = new(ConnectionString);
			string query = @"SELECT a.*,
                             p.name patient_name,
                             p.surname patient_surname,
                             p.id patient_id,
                             s.name service_name,
                             s.id service_id
                            FROM appointments a
                            LEFT JOIN patients p on p.id=a.patient_id
                            LEFT JOIN services s on s.id=a.service_id
                            WHERE a.id=@id";

			MySqlCommand cmd = new(query, con);
			cmd.Parameters.AddWithValue("@id", Convert.ToInt32(id));

			try
			{
				con.Open();
				MySqlDataReader reader = cmd.ExecuteReader();

				if (reader.Read())
				{
					appointmentViewModel = new AppointmentViewModel
					{
						id = Convert.ToInt32(reader["id"]),
						patient_id = Convert.ToInt32(reader["patient_id"]),
						service_id = Convert.ToInt32(reader["service_id"]),
						user_id = Convert.ToInt32(reader["user_id"]),
						organization_id = reader["organization_id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["organization_id"]),
						start_date = reader["start_date"] == DBNull.Value ? Convert.ToDateTime("0001-01-01T00:00:00") : Convert.ToDateTime(reader["start_date"]),
						end_date = reader["end_date"] == DBNull.Value ? Convert.ToDateTime("0001-01-01T00:00:00") : Convert.ToDateTime(reader["end_date"]),
						is_active = Convert.ToBoolean(reader["is_active"]),
						patient_name = reader["patient_name"].ToString(),
						patient_phone = reader["patient_phone"].ToString(),
						patient_surname = reader["patient_surname"].ToString(),
						service_name = reader["service_name"].ToString()
					};
				}
				con.Close();
			}
			catch (Exception ex)
			{
				Medicloud.StandardMessages.CallSerilog(ex);
				Console.WriteLine(ex.Message);
			}
			return appointmentViewModel;
		}

		public bool DeleteAppointment(string id)
		{
			MySqlConnection con = new(ConnectionString);
			var query = $@"UPDATE appointments SET is_active=0 WHERE id=@id";

			MySqlCommand cmd = new(query, con);
			cmd.Parameters.AddWithValue("@id", Convert.ToInt32(id));

			try
			{
				con.Open();
				var result = cmd.ExecuteNonQuery();

				con.Close();
				return true;
			}
			catch (Exception ex)
			{
				Medicloud.StandardMessages.CallSerilog(ex);
				Console.WriteLine(ex.Message);
			}

			return false;
		}

		public bool UpdateAppointment(AppointmentDAO appointment)
		{
			MySqlConnection con = new(ConnectionString);
			string query = $@"UPDATE medicloud.appointments 
SET patient_id = @patient_id,organization_id=@orgID, service_id = @service_id,
    start_date = @start_date, end_date = @end_date, user_id=@user_id,
    patient_phone = @patient_phone
WHERE id = @id";

			MySqlCommand cmd = new(query, con);
			cmd.Parameters.AddWithValue("@id", appointment.id);
			cmd.Parameters.AddWithValue("@patient_id", appointment.patient_id);
			cmd.Parameters.AddWithValue("@orgID", appointment.organization_id);
			cmd.Parameters.AddWithValue("@service_id", appointment.service_id);
			cmd.Parameters.AddWithValue("@start_date", appointment.start_date);
			cmd.Parameters.AddWithValue("@end_date", appointment.end_date);
			cmd.Parameters.AddWithValue("@user_id", appointment.user_id);
			cmd.Parameters.AddWithValue("@patient_phone", appointment.patient_phone);


			try
			{
				con.Open();
				cmd.ExecuteNonQuery();
			}
			catch (MySqlException ex)
			{
				Medicloud.StandardMessages.CallSerilog(ex);
				Console.WriteLine(ex.Message);
				return false;
			}
			finally
			{
				con.Close();
			}
			return true;
		}

        public AppointmentPagedResult GetAllAppointments(long organizationID, string searchQuery, int userID = 0, int pageNumber = 1)
        {
            const int pageSize = 10;
            var result = new AppointmentPagedResult
            {
                Appointments = new List<AppointmentViewModel>(),
                CurrentPage = pageNumber
            };

            int totalRecords = 0;

            try
            {
                using var connection = new MySqlConnection(ConnectionString);
                connection.Open();

                // Base query with conditional filter for userID
                var baseQuery = @"
            FROM appointments a
            LEFT JOIN patients p ON p.id = a.patient_id
            LEFT JOIN services s ON s.id = a.service_id
            LEFT JOIN users u ON u.id = a.user_id
            LEFT JOIN speciality sp ON sp.id = u.specialityID
            WHERE 
                (a.organization_id = @orgID AND a.is_active = 1)
                AND (@userID <= 0 OR a.user_id = @userID)
                AND (
                    @SearchQuery IS NULL
                    OR LOWER(p.name) LIKE LOWER(@SearchQuery)
                    OR LOWER(s.name) LIKE LOWER(@SearchQuery)
                    OR LOWER(u.name) LIKE LOWER(@SearchQuery)
                    OR LOWER(u.surname) LIKE LOWER(@SearchQuery)
                    OR LOWER(sp.name) LIKE LOWER(@SearchQuery)
                )
        ";

                // Count query
                var countQuery = $"SELECT COUNT(*) {baseQuery}";

                using var countCmd = new MySqlCommand(countQuery, connection);
                countCmd.Parameters.AddWithValue("@SearchQuery", string.IsNullOrEmpty(searchQuery) ? DBNull.Value : $"%{searchQuery}%");
                countCmd.Parameters.AddWithValue("@orgID", organizationID);
                countCmd.Parameters.AddWithValue("@userID", userID);

                totalRecords = Convert.ToInt32(countCmd.ExecuteScalar());

                // Main SELECT query
                var appointmentQuery = $@"
            SELECT 
                a.*,
                p.name AS patient_name,
                p.surname AS patient_surname,
                p.id AS patient_id,
                s.name AS service_name,
                s.id AS service_id,
                u.name AS user_name,
                u.surname AS user_surname,
                u.specialityID AS user_speciality_id,
                sp.name AS user_speciality_name
            {baseQuery}
            ORDER BY a.id DESC
            LIMIT {pageSize} OFFSET {(pageNumber - 1) * pageSize}
        ";

                using var appointmentCommand = new MySqlCommand(appointmentQuery, connection);
                appointmentCommand.Parameters.AddWithValue("@SearchQuery", string.IsNullOrEmpty(searchQuery) ? DBNull.Value : $"%{searchQuery}%");
                appointmentCommand.Parameters.AddWithValue("@orgID", organizationID);
                appointmentCommand.Parameters.AddWithValue("@userID", userID);

                using var reader = appointmentCommand.ExecuteReader();

                while (reader.Read())
                {
                    var appointment = new AppointmentViewModel
                    {
                        id = Convert.ToInt32(reader["id"]),
                        patient_id = Convert.ToInt32(reader["patient_id"]),
                        service_id = Convert.ToInt32(reader["service_id"]),
                        user_id = Convert.ToInt32(reader["user_id"]),
                        organization_id = Convert.ToInt32(reader["organization_id"]),
                        start_date = reader["start_date"] == DBNull.Value ? default : Convert.ToDateTime(reader["start_date"]),
                        end_date = reader["end_date"] == DBNull.Value ? default : Convert.ToDateTime(reader["end_date"]),
                        is_active = Convert.ToBoolean(reader["is_active"]),
                        patient_name = reader["patient_name"].ToString(),
                        patient_surname = reader["patient_surname"].ToString(),
                        service_name = reader["service_name"].ToString(),
                        user_name = reader["user_name"]?.ToString(),
                        user_surname = reader["user_surname"]?.ToString(),
                        user_speciality_id = reader["user_speciality_id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["user_speciality_id"]),
                        user_speciality_name = reader["user_speciality_name"]?.ToString()
                    };

                    result.Appointments.Add(appointment);
                }

                result.TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving appointments: " + ex.Message);
                // Optionally handle or re-throw
            }

            return result;
        }


        public List<AppointmentViewModel> GetAppointmentsByRange(DateTime startDate, DateTime endDate, int organizationID, int userID)
        {
            List<AppointmentViewModel> appointments = new();

            string query = @"
        SELECT 
            a.id,
            a.patient_id,
            a.user_id,
            a.service_id,
            a.organization_id,
            a.start_date,
            a.end_date,
            a.is_active,
            u.name AS user_name,
            u.surname AS user_surname,
            u.specialityID AS user_speciality_id,
            sp.name AS user_speciality_name
        FROM medicloud.appointments a
        LEFT JOIN medicloud.users u ON u.id = a.user_id
        LEFT JOIN medicloud.speciality sp ON sp.id = u.specialityID
        WHERE a.start_date BETWEEN @startDate AND @endDate
          AND a.is_active = 1
          AND a.organization_id = @organizationID
          {0}
        ORDER BY a.start_date ASC;";

            string userFilter = userID > 0 ? "AND a.user_id = @userID" : string.Empty;
            query = string.Format(query, userFilter);

            try
            {
                using var con = new MySqlConnection(ConnectionString);
                using var cmd = new MySqlCommand(query, con);

                cmd.Parameters.AddWithValue("@startDate", startDate.Date);
                cmd.Parameters.AddWithValue("@endDate", endDate.Date.AddDays(1).AddSeconds(-1));
                cmd.Parameters.AddWithValue("@organizationID", organizationID);

                if (userID > 0)
                    cmd.Parameters.AddWithValue("@userID", userID);

                con.Open();
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var appointment = new AppointmentViewModel
                    {
                        id = Convert.ToInt32(reader["id"]),
                        patient_id = Convert.ToInt32(reader["patient_id"]),
                        user_id = Convert.ToInt32(reader["user_id"]),
                        service_id = Convert.ToInt32(reader["service_id"]),
                        organization_id = reader["organization_id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["organization_id"]),
                        start_date = reader["start_date"] == DBNull.Value ? default : Convert.ToDateTime(reader["start_date"]),
                        end_date = reader["end_date"] == DBNull.Value ? default : Convert.ToDateTime(reader["end_date"]),
                        is_active = Convert.ToBoolean(reader["is_active"]),
                        user_name = reader["user_name"]?.ToString(),
                        user_surname = reader["user_surname"]?.ToString(),
                        user_speciality_id = reader["user_speciality_id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["user_speciality_id"]),
                        user_speciality_name = reader["user_speciality_name"]?.ToString()
                    };

                    appointments.Add(appointment);
                }
            }
            catch (Exception ex)
            {
                Medicloud.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);
            }

            return appointments;
        }


        public List<AppointmentViewModel> GetAppointmentByDate(DateTime date, int userID, int organizationID)
        {
            List<AppointmentViewModel> appointments = new();

            string query = @"
        SELECT a.*,
               p.name AS patient_name,
               p.surname AS patient_surname,
               p.clientPhone AS patient_phone,
               s.name AS service_name,
               u.name AS user_name,
               u.surname AS user_surname,
               u.specialityID AS user_speciality_id,
               sp.name AS user_speciality_name
        FROM medicloud.appointments a
        LEFT JOIN medicloud.patients p ON p.id = a.patient_id
        LEFT JOIN medicloud.services s ON s.id = a.service_id
        LEFT JOIN medicloud.users u ON u.id = a.user_id
        LEFT JOIN medicloud.speciality sp ON sp.id = u.specialityID
        WHERE DATE(a.start_date) = DATE(@date)
          AND a.is_active = 1
          AND a.organization_id = @organizationID
          AND (@userID = 0 OR a.user_id = @userID)
        ORDER BY a.start_date ASC;";

            try
            {
                using var con = new MySqlConnection(ConnectionString);
                using var cmd = new MySqlCommand(query, con);

                cmd.Parameters.AddWithValue("@date", date.Date);
                cmd.Parameters.AddWithValue("@userID", userID);
                cmd.Parameters.AddWithValue("@organizationID", organizationID);

                con.Open();
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var appointment = new AppointmentViewModel
                    {
                        id = Convert.ToInt32(reader["id"]),
                        patient_id = Convert.ToInt32(reader["patient_id"]),
                        user_id = Convert.ToInt32(reader["user_id"]),
                        service_id = Convert.ToInt32(reader["service_id"]),
                        organization_id = reader["organization_id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["organization_id"]),
                        start_date = reader["start_date"] == DBNull.Value ? default : Convert.ToDateTime(reader["start_date"]),
                        end_date = reader["end_date"] == DBNull.Value ? default : Convert.ToDateTime(reader["end_date"]),
                        is_active = Convert.ToBoolean(reader["is_active"]),
                        patient_name = reader["patient_name"]?.ToString(),
                        patient_surname = reader["patient_surname"]?.ToString(),
                        patient_phone = reader["patient_phone"]?.ToString(),
                        service_name = reader["service_name"]?.ToString(),
                        user_name = reader["user_name"]?.ToString(),
                        user_surname = reader["user_surname"]?.ToString(),
                        user_speciality_id = reader["user_speciality_id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["user_speciality_id"]),
                        user_speciality_name = reader["user_speciality_name"]?.ToString()
                    };

                    appointments.Add(appointment);
                }
            }
            catch (Exception ex)
            {
                Medicloud.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);
            }

            return appointments;
        }


    }
}
