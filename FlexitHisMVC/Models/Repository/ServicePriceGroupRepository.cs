using System;
using System.Collections.Generic;
using System.Dynamic;
using MySql.Data.MySqlClient;

namespace FlexitHisMVC.Models
{
    public class ServicePriceGroupRepository
    {
        private string _connectionString;

        public ServicePriceGroupRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<dynamic> GetServicesByPriceGroupID(int priceGroupID)
        {
            List<dynamic> results = new List<dynamic>();

            using (MySqlConnection con = new MySqlConnection(_connectionString))
            {
                con.Open();

                string query = @"
                    SELECT a.*, s.name AS serviceName
                    FROM medicloud.service_pricegroup a
                    JOIN medicloud.services s ON a.serviceID = s.id
                    WHERE a.priceGroupID = @priceGroupID";

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@priceGroupID", priceGroupID);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dynamic result = new ExpandoObject();
                            // Map columns to dynamic object properties
                            result.ID = reader.GetInt32("ID");
                            result.priceGroupID = reader.GetInt32("priceGroupID");
                            result.serviceID = reader.GetInt32("serviceID");
                            result.serviceName = reader.GetString("serviceName");
                            results.Add(result);
                        }
                    }
                }
            }

            return results;
        }
        public List<dynamic> GetActiveServicesByPriceGroupID(int priceGroupID)
        {
            List<dynamic> results = new List<dynamic>();

            using (MySqlConnection con = new MySqlConnection(_connectionString))
            {
                con.Open();

                string query = @"
                 SELECT sp.*,
       s.name AS serviceName,
       s.isActive AS serviceIsActive,
       sd.departmentCount
FROM medicloud.service_pricegroup sp
JOIN services s ON sp.serviceID = s.id
JOIN (
   SELECT serviceID, COUNT(*) AS departmentCount
   FROM service_dep_rel
   GROUP BY serviceID
   HAVING COUNT(*) > 0
) sd ON s.id = sd.serviceID
WHERE sp.priceGroupID = @priceGroupID
  AND s.isActive = 1;


";

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@priceGroupID", priceGroupID);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dynamic result = new ExpandoObject();
                            // Map columns to dynamic object properties
                            result.ID = reader.GetInt32("ID");
                            result.priceGroupID = reader.GetInt32("priceGroupID");
                            result.serviceID = reader.GetInt32("serviceID");
                            result.serviceName = reader.GetString("serviceName");
                            result.servicePrice = reader.GetString("price");
                            result.isActive = reader.GetString("serviceIsActive");
                            results.Add(result);
                        }
                    }
                }
            }

            return results;
        }
    }
}

