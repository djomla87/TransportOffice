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
    
    public partial class Subjekt
    {
        public Subjekt()
        {
            this.DnevnikPrevoza = new HashSet<DnevnikPrevoza>();
            this.DnevnikPrevoza1 = new HashSet<DnevnikPrevoza>();
            this.NalogZaUtovar = new HashSet<NalogZaUtovar>();
            this.Vozaci = new HashSet<Vozaci>();
            this.Ponuda = new HashSet<Ponuda>();
        }
    
        public int IdSubjekt { get; set; }
        public string Naziv { get; set; }
        public string Adresa { get; set; }
        public string Telefon { get; set; }
        public string Email { get; set; }
        public string KontaktOsoba { get; set; }
        public string Timocom { get; set; }
        public Nullable<System.DateTime> DatumKreiranja { get; set; }
        public Nullable<System.DateTime> DatumZapisa { get; set; }
        public bool ZapisAktivan { get; set; }
        public string Opis { get; set; }
        public string Grad { get; set; }
        public string PIB { get; set; }
        public string PTT { get; set; }
        public string JIB { get; set; }
        public string Drzava { get; set; }
        public Nullable<int> RedniBroj { get; set; }
    
        public virtual ICollection<DnevnikPrevoza> DnevnikPrevoza { get; set; }
        public virtual ICollection<DnevnikPrevoza> DnevnikPrevoza1 { get; set; }
        public virtual ICollection<NalogZaUtovar> NalogZaUtovar { get; set; }
        public virtual ICollection<Vozaci> Vozaci { get; set; }
        public virtual ICollection<Ponuda> Ponuda { get; set; }
    }
}
