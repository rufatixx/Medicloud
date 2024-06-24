using System;
using System.Net.NetworkInformation;
using Medicloud.Models;
using MySql.Data.MySqlClient;

namespace Medicloud.DAL.Repository
{
    public class OrganizationRepo
    {
        private readonly string ConnectionString;

        public OrganizationRepo(string conString)
        {
            ConnectionString = conString;
        }

        public long InsertOrganization(string organizationName)
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                string query = "INSERT INTO organizations (name) VALUES (@name);";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    // Add parameter to prevent SQL injection
                    command.Parameters.AddWithValue("@name", organizationName);

                    command.ExecuteNonQuery();

                    // Retrieve the ID of the last inserted row
                    long lastId = command.LastInsertedId;

                    connection.Close();

                    // Return the last inserted ID
                    return lastId;
                }
            }
        }
        public List<Organization> GetOrganizationListByUser(int userID)
        {
            List<Organization> organizationList = new List<Organization>();

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {

                connection.Open();
                using (MySqlCommand organizationCom = new MySqlCommand("SELECT *,(select name from organizations where id = a.organizationID) as hName FROM user_organization_rel a where userID = @userID;", connection))
                {

                    organizationCom.Parameters.AddWithValue("@userID", userID);

                    using (MySqlDataReader organizationReader = organizationCom.ExecuteReader())
                    {
                        if (organizationReader.HasRows)
                        {
                            while (organizationReader.Read())
                            {
                                Organization organization = new Organization();
                                organization.id = organizationReader["id"] == DBNull.Value ? 0 : Convert.ToInt32(organizationReader["id"]);
                                organization.organizationName = organizationReader["hName"] == DBNull.Value ? "" : organizationReader["hName"].ToString();
                           
                                organization.userID = organizationReader["userID"] == DBNull.Value ? 0 : Convert.ToInt32(organizationReader["userID"]);
                                organization.organizationID = organizationReader["organizationID"] == DBNull.Value ? 0 : Convert.ToInt32(organizationReader["organizationID"]); 
                              
                                organizationList.Add(organization);
                            }


                        }
                      
                    }

                }

                connection.Close();
            }
            return organizationList;
        }
        public List<Organization> GetOrganizationList()
        {
            List<Organization> organizationList = new List<Organization>();

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {

                connection.Open();
                using (MySqlCommand organizationCom = new MySqlCommand("SELECT * from organizations", connection))
                {

                  

                    using (MySqlDataReader organizationReader = organizationCom.ExecuteReader())
                    {
                        if (organizationReader.HasRows)
                        {
                            while (organizationReader.Read())
                            {
                                Organization organization = new Organization();
                                organization.id = organizationReader["id"] == DBNull.Value ? 0 : Convert.ToInt32(organizationReader["id"]); 
                              
                                organization.organizationName = organizationReader["name"] == DBNull.Value ? "" : organizationReader["name"].ToString();
                                organizationList.Add(organization);
                            }


                        }

                    }

                }

                connection.Close();
            }
            return organizationList;
        }
        public long InsertOrganizationToUser( long userID, long organizationID)
        {
            long lastID = 0;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {


                    var sql = "";

                    connection.Open();

                    sql = @"Insert INTO user_organization_rel (organizationID,userID)
SELECT @organizationID,@userID FROM DUAL
WHERE NOT EXISTS 
  (SELECT * FROM user_organization_rel WHERE organizationID=@organizationID and userID=@userID )";



                    using (MySqlCommand com = new MySqlCommand(sql, connection))
                    {
                        com.Parameters.AddWithValue("@organizationID", organizationID);
                        com.Parameters.AddWithValue("@userID", userID);


                         com.ExecuteNonQuery();
                        lastID = com.LastInsertedId;

                    }
                    connection.Close();



                }


            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);


            }
            return lastID;
        }
        public int RemoveOrganizationFromUser(int userID, int organizationID)
        {
            int lastID = 0;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {


                    var sql = "";

                    connection.Open();

                    sql = @"DELETE FROM user_organization_rel WHERE userID = @userID and organizationID = @organizationID;";



                    using (MySqlCommand com = new MySqlCommand(sql, connection))
                    {
                        com.Parameters.AddWithValue("@organizationID", organizationID);
                        com.Parameters.AddWithValue("@userID", userID);


                        lastID = com.ExecuteNonQuery();


                    }
                    connection.Close();



                }


            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);


            }
            return lastID;
        }

    }
}

