using System;
using FlexitHisMVC.Models.Domain;
using FlexitHisMVC.Models.DTO;
using MySql.Data.MySqlClient;

namespace FlexitHisMVC.Models.Repository
{
	public class PatientRecordRelRepo
	{
        private readonly string ConnectionString;

        public PatientRecordRelRepo(string conString)
        {
            ConnectionString = conString;
        }
        public long InsertIntoPatientRecordRel(long patientID, long recordID)
        {
            long lastID;
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {

                connection.Open();
                var query = "INSERT INTO patient_record_rel (patientID, recordID) VALUES (@patientID, @recordID)";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@patientID", patientID);
                    command.Parameters.AddWithValue("@recordID", recordID);
                    command.ExecuteNonQuery();
                    lastID = command.LastInsertedId;
                }
            }
            return lastID;
        }
        public List<RecordDTO> GetRecords(int patientID)
        {

            List<RecordDTO> diagnoseList = new List<RecordDTO>();

            try
            {
                if (patientID > 0)
                {
                    using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                    {


                        var sql = "";

                        connection.Open();

                        sql = @"SELECT a.*, b.name, b.path 
FROM patient_record_rel a 
INNER JOIN records b ON a.recordID = b.id 
WHERE a.patientID = @patientID";



                        using (MySqlCommand com = new MySqlCommand(sql, connection))
                        {
                            com.Parameters.AddWithValue("@patientID", patientID);
                            MySqlDataReader reader = com.ExecuteReader();
                            if (reader.HasRows)
                            {



                                while (reader.Read())
                                {

                                    RecordDTO record = new RecordDTO();
                                    record.id = Convert.ToInt64(reader["recordID"]);
                                    record.patientRecRellID = Convert.ToInt64(reader["id"]);
                                    record.name = reader["name"] == DBNull.Value ? "" : reader["name"].ToString();
                                    record.path = reader["path"] == DBNull.Value ? "" : reader["path"].ToString();

                                    diagnoseList.Add(record);


                                }

                                //response.data.Reverse();

                                //response.status = 1;
                            }



                        }
                        connection.Close();



                    }
                }



            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);


            }
            return diagnoseList;
        }
        public long RemovePatientToRec(int relID)
        {
            long lastID = 0;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {


                    var sql = "";

                    connection.Open();

                    sql = @"DELETE FROM patient_record_rel WHERE id = @relID;";



                    using (MySqlCommand com = new MySqlCommand(sql, connection))
                    {
                        com.Parameters.AddWithValue("@relID", relID);



                       com.ExecuteNonQuery();

                        lastID = com.LastInsertedId;
                    }
                    connection.Close();



                }


            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);


            }
            return lastID;
        }
    }
}

