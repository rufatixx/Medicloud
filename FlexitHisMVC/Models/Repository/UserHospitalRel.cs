using System;
using FlexitHisMVC.Models;
using FlexitHisMVC.Models.DTO;
using MySql.Data.MySqlClient;

namespace FlexitHisMVC.Repository
{
    public class UserHospitalRel
    {
        private readonly string ConnectionString;

        public UserHospitalRel(string conString)
        {
            ConnectionString = conString;
        }
        public List<User> GetUsersByHospital(int hospitalID)

        {

            List<User> userDepRelList = new List<User>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {


                    connection.Open();

                    using (MySqlCommand com = new MySqlCommand($@"SELECT a.*, b.name, b.surname, c.name as speciality
FROM user_hospital_rel a
INNER JOIN users b ON a.userID = b.id
INNER JOIN speciality c ON b.specialityID = c.id
WHERE a.hospitalID = @hospitalID;", connection))
                    {
                        com.Parameters.AddWithValue("@hospitalID", hospitalID);

                        MySqlDataReader reader = com.ExecuteReader();
                        if (reader.HasRows)
                        {


                            while (reader.Read())
                            {

                                User user = new User();
                                user.ID = Convert.ToInt32(reader["userID"]);
                                user.speciality = reader["speciality"].ToString();
                                user.name = reader["name"].ToString();
                                user.surname = reader["surname"].ToString();
                               


                                userDepRelList.Add(user);


                            }

                            //response.data.Reverse();

                            //response.status = 1;
                        }

                    }


                    connection.Close();


                }


            }
            catch (Exception ex)
            {
                //FlexitHis_API.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);

            }


            return userDepRelList;
        }


    }
}

