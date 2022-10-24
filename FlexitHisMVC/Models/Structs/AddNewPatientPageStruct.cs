using System;
using System.Collections.Generic;

namespace FlexitHis_API.Models.Structs
{
    public class AddNewPatientPageStruct
    {
       public List<RequestType> requestTypes { get; set; }
        public List<Service> services { get; set; }
        public List<Department> departments { get; set; }
        public List<Personal> personal { get; set; }
        public List<Referer> referers { get; set; }
        public int status { get; set; }
        public string requestToken { get; set; }
       
    }
    public class RequestType
    {
        public int ID { get; set; }
        public string name { get; set; }
       

    }
    public class Service
    {
        public int ID { get; set; }
        public int depID { get; set; }
        public string name { get; set; }
        public double price { get; set; }

    }
    public class Department
    {
        public long ID { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public long typeID { get; set; }
        public long buildingID { get; set; }
        public string buildingName { get; set; }
        public long groupID { get; set; }
        public int genderID { get; set; }
        public int docIsRequired { get; set; }
        public int isActive { get; set; }
        public int isRandevuActive { get; set; }


    }
    public class Personal
    {
        public int ID { get; set; }
        public int depID { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string father { get; set; }


    }
    public class Referer
    {
        public int ID { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string father { get; set; }


    }

}
