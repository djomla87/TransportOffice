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
    
    public partial class RedoslijedUtovarIstovar
    {
        public int IdRedoslijed { get; set; }
        public int IdDnevnik { get; set; }
        public Nullable<int> IdDodatniUtovar { get; set; }
        public Nullable<int> IdDodatniIstovar { get; set; }
        public string Aktivnost { get; set; }
        public int RedniBroj { get; set; }
    
        public virtual DnevnikIstovar DnevnikIstovar { get; set; }
        public virtual DnevnikPrevoza DnevnikPrevoza { get; set; }
        public virtual DnevnikUtovar DnevnikUtovar { get; set; }
    }
}