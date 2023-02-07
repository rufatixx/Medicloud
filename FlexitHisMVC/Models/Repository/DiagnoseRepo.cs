using System;
using FlexitHisMVC.Models.Domain;
using MySql.Data.MySqlClient;

namespace FlexitHisMVC.Models.Repository
{
    public class DiagnoseRepo
    {

        private readonly string ConnectionString;

        public DiagnoseRepo(string conString)
        {
            ConnectionString = conString;
        }
        public List<Diagnose> SearchDiagnose(string icdID, string name)

        {

           

            List<Diagnose> diagnoseList = new List<Diagnose>();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {


                    connection.Open();

                    using (MySqlCommand com = new MySqlCommand($@"SELECT * FROM diagnose 
WHERE lower(icd_id) LIKE lower(@icdID) or lower(name) LIKE lower(@name);", connection))
                    {
                        com.Parameters.AddWithValue("@icdID", icdID?.Length > 0 ? $"%{icdID}%" : "");
                        com.Parameters.AddWithValue("@name", name?.Length > 0 ? $"%{name}%" : "");
                       

                        MySqlDataReader reader = com.ExecuteReader();
                        if (reader.HasRows)
                        {


                            while (reader.Read())
                            {

                                Diagnose diagnose = new Diagnose();
                                diagnose.ID = Convert.ToInt64(reader["id"]);
                                diagnose.icdID = reader["icd_id"] == DBNull.Value ? "" : reader["icd_id"].ToString();
                                diagnose.name = reader["name"] == DBNull.Value ? "" : reader["name"].ToString();
                                //department.typeID = Convert.ToInt64(reader["depTypeID"]);
                                //department.type = reader["typeName"] == DBNull.Value ? "" : reader["typeName"].ToString();



                                diagnoseList.Add(diagnose);


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


            return diagnoseList;
        }
    }
}

