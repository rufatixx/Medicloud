using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text.RegularExpressions;
using crypto;
using FlexitHis_API.Models.Structs;
using FlexitHis_API.Models.Structs.Admin;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace FlexitHis_API.Models.Db
{
    public class AdminSelect
    {
        private readonly string ConnectionString;
        public IConfiguration Configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public AdminSelect(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            ConnectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
            _hostingEnvironment = hostingEnvironment;

        }
        public ResponseStruct<Building> GetBuildings(long hospitalID)

        {
           
            ResponseStruct<Building> response = new ResponseStruct<Building>();
            response.data = new List<Building>();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {


                    connection.Open();

                    using (MySqlCommand com = new MySqlCommand($@"SELECT * FROM buildings where hospitalID = @hospitalID", connection))
                    {

                        com.Parameters.AddWithValue("@hospitalID", hospitalID);
                        MySqlDataReader reader = com.ExecuteReader();
                        if (reader.HasRows)
                        {


                            while (reader.Read())
                            {

                                Building building = new Building();
                                building.id = Convert.ToInt64(reader["id"]);
                                building.name = reader["name"] == DBNull.Value ? "" : reader["name"].ToString();
                                //department.typeID = Convert.ToInt64(reader["depTypeID"]);
                                //department.type = reader["typeName"] == DBNull.Value ? "" : reader["typeName"].ToString();



                                response.data.Add(building);


                            }

                            response.data.Reverse();

                            response.status = 1;
                        }
                        else
                        {
                            response.status = 2;



                        }
                    }


                    connection.Close();

                 
                }

            }
            catch (Exception ex)
            {
                //FlexitHis_API.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);
                response.status = 4;
            }


            return response;
        }

        public ResponseStruct<Department> AdminGetDepartments()

        {
            ResponseStruct<Department> response = new ResponseStruct<Department>();
            response.data = new List<Department>();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {


                    connection.Open();

                    using (MySqlCommand com = new MySqlCommand($@"SELECT * FROM departments", connection))
                    {


                        MySqlDataReader reader = com.ExecuteReader();
                        if (reader.HasRows)
                        {


                            while (reader.Read())
                            {

                                Department department = new Department();
                                department.ID = Convert.ToInt64(reader["id"]);
                                department.name = reader["name"] == DBNull.Value ? "" : reader["name"].ToString();
                                //department.typeID = Convert.ToInt64(reader["depTypeID"]);
                                //department.type = reader["typeName"] == DBNull.Value ? "" : reader["typeName"].ToString();



                                response.data.Add(department);


                            }

                            response.data.Reverse();

                            response.status = 1;
                        }
                        else
                        {
                            response.status = 2;



                        }
                    }


                    connection.Close();


                }


            }
            catch (Exception ex)
            {
                //FlexitHis_API.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);
                response.status = 4;
            }


            return response;
        }

        public ResponseStruct<DepartmentType> AdminGetDepartmentTypes()

        {
            ResponseStruct<DepartmentType> response = new ResponseStruct<DepartmentType>();
            response.data = new List<DepartmentType>();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {


                    connection.Open();
                    using (MySqlCommand com = new MySqlCommand($@"SELECT * FROM department_type ", connection))
                    {


                        MySqlDataReader reader = com.ExecuteReader();
                        if (reader.HasRows)
                        {


                            while (reader.Read())
                            {

                                DepartmentType departmentType = new DepartmentType();
                                departmentType.ID = Convert.ToInt64(reader["id"]);
                                departmentType.name = reader["name"] == DBNull.Value ? "" : reader["name"].ToString();
                                response.data.Add(departmentType);


                            }

                            response.data.Reverse();

                            response.status = 1;
                        }
                        else
                        {
                            response.status = 2;



                        }
                    }
                    connection.Close();
                    
                }

            }
            catch (Exception ex)
            {
                //FlexitHis_API.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);
                response.status = 4;
            }


            return response;
        }

        public ResponseStruct<Department> GetDepartmentsByBuilding( int buildingID)

        {
            ResponseStruct<Department> response = new ResponseStruct<Department>();
            response.data = new List<Department>();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {


                    connection.Open();
                    using (MySqlCommand com = new MySqlCommand($@"SELECT *,
(select name from department_type where id= a.depTypeID)as depTypeName,
(select name from buildings where id= a.buildingID)as buildingName
FROM building_to_dep_rel a where buildingID = @buildingID ", connection))
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
                                response.data.Add(department);


                            }

                            response.data.Reverse();

                            response.status = 1;
                        }
                        else
                        {
                            response.status = 2;



                        }
                    }
                    connection.Close();
                   

                }


            }
            catch (Exception ex)
            {
                //FlexitHis_API.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);
                response.status = 4;
            }


            return response;
        }



        public ResponseStruct<CompanyGroup> GetCompanyGroups( int hospitalID)

        {

            ResponseStruct<CompanyGroup> response = new ResponseStruct<CompanyGroup>();
            response.data = new List<CompanyGroup>();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {


                    connection.Open();

                    using (MySqlCommand com = new MySqlCommand($@"SELECT * FROM company_group where hospitalID = @hospitalID", connection))
                    {

                        com.Parameters.AddWithValue("@hospitalID", hospitalID);
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



                                response.data.Add(cGroup);


                            }

                            response.data.Reverse();

                            response.status = 1;
                        }
                        else
                        {
                            response.status = 2;



                        }
                    }


                    connection.Close();

                   

                }


            }
            catch (Exception ex)
            {
                //FlexitHis_API.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);
                response.status = 4;
            }


            return response;
        }


        public ResponseStruct<Company> GetCompanies( int hospitalID)

        {

            ResponseStruct<Company> response = new ResponseStruct<Company>();
            response.data = new List<Company>();
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



                                response.data.Add(cGroup);


                            }

                            response.data.Reverse();

                            response.status = 1;
                        }
                        else
                        {
                            response.status = 2;



                        }
                    }


                    connection.Close();


                }

            }
            catch (Exception ex)
            {
                FlexitHisMVC.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);
                response.status = 4;
            }


            return response;
        }
    }
}
