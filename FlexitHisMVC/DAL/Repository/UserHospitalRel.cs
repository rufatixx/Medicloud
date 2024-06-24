using System;
using Medicloud.Models;
using Medicloud.Models.DTO;
using MySql.Data.MySqlClient;

namespace Medicloud.Repository
{
    public class UserOrganizationRel
    {
        private readonly string ConnectionString;

        public UserOrganizationRel(string conString)
        {
            ConnectionString = conString;
        }
        public List<User> GetDoctorsByOrganization(int organizationID)

        {

            List<User> userDepRelList = new List<User>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {


                    connection.Open();

                    using (MySqlCommand com = new MySqlCommand($@"SELECT a.*, b.name,b.isDr, b.surname, c.name as speciality
FROM user_organization_rel a
INNER JOIN users b ON a.userID = b.id
INNER JOIN speciality c ON b.specialityID = c.id
WHERE a.organizationID = @organizationID and b.isDr = 1;", connection))
                    {
                        com.Parameters.AddWithValue("@organizationID", organizationID);

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

