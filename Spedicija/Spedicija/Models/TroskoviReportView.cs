using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Spedicija.Models
{
    public class TroskoviReportView
    {
        public String iznosTroska { get; set; }
        public String VrstaTrosak { get; set; }
        public String IdTroskovi { get; set; }
        public String Opis { get; set; }
        public int Rb { get; set; }

        public String Trosak { get; set; }

        public String IznosTroskaBAM { get; set; }

    }
}