using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Spedicija.Models
{
    public class SortUImodel
    {
       public String Aktivnost { get; set; }   
       public int IdDnevnik { get; set; }
       public Nullable<int> IdDodatniIstovar { get; set; }
       public Nullable<int> IdDodatniUtovar { get; set; }
    
       public int IdRedoslijed { get; set; }

    }
}