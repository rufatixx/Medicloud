using System;
using System.Collections.Generic;
using FlexitHisMVC.Models;
using MySql.Data.MySqlClient;

namespace FlexitHisMVC.Data
{
    public class CompanyRepo
    {
        private readonly string ConnectionString;

        public CompanyRepo(string conString)
        {
            ConnectionString = conString;
        }
        public List<Company> GetCompanies(int hospitalID)

        {


            List<Company> companyList = new List<Company>();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {


                    connection.Open();

                    using (MySqlCommand com = new MySqlCommand($@"SELECT * FROM company where hospitalID = @hospitalID", connection))
                    {

                        com.Parameters.AddWithValue("@hospitalID", hospitalID);

                        MySqlDataReader reader = com.ExecuteReader();
                        if (reader.HasRows)
                        {


                            while (reader.Read())
                            {

                                Company cGroup = new Company();
                                cGroup.id = Convert.ToInt64(reader["id"]);
                                cGroup.groupID = Convert.ToInt32(reader["groupID"]);
                                cGroup.name = reader["name"] == DBNull.Value ? "" : reader["name"].ToString();
                                cGroup.isActive = Convert.ToInt32(reader["isActive"]);
                                cGroup.cdate = Convert.ToDateTime(reader["cdate"]);
                                cGroup.cUserID = Convert.ToInt32(reader["userID"]);
                                //department.typeID = Convert.ToInt64(reader["depTypeID"]);
                                //department.type = reader["typeName"] == DBNull.Value ? "" : reader["typeName"].ToString();



                                companyList.Add(cGroup);


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
                FlexitHisMVC.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);

            }


            return companyList;
        }
        public List<Company> GetActiveCompanies(int hospitalID)

        {


            List<Company> companyList = new List<Company>();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {


                    connection.Open();

                    using (MySqlCommand com = new MySqlCommand($@"SELECT * FROM company where hospitalID = @hospitalID and isActive = 1", connection))
                    {

                        com.Parameters.AddWithValue("@hospitalID", hospitalID);

                        MySqlDataReader reader = com.ExecuteReader();
                        if (reader.HasRows)
                        {


                            while (reader.Read())
                            {

                                Company cGroup = new Company();
                                cGroup.id = Convert.ToInt64(reader["id"]);
                                cGroup.groupID = Convert.ToInt32(reader["groupID"]);
                                cGroup.name = reader["name"] == DBNull.Value ? "" : reader["name"].ToString();
                                cGroup.isActive = Convert.ToInt32(reader["isActive"]);
                                cGroup.cdate = Convert.ToDateTime(reader["cdate"]);
                                cGroup.cUserID = Convert.ToInt32(reader["userID"]);
                                //department.typeID = Convert.ToInt64(reader["depTypeID"]);
                                //department.type = reader["typeName"] == DBNull.Value ? "" : reader["typeName"].ToString();



                                companyList.Add(cGroup);


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
                FlexitHisMVC.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);

            }


            return companyList;
        }

        public bool InsertCompany(int userID, int hospitalID, string companyName, int cGroupID)
        {

            try
            {
                long lastID = 0;
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {

                    connection.Open();





                    using (MySqlCommand com = new MySqlCommand(@"Insert INTO company (name,groupID,hospitalID,userID) values (@companyName,@cGroupID,@hospitalID,@userID)", connection))

                    {
                        com.Parameters.AddWithValue("@companyName", companyName);
                        com.Parameters.AddWithValue("@cGroupID", cGroupID);
                        com.Parameters.AddWithValue("@hospitalID", hospitalID);
                        com.Parameters.AddWithValue("@userID", userID);

                        com.ExecuteNonQuery();
                        lastID = com.LastInsertedId;
                    }



                    if (lastID > 0)
                    {


                        return true;
                        //response.status = 1; //inserted
                    }


                    connection.Close();
                }

            }
            catch (Exception ex)
            {
                FlexitHisMVC.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);
                //response.status = 4; // not inserted (error)
            }


            return false;
        }
        public bool UpdateCompany(int userID, int hospitalID, int id, string name, int isActive)
        {
            try
            {
                long lastID = 0;
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {

                    connection.Open();





                    using (MySqlCommand com = new MySqlCommand(@"UPDATE company
SET name = @name,isActive = @isActive WHERE id = @id and hospitalID = @hospitalID;", connection))

                    {
                        com.Parameters.AddWithValue("@id", id);
                        com.Parameters.AddWithValue("@name", name);
                        com.Parameters.AddWithValue("@isActive", isActive);
                        com.Parameters.AddWithValue("@hospitalID", hospitalID);
                        com.Parameters.AddWithValue("@userID", userID);

                        com.ExecuteNonQuery();
                        lastID = com.LastInsertedId;
                    }



                    if (lastID > 0)
                    {


                        return true;
                        //response.status = 1; //inserted
                    }


                    connection.Close();
                }

            }
            catch (Exception ex)
            {
                FlexitHisMVC.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);

            }


            return false;
        }

    }

}

