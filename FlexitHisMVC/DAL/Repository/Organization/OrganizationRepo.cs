using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Medicloud.DAL.Entities;
using Medicloud.DAL.Infrastructure.Abstract;
using Medicloud.DAL.Repository.Organization;
using Medicloud.Models;
using MySql.Data.MySqlClient;

namespace Medicloud.DAL.Repository.Organization
{
    public class OrganizationRepo:IOrganizationRepo
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrganizationRepo(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Inserts a new organization record and returns its primary key.
        /// </summary>
        public long InsertOrganization(string organizationName)
        {
            _unitOfWork.BeginConnection();
            var con = _unitOfWork.GetConnection();

            const string query = @"INSERT INTO organizations (name) VALUES (@name);";

            // Execute insert
            con.Execute(query, new { name = organizationName });

            // Fetch the last inserted ID
            long lastId = con.QuerySingle<long>("SELECT LAST_INSERT_ID();");
            return lastId;
        }

        /// <summary>
        /// Returns list of organizations where this user is a manager (role_id=3).
        /// </summary>
        public List<OrganizationDAO> GetOrganizationListWhereUserIsManager(int userID)
        {
            _unitOfWork.BeginConnection();
            var con = _unitOfWork.GetConnection();

            const string query = @"
                SELECT  
                    a.*,
                    o.name AS organizationName
                FROM user_organization_rel a
                JOIN organizations o ON o.id = a.organizationID
                WHERE a.userID = @userID 
                  AND a.role_id = 3
                GROUP BY a.organizationID;";

            var result = con.Query<OrganizationDAO>(query, new { userID }).ToList();
            return result;
        }

        /// <summary>
        /// Returns a list of all organizations for which the user has any role.
        /// </summary>
        public List<OrganizationDAO> GetOrganizationListByUser(int userID)
        {
            _unitOfWork.BeginConnection();
            var con = _unitOfWork.GetConnection();

            const string query = @"
                SELECT 
                    a.*,
                    o.name AS organizationName
                FROM user_organization_rel a
                JOIN organizations o ON o.id = a.organizationID
                WHERE a.userID = @userID
                GROUP BY a.organizationID;";

            var result = con.Query<OrganizationDAO>(query, new { userID }).ToList();
            return result;
        }

        /// <summary>
        /// Returns all organizations.
        /// </summary>
        public List<OrganizationDAO> GetOrganizationList()
        {
            _unitOfWork.BeginConnection();
            var con = _unitOfWork.GetConnection();

            // Map fields directly to OrganizationDAO
            const string query = @"SELECT 
                                       id AS id, 
                                       name AS organizationName 
                                   FROM organizations;";

            var organizationList = con.Query<OrganizationDAO>(query).ToList();
            return organizationList;
        }

        /// <summary>
        /// Inserts a record into user_organization_rel if it doesn't already exist.
        /// Returns the ID of the newly inserted row (or 0 if no insert occurred).
        /// </summary>
        public long InsertOrganizationToUser(long userID, long organizationID)
        {
            _unitOfWork.BeginConnection();
            var con = _unitOfWork.GetConnection();

            const int roleId = 4;
            const string sql = @"
                INSERT INTO user_organization_rel (organizationID, userID, role_id)
                SELECT @organizationID, @userID, @roleId 
                FROM DUAL
                WHERE NOT EXISTS (
                    SELECT 1 
                    FROM user_organization_rel 
                    WHERE organizationID=@organizationID 
                      AND userID=@userID  
                      AND role_id=@roleId
                );";

            // Execute the insert. If the row already exists, this inserts nothing.
            con.Execute(sql, new { organizationID, userID, roleId });

            // Fetch the last inserted ID
            long lastID = con.QuerySingle<long>("SELECT LAST_INSERT_ID();");
            return lastID;
        }

        /// <summary>
        /// Removes association between a user and an organization.
        /// Returns the number of rows affected.
        /// </summary>
        public int RemoveOrganizationFromUser(int userID, int organizationID)
        {
            _unitOfWork.BeginConnection();
            var con = _unitOfWork.GetConnection();

            const string sql = @"
                DELETE FROM user_organization_rel
                WHERE userID = @userID AND organizationID = @organizationID;";

            int rowsAffected = con.Execute(sql, new { userID, organizationID });
            return rowsAffected;
        }
    }
}
