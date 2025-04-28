using Medicloud.DAL.Entities;
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
        public List<UserDAO> GetDoctorsByOrganization(int organizationID)

        {
			Console.WriteLine(organizationID);
            List<UserDAO> userDepRelList = new List<UserDAO>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {


                    connection.Open();

                    using (MySqlCommand com = new MySqlCommand($@"SELECT a.*, b.name,b.isDr,b.specialityID, b.surname, c.name as speciality
FROM user_organization_rel a
INNER JOIN users b ON a.userID = b.id
LEFT JOIN speciality c ON b.specialityID = c.id
WHERE a.organizationID = @organizationID and b.isDr = 1 group by userID;", connection))
                    {
                        com.Parameters.AddWithValue("@organizationID", organizationID);

                        MySqlDataReader reader = com.ExecuteReader();
                        if (reader.HasRows)
                        {


                            while (reader.Read())
                            {

                                UserDAO user = new UserDAO();
                                user.ID = Convert.ToInt32(reader["userID"]);
                                user.specialityID = Convert.ToInt64(reader["specialityID"]);
                                user.speciality = new Speciality
                                {
                                    id = Convert.ToInt64(reader["specialityID"]),
                                    name = reader["speciality"].ToString()
                                };
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

