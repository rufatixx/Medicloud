using System;
using crypto;
using System.Configuration;
using FlexitHisMVC.Models;
using FlexitHisMVC.Models.NewPatient;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using FlexitHisMVC.Models.Domain;
using System.Dynamic;

namespace FlexitHisMVC.Models.Repository
{
    public class PatientCardServiceRelRepo
    {
        private readonly string ConnectionString;

        public PatientCardServiceRelRepo(string conString)
        {
            ConnectionString = conString;
        }
        public bool InsertServiceToPatientCard(long patientCardID, int serviceID,int depID, int senderDocID, int docID)
        {
            try
            {

                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {

                    connection.Open();





                    using (MySqlCommand com = new MySqlCommand(@"INSERT INTO patient_card_service_rel (patientCardID,serviceID,depID,senderDocID,docID)
                      Values (@patientCardID, @serviceID,@depID,@senderDocID,@docID)"
                    , connection))

                    {
                        com.Parameters.AddWithValue("@patientCardID", patientCardID);
                        com.Parameters.AddWithValue("@serviceID", serviceID);
                        com.Parameters.AddWithValue("@depID", depID);
                        com.Parameters.AddWithValue("@senderDocID", senderDocID);
                        com.Parameters.AddWithValue("@docID", docID);
                        

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
        public List<dynamic> GetServicesFromPatientCard(int patientCardID)
        {
            try
            {
                List<dynamic> results = new List<dynamic>();

                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();

                    string query = @"
                SELECT pcsr.*, 
                       s.name AS service_name, 
                       s.price as service_price,
                       s.code as service_code,
                       sg.name AS service_group_name, 
                       doc.name AS doc_name, 
                       doc.surname AS doc_surname, 
                       senderDoc.name AS sender_doc_name, 
                       senderDoc.surname AS sender_doc_surname
                FROM patient_card_service_rel pcsr
                JOIN services s ON pcsr.serviceID = s.id
                JOIN service_group sg ON s.serviceGroupID = sg.id
                JOIN users doc ON pcsr.docID = doc.id
                JOIN users senderDoc ON pcsr.senderDocID = senderDoc.id
                WHERE pcsr.patientCardID = @patientCardID";

                    using (MySqlCommand com = new MySqlCommand(query, connection))
                    {
                        com.Parameters.AddWithValue("@patientCardID", patientCardID);

                        using (MySqlDataReader reader = com.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                dynamic result = new ExpandoObject();
                                result.PatientCardID = Convert.ToInt32(reader["patientCardID"]);
                                result.ServiceID = Convert.ToInt32(reader["serviceID"]);
                                result.SenderDocID = Convert.ToInt32(reader["senderDocID"]);
                                result.DocID = Convert.ToInt32(reader["docID"]);
                                result.ServiceName = reader["service_name"].ToString();
                                result.ServicePrice = reader["service_price"].ToString();
                                result.ServiceCode = reader["service_code"].ToString();
                                result.ServiceGroup = reader["service_group_name"].ToString();
                                result.DocName = reader["doc_name"].ToString();
                                result.DocSurname = reader["doc_surname"].ToString();
                                result.SenderDocName = reader["sender_doc_name"].ToString();
                                result.SenderDocSurname = reader["sender_doc_surname"].ToString();

                                results.Add(result);
                            }
                        }
                    }

                    connection.Close();
                }

                return results;
            }
            catch (Exception ex)
            {
                FlexitHisMVC.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public List<dynamic> GetDoctorsFromPatientCard(int patientCardID)
        {
            try
            {
                List<dynamic> results = new List<dynamic>();

                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();

                    string query = @"
            SELECT pcsr.docID, 
                   doc.name AS doc_name, 
                   doc.surname AS doc_surname 
            FROM patient_card_service_rel pcsr
            JOIN users doc ON pcsr.docID = doc.id
            WHERE pcsr.patientCardID = @patientCardID
            GROUP BY pcsr.docID";

                    using (MySqlCommand com = new MySqlCommand(query, connection))
                    {
                        com.Parameters.AddWithValue("@patientCardID", patientCardID);

                        using (MySqlDataReader reader = com.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                dynamic result = new ExpandoObject();
                                result.RefDocID = Convert.ToInt32(reader["docID"]);
                                result.RefDocName = reader["doc_name"].ToString();
                                result.RefDocSurname = reader["doc_surname"].ToString();

                                results.Add(result);
                            }
                        }
                    }

                    connection.Close();
                }

                return results;
            }
            catch (Exception ex)
            {
                FlexitHisMVC.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);
                return null;
            }
        }

    }
}

