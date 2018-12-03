using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Spedicija.Models
{
    public class UtovarSpedicije
    {

        public String Tip { get; set; }
        public int Rb { get; set; }
        public String Firma { get; set; }
        public String Adresa { get; set; }
        public String Grad  { get; set; }
        public String Spedicija  { get; set; }
        public String Roba { get; set; }

        public String Datum { get; set; }
        public String Kontakt { get; set; }
    }
}