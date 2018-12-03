using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Spedicija.Models
{
    public class SumePoddnevnika
    {
        public decimal drugiprevoznik { get;  set; }
        public decimal mojacijena { get;  set; }
        public String SerijskiBroj { get; set; }
        public decimal suma { get; set; }

        public decimal troskovi { get; set; }
        public decimal troskoviStvarni { get; internal set; }
    }
}