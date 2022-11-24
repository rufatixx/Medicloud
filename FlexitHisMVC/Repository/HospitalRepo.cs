using System;
using System.Net.NetworkInformation;
using FlexitHisMVC.Models;
using MySql.Data.MySqlClient;

namespace FlexitHisMVC.Data
{
    public class HospitalRepo
    {
        private readonly string ConnectionString;

        public HospitalRepo(string conString)
        {
            ConnectionString = conString;
        }
        public List<Hospital> GetHospitalListByUser(int userID)
        {
            List<Hospital> hospitalList = new List<Hospital>();

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {

                connection.Open();
                using (MySqlCommand hospitalCom = new MySqlCommand("SELECT *,(select name from hospital where id = a.hospitalID) as hName FROM user_hospital_rel a where userID = @userID;", connection))
                {

                    hospitalCom.Parameters.AddWithValue("@userID", userID);

                    using (MySqlDataReader hospitalReader = hospitalCom.ExecuteReader())
                    {
                        if (hospitalReader.HasRows)
                        {
                            while (hospitalReader.Read())
                            {
                                Hospital hospital = new Hospital();
                                hospital.id = hospitalReader["id"] == DBNull.Value ? 0 : Convert.ToInt32(hospitalReader["id"]);
                                hospital.hospitalName = hospitalReader["hName"] == DBNull.Value ? "" : hospitalReader["hName"].ToString();
                           
                                hospital.userID = hospitalReader["userID"] == DBNull.Value ? 0 : Convert.ToInt32(hospitalReader["userID"]);
                                hospital.hospitalID = hospitalReader["hospitalID"] == DBNull.Value ? 0 : Convert.ToInt32(hospitalReader["hospitalID"]); 
                              
                                hospitalList.Add(hospital);
                            }


                        }
                      
                    }

                }

                connection.Close();
            }
            return hospitalList;
        }
        public List<Hospital> GetHospitalList()
        {
            List<Hospital> hospitalList = new List<Hospital>();

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {

                connection.Open();
                using (MySqlCommand hospitalCom = new MySqlCommand("SELECT * from hospital", connection))
                {

                  

                    using (MySqlDataReader hospitalReader = hospitalCom.ExecuteReader())
                    {
                        if (hospitalReader.HasRows)
                        {
                            while (hospitalReader.Read())
                            {
                                Hospital hospital = new Hospital();
                                hospital.id = hospitalReader["id"] == DBNull.Value ? 0 : Convert.ToInt32(hospitalReader["id"]); 
                              
                                hospital.hospitalName = hospitalReader["name"] == DBNull.Value ? "" : hospitalReader["name"].ToString();
                                hospitalList.Add(hospital);
                            }


                        }

                    }

                }

                connection.Close();
            }
            return hospitalList;
        }
        public int InsertHospital( int userID, int hospitalID)
        {
            int lastID = 0;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {


                    var sql = "";

                    connection.Open();

                    sql = @"Insert INTO user_hospital_rel (hospitalID,userID)
SELECT @hospitalID,@userID FROM DUAL
WHERE NOT EXISTS 
  (SELECT * FROM user_hospital_rel WHERE hospitalID=@hospitalID and userID=@userID )";



                    using (MySqlCommand com = new MySqlCommand(sql, connection))
                    {
                        com.Parameters.AddWithValue("@hospitalID", hospitalID);
                        com.Parameters.AddWithValue("@userID", userID);


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
        public int DeleteHospital(int userID, int hospitalID)
        {
            int lastID = 0;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {


                    var sql = "";

                    connection.Open();

                    sql = @"DELETE FROM user_hospital_rel WHERE userID = @userID and hospitalID = @hospitalID;";



                    using (MySqlCommand com = new MySqlCommand(sql, connection))
                    {
                        com.Parameters.AddWithValue("@hospitalID", hospitalID);
                        com.Parameters.AddWithValue("@userID", userID);


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

