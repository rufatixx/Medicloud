using System;
namespace FlexitHis_API.Models.Structs
{
    public class RecipeStruct
    {
        public long id { get; set; }
        public long pOperationID { get; set; }
        public long patientID { get; set; }
        public long userID { get; set; }
        public double price { get; set; }
        public string pTypeName { get; set; }
        public DateTime cdate { get; set; }
    }
}
