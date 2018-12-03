using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Spedicija.Models
{
    public class SearchViewModel
    {

        public List<Subjekt>        Subjekt { get; set; }
        public List<DnevnikPrevoza> Dnevnik { get; set; }
        public List<Vozilo>         Vozila { get; set; }
        public List<Vozaci>         Vozaci { get; set; }
        public List<Dokument>       Dokument { get; set; }
        public List<NalogZaUtovar> Nalozi { get; set; }

        public SearchViewModel()
        {
            this.Subjekt = new List<Subjekt>();
            this.Dnevnik = new List<DnevnikPrevoza>();
            this.Vozaci = new List<Vozaci>();
            this.Vozila = new List<Vozilo>();
            this.Dokument = new List<Dokument>();
            this.Nalozi = new List<NalogZaUtovar>();
        }

    }
}