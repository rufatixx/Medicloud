using System;
using System.Configuration;
using System.Globalization;

using Medicloud.Models.Domain;
using Medicloud.Models.DTO;
using MySql.Data.MySqlClient;

namespace Medicloud.Models.Repository
{
    public class PaymentOperationsRepo
    {
        private readonly string ConnectionString;

        public PaymentOperationsRepo(string conString)
        {
            ConnectionString = conString;
        }
        //        public bool InsertPaymentOperation(int userID, long kassaID, long payment_typeID, long patientID)
        //        {
        //            bool response = new bool();
        //            try
        //            {

        //                DateTime now = DateTime.Now;

        //                long lastID = 0;
        //                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        //                {

        //                    connection.Open();





        //                    using (MySqlCommand com = new MySqlCommand(@"
        //Insert INTO payment_operations (kassaID,paymentOperationType,userID,payment_typeID,patientID,price) values (@kassaID,@payOperType,@userID,@payTypeID,@patientID,
        //(SELECT ifnull(SUM(s.price),0) AS totalServicePrice
        //FROM patient_card a
        //        Join patient_card_service_rel spcr on a.id = spcr.patientCardID
        //        JOIN services s ON spcr.serviceID = s.id
        //        WHERE a.patientID=@patientID))", connection))

        //                    {
        //                        com.Parameters.AddWithValue("@userID", userID);
        //                        com.Parameters.AddWithValue("@payOperType", 1);
        //                        com.Parameters.AddWithValue("@kassaID", kassaID);
        //                        com.Parameters.AddWithValue("@payTypeID", payment_typeID);
        //                        com.Parameters.AddWithValue("@patientID", patientID);

        //                        com.ExecuteNonQuery();
        //                        lastID = com.LastInsertedId;
        //                    }



        //                    if (lastID > 0)
        //                    {
        //                        response = true; //inserted
        //                    }
        //                    else
        //                    {
        //                        response = false; //inserted
        //                    }

        //                    connection.Close();
        //                }





        //            }
        //            catch (Exception ex)
        //            {
        //                //FlexitHis_API.StandardMessages.CallSerilog(ex);
        //                Console.WriteLine(ex.Message);
        //                response = false;

        //            }


        //            return response;
        //        }

