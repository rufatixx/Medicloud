using System;
using System.Net.NetworkInformation;
using FlexitHisMVC.Models;
using MySql.Data.MySqlClient;

namespace FlexitHisMVC.Data
{
    public class KassaRepo
    {
        private readonly string ConnectionString;

        public KassaRepo(string conString)
        {
            ConnectionString = conString;
        }
        public List<Kassa> GetUserAllowedKassaList(int userID)
        {
            List<Kassa> kassaList = new List<Kassa>();

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {

                connection.Open();

                using (MySqlCommand kassaCom = new MySqlCommand("SELECT *,(select name from kassa where id = a.kassaID)as kassaName FROM kassa_user_rel a where userID =@userID;", connection))
                {

                    kassaCom.Parameters.AddWithValue("@userID", userID);

                    using (MySqlDataReader kassaReader = kassaCom.ExecuteReader())
                    {
                        if (kassaReader.HasRows)
                        {
                            while (kassaReader.Read())
                            {
                                Kassa kassa = new Kassa();
                                kassa.id = Convert.ToInt32(kassaReader["id"]);

                                kassa.kassaID = Convert.ToInt32(kassaReader["kassaID"]);
                                kassa.name = kassaReader["kassaName"].ToString();
                                kassaList.Add(kassa);
                            }


                        }
                        
                    }

                }
                connection.Close();
            }
            return kassaList;
        }
    }
}

