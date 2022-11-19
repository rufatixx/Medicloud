using System;
using System.Collections.Generic;
using FlexitHisMVC.Models;
using MySql.Data.MySqlClient;

namespace FlexitHisMVC.Data
{
    public class DepartmentsRepo
    {
        private readonly string ConnectionString;

        public DepartmentsRepo(string conString)
        {
            ConnectionString = conString;
        }

        public List<Department> GetDepartmentsByUser(int userID)

        {
          
            List <Department> depList = new List<Department>();
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

                                Department department = new Department();
                                department.ID = Convert.ToInt64(reader["id"]);
                             


                                depList.Add(department);


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


            return depList;
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
                                department.name = reader["depName"] == DBNull.Value ? "" : reader["depName"].ToString();
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
        public List<Department> GetDepartmentsByHospital(int hospitalID)

        {

            List<Department> depListByBuilding = new List<Department>();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {


                    connection.Open();
                    using (MySqlCommand com = new MySqlCommand($@"SELECT *,
(select name from department_type where id= a.depTypeID)as depTypeName,
(select name from buildings where id= a.buildingID)as buildingName
FROM departments a where buildingID in (select id from buildings where hospitalID=@hospitalID) ", connection))
                    {

                        com.Parameters.AddWithValue("@hospitalID", hospitalID);
                        MySqlDataReader reader = com.ExecuteReader();
                        if (reader.HasRows)
                        {


                            while (reader.Read())
                            {

                                Department department = new Department();
                                department.ID = Convert.ToInt64(reader["id"]);
                                department.name = reader["depName"] == DBNull.Value ? "" : reader["depName"].ToString();
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

        public bool InsertDepartments(long buildingID, string depName, int depTypeID)
        {
           
            try
            {
                long lastID = 0;
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {

                    connection.Open();





                    using (MySqlCommand com = new MySqlCommand(@"Insert INTO departments (buildingID,depName,depTypeID) values (@buildingID,@depName,@depTypeID)", connection))

                    {
                        com.Parameters.AddWithValue("@buildingID", buildingID);
                        com.Parameters.AddWithValue("@depName", depName);
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
                FlexitHisMVC.StandardMessages.CallSerilog(ex);
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
                FlexitHisMVC.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);
                //response.status = 4; // not inserted (error)
            }

            return false;
         
        }



    }
}

