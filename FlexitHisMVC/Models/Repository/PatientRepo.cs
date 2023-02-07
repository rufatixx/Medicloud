using System;
using FlexitHisCore.Models;
using FlexitHisMVC.Data;
using FlexitHisMVC.Models;
using MySql.Data.MySqlClient;

namespace FlexitHisMVC.Models.NewPatient
{
    public class PatientRepo
    {
        private readonly string ConnectionString;

        public PatientRepo(string conString)
        {
            ConnectionString = conString;
        }
      

        public long InsertPatient(int userID, AddPatientDTO newPatient)
        {
            ResponseDTO<int> response = new ResponseDTO<int>();
            long lastID = 0;
            try
            {
               
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {

                    connection.Open();





                    using (MySqlCommand com = new MySqlCommand(@"

INSERT INTO patients (userID,hospitalID,name,surname,father,clientPhone,bDate,genderID,fin) 
  SELECT @userID,@hospitalID,@name,@surname,@father,@clientPhone,@bDate,@genderID,@fin FROM DUAL
WHERE NOT EXISTS 
  (SELECT * FROM patients WHERE name=@name and surname=@surname and father= @father and hospitalID = @hospitalID);", connection))

                    {
                        com.Parameters.AddWithValue("@userID", userID);
                        com.Parameters.AddWithValue("@hospitalID", newPatient.hospitalID);
                        com.Parameters.AddWithValue("@name", newPatient.name);
                        com.Parameters.AddWithValue("@surname", newPatient.surname);
                        com.Parameters.AddWithValue("@father", newPatient.father);
                        com.Parameters.AddWithValue("@clientPhone", newPatient.clientPhone);

                        //com.Parameters.AddWithValue("@bDate", DateTime.Parse(newPatient.birthDate));
                        com.Parameters.AddWithValue("@bDate", newPatient.birthDate);
                        com.Parameters.AddWithValue("@genderID", newPatient.genderID);
                        com.Parameters.AddWithValue("@fin", newPatient.fin);

                        com.ExecuteNonQuery();
                        lastID = com.LastInsertedId;
                    }


                    connection.Close();
                   

                }


            }
            catch (Exception ex)
            {
                //FlexitHisMVC.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);
                //response.status = 4; // not inserted (error)
            }


            return lastID ;
        }

        public List<PatientKassaDTO> SearchForPatients(string fullNamePattern, long hospitalID)

        {
           
             List<PatientKassaDTO> patientList = new List<PatientKassaDTO>();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {


                    connection.Open();

                    using (MySqlCommand com = new MySqlCommand($@"SELECT * 
FROM patients
WHERE LOWER(CONCAT( name,  ' ', surname )) LIKE  LOWER(@fullNamePattern) and hospitalID = @hospitalID ", connection))
                    {
                        com.Parameters.AddWithValue("@fullNamePattern", $"%{fullNamePattern}%");
                        com.Parameters.AddWithValue("@hospitalID", hospitalID);

                        MySqlDataReader reader = com.ExecuteReader();
                        if (reader.HasRows)
                        {


                            while (reader.Read())
                            {

                                PatientKassaDTO patient = new PatientKassaDTO();
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

                            //response.data.Reverse();

                            //response.status = 1;
                        }
                       
                    }


                    connection.Close();

                }

            }
            catch (Exception ex)
            {
                FlexitHisMVC.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);
                //response.status = 4;
            }


            return patientList;
        }

    }
}

