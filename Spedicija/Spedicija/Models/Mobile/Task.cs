using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Spedicija.Models.Mobile
{
    public class Task
    {
            public int IdTask { get; set; }
         public String SerijskiBroj         { get; set; }
         public String Vozilo               { get; set; }
         public String Utovar               { get; set; }
         public String Istovar              { get; set; }
         public String Roba                 { get; set; }
         public String Status               { get; set; }
         public String DatumAzuriranja      { get; set; }
        public string UvoznaSpedicija { get; internal set; }
        public string IzvoznaSpedicija { get; internal set; }
        public string Uvoznik { get; internal set; }
        public string Izvoznik { get; internal set; }
        public string Napomena { get; internal set; }
        public string RefBroj { get; internal set; }
        public string Pregledano { get; internal set; }
    }
}