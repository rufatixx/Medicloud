﻿using System;
using Medicloud.DAL.Entities;
using Medicloud.Models;
using Medicloud.Models.DTO;
using MySql.Data.MySqlClient;

namespace Medicloud.Repository
{
	public class UserDepRelRepo
	{
        private readonly string ConnectionString;

        public UserDepRelRepo(string conString)
        {
            ConnectionString = conString;
        }
        public List<UserDepRel> GetUserDepartments(int userID)

        {

            List<UserDepRel> userDepRelList = new List<UserDepRel>();
            
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {


                    connection.Open();

                    using (MySqlCommand com = new MySqlCommand($@"SELECT * FROM user_dep_rel where userID = @userID", connection))
                    {
                        com.Parameters.AddWithValue("@userID", userID);

                        MySqlDataReader reader = com.ExecuteReader();
                        if (reader.HasRows)
                        {


                            while (reader.Read())
                            {

                                UserDepRel department = new UserDepRel();
                                department.ID = Convert.ToInt64(reader["id"]);
                                department.depID = Convert.ToInt32(reader["depID"]);
                                department.readOnly = Convert.ToInt32(reader["read_only"]);
                                department.fullAccess = Convert.ToInt32(reader["full_access"]);
                                department.isActive = Convert.ToInt32(reader["isActive"]);


                                userDepRelList.Add(department);


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
                //FlexitHis_API.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);

            }


            return userDepRelList;
        }
        public List<UserDAO> GetUsersByDepartment(int depID)
        {
            List<UserDAO> userList = new List<UserDAO>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();

                    string query = @"
                SELECT b.id, b.name, b.surname, c.name as speciality
                FROM user_dep_rel a
                INNER JOIN users b ON a.userID = b.id
                INNER JOIN speciality c ON b.specialityID = c.id
                WHERE a.depID = @depID";

                    using (MySqlCommand com = new MySqlCommand(query, connection))
                    {
                        com.Parameters.AddWithValue("@depID", depID);

                        MySqlDataReader reader = com.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                UserDAO user = new UserDAO();
                                user.ID = Convert.ToInt32(reader["id"]);
                                user.name = reader["name"].ToString();
                                user.surname = reader["surname"].ToString();
                                user.speciality = new Speciality {id = Convert.ToInt64(reader["specialityID"]) , name = reader["speciality"].ToString() };

                                userList.Add(user);
                            }
                        }
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return userList;
        }
        public int InsertDepToUser(int userID, int depID, bool read_only, bool full_access)
        {
            int lastID = 0;
           
            try
            {
                if (userID > 0 && depID > 0)
                {
                    using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                    {


                        var sql = "";

                        connection.Open();

                        sql = @"Insert INTO user_dep_rel (depID,userID,read_only,full_access )
SELECT @depID,@userID,@read_only,@full_access FROM DUAL
WHERE NOT EXISTS 
  (SELECT * FROM user_dep_rel WHERE depID=@depID and userID=@userID )";



                        using (MySqlCommand com = new MySqlCommand(sql, connection))
                        {
                            com.Parameters.AddWithValue("@depID", depID);
                            com.Parameters.AddWithValue("@userID", userID);
                            com.Parameters.AddWithValue("@read_only", read_only);
                            com.Parameters.AddWithValue("@full_access", full_access);


                            lastID = com.ExecuteNonQuery();


                        }
                        connection.Close();



                    }
                }
              


            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);


            }
            return lastID;
        }
        public int RemoveDepFromUser(int userID, int depID)
        {
            int lastID = 0;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {


                    var sql = "";

                    connection.Open();

                    sql = @"DELETE FROM user_dep_rel WHERE userID = @userID and depID = @depID;";



                    using (MySqlCommand com = new MySqlCommand(sql, connection))
                    {
                        com.Parameters.AddWithValue("@depID", depID);
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

