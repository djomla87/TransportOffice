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
    
    public partial class Vozilo
    {
        public Vozilo()
        {
            this.DnevnikPrevoza = new HashSet<DnevnikPrevoza>();
            this.DnevnikPrevoza1 = new HashSet<DnevnikPrevoza>();
            this.IDnevnikPrevozaVozac = new HashSet<IDnevnikPrevozaVozac>();
            this.VoziloPodsjetnik = new HashSet<VoziloPodsjetnik>();
        }
    
        public int IdVozilo { get; set; }
        public string TipVozila { get; set; }
        public string RegistarskiBroj { get; set; }
        public string DodatniPodaci { get; set; }
        public Nullable<System.DateTime> DatumZapisa { get; set; }
        public bool ZapisAktivan { get; set; }
        public string VrstaVozila { get; set; }
    
        public virtual ICollection<DnevnikPrevoza> DnevnikPrevoza { get; set; }
        public virtual ICollection<DnevnikPrevoza> DnevnikPrevoza1 { get; set; }
        public virtual ICollection<IDnevnikPrevozaVozac> IDnevnikPrevozaVozac { get; set; }
        public virtual ICollection<VoziloPodsjetnik> VoziloPodsjetnik { get; set; }
    }
}
