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
    
    public partial class IzracunDnevnik
    {
        public int IdIzracunDnevnik { get; set; }
        public int IdIzracunZarada { get; set; }
        public int IdDnevnik { get; set; }
    
        public virtual DnevnikPrevoza DnevnikPrevoza { get; set; }
        public virtual IzracunZarada IzracunZarada { get; set; }
    }
}
