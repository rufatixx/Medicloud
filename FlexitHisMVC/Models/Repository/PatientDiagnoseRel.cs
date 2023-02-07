using System;
using System.ComponentModel;
using FlexitHisMVC.Models.Domain;
using Microsoft.CodeAnalysis;
using MySql.Data.MySqlClient;

namespace FlexitHisMVC.Models.Repository
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
        public int InsertPatientToDiagnose(int patientID, long diagnoseID)
        {
            int lastID = 0;

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



                            lastID = com.ExecuteNonQuery();


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
        public int RemovePatientToDiagnose(int relID)
        {
            int lastID = 0;
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



                        lastID = com.ExecuteNonQuery();


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

