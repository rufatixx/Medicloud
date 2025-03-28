using System;
using MySql.Data.MySqlClient;
using System.Configuration;
using FlexitHisCore.Models;
using System.Collections.Generic;

namespace Medicloud.Models.Repository
{
    public class PaymentTypeRepo
    {
        private readonly string ConnectionString;

        public PaymentTypeRepo(string conString)
        {
            ConnectionString = conString;
        }
        public List<PaymentTypeDTO> GetPaymentTypes()

        {

            List<PaymentTypeDTO> paymentTypeDTOs = new List<PaymentTypeDTO>();
            try
            {


                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {


                    connection.Open();

                    using (MySqlCommand com = new MySqlCommand($@"SELECT * FROM payment_type;", connection))
                    {

                        MySqlDataReader reader = com.ExecuteReader();
                        if (reader.HasRows)
                        {


                            while (reader.Read())
                            {

                                PaymentTypeDTO pType = new PaymentTypeDTO();
                                pType.id = Convert.ToInt64(reader["id"]);
                                pType.name = reader["name"].ToString();

                                paymentTypeDTOs.Add(pType);


                            }


                           
                        }
                        
                    }


                    connection.Close();

                }



            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);

            }


            return paymentTypeDTOs;
        }

    }
}

