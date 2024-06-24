using System;
using System.Collections.Generic;
using Medicloud.Models;
using MySql.Data.MySqlClient;

namespace Medicloud.Data
{
    public class CompanyGroupRepo
    {
        private readonly string ConnectionString;

        public CompanyGroupRepo(string conString)
        {
            ConnectionString = conString;
        }
        public List<CompanyGroup> GetCompanyGroups(int organizationID)

        {

           
            List <CompanyGroup> companyGroupList= new List<CompanyGroup>();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {


                    connection.Open();

                    using (MySqlCommand com = new MySqlCommand($@"SELECT * FROM company_group where organizationID = @organizationID", connection))
                    {

                        com.Parameters.AddWithValue("@organizationID", organizationID);
                        MySqlDataReader reader = com.ExecuteReader();
                        if (reader.HasRows)
                        {


                            while (reader.Read())
                            {

                                CompanyGroup cGroup = new CompanyGroup();
                                cGroup.id = Convert.ToInt64(reader["id"]);
                                cGroup.type = Convert.ToInt32(reader["type"]);
                                cGroup.name = reader["name"] == DBNull.Value ? "" : reader["name"].ToString();
                                cGroup.isActive = Convert.ToInt32(reader["isActive"]);
                                cGroup.cdate = Convert.ToDateTime(reader["cdate"]);
                                //department.typeID = Convert.ToInt64(reader["depTypeID"]);
                                //department.type = reader["typeName"] == DBNull.Value ? "" : reader["typeName"].ToString();



                                companyGroupList.Add(cGroup);


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


            return companyGroupList;
        }

        public bool InsertCompanyGroup(int userID, int organizationID, string cGroupName, int cGroupType)
        {
         
            try
            {
                long lastID = 0;
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {

                    connection.Open();





                    using (MySqlCommand com = new MySqlCommand(@"Insert INTO company_group (name,type,organizationID) values (@cGroupName,@groupType,@organizationID)", connection))

                    {
                        com.Parameters.AddWithValue("@cGroupName", cGroupName);
                        com.Parameters.AddWithValue("@groupType", cGroupType);
                        com.Parameters.AddWithValue("@organizationID", organizationID);

                        com.ExecuteNonQuery();
                        lastID = com.LastInsertedId;
                    }



                    if (lastID > 0)
                    {


                        return true;
                        //response.status = 1; //inserted
                    }
                    else
                   

                    connection.Close();
                }


            }
            catch (Exception ex)
            {
                Medicloud.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);
               
            }


            return false;
        }
        public bool UpdateCompanyGroup(int userID, int organizationID, int id, string name, int isActive)
        {
        
            try
            {
                long lastID = 0;
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {

                    connection.Open();





                    using (MySqlCommand com = new MySqlCommand(@"UPDATE company_group
SET name = @name,isActive = @isActive WHERE id = @id and organizationID = @organizationID;", connection))

                    {
                        com.Parameters.AddWithValue("@id", id);
                        com.Parameters.AddWithValue("@name", name);
                        com.Parameters.AddWithValue("@isActive", isActive);
                        com.Parameters.AddWithValue("@organizationID", organizationID);
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
                Medicloud.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);
               
            }


            return false;
        }
    }

}

