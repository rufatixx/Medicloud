using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text.RegularExpressions;
using crypto;
using FlexitHis_API.Models.Structs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace FlexitHis_API.Models.Db
{
    public class Select
    {
        private readonly string ConnectionString;
        public IConfiguration Configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public Select(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            ConnectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
            _hostingEnvironment = hostingEnvironment;

        }
        public long regexPhone(long phone)
        {
            long formattedPhone = 0;


            if (Regex.Match(phone.ToString(), @"[0-9]{12}").Success)
            {
                formattedPhone = Convert.ToInt64(phone.ToString().Substring(3));
            }

            return formattedPhone;
        }
        public LogInStruct LogIn(string username, string pass)
        {

            //long formattedPhone = regexPhone(phone);
            LogInStruct status = new LogInStruct();
            status.hospitals = new List<Hospital>();
            status.kassaList = new List<AllowedKassa>();
            int userID = 0;
            int isActive = 0;
            int isUser = 0;
            try
            {

                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {

                    connection.Open();

                    using (MySqlCommand com = new MySqlCommand("select * from personal where pwd=SHA2(@pass,256) and username = @username and isActive=1 and isUser=1", connection))
                    {

                        com.Parameters.AddWithValue("@pass", pass);
                        com.Parameters.AddWithValue("@username", username);
                        using (MySqlDataReader reader = com.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                status.status = 1;

                                while (reader.Read())
                                {
                                    userID = Convert.ToInt32(reader["id"]);
                                    isActive = Convert.ToInt32(reader["isActive"]);
                                    isUser = Convert.ToInt32(reader["isUser"]);
                                    status.name = reader["name"].ToString();
                                    status.surname = reader["surname"].ToString();
                                }
                                
                                if (isActive == 1 && isUser == 1)
                                {
                                    connection.Close();
                                    connection.Open();


                                    using (MySqlCommand hospitalCom = new MySqlCommand("SELECT *,(select name from hospital where id = a.hospitalID) as hName FROM user_hospital_rel a where userID = @userID;", connection))
                                    {

                                        hospitalCom.Parameters.AddWithValue("@userID", userID);

                                        using (MySqlDataReader hospitalReader = hospitalCom.ExecuteReader())
                                        {
                                            if (hospitalReader.HasRows)
                                            {
                                                while (hospitalReader.Read())
                                                {
                                                    Hospital hospital = new Hospital();
                                                    hospital.id = Convert.ToInt32(hospitalReader["id"]);
                                                    hospital.userID = Convert.ToInt32(hospitalReader["userID"]);
                                                    hospital.hospitalID = Convert.ToInt32(hospitalReader["hospitalID"]);
                                                    hospital.hospitalName = hospitalReader["hName"].ToString();
                                                    status.hospitals.Add(hospital);
                                                }


                                            }
                                            else
                                            {
                                                status.status = 2;
                                                // status.requestToken = security.requestTokenGenerator(userToken, userID);

                                                //  status.responseString = "Access danied";
                                            }
                                        }

                                    }

                                    connection.Close();
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
                                                    AllowedKassa kassa = new AllowedKassa();
                                                    kassa.id = Convert.ToInt32(kassaReader["id"]);

                                                    kassa.kassaID = Convert.ToInt32(kassaReader["kassaID"]);
                                                    kassa.name = kassaReader["kassaName"].ToString();
                                                    status.kassaList.Add(kassa);
                                                }


                                            }
                                            else
                                            {
                                                status.status = 2;
                                                // status.requestToken = security.requestTokenGenerator(userToken, userID);

                                                //  status.responseString = "Access danied";
                                            }
                                        }

                                    }
                                }

                                else {
                                    status.status = 2;
                                }

                            }
                            else
                            {
                                status.status = 2;
                                // status.requestToken = security.requestTokenGenerator(userToken, userID);

                                //  status.responseString = "Access danied";
                            }
                        }

                    }
                   


                    connection.Close();

                }


            }
            catch (Exception ex)
            {
                status.status = 4;
                FlexitHisMVC.StandardMessages.CallSerilog(ex);
                Console.WriteLine($"Exception: {ex.Message}");
                // status.responseString = $"Exception: {ex.Message}";
            }
            return status;




        }

        //public DebtorPatientStruct GetYesterdaySum()
        //{
        //    DebtorPatientStruct sum = new DebtorPatientStruct();
        //    DateTime yesterday = DateTime.Today;
        //    using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        //    {

        //        connection.Open();
        //        using (MySqlCommand com = new MySqlCommand(@"select * from daily_sum WHERE DATE(`cdate`) = @yesterday", connection))

        //        {
        //            com.Parameters.AddWithValue("@yesterday", yesterday.AddDays(-1));
        //            using (MySqlDataReader reader = com.ExecuteReader())
        //            {

        //                if (reader.HasRows)
        //                {
        //                    while (reader.Read())
        //                    {
        //                        sum.dayEndMoney = Convert.ToDouble(reader["dayEndMoney"]);
        //                        sum.dayStartMoney = Convert.ToDouble(reader["dayStartMoney"]);
        //                        sum.income = Convert.ToDouble(reader["income"]);
        //                        sum.outcome = Convert.ToDouble(reader["outcome"]);
        //                    }

        //                }

        //            }

        //        }
        //        connection.Close();
        //    }
        //    return sum;

        //}
        //public long GetTodaySumID() {
        //    long todaySumID = 0;
        //    DateTime today = DateTime.Today;
        //    using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        //    {

        //        connection.Open();
        //        using (MySqlCommand com = new MySqlCommand(@"select * from daily_sum WHERE DATE(`cdate`) = @today", connection))

        //        {
        //            com.Parameters.AddWithValue("@today",today);
        //            using (MySqlDataReader reader = com.ExecuteReader())
        //            {

        //                if (reader.HasRows)
        //                {
        //                    while (reader.Read())
        //                    {
        //                        todaySumID = Convert.ToInt64(reader["id"]);
        //                    }

        //                }

        //            }

        //        }
        //        connection.Close();
        //    }
        //    return todaySumID;

        //}

        //public ResponseStruct<ProductModel> GetModels(string userToken,string requestToken)

        //{
        //    ResponseStruct<ProductModel> response = new ResponseStruct<ProductModel>();
        //    response.data = new List<ProductModel>();
        //    try
        //    {
        //        Security security = new Security(Configuration, _hostingEnvironment);
        //        int userID1 = security.selectUserToken(userToken);
        //        int userID2 = security.selectRequestToken(requestToken);

        //        if (userID1 == userID2&&userID1>0&&userID2>0)
        //        {
        //            using (MySqlConnection connection = new MySqlConnection(ConnectionString)) {


        //            connection.Open();

        //            using (MySqlCommand com = new MySqlCommand("Select * from product_models order by id desc", connection)) {

        //                MySqlDataReader reader = com.ExecuteReader();
        //                if (reader.HasRows)
        //                {


        //                    while (reader.Read())
        //                    {

        //                        ProductModel pModel = new ProductModel();
        //                        pModel.id = Convert.ToInt64(reader["id"]);
        //                        pModel.name = reader["model_name"].ToString();
        //                        pModel.price = Convert.ToDouble(reader["price"]);



        //                        response.data.Add(pModel);


        //                    }
                            

        //                    response.status = 1;
        //                }
        //                else
        //                {
        //                    response.status = 2;

                            

        //                }
        //            }


        //            connection.Close();
        //                response.requestToken = security.requestTokenGenerator(userToken, userID1);

        //            }

        //            }
        //        else
        //        {
        //            response.status = 3;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        response.status = 4;

        //    }

           
        //    return response;
        //}
        public AddNewPatientPageStruct GetNewPatientPage ()

        {
            
       
           AddNewPatientPageStruct pageStruct = new AddNewPatientPageStruct();
            pageStruct.requestTypes = new List<RequestType>();
            pageStruct.personal = new List<Personal>();
            pageStruct.departments = new List<Department>();
            pageStruct.referers = new List<Referer>();
            pageStruct.services = new List<Service>();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {


                    connection.Open();

                    using (MySqlCommand com = new MySqlCommand("SELECT * FROM request_type;", connection))
                    {

                        MySqlDataReader reader = com.ExecuteReader();
                        if (reader.HasRows)
                        {


                            while (reader.Read())
                            {

                                RequestType requestType = new RequestType();
                                requestType.ID = Convert.ToInt32(reader["id"]);
                                requestType.name = reader["name"].ToString();




                                pageStruct.requestTypes.Add(requestType);


                            }



                        }

                    }
                    connection.Close();
                    connection.Open();
                    using (MySqlCommand com = new MySqlCommand("SELECT * FROM services;", connection))
                    {

                        MySqlDataReader reader = com.ExecuteReader();
                        if (reader.HasRows)
                        {


                            while (reader.Read())
                            {

                                Service services = new Service();
                                services.ID = Convert.ToInt32(reader["id"]);
                                services.depID = Convert.ToInt32(reader["departmentID"]);
                                services.name = reader["name"].ToString();
                                services.price = Convert.ToDouble(reader["price"]);



                                pageStruct.services.Add(services);


                            }



                        }

                    }
                    connection.Close();
                    connection.Open();
                    using (MySqlCommand com = new MySqlCommand("SELECT * FROM departments", connection))
                    {

                        MySqlDataReader reader = com.ExecuteReader();
                        if (reader.HasRows)
                        {


                            while (reader.Read())
                            {

                                Department departments = new Department();
                                departments.ID = Convert.ToInt32(reader["id"]);
                                departments.name = reader["name"].ToString();




                                pageStruct.departments.Add(departments);


                            }



                        }

                    }
                    connection.Close();
                    connection.Open();
                    using (MySqlCommand com = new MySqlCommand("SELECT * FROM personal where depTypeID = 1 and referral != 1;", connection))
                    {

                        MySqlDataReader reader = com.ExecuteReader();
                        if (reader.HasRows)
                        {


                            while (reader.Read())
                            {

                                Personal personal = new Personal();
                                personal.ID = Convert.ToInt32(reader["id"]);
                                personal.depID = Convert.ToInt32(reader["departmentID"]);
                                personal.name = reader["name"].ToString();
                                personal.surname = reader["surname"].ToString();
                                personal.father = reader["father"].ToString();
                                pageStruct.personal.Add(personal);


                            }



                        }

                    }
                    connection.Close();
                    connection.Open();
                    using (MySqlCommand com = new MySqlCommand("SELECT * FROM personal where referral = 1;", connection))
                    {

                        MySqlDataReader reader = com.ExecuteReader();
                        if (reader.HasRows)
                        {


                            while (reader.Read())
                            {

                                Referer referralPersonal = new Referer();
                                referralPersonal.ID = Convert.ToInt32(reader["id"]);
                                referralPersonal.name = reader["name"].ToString();
                                referralPersonal.surname = reader["surname"].ToString();
                                referralPersonal.father = reader["father"].ToString();
                                pageStruct.referers.Add(referralPersonal);


                            }



                        }

                    }

                    connection.Close();
                    pageStruct.status = 1;
                  

                }


            }
            catch (Exception ex)
            {
                FlexitHisMVC.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);
                pageStruct.status = 4;

            }
           
            return pageStruct;
        }
        //public ResponseStruct<DebtorPatientStruct> GetDailySum(string userToken, string requestToken)

        //{
        //    ResponseStruct<DebtorPatientStruct> response = new ResponseStruct<DebtorPatientStruct>();
        //    response.data = new List<DebtorPatientStruct>();
        //    try
        //    {
        //        Security security = new Security(Configuration, _hostingEnvironment);
        //        int userID1 = security.selectUserToken(userToken);
        //        int userID2 = security.selectRequestToken(requestToken);

        //        if (userID1 == userID2 && userID1 > 0 && userID2 > 0)
        //        {
                    
        //            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        //            {


        //                connection.Open();

        //                using (MySqlCommand com = new MySqlCommand("Select * from daily_sum order by cdate desc", connection))
        //                {

        //                    MySqlDataReader reader = com.ExecuteReader();
        //                    if (reader.HasRows)
        //                    {


        //                        while (reader.Read())
        //                        {
                                  
        //                            DebtorPatientStruct dSumStruct = new DebtorPatientStruct();
        //                            dSumStruct.ID = Convert.ToInt64(reader["id"]);
        //                            dSumStruct.cDate = Convert.ToDateTime(reader["cdate"]);
        //                            dSumStruct.dayStartMoney = Convert.ToDouble(reader["dayStartMoney"]);
        //                            dSumStruct.dayEndMoney = Convert.ToDouble(reader["dayEndMoney"]);
        //                            dSumStruct.income = Convert.ToDouble(reader["income"]);
        //                            dSumStruct.outcome = Convert.ToDouble(reader["outcome"]);



        //                            response.data.Add(dSumStruct);


        //                        }


        //                        response.status = 1;
        //                    }
        //                    else
        //                    {
        //                        response.status = 2;



        //                    }
        //                }


        //                connection.Close();
        //                response.requestToken = security.requestTokenGenerator(userToken, userID1);

        //            }

        //        }
        //        else
        //        {
        //            response.status = 3;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        response.status = 4;
        //    }


        //    return response;
        //}

        public ResponseStruct<PatientStruct> GetDebtorPatients(string userToken, string requestToken,long hospitalID)

        {
            ResponseStruct<PatientStruct> response = new ResponseStruct<PatientStruct>();
            response.data = new List<PatientStruct>();
            try
            {
                Security security = new Security(Configuration, _hostingEnvironment);
                int userID1 = security.selectUserToken(userToken);
                int userID2 = security.selectRequestToken(requestToken);

                if (userID1 == userID2 && userID1 > 0 && userID2 > 0)
                {

                    using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                    {


                        connection.Open();

                        using (MySqlCommand com = new MySqlCommand($@"SELECT id,patientID,serviceID,(select name from patients where id = a.patientID )as name,
(select surname from patients where id = a.patientID )as surname,
(select father from patients where id = a.patientID )as father,
sum((select price from services where id = a.serviceID ))as serviceSum
FROM patient_request a where hospitalID =@hospitalID and finished=0 group by patientID order by serviceSum ;", connection))
                        {
                            com.Parameters.AddWithValue("@hospitalID", hospitalID);
                            MySqlDataReader reader = com.ExecuteReader();
                            if (reader.HasRows)
                            {


                                while (reader.Read())
                                {

                                    PatientStruct dSumStruct = new PatientStruct();
                                    dSumStruct.ID = Convert.ToInt64(reader["paientID"]);
                                    dSumStruct.name =reader["name"].ToString();
                                    dSumStruct.surname = reader["surname"].ToString();
                                    dSumStruct.father = reader["father"].ToString();
                                    dSumStruct.price = Convert.ToDouble(reader["serviceSum"]);
    
                                    response.data.Add(dSumStruct);


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
                        response.requestToken = security.requestTokenGenerator(userToken, userID1);

                    }

                }
                else
                {
                    response.status = 3;
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



        public ResponseStruct<PaymentType> GetPaymentTypes(string userToken, string requestToken)

        {
            ResponseStruct<PaymentType> response = new ResponseStruct<PaymentType>();
            response.data = new List<PaymentType>();
            try
            {
                Security security = new Security(Configuration, _hostingEnvironment);
                int userID1 = security.selectUserToken(userToken);
                int userID2 = security.selectRequestToken(requestToken);

                if (userID1 == userID2 && userID1 > 0 && userID2 > 0)
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

                                    PaymentType pType = new PaymentType();
                                    pType.id = Convert.ToInt64(reader["id"]);
                                    pType.name = reader["name"].ToString();
                                    
                                    response.data.Add(pType);


                                }


                                response.status = 1;
                            }
                            else
                            {
                                response.status = 2;



                            }
                        }


                        connection.Close();
                        response.requestToken = security.requestTokenGenerator(userToken, userID1);

                    }

                }
                else
                {
                    response.status = 3;
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


        public ResponseStruct<KassaStruct> GetPaymentOperations(string userToken, string requestToken,long kassaID)

        {
            ResponseStruct<KassaStruct> response = new ResponseStruct<KassaStruct>();
            response.data = new List<KassaStruct>();
          
            try
            {
                Security security = new Security(Configuration, _hostingEnvironment);
                int userID1 = security.selectUserToken(userToken);
                int userID2 = security.selectRequestToken(requestToken);

                if (userID1 == userID2 && userID1 > 0 && userID2 > 0)
                {

                    using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                    {


                        connection.Open();


                        using (MySqlCommand com = new MySqlCommand($@"
SELECT sum(price)as sumPrice FROM payment_operations where kassaID =@kassaID and userID = @userID;", connection))
                        {
                            com.Parameters.AddWithValue("@userID", userID1);
                            com.Parameters.AddWithValue("@kassaID", kassaID);
                            MySqlDataReader reader = com.ExecuteReader();
                            if (reader.HasRows)
                            {


                                while (reader.Read())
                                {

                                    KassaStruct kassa = new KassaStruct();
                                    kassa.recipeList = new List<RecipeStruct>();

                                    kassa.id = kassaID;
                                   
                                    kassa.kassaSum = Convert.ToDouble(reader["sumPrice"]);


                                    response.data.Add(kassa);


                                }


                                response.status = 1;
                            }
                            else
                            {
                                response.status = 2;



                            }
                        }
                        connection.Close();
                        connection.Open();
                        using (MySqlCommand com = new MySqlCommand($@"
SELECT *,(select name from payment_type where id = a.payment_typeID)as pTypeName,
 (select name from payment_operation_type where id=a.paymentOperationType)as operationType
FROM payment_operations a where kassaID =@kassaID and userID = @userID", connection))
                        {
                            com.Parameters.AddWithValue("@userID", userID1);
                            com.Parameters.AddWithValue("@kassaID", kassaID);

                            MySqlDataReader reader = com.ExecuteReader();
                            if (reader.HasRows)
                            {


                                while (reader.Read())
                                {

                                    RecipeStruct recipe = new RecipeStruct();
                                    recipe.id = reader["id"]== DBNull.Value ? 0 : Convert.ToInt64(reader["id"]);
                                    recipe.patientID = reader["patientID"] == DBNull.Value ? 0 : Convert.ToInt64(reader["patientID"]);
                                    recipe.pTypeName = reader["pTypeName"] == DBNull.Value ? "" : reader["pTypeName"].ToString();
                                    recipe.price = reader["price"] == DBNull.Value ? 0 : Convert.ToDouble(reader["price"]);
                                    recipe.userID = reader["userID"] == DBNull.Value ? 0 : Convert.ToInt64(reader["userID"]);
                                    recipe.cdate = Convert.ToDateTime(reader["cdate"]);

                                    response.data[0].recipeList.Add(recipe);


                                }

                                response.data[0].recipeList.Reverse();
                                response.status = 1;
                            }
                            else
                            {
                                response.status = 2;



                            }
                        }


                        connection.Close();
                        response.requestToken = security.requestTokenGenerator(userToken, userID1);

                    }

                }
                else
                {
                    response.status = 3;
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


        public long getUserID(long phone)
        {
            long userID = 0;

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {


                try
                {




                    connection.Open();

                    using (MySqlCommand com = new MySqlCommand("select userID from user where mobile=@phone order by userID desc limit 1", connection))
                    {
                        com.Parameters.AddWithValue("@phone", phone);

                        MySqlDataReader reader = com.ExecuteReader();
                        if (reader.HasRows)
                        {


                            while (reader.Read())
                            {
                                userID = Convert.ToInt32(reader[0]);
                            }
                        }


                        connection.Close();


                    }



                }
                catch(Exception ex)
                {

                    FlexitHisMVC.StandardMessages.CallSerilog(ex);
                    connection.Close();


                }

            }
            return userID;
        }



        public ResponseStruct<PatientStruct> SearchForPatients(string fullNamePattern, long hospitalID)

        {
            ResponseStruct<PatientStruct> response = new ResponseStruct<PatientStruct>();
            response.data = new List<PatientStruct>();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {


                    connection.Open();

                    using (MySqlCommand com = new MySqlCommand($@"SELECT * 
FROM patients
WHERE CONCAT( name,  ' ', surname ) LIKE  @fullNamePattern and hospitalID = @hospitalID ", connection))
                    {
                        com.Parameters.AddWithValue("@fullNamePattern", $"%{fullNamePattern}%");
                        com.Parameters.AddWithValue("@hospitalID", hospitalID);

                        MySqlDataReader reader = com.ExecuteReader();
                        if (reader.HasRows)
                        {


                            while (reader.Read())
                            {

                                PatientStruct patient = new PatientStruct();
                                patient.ID = Convert.ToInt64(reader["id"]);
                                patient.name = reader["name"] == DBNull.Value ? "" : reader["name"].ToString();
                                patient.surname = reader["surname"] == DBNull.Value ? "" : reader["surname"].ToString();
                                patient.father = reader["father"] == DBNull.Value ? "" : reader["father"].ToString();
                                patient.genderID = reader["genderID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["genderID"]);
                                patient.fin = reader["fin"] == DBNull.Value ? "" : Convert.ToString(reader["fin"]);
                                patient.phone = reader["clientPhone"] == DBNull.Value ? 0 : Convert.ToInt64(reader["clientPhone"]);
                                patient.bDate = reader["bDate"] == DBNull.Value ? Convert.ToDateTime("0001-01-01T00:00:00") : Convert.ToDateTime(reader["bDate"]);


                                response.data.Add(patient);


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


        //public UserStruct getProfile(string userToken, string requestToken)
        //{
        //    Security security = new Security(Configuration, _hostingEnvironment);
        //    int userID1 = security.selectUserToken(userToken);
        //    int userID2 = security.selectRequestToken(requestToken);
        //    UserStruct user = new UserStruct();
        //    if (userID1 == userID2)
        //    {
        //        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        //        {


        //            try
        //            {




        //                connection.Open();

        //                using (MySqlCommand com = new MySqlCommand("select * from user where userID=@userID order by userID desc limit 1", connection))
        //                {
        //                    com.Parameters.AddWithValue("@userID", userID1);

        //                    MySqlDataReader reader = com.ExecuteReader();
        //                    if (reader.HasRows)
        //                    {


        //                        while (reader.Read())
        //                        {
        //                            user.name = reader["name"] == DBNull.Value ? "" : reader["name"].ToString();
        //                            user.surname = reader["surname"] == DBNull.Value ? "" : reader["surname"].ToString();
        //                            user.email = reader["email"] == DBNull.Value ? "" : reader["email"].ToString();
        //                            user.phone = reader["mobile"] == DBNull.Value ? "" : reader["mobile"].ToString();
        //                        }
        //                        user.response = 0;
        //                        user.responseString = "OK";
        //                        user.requestToken = security.requestTokenGenerator(userToken, userID1);
        //                    }
        //                    else
        //                    {
        //                        user.response = 2;
        //                        user.responseString = "user not found";
        //                    }


        //                    connection.Close();


        //                }



        //            }
        //            catch (Exception ex)
        //            {
        //                connection.Close();
        //                user.response = 1;
        //                user.responseString = $"Exception:{ex.Message}";


        //            }

        //        }
        //    }
        //    else
        //    {
        //        user.response = 3;
        //        user.responseString = $"Access danied!";
        //    }



        //    return user;
        //}
        //        public ResponseStruct<ServiceStruct> getServices(string userToken, string requestToken, string administrative_area_level_2)

        //        {
        //            Security security = new Security(Configuration, _hostingEnvironment);
        //            int userID1 = security.selectUserToken(userToken);
        //            int userID2 = security.selectRequestToken(requestToken);
        //            ResponseStruct<ServiceStruct> serviceResponse = new ResponseStruct<ServiceStruct>();
        //            serviceResponse.data = new List<ServiceStruct>();
        //            UserStruct user = new UserStruct();
        //            try
        //            {
        //                if (userID1 == userID2)
        //                {

        //                    serviceResponse.status = 1;//authorized

        //                    using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        //                    {

        //                        connection.Open();

        //                        using (MySqlCommand com = new MySqlCommand(@"select *,
        //(select name from service where serviceId=a.serviceid)as serviceName,
        //(select serviceImgUrl from service where serviceId=a.serviceid)as serviceImgUrl from relservicecity a
        //where cityId = (select CityId from city where administrative_area_level_2 = @administrative_area_level_2)
        //", connection))
        //                        {
        //                            com.Parameters.AddWithValue("@administrative_area_level_2", administrative_area_level_2);



        //                            MySqlDataReader reader = com.ExecuteReader();
        //                            if (reader.HasRows)
        //                            {
        //                                serviceResponse.status = 1;//authorized and has rows

        //                                while (reader.Read())
        //                                {

        //                                    ServiceStruct service = new ServiceStruct();
        //                                    service.id = Convert.ToInt32(reader["serviceID"]);
        //                                    service.name = reader["serviceName"].ToString();
        //                                    service.serviceImgUrl = reader["serviceImgUrl"].ToString();
        //                                    //ads.countryID = Convert.ToInt32(reader["countryId"]);



        //                                    serviceResponse.data.Add(service);


        //                                }
        //                                connection.Close();


        //                            }
        //                            else
        //                            {

        //                                connection.Close();
        //                                serviceResponse.status = 2;//authorized and no rows

        //                            }
        //                            serviceResponse.requestToken = security.requestTokenGenerator(userToken, userID1);

        //                        }

        //                    }




        //                }
        //                else
        //                {
        //                    serviceResponse.status = 3;//access danied
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                serviceResponse.status = 4; //error
        //            }

        //            return serviceResponse;


        //        }



    }
}
