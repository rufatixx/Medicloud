using System;
using System.Collections.Generic;

namespace FlexitHisMVC.Models.DTO
{
    public class LogInDTO
    {
        public User? personal { get; set; }
        public List<Hospital>? hospitals { get; set; }
        public List<Kassa>? kassaList { get; set; }
    }


}