        public bool InsertPaymentOperation(int userID, long kassaID, long payment_typeID, long patientID, List<PatientServiceDTO> services, string paymentAmount,long patientCardID)
        {
            bool response = false;
            try
            {
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();
                    var transaction = connection.BeginTransaction();
                    try
                    {
                        long paymentOperationID = 0;
                        // Вставляем запись об общей операции оплаты
                        var cmdInsertPaymentOperation = new MySqlCommand("INSERT INTO payment_operations (kassaID, paymentOperationType, userID, payment_typeID, patientID, price) VALUES (@kassaID, @payOperType, @userID, @payTypeID, @patientID, @price)", connection, transaction);
                        cmdInsertPaymentOperation.Parameters.AddWithValue("@kassaID", kassaID);
                        cmdInsertPaymentOperation.Parameters.AddWithValue("@payOperType", 1); // Предполагаемый тип операции
                        cmdInsertPaymentOperation.Parameters.AddWithValue("@userID", userID);
                        cmdInsertPaymentOperation.Parameters.AddWithValue("@payTypeID", payment_typeID);
                        cmdInsertPaymentOperation.Parameters.AddWithValue("@patientID", patientID);
                        cmdInsertPaymentOperation.Parameters.AddWithValue("@price", paymentAmount);
                        cmdInsertPaymentOperation.ExecuteNonQuery();
                        paymentOperationID = cmdInsertPaymentOperation.LastInsertedId;

                        if (paymentOperationID > 0)
                        {
                            // Перебор услуг и обработка платежей
                            foreach (var service in services)
                            {
                                // Оплата услуги
                                var cmdInsertServicePayment = new MySqlCommand("INSERT INTO service_payments (payment_id, patient_card_service_rel_id, amount,patient_card_id) VALUES (@paymentID, @serviceCardRelID, @amount,@patientCardID)", connection, transaction);
                                cmdInsertServicePayment.Parameters.AddWithValue("@paymentID", paymentOperationID);
                                cmdInsertServicePayment.Parameters.AddWithValue("@serviceCardRelID", service.cardServiceRelId);
                                cmdInsertServicePayment.Parameters.AddWithValue("@amount", service.CurrentPaymentAmount);

                                cmdInsertServicePayment.Parameters.AddWithValue("@patientCardID", patientCardID);
                                cmdInsertServicePayment.ExecuteNonQuery();

                                // Получение общей суммы платежей по услуге
                                var cmdGetTotalPayments = new MySqlCommand("SELECT SUM(amount) FROM service_payments WHERE patient_card_service_rel_id = @serviceCardRelID", connection, transaction);
                                cmdGetTotalPayments.Parameters.AddWithValue("@serviceCardRelID", service.cardServiceRelId);
                                var totalPaymentsResult = cmdGetTotalPayments.ExecuteScalar();
                                var totalPayments = totalPaymentsResult != DBNull.Value ? (double)totalPaymentsResult : 0;

                                // Получение стоимости услуги
                                var cmdGetServicePrice = new MySqlCommand("SELECT price FROM services WHERE id = @serviceID", connection, transaction);
                                cmdGetServicePrice.Parameters.AddWithValue("@serviceID", service.ID);
                                var servicePriceResult = cmdGetServicePrice.ExecuteScalar();
                                var servicePrice = servicePriceResult != DBNull.Value ? (double)servicePriceResult : 0;
                                //double servicePriceValue;
                                //bool parseResult = double.TryParse(service.CurrentPaymentAmount, NumberStyles.Any, CultureInfo.InvariantCulture, out servicePriceValue);

                                // Проверка, достаточно ли оплачено
                                //bool isFullyPaid = totalPayments + servicePriceValue >= servicePrice;
                                bool isFullyPaid = totalPayments  >= servicePrice;

                               

                          

                                // Если услуга полностью оплачена, обновляем is_paid
                                if (isFullyPaid)
                                {
                                    var cmdUpdateServicePayment = new MySqlCommand("UPDATE patient_card_service_rel SET is_paid = 1 WHERE id = @serviceCardRelID", connection, transaction);
                                    cmdUpdateServicePayment.Parameters.AddWithValue("@serviceCardRelID", service.cardServiceRelId);
                                    cmdUpdateServicePayment.ExecuteNonQuery();
                                }

                            }


                            // Проверяем, все ли услуги оплачены
                            var cmdCheckAllPaid = new MySqlCommand("SELECT COUNT(*) FROM patient_card_service_rel WHERE patientCardId = @patientCardID and is_paid = 0", connection, transaction);
                            cmdCheckAllPaid.Parameters.AddWithValue("@patientCardID", patientCardID);
                            var allServicesPaid = (long)cmdCheckAllPaid.ExecuteScalar() == 0;

                            if (allServicesPaid)
                            {
                                // Обновляем статус карточки пациента
                                var cmdUpdatePatientCard = new MySqlCommand("UPDATE patient_card SET finished = 1 WHERE id = @patientCardID", connection, transaction);
                                cmdUpdatePatientCard.Parameters.AddWithValue("@patientCardID", patientCardID);
                                cmdUpdatePatientCard.ExecuteNonQuery();
                            }

                            transaction.Commit();
                          
                            response = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
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
(SELECT CONCAT(name, ' ', surname) FROM patients WHERE id = a.patientID) AS patientFullName,
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
                                recipe.patientFullName = reader["patientFullName"] == DBNull.Value ? "" : reader["patientFullName"].ToString();
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

        public PaymentOperationStatisticsDTO GetPaymentOperationsStatistics(long organizationID)

        {
            PaymentOperationStatisticsDTO statisticsDTO = new PaymentOperationStatisticsDTO();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {


                    connection.Open();

                    using (MySqlCommand com = new MySqlCommand($@"
SELECT
  kt.organizationID,
  CURRENT_DATE AS report_month,
  SUM(
    CASE 
      WHEN DATE_FORMAT(po.cdate, '%Y-%m') = DATE_FORMAT(CURRENT_DATE, '%Y-%m') AND po.paymentOperationType = 1 THEN po.price 
      ELSE 0 
    END
  ) AS income_this_month,
  SUM(
    CASE 
      WHEN DATE_FORMAT(po.cdate, '%Y-%m') = DATE_FORMAT(CURRENT_DATE - INTERVAL 1 MONTH, '%Y-%m') AND po.paymentOperationType = 1 THEN po.price 
      ELSE 0 
    END
  ) AS income_last_month,
  IF(
    SUM(CASE WHEN DATE_FORMAT(po.cdate, '%Y-%m') = DATE_FORMAT(CURRENT_DATE - INTERVAL 1 MONTH, '%Y-%m') AND po.paymentOperationType = 1 THEN po.price ELSE 0 END) = 0 
    AND SUM(CASE WHEN DATE_FORMAT(po.cdate, '%Y-%m') = DATE_FORMAT(CURRENT_DATE, '%Y-%m') AND po.paymentOperationType = 1 THEN po.price ELSE 0 END) > 0,
    100.00,
    IFNULL(
      ROUND(
        (
          COALESCE(SUM(CASE WHEN DATE_FORMAT(po.cdate, '%Y-%m') = DATE_FORMAT(CURRENT_DATE, '%Y-%m') AND po.paymentOperationType = 1 THEN po.price ELSE 0 END), 0) -
          COALESCE(SUM(CASE WHEN DATE_FORMAT(po.cdate, '%Y-%m') = DATE_FORMAT(CURRENT_DATE - INTERVAL 1 MONTH, '%Y-%m') AND po.paymentOperationType = 1 THEN po.price ELSE 0 END), 0)
        ) 
        / NULLIF(
          COALESCE(SUM(CASE WHEN DATE_FORMAT(po.cdate, '%Y-%m') = DATE_FORMAT(CURRENT_DATE - INTERVAL 1 MONTH, '%Y-%m') AND po.paymentOperationType = 1 THEN po.price ELSE 0 END), 0), 
          0
        ) * 100, 
        2
      ),
      0
    )
  ) AS percentage_change,
  CASE
    WHEN 
      COALESCE(SUM(CASE WHEN DATE_FORMAT(po.cdate, '%Y-%m') = DATE_FORMAT(CURRENT_DATE, '%Y-%m') AND po.paymentOperationType = 1 THEN po.price ELSE 0 END), 0) >
      COALESCE(SUM(CASE WHEN DATE_FORMAT(po.cdate, '%Y-%m') = DATE_FORMAT(CURRENT_DATE - INTERVAL 1 MONTH, '%Y-%m') AND po.paymentOperationType = 1 THEN po.price ELSE 0 END), 0)
    THEN 1
    ELSE 0
  END AS is_grow
FROM
  medicloud.payment_operations AS po
JOIN
  medicloud.kassa AS kt
ON
  po.kassaID = kt.id
WHERE
  kt.organizationID = @organizationID
GROUP BY
  kt.organizationID;

", connection))
                    {

                        com.Parameters.AddWithValue("@organizationID", organizationID);

                        MySqlDataReader reader = com.ExecuteReader();
                        if (reader.HasRows)
                        {


                            while (reader.Read())
                            {


                                statisticsDTO.incomeThisMonth = reader["income_this_month"] == DBNull.Value ? "" : reader["income_this_month"].ToString();
                                statisticsDTO.incomeLasMonth = reader["income_last_month"] == DBNull.Value ? "" : reader["income_last_month"].ToString();
                                statisticsDTO.percentage = reader["percentage_change"] == DBNull.Value ? "" : reader["percentage_change"].ToString();

                                statisticsDTO.isGrow = reader["is_grow"] == DBNull.Value ? 0 : Convert.ToInt32(reader["is_grow"]);

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
                //response.status = 4;
            }


            return statisticsDTO;
        }

        public List<DailyIncomeStatistics> GetDailyStatistics(long kassaID)

        {
            List<DailyIncomeStatistics> list = new List<DailyIncomeStatistics>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {


                    connection.Open();

                    using (MySqlCommand com = new MySqlCommand($@"
SELECT 
  DaysOfWeek.WeekDay,
  COALESCE(IncomeStats.IncomeThisWeek, 0) AS IncomeThisWeek,
  COALESCE(IncomeStats.IncomePreviousWeek, 0) AS IncomePreviousWeek
FROM (
  SELECT 'Monday' AS WeekDay UNION
  SELECT 'Tuesday' UNION
  SELECT 'Wednesday' UNION
  SELECT 'Thursday' UNION
  SELECT 'Friday' UNION
  SELECT 'Saturday' UNION
  SELECT 'Sunday'
) DaysOfWeek
LEFT JOIN (
  SELECT 
    DAYNAME(cdate) AS WeekDay,
    SUM(CASE WHEN YEARWEEK(cdate, 1) = YEARWEEK(CURDATE(), 1) THEN price ELSE 0 END) AS IncomeThisWeek,
    SUM(CASE WHEN YEARWEEK(cdate, 1) = YEARWEEK(CURDATE() - INTERVAL 1 WEEK, 1) THEN price ELSE 0 END) AS IncomePreviousWeek
  FROM 
    medicloud.payment_operations
  WHERE 
    kassaID = @kassaID
    AND cdate >= CURDATE() - INTERVAL 7 DAY
  GROUP BY DAYNAME(cdate)
) IncomeStats ON DaysOfWeek.WeekDay = IncomeStats.WeekDay
ORDER BY FIELD(DaysOfWeek.WeekDay, 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday');


", connection))
                    {

                        com.Parameters.AddWithValue("@kassaID", kassaID);

                        MySqlDataReader reader = com.ExecuteReader();
                        if (reader.HasRows)
                        {


                            while (reader.Read())
                            {
                                DailyIncomeStatistics dailyIncomeStatistics = new DailyIncomeStatistics();


                                dailyIncomeStatistics.weekDay = reader["WeekDay"] == DBNull.Value ? "" : reader["WeekDay"].ToString();
                                dailyIncomeStatistics.incomeThisWeek = reader["IncomeThisWeek"] == DBNull.Value ? "" : reader["IncomeThisWeek"].ToString();
                                dailyIncomeStatistics.incomePreviousWeek = reader["IncomePreviousWeek"] == DBNull.Value ? "" : reader["IncomePreviousWeek"].ToString();
                                list.Add(dailyIncomeStatistics);
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
                //response.status = 4;
            }


            return list;
        }

        public List<WeeklyIncomeStatisticsDTO> GetWeeklyStatistics(long kassaID)

        {
            List<WeeklyIncomeStatisticsDTO> list = new List<WeeklyIncomeStatisticsDTO>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {


                    connection.Open();

                    using (MySqlCommand com = new MySqlCommand($@"
SELECT 
  'This Week' AS Period,
  COALESCE(SUM(CASE WHEN YEARWEEK(cdate, 1) = YEARWEEK(CURDATE(), 1) THEN price ELSE 0 END), 0) AS TotalIncome
FROM 
  medicloud.payment_operations
WHERE 
   kassaID = @kassaID
UNION ALL
SELECT 
  'Previous Week',
  COALESCE(SUM(CASE WHEN YEARWEEK(cdate, 1) = YEARWEEK(CURDATE() - INTERVAL 1 WEEK, 1) THEN price ELSE 0 END), 0)
FROM 
  medicloud.payment_operations
WHERE 
  kassaID = @kassaID
UNION ALL
SELECT 
  'Today',
  COALESCE(SUM(CASE WHEN DATE(cdate) = CURDATE() THEN price ELSE 0 END), 0)
FROM 
  medicloud.payment_operations
WHERE 
  DATE(cdate) = CURDATE()
  AND kassaID = @kassaID;


", connection))
                    {

                        com.Parameters.AddWithValue("@kassaID", kassaID);

                        MySqlDataReader reader = com.ExecuteReader();
                        if (reader.HasRows)
                        {


                            while (reader.Read())
                            {
                                WeeklyIncomeStatisticsDTO weeklyIncomeStatisticsDTO = new WeeklyIncomeStatisticsDTO();


                                weeklyIncomeStatisticsDTO.period = reader["Period"] == DBNull.Value ? "" : reader["Period"].ToString();
                                weeklyIncomeStatisticsDTO.totalIncome = reader["TotalIncome"] == DBNull.Value ? "" : reader["TotalIncome"].ToString();
                               
                                list.Add(weeklyIncomeStatisticsDTO);
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
                //response.status = 4;
            }


            return list;
        }

    }
}

