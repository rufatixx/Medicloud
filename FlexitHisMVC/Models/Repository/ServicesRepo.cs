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
        public List<ServiceObj> GetServicesByHospital(int hospitalID)
        {
            List<ServiceObj> serviceList = new List<ServiceObj>();

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {

                connection.Open();
                using (MySqlCommand com = new MySqlCommand("SELECT * FROM services where hospitalID=@hospitalID;", connection))
                {
                    com.Parameters.AddWithValue("hospitalID", hospitalID);

                    MySqlDataReader reader = com.ExecuteReader();
                    if (reader.HasRows)
                    {


                        while (reader.Read())
                        {
                            ServiceObj service = new ServiceObj();

                            service.ID = Convert.ToInt32(reader["id"]);
                            service.hospitalID = Convert.ToInt32(reader["hospitalID"]);
                            service.serviceTypeID = Convert.ToInt32(reader["serviceTypeID"]);
                           
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

        public List<ServiceObj> GetServicesWithServiceGroupName(int hospitalID, int serviceGroupID = 0)
        {
            List<ServiceObj> services = new List<ServiceObj>();

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                string query = @"
            SELECT s.id, s.name, s.price, s.hospitalID, s.cDate, s.serviceGroupID,s.serviceTypeID,s.code,s.isActive, sg.name AS serviceGroupName
            FROM services s
            INNER JOIN service_group sg ON s.serviceGroupID = sg.id
            WHERE s.hospitalID = @hospitalID";

                if (serviceGroupID > 0)
                {
                    query += " AND s.serviceGroupID = @serviceGroupID";
                }

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@hospitalID", hospitalID); // Set the hospital ID parameter value

                    if (serviceGroupID > 0)
                    {
                        command.Parameters.AddWithValue("@serviceGroupID", serviceGroupID);
                    }

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ServiceObj service = new ServiceObj();
                            service.ID = Convert.ToInt32(reader["id"]);
                            service.code = reader["code"].ToString();
                            service.name = reader["name"].ToString();
                            service.price = Convert.ToDouble(reader["price"]);
                            service.hospitalID = Convert.ToInt32(reader["hospitalID"]);
                            service.serviceGroupID = Convert.ToInt32(reader["serviceGroupID"]);
                            service.serviceTypeID = Convert.ToInt32(reader["serviceTypeID"]);
                            service.serviceGroup = reader["serviceGroupName"].ToString();
                            service.isActive = Convert.ToBoolean(reader["isActive"]);

                            services.Add(service);
                        }
                    }
                }

                connection.Close();
            }
            services.Reverse();
            return services;
        }

        public int UpdateService(ServiceObj service)
        {
            int rowAffected = 0;
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                string query = @"
            UPDATE services
            SET name = @name, price = @price, serviceGroupID = @serviceGroupID,serviceTypeID = @serviceTypeID, isActive = @isActive
            WHERE id = @id";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", service.name);
                    command.Parameters.AddWithValue("@price", service.price);
                    command.Parameters.AddWithValue("@serviceGroupID", service.serviceGroupID);
                    command.Parameters.AddWithValue("@serviceTypeID", service.serviceTypeID);
                    command.Parameters.AddWithValue("@id", service.ID);
                    command.Parameters.AddWithValue("@isActive", service.isActive);

                   rowAffected = command.ExecuteNonQuery();
                }

                connection.Close();
            }
            return rowAffected;
        }

        public int InsertService(ServiceObj service)
        {
            int rowAffected = 0;
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                string checkQuery = @"
            SELECT 1
            FROM services
            WHERE code = @code OR name = @name;
        ";

                string insertQuery = @"
            INSERT INTO services (code, name, price, serviceGroupID, hospitalID, serviceTypeID, isActive)
            VALUES (@code, @name, @price, @serviceGroupID, @hospitalID, @serviceTypeID, @isActive);
        ";

                using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@code", service.code);
                    checkCommand.Parameters.AddWithValue("@name", service.name);

                    using (MySqlDataReader reader = checkCommand.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            rowAffected = -1;
                        }
                        else
                        {
                            reader.Close();

                            // No matching record found, proceed with the insert
                            using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
                            {
                                insertCommand.Parameters.AddWithValue("@code", service.code);
                                insertCommand.Parameters.AddWithValue("@name", service.name);
                                insertCommand.Parameters.AddWithValue("@price", service.price);
                                insertCommand.Parameters.AddWithValue("@hospitalID", service.hospitalID);
                                insertCommand.Parameters.AddWithValue("@serviceGroupID", service.serviceGroupID);
                                insertCommand.Parameters.AddWithValue("@serviceTypeID", service.serviceTypeID);
                                insertCommand.Parameters.AddWithValue("@isActive", service.isActive);

                                rowAffected = insertCommand.ExecuteNonQuery();
                            }
                        }
                    }
                }

                connection.Close();
            }
            return rowAffected;
        }


    }
}

