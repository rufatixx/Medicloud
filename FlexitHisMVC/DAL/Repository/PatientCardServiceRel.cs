using System;

using System.Configuration;
using Medicloud.Models;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using Medicloud.Models.Domain;
using System.Dynamic;

namespace Medicloud.Models.Repository
{
    public class PatientCardServiceRelRepo
    {
        private readonly string ConnectionString;

        public PatientCardServiceRelRepo(string conString)
        {
            ConnectionString = conString;
        }
        public bool InsertServiceToPatientCard(long patientCardID, int serviceID, int? depID, int? senderDocID, int? docID)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();

                    string sql = "INSERT INTO patient_card_service_rel (patientCardID, serviceID";
                    string values = "VALUES (@patientCardID, @serviceID";

                    // Dynamically building the query based on available parameters
                    if (depID.HasValue)
                    {
                        sql += ", depID";
                        values += ", @depID";
                    }

                    if (senderDocID.HasValue)
                    {
                        sql += ", senderDocID";
                        values += ", @senderDocID";
                    }

                    if (docID.HasValue)
                    {
                        sql += ", docID";
                        values += ", @docID";
                    }

                    sql += ") " + values + ")";

                    using (MySqlCommand com = new MySqlCommand(sql, connection))
                    {
                        com.Parameters.AddWithValue("@patientCardID", patientCardID);
                        com.Parameters.AddWithValue("@serviceID", serviceID);

                        if (depID.HasValue)
                            com.Parameters.AddWithValue("@depID", depID.Value);
                        if (senderDocID.HasValue)
                            com.Parameters.AddWithValue("@senderDocID", senderDocID.Value);
                        if (docID.HasValue)
                            com.Parameters.AddWithValue("@docID", docID.Value);

                        com.ExecuteNonQuery();
                    }

                    connection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                Medicloud.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool RemoveServiceFromPatientCard(long patientCardID, int serviceID)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();

                    string sql = @"
                UPDATE patient_card_service_rel 
                SET is_removed = 1 
                WHERE patientCardID = @patientCardID 
                  AND serviceID = @serviceID
                  AND is_removed = 0;";  // Ensures that only non-removed services are marked as removed

