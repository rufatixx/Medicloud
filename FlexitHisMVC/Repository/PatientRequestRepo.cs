using System;
using FlexitHisMVC.Models;
using FlexitHisMVC.Models.NewPatient;
using MySql.Data.MySqlClient;

namespace FlexitHisMVC.Data
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
    }
}

