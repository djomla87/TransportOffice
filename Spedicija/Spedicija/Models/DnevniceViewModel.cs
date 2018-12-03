using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Spedicija.Models
{
    public class DnevniceViewModel 
    {
        
         public int Mjesec {get ; set;}
          public int   Godina {get ; set;}
          public Nullable<decimal> Plata { get; set; }
          public Nullable<decimal> Dnevnice { get; set; }
           public int IdVozacDnevnica { get; set; }
        

    }
}