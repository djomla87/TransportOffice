using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Spedicija.Models
{
    public class FakturaView
    {

               public int Rb       { get; set; }
               public String SerijskiBroj { get; set; }
               public String Opis     { get; set; }
               public Decimal CijenaBezPdv { get; set; }
               public Decimal CijenaBezPdvBAM { get; set; }
               public Decimal Pdv     { get; set; }
               public Decimal PdvBAM  { get; set; }
               public String Valuta  { get; set; }

    }
}