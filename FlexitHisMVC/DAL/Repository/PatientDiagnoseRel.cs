using System;
using System.ComponentModel;
using Medicloud.Models.Domain;
using Microsoft.CodeAnalysis;
using MySql.Data.MySqlClient;

namespace Medicloud.Models.Repository
{
    public class PatientDiagnoseRel
    {
        private readonly string ConnectionString;

        public PatientDiagnoseRel(string conString)
        {
            ConnectionString = conString;
        }
        public List<Diagnose> GetPatientToDiagnose(int patientID)
        {

            List<Diagnose> diagnoseList = new List<Diagnose>();

            try
            {
                if (patientID > 0)
                {
                    using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                    {


                        var sql = "";

                        connection.Open();

                        sql = @"SELECT a.*, b.icd_id AS icdID, b.name 
FROM patient_diagnose_rel a 
INNER JOIN diagnose b ON a.diagnoseID = b.id 
WHERE a.patientID = @patientID";



                        using (MySqlCommand com = new MySqlCommand(sql, connection))
                        {
                            com.Parameters.AddWithValue("@patientID",patientID);
                            MySqlDataReader reader = com.ExecuteReader();
                            if (reader.HasRows)
                            {



                                while (reader.Read())
                                {

                                    Diagnose diagnose = new Diagnose();
                                    diagnose.ID = Convert.ToInt64(reader["id"]);
                                    diagnose.name = reader["name"] == DBNull.Value ? "" : reader["name"].ToString();
                                    diagnose.icdID = reader["icdID"] == DBNull.Value ? "" : reader["icdID"].ToString();

                                    diagnoseList.Add(diagnose);


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
        public long InsertPatientToDiagnose(int patientID, long diagnoseID)
        {
            long lastID = 0;

            try
            {
                if (patientID > 0 && diagnoseID > 0)
                {
                    using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                    {


                        var sql = "";

                        connection.Open();

                        sql = @"INSERT INTO patient_diagnose_rel (patientID, diagnoseID)
SELECT @patientID, @diagnoseID
FROM patient_diagnose_rel
WHERE NOT EXISTS (SELECT 1 FROM patient_diagnose_rel WHERE patientID = @patientID AND diagnoseID = @diagnoseID)
LIMIT 1;
";



                        using (MySqlCommand com = new MySqlCommand(sql, connection))
                        {
                            com.Parameters.AddWithValue("@patientID", patientID);
                            com.Parameters.AddWithValue("@diagnoseID", diagnoseID);

                            com.ExecuteNonQuery();

                            lastID = com.LastInsertedId;


                        }
                        connection.Close();



                    }
                }



            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);


            }
            return lastID;
        }
        public long RemovePatientToDiagnose(int relID)
        {
            long lastID = 0;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {


                    var sql = "";

                    connection.Open();

                    sql = @"DELETE FROM patient_diagnose_rel WHERE id = @relID;";



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

