using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Spedicija.Repository
{
    public class VozacRepository : Repository<Vozaci>, IVozacRepository
    {
        public VozacRepository(uvhszjiy_spedicijaEntities context) : base(context) { }
    }
}