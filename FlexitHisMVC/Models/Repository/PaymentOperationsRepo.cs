using System;
using System.Configuration;
using crypto;
using MySql.Data.MySqlClient;

namespace FlexitHisMVC.Models.Repository
{
    public class PaymentOperationsRepo
    {
        private readonly string ConnectionString;

        public PaymentOperationsRepo(string conString)
        {
            ConnectionString = conString;
        }
        public bool InsertPaymentOperation(int userID, long kassaID, long payment_typeID, long patientID)
        {
            bool response = new bool();
            try
            {

                DateTime now = DateTime.Now;

                long lastID = 0;
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {

                    connection.Open();





                    using (MySqlCommand com = new MySqlCommand(@"
Insert INTO payment_operations (kassaID,paymentOperationType,userID,payment_typeID,patientID,price) values (@kassaID,@payOperType,@userID,@payTypeID,@patientID,(SELECT  sum((select price from services where id = a.serviceID ))as serviceSum
FROM patient_request a where hospitalID =(select hospitalID from patients where id = @patientID)
and finished=0 and patientID = @patientID  group by patientID order by serviceSum))", connection))

                    {
                        com.Parameters.AddWithValue("@userID", userID);
                        com.Parameters.AddWithValue("@payOperType", 1);
                        com.Parameters.AddWithValue("@kassaID", kassaID);
                        com.Parameters.AddWithValue("@payTypeID", payment_typeID);
                        com.Parameters.AddWithValue("@patientID", patientID);

                        com.ExecuteNonQuery();
                        lastID = com.LastInsertedId;
                    }



                    if (lastID > 0)
                    {
                        response = true; //inserted
                    }
                    else
                    {
                        response = false; //inserted
                    }

                    connection.Close();
                }





            }
            catch (Exception ex)
            {
                //FlexitHis_API.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);
                response = false;

            }


            return response;
        }

        public KassaDTO GetPaymentOperations(int userID, long kassaID)

        {
            KassaDTO response = new KassaDTO();
            response.recipeList = new List<Recipe>();

            try
            {



                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {


                    connection.Open();


                    using (MySqlCommand com = new MySqlCommand($@"
SELECT sum(price)as sumPrice FROM payment_operations where kassaID =@kassaID and userID = @userID;", connection))
                    {
                        com.Parameters.AddWithValue("@userID", userID);
                        com.Parameters.AddWithValue("@kassaID", kassaID);
                        MySqlDataReader reader = com.ExecuteReader();
                        if (reader.HasRows)
                        {


                            while (reader.Read())
                            {


                              

                                response.id = kassaID;

                                response.kassaSum = Convert.ToDouble(reader["sumPrice"]);


                               


                            }


                           
                        }
                       
                    }
                    connection.Close();
                    connection.Open();
                    using (MySqlCommand com = new MySqlCommand($@"
SELECT *,(select name from payment_type where id = a.payment_typeID)as pTypeName,
 (select name from payment_operation_type where id=a.paymentOperationType)as operationType
FROM payment_operations a where kassaID =@kassaID and userID = @userID", connection))
                    {
                        com.Parameters.AddWithValue("@userID", userID);
                        com.Parameters.AddWithValue("@kassaID", kassaID);

                        MySqlDataReader reader = com.ExecuteReader();
                        if (reader.HasRows)
                        {


                            while (reader.Read())
                            {

                                Recipe recipe = new Recipe();
                                recipe.id = reader["id"] == DBNull.Value ? 0 : Convert.ToInt64(reader["id"]);
                                recipe.patientID = reader["patientID"] == DBNull.Value ? 0 : Convert.ToInt64(reader["patientID"]);
                                recipe.pTypeName = reader["pTypeName"] == DBNull.Value ? "" : reader["pTypeName"].ToString();
                                recipe.price = reader["price"] == DBNull.Value ? 0 : Convert.ToDouble(reader["price"]);
                                recipe.userID = reader["userID"] == DBNull.Value ? 0 : Convert.ToInt64(reader["userID"]);
                                recipe.cdate = Convert.ToDateTime(reader["cdate"]);

                                response.recipeList.Add(recipe);


                            }

                            response.recipeList.Reverse();
                           
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


            return response;
        }



    }
}

