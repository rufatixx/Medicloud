using System;
using System.Collections.Generic;

namespace FlexitHis_API.Models.Structs
{
    public class KassaStruct
    {
        public long id { get; set; }
       
        public double kassaSum { get; set; }
       public List<RecipeStruct> recipeList { get; set; }
    }
    
}
