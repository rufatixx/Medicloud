using System;
using FlexitHisMVC.Models;
using MySql.Data.MySqlClient;

namespace FlexitHisMVC.Data
{
    public class ServiceGroupsRepo
    {
        private readonly string ConnectionString;

        public ServiceGroupsRepo(string conString)
        {
            ConnectionString = conString;
        }
        public List<ServiceGroup> GetGroupsByHospital(int hospitalID)
        {
            List<ServiceGroup> serviceList = new List<ServiceGroup>();

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {

                connection.Open();
                using (MySqlCommand com = new MySqlCommand("SELECT * FROM service_group where hospitalID=@hospitalID;", connection))
                {
                    com.Parameters.AddWithValue("hospitalID", hospitalID);

                    MySqlDataReader reader = com.ExecuteReader();
                    if (reader.HasRows)
                    {


                        while (reader.Read())
                        {
                            ServiceGroup serviceGroup = new ServiceGroup();

                            serviceGroup.ID = Convert.ToInt32(reader["id"]);
                            serviceGroup.hospitalID = Convert.ToInt32(reader["hospitalID"]);
                            serviceGroup.name = reader["name"].ToString();
                            serviceGroup.isHeading = Convert.ToBoolean(reader["isHeading"]);
                            serviceGroup.parent = Convert.ToInt32(reader["parent"]);

                            serviceList.Add(serviceGroup);

                           


                        }



                    }

                }
                connection.Close();
            }
            serviceList.Reverse();
            return serviceList;
        }
        public void InsertServiceGroup(ServiceGroup serviceGroup)
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                using (MySqlCommand com = new MySqlCommand("INSERT INTO service_group (hospitalID, name, isHeading,parent) VALUES (@hospitalID, @name, @isHeading,@parent)", connection))
                {
                    com.Parameters.AddWithValue("@hospitalID", serviceGroup.hospitalID);
                    com.Parameters.AddWithValue("@name", serviceGroup.name);
                    com.Parameters.AddWithValue("@isHeading", serviceGroup.isHeading);
                    com.Parameters.AddWithValue("@parent", serviceGroup.parent);

                    com.ExecuteNonQuery();
                }

                connection.Close();
            }
        }
        public void DeleteServiceGroup(int serviceGroupId)
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                using (MySqlCommand com = new MySqlCommand("DELETE FROM service_group WHERE id = @id", connection))
                {
                    com.Parameters.AddWithValue("@id", serviceGroupId);

                    com.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

       

    }
}

