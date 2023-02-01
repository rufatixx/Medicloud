using System;
using crypto;
using System.Configuration;
using FlexitHisMVC.Models;
using FlexitHisMVC.Models.NewPatient;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace FlexitHisMVC.Models.Repository
{
    public class PatientRequestRepo
    {
        private readonly string ConnectionString;

        public PatientRequestRepo(string conString)
        {
            ConnectionString = conString;
        }
        public bool InsertPatientRequest(AddPatientDTO newPatient, int userID, long lastID)
        {
            try
            {

                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {

                    connection.Open();





                    using (MySqlCommand com = new MySqlCommand(@"INSERT INTO patient_request (requestTypeID,userID,patientID,hospitalID,departmentID,serviceID,docID,priceGroupID,note,referDocID)
                      Values (@requestTypeID, @userID,@patientID,@hospitalID,@depID,@serviceID,@docID,@priceGroupID,@note,@referDocID)"
                    , connection))

                    {
                        com.Parameters.AddWithValue("@requestTypeID", newPatient.requestTypeID);
                        com.Parameters.AddWithValue("@userID", userID);
                        com.Parameters.AddWithValue("@patientID", lastID);
                        com.Parameters.AddWithValue("@hospitalID", newPatient.hospitalID);
                        com.Parameters.AddWithValue("@depID", newPatient.depID);
                        com.Parameters.AddWithValue("@serviceID", newPatient.serviceID);
                        com.Parameters.AddWithValue("@docID", newPatient.docID);
                        com.Parameters.AddWithValue("@priceGroupID", newPatient.priceGroupID);
                        com.Parameters.AddWithValue("@note", newPatient.note);
                        com.Parameters.AddWithValue("@referDocID", newPatient.referDocID);


                        com.ExecuteNonQuery();

                    }


                    connection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                FlexitHisMVC.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public List<Patient> GetDebtorPatients(long hospitalID)

        {

            List<Patient> patientList = new List<Patient>();
            try
            {

                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {


                    connection.Open();

                    using (MySqlCommand com = new MySqlCommand($@"SELECT id,patientID,serviceID,(select name from patients where id = a.patientID )as name,
(select surname from patients where id = a.patientID )as surname,
(select father from patients where id = a.patientID )as father,
sum((select price from services where id = a.serviceID ))as serviceSum
FROM patient_request a where hospitalID =@hospitalID and finished=0 group by patientID order by serviceSum ;", connection))
                    {
                        com.Parameters.AddWithValue("@hospitalID", hospitalID);
                        MySqlDataReader reader = com.ExecuteReader();
                        if (reader.HasRows)
                        {


                            while (reader.Read())
                            {

                                Patient dSumStruct = new Patient();
                                dSumStruct.ID = Convert.ToInt64(reader["patientID"]);
                                dSumStruct.name = reader["name"].ToString();
                                dSumStruct.surname = reader["surname"].ToString();
                                dSumStruct.father = reader["father"].ToString();
                                dSumStruct.price = Convert.ToDouble(reader["serviceSum"]);

                                patientList.Add(dSumStruct);


                            }
                            patientList.Reverse();


                        }

                    }


                    connection.Close();


                }

            }
            catch (Exception ex)
            {
                StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);

            }


            return patientList;
        }

        public bool UpdatePatientRequest(long patientID)
        {
            bool response = new bool();
            try
            {


                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {

                    connection.Open();









                    using (MySqlCommand com = new MySqlCommand(@"UPDATE patient_request SET finished=1 WHERE patientID=@patientID", connection))

                    {

                        com.Parameters.AddWithValue("@patientID", patientID);

                        com.ExecuteNonQuery();

                    }


                    response = true; //inserted


                    connection.Close();
                }





            }
            catch (Exception ex)
            {
                //FlexitHis_API.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);
                response = false;

            }


            return response;
        }


    }
}

