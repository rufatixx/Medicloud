using System;
using Medicloud.BLL.Models;
using Medicloud.Models;
using Medicloud.Models.DTO;
using MySql.Data.MySqlClient;

namespace Medicloud.Models.Repository
{
    public class ServicesRepo
    {
        private readonly string ConnectionString;

        public ServicesRepo(string conString)
        {
            ConnectionString = conString;
        }
        public List<ServiceObj> GetServicesByOrganization(int organizationID)
        {
            List<ServiceObj> serviceList = new List<ServiceObj>();

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {

                connection.Open();
                using (MySqlCommand com = new MySqlCommand("SELECT * FROM services where organizationID=@organizationID and isActive=1 order by id desc;", connection))
                {
                    com.Parameters.AddWithValue("organizationID", organizationID);

                    MySqlDataReader reader = com.ExecuteReader();
                    if (reader.HasRows)
                    {


                        while (reader.Read())
                        {
                            ServiceObj service = new ServiceObj();

                            service.ID = Convert.ToInt32(reader["id"]);
                            service.organizationID = Convert.ToInt32(reader["organizationID"]);
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

        public List<ServiceObj> GetServicesWithServiceGroupName(int organizationID, int serviceGroupID = 0)
        {
            List<ServiceObj> services = new List<ServiceObj>();

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                string query = @"
            SELECT s.id, s.name, s.price, s.organizationID, s.cDate, s.serviceGroupID,s.serviceTypeID,s.code,s.isActive, sg.name AS serviceGroupName
            FROM services s
            INNER JOIN service_group sg ON s.serviceGroupID = sg.id
            WHERE s.organizationID = @organizationID";

                if (serviceGroupID > 0)
                {
                    query += " AND s.serviceGroupID = @serviceGroupID";
                }

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@organizationID", organizationID); // Set the organization ID parameter value

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
                            service.organizationID = Convert.ToInt32(reader["organizationID"]);
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

                string insertQuery = @"
            INSERT INTO services (code, name, price, serviceGroupID, organizationID, serviceTypeID, isActive)
            VALUES (@code, @name, @price, @serviceGroupID, @organizationID, @serviceTypeID, @isActive);
        ";

                using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
                {
                    if (!string.IsNullOrEmpty(service.code))
                    {
                        insertCommand.Parameters.AddWithValue("@code", service.code);
                    }
                    else
                    {
                        insertCommand.Parameters.AddWithValue("@code", DBNull.Value);
                    }

                    if (!string.IsNullOrEmpty(service.name))
                    {
                        insertCommand.Parameters.AddWithValue("@name", service.name);
                    }
                    else
                    {
                        insertCommand.Parameters.AddWithValue("@name", DBNull.Value);
                    }

                    insertCommand.Parameters.AddWithValue("@price", service.price);

                    if (service.organizationID != null)
                    {
                        insertCommand.Parameters.AddWithValue("@organizationID", service.organizationID);
                    }
                    else
                    {
                        insertCommand.Parameters.AddWithValue("@organizationID", DBNull.Value);
                    }

                    if (service.serviceGroupID != null)
                    {
                        insertCommand.Parameters.AddWithValue("@serviceGroupID", service.serviceGroupID);
                    }
                    else
                    {
                        insertCommand.Parameters.AddWithValue("@serviceGroupID", DBNull.Value);
                    }

                    if (service.serviceTypeID != null)
                    {
                        insertCommand.Parameters.AddWithValue("@serviceTypeID", service.serviceTypeID);
                    }
                    else
                    {
                        insertCommand.Parameters.AddWithValue("@serviceTypeID", DBNull.Value);
                    }

                    insertCommand.Parameters.AddWithValue("@isActive", service.isActive);

                    rowAffected = insertCommand.ExecuteNonQuery();
                }

                connection.Close();
            }
            return rowAffected;
        }


        public List<ServiceStatisticsDTO> GetTop5SellingServiceStatistics(long organizationID)

        {
            List<ServiceStatisticsDTO> list = new List<ServiceStatisticsDTO>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {


                    connection.Open();

                    using (MySqlCommand com = new MySqlCommand($@"
SELECT 
s.id as ID,
    s.name AS service_name,
    pcs.cdate AS last_purchase_date,
    s.price AS price,
    COUNT(pcs.id) AS quantity,
    (s.price * COUNT(pcs.id)) AS amount
FROM 
    medicloud.patient_card_service_rel pcs
JOIN 
    medicloud.services s ON pcs.serviceID = s.id
JOIN 
    medicloud.patient_card pc ON pcs.patientCardID = pc.id
WHERE 
    pc.organizationID = @organizationID
GROUP BY 
    pcs.serviceID
ORDER BY 
    quantity DESC, pcs.cdate DESC
LIMIT 5;


", connection))
                    {

                        com.Parameters.AddWithValue("@organizationID", organizationID);

                        MySqlDataReader reader = com.ExecuteReader();
                        if (reader.HasRows)
                        {


                            while (reader.Read())
                            {
                                ServiceStatisticsDTO serviceStatisticsDTO = new ServiceStatisticsDTO();

                                serviceStatisticsDTO.ID = reader["ID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ID"]);
                                serviceStatisticsDTO.name = reader["service_name"] == DBNull.Value ? "" : reader["service_name"].ToString();
                                serviceStatisticsDTO.lastPurchaseDate = Convert.ToDateTime(reader["last_purchase_date"]);
                                serviceStatisticsDTO.price = reader["price"] == DBNull.Value ? "" : reader["price"].ToString();
                                serviceStatisticsDTO.quantity = reader["quantity"] == DBNull.Value ? "" : reader["quantity"].ToString();
                                serviceStatisticsDTO.amount = reader["amount"] == DBNull.Value ? "" : reader["amount"].ToString();

                                list.Add(serviceStatisticsDTO);

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
                Medicloud.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);
                //response.status = 4;
            }


            return list;
        }

        public List<ServiceObj> GetAllServices(string keyword)
        {
            List<ServiceObj> services = new();
            MySqlConnection con = new(ConnectionString);

            string query = @"SELECT * FROM services
WHERE isActive = 1 AND 
(name like concat('%', @search, '%') or
code like concat('%', @search, '%'))
";

            MySqlCommand cmd = new(query, con);
            cmd.Parameters.AddWithValue("@search", keyword);

            try
            {
                con.Open();
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {

                    //  reader["new_customers_this_month"] == DBNull.Value ? "" : reader["new_customers_this_month"].ToString();
                    
                    ServiceObj service = new ServiceObj
                    {
                        ID = Convert.ToInt32(reader["id"]),
                        code = reader["code"] == DBNull.Value ? "" : reader["code"].ToString(),
                        name = reader["name"] == DBNull.Value ? "" : reader["name"].ToString(),
                        price = Convert.ToDouble(reader["price"]),
                        organizationID = Convert.ToInt32(reader["organizationID"]),
                        serviceGroupID = Convert.ToInt32(reader["serviceGroupID"]),
                        serviceTypeID = Convert.ToInt32(reader["serviceTypeID"])
                    };

                    services.Add(service);
                }
                services.Reverse();
                con.Close();
            }
            catch (Exception ex)
            {
                Medicloud.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);
            }

            return services;
        }
    }
}

