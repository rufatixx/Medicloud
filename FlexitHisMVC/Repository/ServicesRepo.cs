using System;
using FlexitHisMVC.Models;
using MySql.Data.MySqlClient;

namespace FlexitHisMVC.Data
{
    public class ServicesRepo
    {
        private readonly string ConnectionString;

        public ServicesRepo(string conString)
        {
            ConnectionString = conString;
        }
        public List<Service> GetServices()
        {
            List<Service> serviceList = new List<Service>();

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {

                connection.Open();
                using (MySqlCommand com = new MySqlCommand("SELECT * FROM services;", connection))
                {

                    MySqlDataReader reader = com.ExecuteReader();
                    if (reader.HasRows)
                    {


                        while (reader.Read())
                        {
                            Service service = new Service();

                            service.ID = Convert.ToInt32(reader["id"]);
                            service.depID = Convert.ToInt32(reader["departmentID"]);
                            service.name = reader["name"].ToString();
                            service.price = Convert.ToDouble(reader["price"]);

                            serviceList.Add(service);

                           


                        }



                    }

                }
                connection.Close();
            }
            return serviceList;
        }
    }
}

