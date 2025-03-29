using System;
using System.Collections.Generic;
using System.Dynamic;
using Dapper;
using Medicloud.DAL.Infrastructure.Abstract;
using Medicloud.DAL.Repository.Abstract;
using MySql.Data.MySqlClient;

namespace Medicloud.DAL.Repository.Concrete
{
    public class ServicePriceGroupRepository : IServicePriceGroupRepository
    {
        private string _connectionString;
        private readonly IUnitOfWork _unitOfWork;
        public ServicePriceGroupRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<dynamic> GetServicesByPriceGroupID(int priceGroupID)
        {
            _unitOfWork.BeginConnection();
            var con = _unitOfWork.GetConnection();
            string query = @"
            SELECT a.*, s.name AS serviceName
            FROM medicloud.service_pricegroup a
            JOIN medicloud.services s ON a.serviceID = s.id
            WHERE a.priceGroupID = @priceGroupID";

                var results = con.Query(query, new { priceGroupID }).ToList();
                return results;
            
        }

        public List<dynamic> GetActiveServicesByPriceGroupID(int priceGroupID, long organizationID)
        {
            _unitOfWork.BeginConnection();
            var con = _unitOfWork.GetConnection();
                string query = @"
        SELECT sp.*,
               s.name AS serviceName,
               s.isActive AS serviceIsActive,
               IFNULL(sd.departmentCount, 0) AS departmentCount
        FROM medicloud.service_pricegroup sp
        JOIN medicloud.services s ON sp.serviceID = s.id
        LEFT JOIN (
            SELECT serviceID, COUNT(*) AS departmentCount
            FROM medicloud.service_dep_rel
            GROUP BY serviceID
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

