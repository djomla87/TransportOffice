using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Spedicija.Repository
{
    public class Spedicija : ISpedicija
    {
         private readonly uvhszjiy_spedicijaEntities _context;

        public Spedicija(uvhszjiy_spedicijaEntities context)
        {
            _context = context;
            Logovi = new LogRepository(_context);
            Vozaci = new VozacRepository(_context);
        }

        public ILogRepository Logovi { get; private set; }
        public IVozacRepository Vozaci { get; private set; }

        public int Save()
        {

            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}