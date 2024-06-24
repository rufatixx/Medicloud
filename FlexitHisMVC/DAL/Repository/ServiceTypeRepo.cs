using System;
using Medicloud.Models;
using Medicloud.Models.Domain;
using MySql.Data.MySqlClient;

namespace Medicloud.Data
{
    public class ServiceTypeRepo
    {
        private readonly string ConnectionString;

        public ServiceTypeRepo(string conString)
        {
            ConnectionString = conString;
        }
        public List<ServiceType> GetServiceTypes()
        {
            List<ServiceType> serviceList = new List<ServiceType>();
            try
            {

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {

                connection.Open();
                using (MySqlCommand com = new MySqlCommand("SELECT * FROM service_type;", connection))
                {
                

                    MySqlDataReader reader = com.ExecuteReader();
                    if (reader.HasRows)
                    {


                        while (reader.Read())
                        {
                            ServiceType service = new ServiceType();

                            service.id = Convert.ToInt32(reader["id"]);
                            service.name = reader["name"].ToString();

                            serviceList.Add(service);

                           


                        }



                    }

                }
                connection.Close();
            }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return serviceList;
        }

      

    }
}

