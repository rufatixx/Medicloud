using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace Medicloud.Models
{
    public class BuildingRepo
    {
        private readonly string ConnectionString;

        public BuildingRepo(string conString)
        {
            ConnectionString = conString;
        }
        public List<Building> GetBuildings(long organizationID)

        {

            
            List <Building> buildingList = new List<Building>();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {


                    connection.Open();

                    using (MySqlCommand com = new MySqlCommand($@"SELECT * FROM buildings where organizationID = @organizationID", connection))
                    {

                        com.Parameters.AddWithValue("@organizationID", organizationID);
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



                                buildingList.Add(building);


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
                Medicloud.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);
               
            }


            return buildingList;
        }
        public List<Building> GetAllBuildings()

        {


            List<Building> buildingList = new List<Building>();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {


                    connection.Open();

                    using (MySqlCommand com = new MySqlCommand($@"SELECT * FROM buildings", connection))
                    {

                      
                        MySqlDataReader reader = com.ExecuteReader();
                        if (reader.HasRows)
                        {


                            while (reader.Read())
                            {

                                Building building = new Building();
                                building.id = Convert.ToInt64(reader["id"]);
                                building.name = reader["name"] == DBNull.Value ? "" : reader["name"].ToString();
                                building.organizationID = reader["organizationID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["organizationID"]);
                                //department.typeID = Convert.ToInt64(reader["depTypeID"]);
                                //department.type = reader["typeName"] == DBNull.Value ? "" : reader["typeName"].ToString();



                                buildingList.Add(building);


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
                Medicloud.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);

            }


            return buildingList;
        }

    }
}