                    using (MySqlCommand com = new MySqlCommand(sql, connection))
                    {
                        com.Parameters.AddWithValue("@patientCardID", patientCardID);
                        com.Parameters.AddWithValue("@serviceID", serviceID);

                        int rowsAffected = com.ExecuteNonQuery();
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
                }
            }
            catch (Exception ex)
            {
                Medicloud.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);
                return false; // Return false if an exception occurred
            }
        }


        public bool RemovePatientServiceById(int id)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();

                    string sql = @"
                UPDATE patient_card_service_rel 
                SET is_removed = 1 
                WHERE id=@id";  

                    using (MySqlCommand com = new MySqlCommand(sql, connection))
                    {
                        com.Parameters.AddWithValue("@id", id);

                        int rowsAffected = com.ExecuteNonQuery();
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
                }
            }
            catch (Exception ex)
            {
                Medicloud.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);
                return false; // Return false if an exception occurred
            }
        }

        //public List<dynamic> GetServicesFromPatientCard(int patientCardID)
        //{
        //    try
        //    {
        //        List<dynamic> results = new List<dynamic>();

        //        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        //        {
        //            connection.Open();

        //            string query = @"
        //        SELECT pcsr.*, 
        //               s.name AS service_name, 
        //               s.price as service_price,
        //               s.code as service_code,
        //               sg.name AS service_group_name, 
        //               doc.name AS doc_name, 
        //               doc.surname AS doc_surname, 
        //               senderDoc.name AS sender_doc_name, 
        //               senderDoc.surname AS sender_doc_surname
        //        FROM patient_card_service_rel pcsr
        //        JOIN services s ON pcsr.serviceID = s.id
        //        JOIN service_group sg ON s.serviceGroupID = sg.id
        //        JOIN users doc ON pcsr.docID = doc.id
        //        JOIN users senderDoc ON pcsr.senderDocID = senderDoc.id
        //        WHERE pcsr.patientCardID = @patientCardID";

        //            using (MySqlCommand com = new MySqlCommand(query, connection))
        //            {
        //                com.Parameters.AddWithValue("@patientCardID", patientCardID);

        //                using (MySqlDataReader reader = com.ExecuteReader())
        //                {
        //                    while (reader.Read())
        //                    {
        //                        dynamic result = new ExpandoObject();
        //                        result.PatientCardID = Convert.ToInt32(reader["patientCardID"]);
        //                        result.ServiceID = Convert.ToInt32(reader["serviceID"]);
        //                        result.SenderDocID = Convert.ToInt32(reader["senderDocID"]);
        //                        result.DocID = Convert.ToInt32(reader["docID"]);
        //                        result.ServiceName = reader["service_name"].ToString();
        //                        result.ServicePrice = reader["service_price"].ToString();
        //                        result.ServiceCode = reader["service_code"].ToString();
        //                        result.ServiceGroup = reader["service_group_name"].ToString();
        //                        result.DocName = reader["doc_name"].ToString();
        //                        result.DocSurname = reader["doc_surname"].ToString();
        //                        result.SenderDocName = reader["sender_doc_name"].ToString();
        //                        result.SenderDocSurname = reader["sender_doc_surname"].ToString();
        //                        result.cDate = Convert.ToDateTime(reader["cDate"]);

        //                        results.Add(result);
        //                    }
        //                }
        //            }

        //            connection.Close();
        //        }

        //        return results;
        //    }
        //    catch (Exception ex)
        //    {
        //        Medicloud.StandardMessages.CallSerilog(ex);
        //        Console.WriteLine(ex.Message);
        //        return null;
        //    }
        //}
        public List<dynamic> GetServicesFromPatientCard(int patientCardID, int organizationID)
        {
            try
            {
                List<dynamic> results = new List<dynamic>();

                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();

                    string query = @"
           SELECT pcsr.*, p.name,p.surname,p.id as patientID,
                   s.name AS service_name, 
                   s.price as service_price,
                   s.code as service_code,
                   sg.name AS service_group_name, 
                   doc.name AS doc_name, 
                   doc.surname AS doc_surname, 
                   senderDoc.name AS sender_doc_name, 
                   senderDoc.surname AS sender_doc_surname,
                   pc.ID as CardId
            FROM patient_card_service_rel pcsr
            left JOIN services s ON pcsr.serviceID = s.id
           left JOIN service_group sg ON s.serviceGroupID = sg.id
            left JOIN users doc ON pcsr.docID = doc.id
           left JOIN users senderDoc ON pcsr.senderDocID = senderDoc.id
           left JOIN patient_card pc ON pcsr.patientCardID = pc.id
           left Join patients p on pc.patientID = p.id
            WHERE pc.organizationID = @organizationID AND pcsr.is_removed = 0" +
                    (!patientCardID.Equals(0) ? " AND pcsr.patientCardID = @patientCardID" : "");

                    using (MySqlCommand com = new MySqlCommand(query, connection))
                    {
                        com.Parameters.AddWithValue("@organizationID", organizationID);
                        if (patientCardID > 0)
                        {
                            com.Parameters.AddWithValue("@patientCardID", patientCardID);
                        }

                        using (MySqlDataReader reader = com.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                dynamic result = new ExpandoObject();
                                result.id=reader["id"] != DBNull.Value ? Convert.ToInt32(reader["id"]) : (int?)null;
                                result.PatientCardID = reader["patientCardID"] != DBNull.Value ? Convert.ToInt32(reader["patientCardID"]) : (int?)null;
                                result.ServiceID = reader["serviceID"] != DBNull.Value ? Convert.ToInt32(reader["serviceID"]) : (int?)null;
                                result.SenderDocID = reader["senderDocID"] != DBNull.Value ? Convert.ToInt32(reader["senderDocID"]) : (int?)null;
                                result.DocID = reader["docID"] != DBNull.Value ? Convert.ToInt32(reader["docID"]) : (int?)null;
                                result.patientID = reader["patientID"] != DBNull.Value ? Convert.ToInt32(reader["patientID"]) : (int?)null;
                                result.patientName = reader["name"] != DBNull.Value ? reader["name"].ToString() : null;
                                result.patientSurname = reader["surname"] != DBNull.Value ? reader["surname"].ToString() : null;
                                result.ServiceName = reader["service_name"] != DBNull.Value ? reader["service_name"].ToString() : null;
                                result.ServicePrice = reader["service_price"] != DBNull.Value ? reader["service_price"].ToString() : null;
                                result.ServiceCode = reader["service_code"] != DBNull.Value ? reader["service_code"].ToString() : null;
                                result.ServiceGroup = reader["service_group_name"] != DBNull.Value ? reader["service_group_name"].ToString() : null;
                                result.DocName = reader["doc_name"] != DBNull.Value ? reader["doc_name"].ToString() : null;
                                result.DocSurname = reader["doc_surname"] != DBNull.Value ? reader["doc_surname"].ToString() : null;
                                result.SenderDocName = reader["sender_doc_name"] != DBNull.Value ? reader["sender_doc_name"].ToString() : null;
                                result.SenderDocSurname = reader["sender_doc_surname"] != DBNull.Value ? reader["sender_doc_surname"].ToString() : null;
                                result.cDate = reader["cDate"] != DBNull.Value ? Convert.ToDateTime(reader["cDate"]) : (DateTime?)null;
                                result.card_id=reader["CardId"] != DBNull.Value ? Convert.ToInt32(reader["CardId"]) : (int?)null;

                                results.Add(result);
                            }
                        }
                    }

                    connection.Close();
                }
                results.Reverse();

                return results;
            }
            catch (Exception ex)
            {
                // Обработка ошибок
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
                Medicloud.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);
                return null;
            }
        }

    }
}

