using System;
using Medicloud.Models;
using MySql.Data.MySqlClient;

namespace Medicloud.Data
{
    public class ServiceGroupsRepo
    {
        private readonly string ConnectionString;

        public ServiceGroupsRepo(string conString)
        {
            ConnectionString = conString;
        }
        public List<ServiceGroup> GetGroupsByOrganization(int organizationID)
        {
            List<ServiceGroup> serviceList = new List<ServiceGroup>();

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {

                connection.Open();
                using (MySqlCommand com = new MySqlCommand("SELECT * FROM service_group where organizationID=@organizationID;", connection))
                {
                    com.Parameters.AddWithValue("organizationID", organizationID);

                    MySqlDataReader reader = com.ExecuteReader();
                    if (reader.HasRows)
                    {


                        while (reader.Read())
                        {
                            ServiceGroup serviceGroup = new ServiceGroup();

                            serviceGroup.ID = Convert.ToInt32(reader["id"]);
                            serviceGroup.organizationID = Convert.ToInt32(reader["organizationID"]);
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

                using (MySqlCommand com = new MySqlCommand("INSERT INTO service_group (organizationID, name, isHeading,parent) VALUES (@organizationID, @name, @isHeading,@parent)", connection))
                {
                    com.Parameters.AddWithValue("@organizationID", serviceGroup.organizationID);
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

