using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Spedicija.Models;

namespace Spedicija.Models
{
    public class FakturaMaster
    {
        public Subjekt Narucioc { get; set; } 
        public String SerijskiBroj { get; set; }
        public String DatumFakture { get; set; }
        public String ValutaPlacanja { get; set; }
        public String Vozac { get; set; }
        public String Vozilo { get; set; }
        public List<FakturaView> Finansije { get; set; }
    }
}