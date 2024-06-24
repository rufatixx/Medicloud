using System;
using System.Collections.Generic;

namespace Medicloud.Models
{
    public class ResponseDTO<T>
    {
        public string requestToken { get; set; }
        public int status { get; set; }
        public List<T> data { get; set; }
    }
}
