using Medicloud.DAL.Entities;
using MySql.Data.MySqlClient;

namespace Medicloud.Data
{
    public class SpecialityRepo
    {
        private readonly string ConnectionString;

        public SpecialityRepo(string conString)
        {
            ConnectionString = conString;
        }
        public List<Speciality> GetSpecialities()
        {
            List<Speciality> specialityList = new List<Speciality>();

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {

                connection.Open();

                using (MySqlCommand kassaCom = new MySqlCommand("SELECT * from speciality;", connection))
                {

                 

                    using (MySqlDataReader kassaReader = kassaCom.ExecuteReader())
                    {
                        if (kassaReader.HasRows)
                        {
                            while (kassaReader.Read())
                            {
                                Speciality speciality = new Speciality();
                                speciality.id = Convert.ToInt32(kassaReader["id"]);
                                speciality.name = kassaReader["name"].ToString();
                                specialityList.Add(speciality);
                            }


                        }
                        
                    }

                }
                connection.Close();
            }
            return specialityList;
        }
    }
}

