using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Spedicija.Models
{
    public class DnavnikPrevozaIndexModel
    {
        public String SerijskiBroj         {get; set;}
        public String DatumDnevnika        {get; set;}
        public String Subjekt              {get; set;}
        public String UvoznikIzvoznik      {get; set;}
        public String Utovari              {get; set;}
        public String DatumUtovara         {get; set;}
        public String Istovari             {get; set;}
        public String DatumIstovara        {get; set;}
        public String Dokument             {get; set;}
        public String VozacVozilo          {get; set;}
        public String Status               {get; set;}
        public String IdDnevnik            {get; set;}
        public String Parenti              {get; set;}
        public String Valute { get; set; }

        public bool Uplaceno { get; set; }
        public string Skladiste { get; set; }
        public string DrugiPrevoznikTranzit { get; internal set; }
        public string PoslataFaktura { get; internal set; }
        public string BrzaPosta { get; internal set; }
    }
}