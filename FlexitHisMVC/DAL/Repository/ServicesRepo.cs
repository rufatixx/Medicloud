using System;
using Dapper;
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
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                string query = "SELECT * FROM services WHERE organizationID = @organizationID AND isActive = 1 ORDER BY id DESC;";
                var serviceList = connection.Query<ServiceObj>(query, new { organizationID }).ToList();
                return serviceList;
            }
        }


        public List<ServiceObj> GetServicesWithServiceGroupName(int organizationID, int serviceGroupID = 0)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                var query = @"
            SELECT s.id, s.name, s.price, s.organizationID, s.cDate, s.serviceGroupID, s.serviceTypeID, s.code, s.isActive, 
                   sg.name AS serviceGroup
            FROM services s
            left JOIN service_group sg ON s.serviceGroupID = sg.id
            WHERE s.organizationID = @organizationID";

                var parameters = new DynamicParameters();
                parameters.Add("@organizationID", organizationID);

                if (serviceGroupID > 0)
                {
                    query += " AND s.serviceGroupID = @serviceGroupID";
                    parameters.Add("@serviceGroupID", serviceGroupID);
                }

                var services = connection.Query<ServiceObj>(query, parameters).ToList();
                services.Reverse();
                return services;
            }
        }


        public int UpdateService(ServiceObj service)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                string query = @"
            UPDATE services
            SET name = @name, 
                price = @price, 
                serviceGroupID = @serviceGroupID, 
                serviceTypeID = @serviceTypeID, 
                isActive = @isActive
            WHERE id = @ID";

                int rowAffected = connection.Execute(query, service);
                return rowAffected;
            }
        }

        public int InsertService(ServiceObj service)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                string insertQuery = @"
            INSERT INTO services (code, name, price, serviceGroupID, organizationID, serviceTypeID, isActive)
            VALUES (@code, @name, @price, @serviceGroupID, @organizationID, @serviceTypeID, @isActive);
        ";

                var parameters = new
                {
                    code = string.IsNullOrEmpty(service.code) ? null : service.code,
                    name = string.IsNullOrEmpty(service.name) ? null : service.name,
                    price = service.price,
                    serviceGroupID = service.serviceGroupID ?? 0,
                    organizationID = service.organizationID,
                    serviceTypeID = service.serviceTypeID,
                    isActive = service.isActive
                };

                int rowAffected = connection.Execute(insertQuery, parameters);
                return rowAffected;
            }
        }



        public List<ServiceStatisticsDTO> GetTop5SellingServiceStatistics(long organizationID)
        {
            try
            {
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = @"
                SELECT 
                    s.id AS ID,
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
            ";

                    var list = connection.Query<ServiceStatisticsDTO>(query, new { organizationID }).ToList();
                    return list;
                }
            }
            catch (Exception ex)
            {
                Medicloud.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);
                return new List<ServiceStatisticsDTO>();
            }
        }


        public List<ServiceObj> GetAllServices(long orgId, string keyword)
        {
            try
            {
                using (var con = new MySqlConnection(ConnectionString))
                {
                    con.Open();
                    string query = @"
                SELECT * FROM services
                WHERE isActive = 1 
                AND organizationID = @orgId
                AND (LOWER(name) LIKE CONCAT('%', LOWER(@search), '%') OR
                     LOWER(code) LIKE CONCAT('%', LOWER(@search), '%'))
            ";

                    var services = con.Query<ServiceObj>(query, new { orgId, search = keyword }).ToList();
                    services.Reverse();
                    return services;
                }
            }
            catch (Exception ex)
            {
                Medicloud.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);
                return new List<ServiceObj>();
            }
        }

    }
}

