using System;
using crypto;
using System.Configuration;
using Medicloud.Models;

using MySql.Data.MySqlClient;
using System.Collections.Generic;
using Medicloud.Models.Domain;
using Medicloud.Models.DTO;
using System.Text;

namespace Medicloud.Models.Repository
{
    public class PatientCardRepo
    {
        private readonly string ConnectionString;

        public PatientCardRepo(string conString)
        {
            ConnectionString = conString;
        }
        public long InsertPatientCardEnterprise(PatientDTO newPatient, int userID, long organizationID, long lastID)
        {
            long cardID = 0;
            try
            {

                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {

                    connection.Open();





                    using (MySqlCommand com = new MySqlCommand(@"INSERT INTO patient_card (requestTypeID,userID,patientID,organizationID,serviceID,docID,priceGroupID,note,referDocID)
                      Values (@requestTypeID, @userID,@patientID,@organizationID,@serviceID,@docID,@priceGroupID,@note,@referDocID)"
                    , connection))

                    {
                        com.Parameters.AddWithValue("@requestTypeID", newPatient.requestTypeID);
                        com.Parameters.AddWithValue("@userID", userID);
                        com.Parameters.AddWithValue("@patientID", lastID);
                        com.Parameters.AddWithValue("@organizationID", organizationID);
                        //com.Parameters.AddWithValue("@depID", newPatient.depID);
                        com.Parameters.AddWithValue("@serviceID", newPatient.serviceID);
                        com.Parameters.AddWithValue("@docID", newPatient.docID);
                        com.Parameters.AddWithValue("@priceGroupID", newPatient.priceGroupID);
                        com.Parameters.AddWithValue("@note", newPatient.note);
                        com.Parameters.AddWithValue("@referDocID", newPatient.referDocID);


                        com.ExecuteNonQuery();
                        cardID = com.LastInsertedId;
                    }


                    connection.Close();
                }
                return cardID;
            }
            catch (Exception ex)
            {
                Medicloud.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);
                return cardID;
            }
        }


