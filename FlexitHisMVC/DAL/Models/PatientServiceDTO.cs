using System;
namespace Medicloud.Models.DTO
{
    public class PatientServiceDTO
    {
        public long ID { get; set; }
        public bool isPaid { get; set; }
        public long cardServiceRelId { get; set; }
        public string ServiceName { get; set; }
        public string ServicePrice { get; set; }
        public string debt { get; set; }
        public string totalPaid { get; set; }
        public string CurrentPaymentAmount { get; set; }

    }
}

