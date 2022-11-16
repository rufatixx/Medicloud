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
    }
}

