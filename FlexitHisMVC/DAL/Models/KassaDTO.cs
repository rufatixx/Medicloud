using System;
using System.Collections.Generic;
using Medicloud.Models;


namespace Medicloud.Models
{
    public class KassaDTO
    {
        public long id { get; set; }
       
        public double kassaSum { get; set; }
       public List<Recipe> recipeList { get; set; }
    }
    
}
