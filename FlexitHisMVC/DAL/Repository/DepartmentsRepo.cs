using System;
using System.Collections.Generic;
using Medicloud.Models;
using MySql.Data.MySqlClient;

namespace Medicloud.Data
{
    public class DepartmentsRepo
    {
        private readonly string ConnectionString;

        public DepartmentsRepo(string conString)
        {
            ConnectionString = conString;
        }

       

        public List<Department> GetDepartmentsByBuilding(int buildingID)

        {

            List <Department> depListByBuilding = new List<Department>();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {


                    connection.Open();
                    using (MySqlCommand com = new MySqlCommand($@"SELECT *,
(select name from department_type where id= a.depTypeID)as depTypeName,
(select name from buildings where id= a.buildingID)as buildingName
FROM departments a where buildingID = @buildingID ", connection))
                    {

                        com.Parameters.AddWithValue("@buildingID", buildingID);
                        MySqlDataReader reader = com.ExecuteReader();
                        if (reader.HasRows)
                        {


                            while (reader.Read())
                            {

                                Department department = new Department();
                                department.ID = Convert.ToInt64(reader["id"]);
                                department.name = reader["name"] == DBNull.Value ? "" : reader["name"].ToString();
                                department.type = reader["depTypeName"] == DBNull.Value ? "" : reader["depTypeName"].ToString();
                                department.typeID = Convert.ToInt64(reader["depTypeID"]);
                                department.docIsRequired = Convert.ToInt32(reader["DrIsRequired"]);
                                department.genderID = Convert.ToInt32(reader["genderID"]);
                                department.buildingID = Convert.ToInt64(reader["buildingID"]);
                                department.buildingName = reader["buildingName"] == DBNull.Value ? "" : reader["buildingName"].ToString();
                                department.isActive = Convert.ToInt32(reader["isActive"]);
                                department.isRandevuActive = Convert.ToInt32(reader["isRandevuActive"]);
                                depListByBuilding.Add(department);


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


            return depListByBuilding;
        }
        public List<Department> GetDepartmentsByOrganization(int organizationID)

        {

            List<Department> depListByBuilding = new List<Department>();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {


                    connection.Open();
                    using (MySqlCommand com = new MySqlCommand($@"SELECT a.*,
       dt.name AS depTypeName,
       b.name AS buildingName
FROM departments a
JOIN department_type dt ON dt.id = a.depTypeID
JOIN buildings b ON b.id = a.buildingID
WHERE a.buildingID IN (SELECT id FROM buildings WHERE organizationID = @organizationID)", connection))
                    {

                        com.Parameters.AddWithValue("@organizationID", organizationID);
                        MySqlDataReader reader = com.ExecuteReader();
                        if (reader.HasRows)
                        {


                            while (reader.Read())
                            {

                                Department department = new Department();
                                department.ID = Convert.ToInt64(reader["id"]);
                                department.name = reader["name"] == DBNull.Value ? "" : reader["name"].ToString();
                                department.type = reader["depTypeName"] == DBNull.Value ? "" : reader["depTypeName"].ToString();
                                department.typeID = Convert.ToInt64(reader["depTypeID"]);
                                department.docIsRequired = Convert.ToInt32(reader["DrIsRequired"]);
                                department.genderID = Convert.ToInt32(reader["genderID"]);
                                department.buildingID = Convert.ToInt64(reader["buildingID"]);
                                department.buildingName = reader["buildingName"] == DBNull.Value ? "" : reader["buildingName"].ToString();
                                department.isActive = Convert.ToInt32(reader["isActive"]);
                                department.isRandevuActive = Convert.ToInt32(reader["isRandevuActive"]);
                                depListByBuilding.Add(department);


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


            return depListByBuilding;
        }
        public List<Department> GetDepartmentsInService(int serviceID)

        {

            List<Department> depListByBuilding = new List<Department>();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {


                    connection.Open();
                    using (MySqlCommand com = new MySqlCommand($@"SELECT a.*, b.name 
FROM medicloud.service_dep_rel a
LEFT JOIN medicloud.departments b ON a.depID = b.id
WHERE a.serviceID = @serviceID;", connection))
                    {

                        com.Parameters.AddWithValue("@serviceID", serviceID);
                        MySqlDataReader reader = com.ExecuteReader();
                        if (reader.HasRows)
                        {


                            while (reader.Read())
                            {

                                Department department = new Department();
                                department.ID = Convert.ToInt64(reader["depID"]);
                                department.name = reader["name"] == DBNull.Value ? "" : reader["name"].ToString();
                               
                                depListByBuilding.Add(department);


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

            depListByBuilding.Reverse();
            return depListByBuilding;
        }
        public bool InsertDepartments(long buildingID, string name, int depTypeID)
        {
           
            try
            {
                long lastID = 0;
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {

                    connection.Open();





                    using (MySqlCommand com = new MySqlCommand(@"Insert INTO departments (buildingID,name,depTypeID) values (@buildingID,@name,@depTypeID)", connection))

                    {
                        com.Parameters.AddWithValue("@buildingID", buildingID);
                        com.Parameters.AddWithValue("@name", name);
                        com.Parameters.AddWithValue("@depTypeID", depTypeID);

                        com.ExecuteNonQuery();
                        lastID = com.LastInsertedId;
                    }


                    connection.Close();

                    if (lastID > 0)
                    {

                        return true;

                        //response.status = 1; //inserted
                    }
                     
                  
                }

            }
            catch (Exception ex)
            {
                Medicloud.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);
              
                //response.status = 4; // not inserted (error)
            }

            return false;

        }
        public bool InsertDepartmentToService(int serviceID, int depID)
        {

            try
            {
                long lastID = 0;
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {

                    connection.Open();





                    using (MySqlCommand com = new MySqlCommand(@"Insert INTO service_dep_rel (serviceID,depID) values (@serviceID,@depID)", connection))

                    {
                        com.Parameters.AddWithValue("@serviceID", serviceID);
                        com.Parameters.AddWithValue("@depID", depID);
                     

                        com.ExecuteNonQuery();
                        lastID = com.LastInsertedId;
                    }


                    connection.Close();

                    if (lastID > 0)
                    {

                        return true;

                        //response.status = 1; //inserted
                    }


                }

            }
            catch (Exception ex)
            {
                Medicloud.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);

                //response.status = 4; // not inserted (error)
            }

            return false;

        }
        public bool UpdateDepartments(int id, int gender, long buildingID, int depTypeID, int drIsRequired, int isActive, int isRandevuActive)
        {
            var affectedRows = 0;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {

                    connection.Open();





                    using (MySqlCommand com = new MySqlCommand(@"UPDATE departments
SET buildingID = @buildingID,genderID = @genderID, depTypeID= @depTypeID, DrIsRequired = @DrIsRequired, isActive = @isActive, isRandevuActive =@isRandevuActive
WHERE id = @id;", connection))

                    {
                        com.Parameters.AddWithValue("@id", id);
                        com.Parameters.AddWithValue("@buildingID", buildingID);
                        com.Parameters.AddWithValue("@genderID", gender);
                        com.Parameters.AddWithValue("@depTypeID", depTypeID);
                        com.Parameters.AddWithValue("@DrIsRequired", drIsRequired);
                        com.Parameters.AddWithValue("@isActive", isActive);
                        com.Parameters.AddWithValue("@isRandevuActive", isRandevuActive);

                     affectedRows  =  com.ExecuteNonQuery();

                    }

                    //response.status = 1; //inserted
                    if (affectedRows>0)
                    {
                        return true;
                    }

                    connection.Close();
                }

            }
            catch (Exception ex)
            {
                Medicloud.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);
                //response.status = 4; // not inserted (error)
            }

            return false;
         
        }

        

    }
}

