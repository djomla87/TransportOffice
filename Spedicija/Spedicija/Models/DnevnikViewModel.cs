using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Spedicija.Models
{
    public class DnevnikViewModel
    {

        public DnevnikPrevoza Novo { get; set; }
        public DnevnikPrevoza Staro { get; set; }
        public Log Log { get; set; }

    }
}