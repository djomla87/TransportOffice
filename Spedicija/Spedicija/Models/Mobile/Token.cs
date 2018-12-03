using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Spedicija.Models.Mobile
{
    public class Token
    {
        public String Code { get; set; }
        public String Korisnik { get; set; }
        public String KorisnickoIme { get; set; }
        public String VrijemeKreiranjaTokena { get; set; }
        public String Aktivan { get; set; }
        public string Rola { get; internal set; }

        public int IdVozac { get; set; }
    }
}