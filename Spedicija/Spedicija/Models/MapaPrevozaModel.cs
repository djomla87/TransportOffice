using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Spedicija.Models
{
    public class MapaPrevozaModel
    {
        public String Status { get; set; }

        public String Adresa { get; set; }                                           
        public String Opis    {     get; set; }
        public String Long    {     get; set; }
        public String Lat     {     get; set; }
        public String Tip {         get; set; }
        public int IdDnevnik { get; internal set; }
        public Nullable<int> IdDodatniUtovar { get;  set; }
        public Nullable<int> IdDodatniIstovar { get;  set; }
        public char Aktivnost { get;  set; }
        public int RedniBroj { get;  set; }
        public string Header { get;  set; }
        public int IdRedoslijed { get;  set; }
        public int? IdVozilo { get; set; }
        public int? IdVozac { get; set; }
    }
}