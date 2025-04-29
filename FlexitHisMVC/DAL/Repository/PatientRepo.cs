using Medicloud.Models.Domain;
using Medicloud.Models.DTO;
using MySql.Data.MySqlClient;

namespace Medicloud.Models.Repository
{
    public class PatientRepo
    {
        private readonly string ConnectionString;

        public PatientRepo(string conString)
        {
            ConnectionString = conString;
        }


        public long InsertPatient(int userID, long organizationID, PatientDTO newPatient)
        {

            long lastID = 0;
            try
            {

                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {

                    connection.Open();




                    using (MySqlCommand com = new MySqlCommand(@"
INSERT INTO patients (userID, organizationID, name, surname, father, clientPhone,email bDate, genderID, fin) 
SELECT @userID, @organizationID, @name, @surname, @father, @clientPhone,@email @bDate, @genderID, @fin FROM DUAL
WHERE NOT EXISTS (
    SELECT * FROM patients 
    WHERE name = @name 
      AND surname = @surname 
      AND father = @father 
      AND organizationID = @organizationID 
     
);", connection))
                    {
                        com.Parameters.AddWithValue("@userID", userID);
                        com.Parameters.AddWithValue("@organizationID", organizationID);
                        com.Parameters.AddWithValue("@name", newPatient.name);
                        com.Parameters.AddWithValue("@surname", newPatient.surname);
                        com.Parameters.AddWithValue("@father", newPatient.father);
                        com.Parameters.AddWithValue("@clientPhone", newPatient.clientPhone);
                        com.Parameters.AddWithValue("@email", newPatient.clientEmail);
                        com.Parameters.AddWithValue("@bDate", newPatient.birthDate); // Assuming this date is already in the correct format
                        com.Parameters.AddWithValue("@genderID", newPatient.genderID);
                        com.Parameters.AddWithValue("@fin", newPatient.fin);

                        com.ExecuteNonQuery();
                        lastID = com.LastInsertedId; // This will hold the ID of the newly inserted row, or 0 if no row was inserted
                    }



                    connection.Close();


                }


            }
            catch (Exception ex)
            {
                //Medicloud.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);
                //response.status = 4; // not inserted (error)
            }


            return lastID;
        }

        public bool UpdatePatient(PatientDTO updatedPatient)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();

                    using (MySqlCommand com = new MySqlCommand(@"
UPDATE patients SET 
    name = @name, 
    surname = @surname, 
    father = @father, 
    clientPhone = @clientPhone, 
    bDate = @bDate, 
    genderID = @genderID, 
    fin = @fin 
WHERE id = @patientID;", connection))
                    {
                        com.Parameters.AddWithValue("@patientID", updatedPatient.id);
                        com.Parameters.AddWithValue("@name", updatedPatient.name);
                        com.Parameters.AddWithValue("@surname", updatedPatient.surname);
                        com.Parameters.AddWithValue("@father", updatedPatient.father);
                        com.Parameters.AddWithValue("@clientPhone", updatedPatient.clientPhone);
                        com.Parameters.AddWithValue("@bDate", updatedPatient.birthDate); // Ensure this date is correctly formatted
                        com.Parameters.AddWithValue("@genderID", updatedPatient.genderID);
                        com.Parameters.AddWithValue("@fin", updatedPatient.fin);

                        int rowsAffected = com.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Update successful");
                            return true; // Return true if at least one row was updated
                        }
                        else
                        {
                            Console.WriteLine("No rows updated");
                            return false; // No rows updated
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Medicloud.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);
                return false; // Return false if an exception occurred
            }
        }


        public List<Patient> SearchForPatients(string fullNamePattern, long organizationID)

        {

            List<Patient> patientList = new List<Patient>();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {


                    connection.Open();

                    using (MySqlCommand com = new MySqlCommand($@"SELECT * 
FROM patients
WHERE LOWER(CONCAT( name,  ' ', surname )) LIKE  LOWER(@fullNamePattern) and organizationID = @organizationID ", connection))
                    {
                        com.Parameters.AddWithValue("@fullNamePattern", $"%{fullNamePattern}%");
                        com.Parameters.AddWithValue("@organizationID", organizationID);

                        MySqlDataReader reader = com.ExecuteReader();
                        if (reader.HasRows)
                        {


                            while (reader.Read())
                            {

                                Patient patient = new Patient();
                                patient.ID = Convert.ToInt64(reader["id"]);
                                patient.name = reader["name"] == DBNull.Value ? "" : reader["name"].ToString();
                                patient.surname = reader["surname"] == DBNull.Value ? "" : reader["surname"].ToString();
                                patient.father = reader["father"] == DBNull.Value ? "" : reader["father"].ToString();
                                patient.genderID = reader["genderID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["genderID"]);
                                patient.fin = reader["fin"] == DBNull.Value ? "" : Convert.ToString(reader["fin"]);
                                patient.phone = reader["clientPhone"] == DBNull.Value ? 0 : Convert.ToInt64(reader["clientPhone"]);
                                patient.bDate = reader["bDate"] == DBNull.Value ? Convert.ToDateTime("0001-01-01T00:00:00") : Convert.ToDateTime(reader["bDate"]);


                                patientList.Add(patient);


                            }

                            patientList.Reverse();

                            //response.status = 1;
                        }

                    }


                    connection.Close();

                }

            }
            catch (Exception ex)
            {
                Medicloud.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);
                //response.status = 4;
            }


            return patientList;
        }

        public PatientStatisticsDTO GetPatientStatistics(long organizationID)

        {
            PatientStatisticsDTO statisticsDTO = new PatientStatisticsDTO();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {


                    connection.Open();

                    using (MySqlCommand com = new MySqlCommand($@"
SELECT
  new_customers_this_month,
  new_customers_last_month,
  is_grow,
  ROUND(
    CASE
      WHEN new_customers_last_month = 0 THEN NULL
      ELSE (new_customers_this_month - new_customers_last_month) / NULLIF(new_customers_last_month, 0) * 100
    END, 2
  ) AS percentage_change
FROM (
  SELECT
    (SELECT COUNT(*) FROM medicloud.patients
     WHERE DATE_FORMAT(cdate, '%Y-%m') = DATE_FORMAT(CURRENT_DATE, '%Y-%m')
       AND organizationID = @organizationID) AS new_customers_this_month,
    (SELECT COUNT(*) FROM medicloud.patients
     WHERE DATE_FORMAT(cdate, '%Y-%m') = DATE_FORMAT(CURRENT_DATE - INTERVAL 1 MONTH, '%Y-%m')
       AND organizationID = @organizationID) AS new_customers_last_month,
    CASE
      WHEN (SELECT COUNT(*) FROM medicloud.patients
            WHERE DATE_FORMAT(cdate, '%Y-%m') = DATE_FORMAT(CURRENT_DATE, '%Y-%m')
              AND organizationID = @organizationID) >
           (SELECT COUNT(*) FROM medicloud.patients
            WHERE DATE_FORMAT(cdate, '%Y-%m') = DATE_FORMAT(CURRENT_DATE - INTERVAL 1 MONTH, '%Y-%m')
              AND organizationID = @organizationID)
      THEN 1
      ELSE 0
    END AS is_grow
) AS subquery;
", connection))
                    {

                        com.Parameters.AddWithValue("@organizationID", organizationID);

                        MySqlDataReader reader = com.ExecuteReader();
                        if (reader.HasRows)
                        {


                            while (reader.Read())
                            {


                                statisticsDTO.newCustomersThisMonth = reader["new_customers_this_month"] == DBNull.Value ? "" : reader["new_customers_this_month"].ToString();
                                statisticsDTO.newCustomersLasMonth = reader["new_customers_last_month"] == DBNull.Value ? "" : reader["new_customers_last_month"].ToString();
                                statisticsDTO.percentage = reader["percentage_change"] == DBNull.Value ? "" : reader["percentage_change"].ToString();

                                statisticsDTO.isGrow = reader["is_grow"] == DBNull.Value ? 0 : Convert.ToInt32(reader["is_grow"]);




                            }

                            //response.data.Reverse();

                            //response.status = 1;
                        }

                    }


                    connection.Close();

                }

            }
            catch (Exception ex)
            {
                Medicloud.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);
                //response.status = 4;
            }


            return statisticsDTO;
        }

        public IEnumerable<Patient> GetPatientByName(long organizationID,string keyword)
        {
            List<Patient> patients = new List<Patient>();
            MySqlConnection con = new(ConnectionString);
            string query = $@"SELECT * FROM patients WHERE
(
    LOWER(name) LIKE CONCAT('%', LOWER(@search), '%') OR
    LOWER(surname) LIKE CONCAT('%', LOWER(@search), '%') OR
    LOWER(father) LIKE CONCAT('%', LOWER(@search), '%')
)
AND organizationID = @organizationId;";


            MySqlCommand cmd = new(query, con);
            cmd.Parameters.AddWithValue("@search", keyword);
            cmd.Parameters.AddWithValue("@organizationId", organizationID);

            try
            {
                con.Open();
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Patient patient = new Patient
                    {
                        ID = Convert.ToInt64(reader["id"]),
                        name = reader["name"] == DBNull.Value ? "" : reader["name"].ToString(),
                        surname = reader["surname"] == DBNull.Value ? "" : reader["surname"].ToString(),
                        father = reader["father"] == DBNull.Value ? "" : reader["father"].ToString(),
                        genderID = reader["genderID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["genderID"]),
                        fin = reader["fin"] == DBNull.Value ? "" : Convert.ToString(reader["fin"]),
                        phone = reader["clientPhone"] == DBNull.Value ? 0 : Convert.ToInt64(reader["clientPhone"]),
                        bDate = reader["bDate"] == DBNull.Value ? Convert.ToDateTime("0001-01-01T00:00:00") : Convert.ToDateTime(reader["bDate"])
                    };
                    patients.Add(patient);
                }
                patients.Reverse();
                con.Close();
            }

            catch (Exception ex)
            {
                Medicloud.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);
            }

            return patients;
        }

        public Patient GetPatientById(string id)
        {
            Patient patient = new();

            MySqlConnection con = new(ConnectionString);
            string query = $@"SELECT * FROM patients WHERE id=@id";

            MySqlCommand cmd = new(query, con);
            cmd.Parameters.AddWithValue("@id", Convert.ToInt32(id));

            try
            {
                con.Open();
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    patient = new Patient
                    {

                        ID = Convert.ToInt64(reader["id"]),
                        name = reader["name"] == DBNull.Value ? "" : reader["name"].ToString(),
                        surname = reader["surname"] == DBNull.Value ? "" : reader["surname"].ToString(),
                        father = reader["father"] == DBNull.Value ? "" : reader["father"].ToString(),
                        genderID = reader["genderID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["genderID"]),
                        fin = reader["fin"] == DBNull.Value ? "" : Convert.ToString(reader["fin"]),
                        phone = reader["clientPhone"] == DBNull.Value ? 0 : Convert.ToInt64(reader["clientPhone"]),
                        bDate = reader["bDate"] == DBNull.Value ? Convert.ToDateTime("0001-01-01T00:00:00") : Convert.ToDateTime(reader["bDate"])
                    };
                }
                con.Close();
            }
            catch (Exception ex)
            {
                Medicloud.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);
            }
            return patient;
        }
    }
}

