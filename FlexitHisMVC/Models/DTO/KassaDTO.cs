using System;
using System.Collections.Generic;
using FlexitHisMVC.Models;


namespace FlexitHisMVC.Models
{
    public class KassaDTO
    {
        public long id { get; set; }
       
        public double kassaSum { get; set; }
       public List<Recipe> recipeList { get; set; }
    }
    
}
