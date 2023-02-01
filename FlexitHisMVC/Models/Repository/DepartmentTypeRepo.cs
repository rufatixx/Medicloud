using System;
using System.Collections.Generic;
using FlexitHisMVC.Areas.Admin.Model;
using FlexitHisMVC.Models;
using MySql.Data.MySqlClient;

namespace FlexitHisMVC.Data
{
    public class DepartmentTypeRepo
    {
        private readonly string ConnectionString;

        public DepartmentTypeRepo(string conString)
        {
            ConnectionString = conString;
        }

        public List<DepartmentType> GetDepartmentTypes()

        {

            List <DepartmentType> depTypeList = new List<DepartmentType>();
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
                                depTypeList.Add(departmentType);


                            }

                          
                        }
                       
                    }
                    connection.Close();

                }

            }
            catch (Exception ex)
            {
                //FlexitHis_API.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);
                //response.status = 4;
            }


            return depTypeList;
        }
    }
}

