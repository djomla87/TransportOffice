using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Spedicija.Models
{
    public class AndroidChange
    {

        public String Korisnik     { get; set; }
        public String Datum        { get; set; }
        public String Dnevnik      { get; set; }
        public string Long { get; internal set; }
        public string Lat { get; internal set; }
        public int IdLog { get; internal set; }
    }
}