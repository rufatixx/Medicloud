using System;
using System.Collections.Generic;

namespace FlexitHisMVC.Models.DTO
{
    public class LogInDTO
    {
        public Personal? personal { get; set; }
        public List<Hospital>? hospitals { get; set; }
        public List<Kassa>? kassaList { get; set; }
    }


}
