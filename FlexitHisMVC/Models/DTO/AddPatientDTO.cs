using System;
namespace FlexitHisMVC.Models
{
    public class AddPatientDTO
    {
       //public string userToken { get; set; }
       // public string requestToken { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string father { get; set; }
        public long clientPhone { get; set; }
        public int genderID { get; set; }
        public string fin { get; set; }
        public int requestTypeID { get; set; }
        public int priceGroupID { get; set; }
        public int hospitalID { get; set; }
        public int serviceID { get; set; }
        public int depID { get; set; }
        public int docID { get; set; }
        public int referDocID { get; set; }
        public string birthDate { get; set; }
        public string note { get; set; }
    }
}
