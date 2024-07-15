using Medicloud.BLL.Models;
using Medicloud.DAL.Entities;
using Microsoft.Extensions.FileSystemGlobbing;
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

        public bool InsertAppointment(Appointment appointment)
        {
            MySqlConnection con = new(ConnectionString);
            string query = @"INSERT INTO appointments (patient_id, service_id, organization_id, start_date, end_date)
                VALUES (@patient_id, @service_id, @organization_id, @start_date, @end_date)";

            MySqlCommand cmd = new(query, con);
            cmd.Parameters.AddWithValue("@patient_id", appointment.patient_id);
            cmd.Parameters.AddWithValue("@service_id", appointment.service_id);
            cmd.Parameters.AddWithValue("@organization_id", appointment.organization_id);
            cmd.Parameters.AddWithValue("@start_date", appointment.start_date);
            cmd.Parameters.AddWithValue("@end_date", appointment.end_date);

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

        public IEnumerable<AppointmentViewModel> GetAllAppointment()
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
WHERE is_active = 1";

            MySqlCommand cmd = new(query, con);

          
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
							 p.clientPhone patient_phone,
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
                        organization_id = reader["organization_id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["organization_id"]),
                        start_date = reader["start_date"] == DBNull.Value ? Convert.ToDateTime("0001-01-01T00:00:00") : Convert.ToDateTime(reader["start_date"]),
                        end_date = reader["end_date"] == DBNull.Value ? Convert.ToDateTime("0001-01-01T00:00:00") : Convert.ToDateTime(reader["end_date"]),
                        is_active = Convert.ToBoolean(reader["is_active"]),
                        patient_name = reader["patient_name"].ToString(),
						patient_phone = reader["patient_phone"].ToString(),
                        patient_surname =  reader["patient_surname"].ToString(),
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

		public bool UpdateAppointment(Appointment appointment)
		{
			MySqlConnection con = new(ConnectionString);
			string query = $@"UPDATE medicloud.appointments 
SET patient_id = @patient_id, service_id = @service_id, start_date = @start_date, end_date = @end_date
WHERE id = @id";

			MySqlCommand cmd = new(query, con);
			cmd.Parameters.AddWithValue("@id", appointment.id);
			cmd.Parameters.AddWithValue("@patient_id", appointment.patient_id);
			cmd.Parameters.AddWithValue("@service_id", appointment.service_id);
			cmd.Parameters.AddWithValue("@start_date", appointment.start_date);
			cmd.Parameters.AddWithValue("@end_date", appointment.end_date);


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

    }
}
