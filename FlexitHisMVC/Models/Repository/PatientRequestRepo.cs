using System;
using crypto;
using System.Configuration;
using FlexitHisMVC.Models;
using FlexitHisMVC.Models.NewPatient;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using FlexitHisMVC.Models.Domain;

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





                    using (MySqlCommand com = new MySqlCommand(@"INSERT INTO patient_request (requestTypeID,userID,patientID,hospitalID,serviceID,docID,priceGroupID,note,referDocID)
                      Values (@requestTypeID, @userID,@patientID,@hospitalID,@serviceID,@docID,@priceGroupID,@note,@referDocID)"
                    , connection))

                    {
                        com.Parameters.AddWithValue("@requestTypeID", newPatient.requestTypeID);
                        com.Parameters.AddWithValue("@userID", userID);
                        com.Parameters.AddWithValue("@patientID", lastID);
                        com.Parameters.AddWithValue("@hospitalID", newPatient.hospitalID);
                        //com.Parameters.AddWithValue("@depID", newPatient.depID);
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
        public List<PatientKassaDTO> GetDebtorPatients(long hospitalID)

        {

            List<PatientKassaDTO> patientList = new List<PatientKassaDTO>();
            try
            {

                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {


                    connection.Open();

                    using (MySqlCommand com = new MySqlCommand($@"SELECT a.id, a.patientID, a.serviceID, p.name, p.surname, p.father, s.price as servicePrice, s.name as serviceName
FROM patient_request a
JOIN patients p ON a.patientID = p.id
JOIN services s ON a.serviceID = s.id
WHERE a.hospitalID = @hospitalID AND a.finished = 0;
", connection))
                    {
                        com.Parameters.AddWithValue("@hospitalID", hospitalID);
                        MySqlDataReader reader = com.ExecuteReader();
                        if (reader.HasRows)
                        {


                            while (reader.Read())
                            {

                                PatientKassaDTO dSumStruct = new PatientKassaDTO();
                                dSumStruct.ID = Convert.ToInt64(reader["patientID"]);
                                dSumStruct.name = reader["name"].ToString();
                                dSumStruct.surname = reader["surname"].ToString();
                                dSumStruct.father = reader["father"].ToString();
                                dSumStruct.serviceName = reader["serviceName"].ToString();
                                dSumStruct.servicePrice = Convert.ToDouble(reader["servicePrice"]);
                       

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
        public List<PatientDocDTO> GetPatientsByDr(int docID)

        {

            List<PatientDocDTO> patientList = new List<PatientDocDTO>();
            try
            {

                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {


                    connection.Open();

                    using (MySqlCommand com = new MySqlCommand($@"SELECT a.id, a.patientID, a.serviceID,a.note, p.name, p.surname, p.father,p.clientPhone,p.bDate,p.genderID,p.fin
FROM patient_request a
INNER JOIN patients p ON a.patientID = p.id
WHERE a.docID = @docID
GROUP BY a.patientID
 ;", connection))
                    {
                        com.Parameters.AddWithValue("@docID", docID);
                        MySqlDataReader reader = com.ExecuteReader();
                        if (reader.HasRows)
                        {


                            while (reader.Read())
                            {

                                PatientDocDTO patient = new PatientDocDTO();
                                patient.ID = Convert.ToInt64(reader["patientID"]);
                                patient.name = reader["name"].ToString();
                                patient.surname = reader["surname"].ToString();
                                patient.father = reader["father"].ToString();
                                patient.phone = Convert.ToInt64(reader["clientPhone"]);
                                patient.bDate = Convert.ToDateTime(reader["bDate"]);
                                patient.genderID = Convert.ToInt32(reader["genderID"]);
                                patient.fin = reader["fin"].ToString();
                                patient.note = reader["note"].ToString();
                               

                                patientList.Add(patient);


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

