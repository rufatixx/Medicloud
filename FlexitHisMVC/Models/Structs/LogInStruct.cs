using System;
using System.Collections.Generic;

namespace FlexitHis_API.Models.Structs
{
    public class LogInStruct
    {
        public int status { get; set; }
        public int id { get; set; }
       public string userToken { get; set; }
        public string requestToken { get; set; }
        public int userType { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public List<Hospital> hospitals { get; set; }
        public List<AllowedKassa> kassaList { get; set; }
    }
    public class Hospital {
        public int id { get; set; }
        public int hospitalID { get; set; }
        public int userID { get; set; }
        public string hospitalName { get; set; }

    }
    public class AllowedKassa
    {
        public int id { get; set; }
        public int kassaID { get; set; }
        public string name { get; set; }


    }
}
