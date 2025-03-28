using System;
using System.Collections.Generic;
using System.Dynamic;
using Dapper;
using MySql.Data.MySqlClient;

namespace Medicloud.Models
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
            using (var con = new MySqlConnection(_connectionString))
            {
                con.Open();
                string query = @"
            SELECT a.*, s.name AS serviceName
            FROM medicloud.service_pricegroup a
            JOIN medicloud.services s ON a.serviceID = s.id
            WHERE a.priceGroupID = @priceGroupID";

                var results = con.Query(query, new { priceGroupID }).ToList();
                return results;
            }
        }

        public List<dynamic> GetActiveServicesByPriceGroupID(int priceGroupID, long organizationID)
        {
            using (var con = new MySqlConnection(_connectionString))
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
              AND s.isActive = 1
              AND s.organizationID = @organizationID;
        ";

                var results = con.Query(query, new { priceGroupID, organizationID }).ToList();
                return results;
            }
        }

    }
}

