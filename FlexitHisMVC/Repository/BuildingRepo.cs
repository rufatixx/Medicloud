using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace FlexitHisMVC.Models
{
    public class BuildingRepo
    {
        private readonly string ConnectionString;

        public BuildingRepo(string conString)
        {
            ConnectionString = conString;
        }
        public List<Building> GetBuildings(long hospitalID)

        {

            
            List <Building> buildingList = new List<Building>();
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
                FlexitHisMVC.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);
               
            }


            return buildingList;
        }


    }
}

