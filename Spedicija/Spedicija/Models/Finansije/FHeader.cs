using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Spedicija.Models.Finansije
{
    public class FHeader
    {

        public String SerijskiBroj { get; set; }
        public String Vozac { get; set; }
        public String Vozilo { get; set; }
        public String Opis { get; set; }
        public Decimal Prihod { get; set; }
        public Decimal Rashod { get; set; }
        public Decimal Kartica { get; set; }
        public Decimal Dobit { get; set; }

    }
}