        public long CreatePatientCard(int requestTypeID, int userID, long patientID, long organizationID, int? serviceID = 0, int? docID = 0, int? priceGroupID = 0, string note = "", int? referDocID = 0)
        {
            long cardID = 0;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();

                    using (MySqlCommand com = new MySqlCommand(@"INSERT INTO patient_card (requestTypeID, userID, patientID, organizationID, serviceID, docID, priceGroupID, note, referDocID)
                VALUES (@requestTypeID, @userID, @patientID, @organizationID, @serviceID, @docID, @priceGroupID, @note, @referDocID)", connection))
                    {
                        // Mandatory parameters
                        com.Parameters.AddWithValue("@requestTypeID", requestTypeID);
                        com.Parameters.AddWithValue("@userID", userID);
                        com.Parameters.AddWithValue("@patientID", patientID);
                        com.Parameters.AddWithValue("@organizationID", organizationID);

                        // Optional parameters
                        if (serviceID.HasValue)
                            com.Parameters.AddWithValue("@serviceID", serviceID);
                        else
                            com.Parameters.AddWithValue("@serviceID", DBNull.Value);

                        if (docID.HasValue)
                            com.Parameters.AddWithValue("@docID", docID);
                        else
                            com.Parameters.AddWithValue("@docID", DBNull.Value);

                        if (priceGroupID.HasValue)
                            com.Parameters.AddWithValue("@priceGroupID", priceGroupID);
                        else
                            com.Parameters.AddWithValue("@priceGroupID", DBNull.Value);

                        com.Parameters.AddWithValue("@note", note ?? (object)DBNull.Value);

                        if (referDocID.HasValue)
                            com.Parameters.AddWithValue("@referDocID", referDocID);
                        else
                            com.Parameters.AddWithValue("@referDocID", DBNull.Value);

                        com.ExecuteNonQuery();
                        cardID = com.LastInsertedId;
                    }

                    connection.Close();
                }
                return cardID;
            }
            catch (Exception ex)
            {
                Medicloud.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);
                return cardID;
            }
        }


        //        public List<PatientKassaDTO> GetDebtorPatients(long organizationID)

        //        {

        //            List<PatientKassaDTO> patientList = new List<PatientKassaDTO>();
        //            try
        //            {

        //                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        //                {


        //                    connection.Open();

        //                    using (MySqlCommand com = new MySqlCommand($@"SELECT 
        //    a.id, 
        //    a.patientID, 
        //    a.serviceID, 
        //    p.name, 
        //    p.surname, 
        //    p.father, 
        //    s.price AS servicePrice, 
        //    s.name AS serviceName
        //FROM 
        //    patient_card a
        //JOIN 
        //    patients p ON a.patientID = p.id
        //JOIN 
        //    services s ON a.serviceID = s.id
        //WHERE 
        //    a.organizationID = @organizationID AND a.finished = 0

        //UNION

        //SELECT 
        //    a.id, 
        //    a.patientID, 
        //    psr.serviceID AS serviceID, 
        //    p.name, 
        //    p.surname, 
        //    p.father, 
        //    asr.price AS servicePrice, 
        //    asr.name AS serviceName
        //FROM 
        //    patient_card a
        //JOIN 
        //    patients p ON a.patientID = p.id
        //LEFT JOIN 
        //    patient_card_service_rel psr ON a.id = psr.patientCardID
        //LEFT JOIN 
        //    services asr ON psr.serviceID = asr.id
        //WHERE 
        //    a.organizationID = @organizationID AND a.finished = 0 order by id desc;


        //", connection))
        //                    {
        //                        com.Parameters.AddWithValue("@organizationID", organizationID);
        //                        MySqlDataReader reader = com.ExecuteReader();
        //                        if (reader.HasRows)
        //                        {


        //                            while (reader.Read())
        //                            {

        //                                PatientKassaDTO dSumStruct = new PatientKassaDTO();
        //                                dSumStruct.ID = Convert.ToInt64(reader["patientID"]);
        //                                dSumStruct.name = reader["name"].ToString();
        //                                dSumStruct.surname = reader["surname"].ToString();
        //                                dSumStruct.father = reader["father"].ToString();
        //                                dSumStruct.serviceName = reader["serviceName"].ToString();
        //                                dSumStruct.servicePrice = Convert.ToDouble(reader["servicePrice"]);


        //                                patientList.Add(dSumStruct);


        //                            }
        //                            patientList.Reverse();


        //                        }

        //                    }


        //                    connection.Close();


        //                }

        //            }
        //            catch (Exception ex)
        //            {
        //                StandardMessages.CallSerilog(ex);
        //                Console.WriteLine(ex.Message);

        //            }


        //            return patientList;
        //        }


        // Assuming PatientKassaDTO and PatientServiceDTO are defined as before

        public List<PatientCardDTO> GetDebtorPatientCards(long organizationID)
        {
            var list = new List<PatientCardDTO>();

            try
            {
                using (var connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();

                    // Fetch Patient Cards
                    var patientCardQuery = @"
                SELECT 
                    pc.id AS cardID,
                    pc.cDate AS cDate,
                    p.id AS patientID,
p.id as patientID,
                    p.name, 
                    p.surname, 
                    p.father
                FROM 
                    patient_card pc
                JOIN 
                    patients p ON pc.patientID = p.id
                WHERE 
                    pc.organizationID = @organizationID AND pc.finished = 0
                ORDER BY 
                    pc.id DESC;";

                    using (var command = new MySqlCommand(patientCardQuery, connection))
                    {
                        command.Parameters.AddWithValue("@organizationID", organizationID);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DateTime cDateValue;
                                string formattedDate = "";
                                if (DateTime.TryParse(reader["cDate"].ToString(), out cDateValue))
                                {
                                    // Successfully parsed as DateTime, now format it
                                    formattedDate = cDateValue.ToString("MM.dd.yyyy HH:mm");
                                    // Use formattedDate as needed
                                }

                                var patientCard = new PatientCardDTO
                                {
                                    CardID = Convert.ToInt64(reader["cardID"]), // Use cardID for unique identification of each card
                                    ID = Convert.ToInt64(reader["patientID"]), // Use cardID for unique identification of each card
                                    cDate = formattedDate, // Use cardID for unique identification of each card
                                    name = reader["name"].ToString(),
                                    surname = reader["surname"].ToString(),
                                    father = reader["father"].ToString(),
                                    // Initialize Services here or leave it as initialized in the constructor
                                };
                                list.Add(patientCard);
                            }
                        }
                    }

                    connection.Close();
                }

                // Fetch Services for Each Patient Card
                foreach (var card in list)
                {
                    card.Services = new List<PatientServiceDTO>();
                    using (var connection = new MySqlConnection(ConnectionString))
                    {
                        connection.Open();

                        var serviceQuery = @"
                   SELECT 
    s.id AS serviceID, 
  pcsr.is_paid as isPaid,
  pcsr.id as cardServiceRelId,
    s.name AS serviceName, 
    s.price AS servicePrice, 
    COALESCE(SUM(sp.amount), 0) AS totalPaid, 
    (s.price - COALESCE(SUM(sp.amount), 0)) AS debt,
    CASE 
        WHEN s.price > COALESCE(SUM(sp.amount), 0) THEN 0
        ELSE 1
    END AS isFullyPaid
FROM 
    services s
JOIN 
    patient_card_service_rel pcsr ON s.id = pcsr.serviceID
LEFT JOIN 
    service_payments sp ON pcsr.id = sp.patient_card_service_rel_id
WHERE 
    pcsr.patientCardID = @cardID
GROUP BY 
    s.id, s.name, s.price
";

                        using (var serviceCommand = new MySqlCommand(serviceQuery, connection))
                        {
                            serviceCommand.Parameters.AddWithValue("@cardID", card.CardID);
                            using (var serviceReader = serviceCommand.ExecuteReader())
                            {
                                while (serviceReader.Read())
                                {
                                    var service = new PatientServiceDTO
                                    {
                                        ID = Convert.ToInt64(serviceReader["serviceID"]),
                                        isPaid = Convert.ToBoolean(serviceReader["isFullyPaid"]),
                                        cardServiceRelId = Convert.ToInt64(serviceReader["cardServiceRelId"]),
                                        ServiceName = serviceReader["serviceName"].ToString(),
                                        ServicePrice = Convert.ToString(serviceReader["servicePrice"]),
                                        debt = Convert.ToString(serviceReader["debt"]),
                                        totalPaid = Convert.ToString(serviceReader["totalPaid"]),
                                    };
                                    card.Services.Add(service);
                                }
                            }
                        }

                        connection.Close();
                    }


                }



            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine(ex.Message);
            }

            return list;
        }
       


        public List<PatientCardDTO> GetUnpaidPatientCards(long userID, long patientID)

        {

            List<PatientCardDTO> objList = new List<PatientCardDTO>();
            try
            {

                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {


                    connection.Open();

                    using (MySqlCommand com = new MySqlCommand($@"SELECT pc.id,pc.cdate
FROM patient_card pc
INNER JOIN user_organization_rel uhr ON pc.organizationID = uhr.organizationID
WHERE uhr.userID = @userID and pc.patientID = @patientID and finished= 0", connection))
                    {
                        com.Parameters.AddWithValue("@userID", userID);
                        com.Parameters.AddWithValue("@patientID", patientID);
                        MySqlDataReader reader = com.ExecuteReader();
                        if (reader.HasRows)
                        {


                            while (reader.Read())
                            {

                                PatientCardDTO obj = new PatientCardDTO();
                                obj.CardID = Convert.ToInt64(reader["id"]);
                                obj.cDate = reader["cDate"].ToString();



                                objList.Add(obj);


                            }
                            objList.Reverse();


                        }

                    }


                    connection.Close();


                }

            }
            catch (Exception ex)
            {
                StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);

            }


            return objList;
        }

        public List<RecipeDTO> GetUnpaidRecipe(long patientCardID)

        {

            List<RecipeDTO> patientList = new List<RecipeDTO>();
            try
            {

                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {


                    connection.Open();

                    using (MySqlCommand com = new MySqlCommand($@"
SELECT 
    a.serviceID,
    COUNT(*) AS quantity,
    MAX(a.patientCardID) AS patientCardID,
    MAX(pk.finished) AS finished,
    (SELECT name FROM patients WHERE id = pk.patientID) AS name,
    (SELECT surname FROM patients WHERE id = pk.patientID) AS surname,
    (SELECT father FROM patients WHERE id = pk.patientID) AS father,
    (SELECT clientPhone FROM patients WHERE id = pk.patientID) AS phone_number,
    (SELECT price FROM services WHERE id = a.serviceID) AS price,
    (SELECT name FROM services WHERE id = a.serviceID) AS service_name,
    (SELECT name FROM organizations WHERE id = hp.id) AS organization_name
FROM 
    patient_card_service_rel a 
JOIN 
    patient_card pk ON a.patientCardID = pk.id
JOIN 
    organizations hp ON pk.organizationID = hp.id
WHERE 
    patientCardID = @patientCardID AND pk.finished = 0 
GROUP BY 
    a.serviceID;


", connection))
                    {
                        com.Parameters.AddWithValue("@patientCardID", patientCardID);
                        MySqlDataReader reader = com.ExecuteReader();
                        if (reader.HasRows)
                        {


                            while (reader.Read())
                            {

                                RecipeDTO recipe = new RecipeDTO();
                                recipe.serviceID = Convert.ToInt64(reader["serviceID"]);
                                recipe.patientCardID = Convert.ToInt64(reader["patientCardID"]);
                                recipe.finished = Convert.ToInt32(reader["finished"]);
                                recipe.name = reader["name"].ToString();
                                recipe.surname = reader["surname"].ToString();
                                recipe.father = reader["father"].ToString();
                                recipe.phone = reader["phone_number"].ToString();
                                recipe.price = Convert.ToDouble(reader["price"]);
                                recipe.serviceName = reader["service_name"].ToString();
                                recipe.organizationName = reader["organization_name"].ToString();
                                recipe.quantity = Convert.ToInt32(reader["quantity"]);
                                recipe.isPaid = Convert.ToInt32(reader["is_paid"]);


                                patientList.Add(recipe);


                            }
                            patientList.Reverse();


                        }

                    }


                    connection.Close();


                }

            }
            catch (Exception ex)
            {
                StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);

            }


            return patientList;
        }

        public List<PatientDocDTO> GetPatientsByDr(int docID)

        {

            List<PatientDocDTO> patientList = new List<PatientDocDTO>();
            try
            {

                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {


                    connection.Open();

                    using (MySqlCommand com = new MySqlCommand($@"SELECT a.id, a.patientID, a.serviceID,a.note, p.name, p.surname, p.father,p.clientPhone,p.bDate,p.genderID,p.fin
FROM patient_card a
INNER JOIN patients p ON a.patientID = p.id
WHERE a.docID = @docID
GROUP BY a.patientID
 ;", connection))
                    {
                        com.Parameters.AddWithValue("@docID", docID);
                        MySqlDataReader reader = com.ExecuteReader();
                        if (reader.HasRows)
                        {


                            while (reader.Read())
                            {

                                PatientDocDTO patient = new PatientDocDTO();
                                patient.ID = Convert.ToInt64(reader["patientID"]);
                                patient.name = reader["name"].ToString();
                                patient.surname = reader["surname"].ToString();
                                patient.father = reader["father"].ToString();
                                patient.phone = Convert.ToInt64(reader["clientPhone"]);
                                patient.bDate = Convert.ToDateTime(reader["bDate"]);
                                patient.genderID = Convert.ToInt32(reader["genderID"]);
                                patient.fin = reader["fin"].ToString();
                                patient.note = reader["note"].ToString();


                                patientList.Add(patient);


                            }
                            patientList.Reverse();


                        }

                    }


                    connection.Close();


                }

            }
            catch (Exception ex)
            {
                StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);

            }


            return patientList;
        }

        public List<PatientDocDTO> GetPatientsByOrganization(int organizationID)

        {

            List<PatientDocDTO> patientList = new List<PatientDocDTO>();
            try
            {

                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {


                    connection.Open();

                    using (MySqlCommand com = new MySqlCommand($@"SELECT 
    p.*,
    COUNT(pc.id) AS totalCardNumbers,
    MAX(pc.cDate) AS LatestCardDate -- Assuming cDate is the date of the card
FROM 
    patients p
LEFT JOIN 
    patient_card pc ON p.id = pc.patientID AND pc.finished = 0
WHERE 
    p.organizationID = @organizationID
GROUP BY 
    p.id
ORDER BY 
    LatestCardDate DESC
", connection))
                    {
                        com.Parameters.AddWithValue("@organizationID", organizationID);
                        MySqlDataReader reader = com.ExecuteReader();
                        if (reader.HasRows)
                        {


                            while (reader.Read())
                            {

                                PatientDocDTO patient = new PatientDocDTO();
                                patient.ID = Convert.ToInt64(reader["id"]);
                                patient.name = reader["name"].ToString();
                                patient.surname = reader["surname"].ToString();
                                patient.father = reader["father"].ToString();
                                patient.phone = Convert.ToInt64(reader["clientPhone"]);
                                patient.bDate = Convert.ToDateTime(reader["bDate"]);
                                patient.genderID = Convert.ToInt32(reader["genderID"]);
                                patient.totalCardNumbers = Convert.ToInt32(reader["totalCardNumbers"]);
                                patient.fin = reader["fin"].ToString();
                              


                                patientList.Add(patient);


                            }
                            patientList.Reverse();


                        }

                    }


                    connection.Close();


                }

            }
            catch (Exception ex)
            {
                StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);

            }


            return patientList;
        }
        public List<PatientDocDTO> GetAllPatientsCards(long organizationID, long patientID)
        {
            List<PatientDocDTO> patientList = new List<PatientDocDTO>();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();

                    // Start building the base query
                    var queryBuilder = new StringBuilder($@"
SELECT a.id,a.cDate, a.patientID, a.serviceID, a.note, p.name, p.surname, p.father, p.clientPhone, p.bDate, p.genderID, p.fin
FROM patient_card a
INNER JOIN patients p ON a.patientID = p.id
WHERE a.organizationID = @organizationID");

                    // Dynamically add patient condition if patientID is greater than 0
                    if (patientID > 0)
                    {
                        queryBuilder.Append(" AND a.patientID = @patientID");
                    }

                    using (MySqlCommand com = new MySqlCommand(queryBuilder.ToString(), connection))
                    {
                        com.Parameters.AddWithValue("@organizationID", organizationID);
                        // Add the patientID parameter only if it's greater than 0
                        if (patientID > 0)
                        {
                            com.Parameters.AddWithValue("@patientID", patientID);
                        }

                        using (MySqlDataReader reader = com.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    PatientDocDTO patient = new PatientDocDTO
                                    {
                                        ID = Convert.ToInt64(reader["patientID"]),
                                        patientCardID = Convert.ToInt64(reader["id"]),
                                        name = reader["name"].ToString(),
                                        serviceID = Convert.ToInt64(reader["serviceID"]),
                                        cDate = Convert.ToDateTime(reader["cDate"]),
                                        surname = reader["surname"].ToString(),
                                        father = reader["father"].ToString(),
                                        phone = Convert.ToInt64(reader["clientPhone"]),
                                        bDate = Convert.ToDateTime(reader["bDate"]),
                                        genderID = Convert.ToInt32(reader["genderID"]),
                                        fin = reader["fin"].ToString(),
                                        note = reader["note"].ToString()
                                    };

                                    patientList.Add(patient);
                                }

                                // It seems you're reversing the list at the end. If this is intended, keep it; otherwise, you may remove this line.
                                patientList.Reverse();
                            }
                        }
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);
            }

            return patientList;
        }


        public bool ClosePatientCard(long id)
        {
            bool response = new bool();
            try
            {


                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {

                    connection.Open();









                    using (MySqlCommand com = new MySqlCommand(@"UPDATE patient_card SET finished=1 WHERE id=@id", connection))

                    {

                        com.Parameters.AddWithValue("@id", id);

                        com.ExecuteNonQuery();

                    }


                    response = true; //inserted


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



        public PatientCardStatisticsDTO GetPatientCardStatistics(long organizationID)

        {
            PatientCardStatisticsDTO statisticsDTO = new PatientCardStatisticsDTO();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {


                    connection.Open();

                    using (MySqlCommand com = new MySqlCommand($@"
SELECT
  -- Calculate the number of visits this month
  SUM(CASE 
        WHEN YEAR(cdate) = YEAR(CURRENT_DATE) AND MONTH(cdate) = MONTH(CURRENT_DATE) THEN 1 
        ELSE 0 
      END) AS visits_this_month,
  
  -- Calculate the number of visits last month
  SUM(CASE 
        WHEN YEAR(cdate) = YEAR(CURRENT_DATE - INTERVAL 1 MONTH) AND MONTH(cdate) = MONTH(CURRENT_DATE - INTERVAL 1 MONTH) THEN 1 
        ELSE 0 
      END) AS visits_last_month,
  
  -- Calculate growth as a boolean (1 if growth, 0 if not)
  CASE 
    WHEN 
      SUM(CASE 
            WHEN YEAR(cdate) = YEAR(CURRENT_DATE) AND MONTH(cdate) = MONTH(CURRENT_DATE) THEN 1 
            ELSE 0 
          END) > 
      SUM(CASE 
            WHEN YEAR(cdate) = YEAR(CURRENT_DATE - INTERVAL 1 MONTH) AND MONTH(cdate) = MONTH(CURRENT_DATE - INTERVAL 1 MONTH) THEN 1 
            ELSE 0 
          END) 
    THEN 1 
    ELSE 0 
  END AS is_grow,
  
  -- Calculate the percentage change from last month to this month and round to two decimal places
  ROUND(
    (SUM(CASE 
          WHEN YEAR(cdate) = YEAR(CURRENT_DATE) AND MONTH(cdate) = MONTH(CURRENT_DATE) THEN 1 
          ELSE 0 
        END) - 
     SUM(CASE 
          WHEN YEAR(cdate) = YEAR(CURRENT_DATE - INTERVAL 1 MONTH) AND MONTH(cdate) = MONTH(CURRENT_DATE - INTERVAL 1 MONTH) THEN 1 
          ELSE 0 
        END)) 
    / NULLIF(SUM(CASE 
                  WHEN YEAR(cdate) = YEAR(CURRENT_DATE - INTERVAL 1 MONTH) AND MONTH(cdate) = MONTH(CURRENT_DATE - INTERVAL 1 MONTH) THEN 1 
                  ELSE 0 
                END), 0) * 100, 2) AS percentage_change

FROM medicloud.patient_card
WHERE organizationID = @organizationID;

", connection))
                    {

                        com.Parameters.AddWithValue("@organizationID", organizationID);

                        MySqlDataReader reader = com.ExecuteReader();
                        if (reader.HasRows)
                        {


                            while (reader.Read())
                            {


                                statisticsDTO.visitsThisMonth = reader["visits_this_month"] == DBNull.Value ? "" : reader["visits_this_month"].ToString();
                                statisticsDTO.visitsLastMonth = reader["visits_last_month"] == DBNull.Value ? "" : reader["visits_last_month"].ToString();
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
    }
}

