//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Spedicija
{
    using System;
    using System.Collections.Generic;
    
    public partial class VozacTroskovi
    {
        public int IdCozacTroskovi { get; set; }
        public decimal Iznos { get; set; }
        public int IdValuta { get; set; }
        public string Tip { get; set; }
        public Nullable<int> IdVrstaTroska { get; set; }
        public bool OdobrenoZaduzenje { get; set; }
        public Nullable<bool> PrihvacenRashod { get; set; }
        public Nullable<bool> RazmjenjenoMjenjacnica { get; set; }
        public System.DateTime Datum { get; set; }
        public string Napomena { get; set; }
        public bool Aktivno { get; set; }
        public int IdVozac { get; set; }
        public Nullable<bool> Zakljucano { get; set; }
        public Nullable<bool> Kartica { get; set; }
        public Nullable<int> IdDnevnik { get; set; }
    
        public virtual Valuta Valuta { get; set; }
        public virtual VozacVrstaTroskova VozacVrstaTroskova { get; set; }
        public virtual DnevnikPrevoza DnevnikPrevoza { get; set; }
    }
}
