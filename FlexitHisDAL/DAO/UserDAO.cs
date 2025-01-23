namespace Medicloud.DAL.DAO
{
    public class UserDAO
    {
        public int id { get; set; }
        public string? name { get; set; }
        public string? surname { get; set; }
        public string? father { get; set; }
        public string? username { get; set; }
        public string? mobile { get; set; }
        public string? passportSerialNum { get; set; }
        public string? fin { get; set; }
        public string? email { get; set; }
        public string? bDate { get; set; }
        public DateTime? cDate { get; set; }
        public bool isActive { get; set; }
        public bool isRegistered { get; set; }
        public string? imagePath { get; set; }
        public string? pwd { get; set; }
    }
}
