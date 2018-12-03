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
    
    public partial class Vozaci
    {
        public Vozaci()
        {
            this.DnevnikPrevoza = new HashSet<DnevnikPrevoza>();
            this.VozacDnevnica = new HashSet<VozacDnevnica>();
            this.IDnevnikPrevozaVozac = new HashSet<IDnevnikPrevozaVozac>();
        }
    
        public int IdVozac { get; set; }
        public string ImeVozaca { get; set; }
        public string Telefon { get; set; }
        public string Adresa { get; set; }
        public string BrojPasosa { get; set; }
        public string JMBG { get; set; }
        public string BrojLK { get; set; }
        public string BrojVozackeDozvole { get; set; }
        public Nullable<int> IdSubjekt { get; set; }
        public Nullable<bool> ZapisAktivan { get; set; }
        public string ZiroRacun { get; set; }
        public string Napomene { get; set; }
    
        public virtual ICollection<DnevnikPrevoza> DnevnikPrevoza { get; set; }
        public virtual Subjekt Subjekt { get; set; }
        public virtual ICollection<VozacDnevnica> VozacDnevnica { get; set; }
        public virtual ICollection<IDnevnikPrevozaVozac> IDnevnikPrevozaVozac { get; set; }
    }
}
