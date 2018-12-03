using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spedicija.Repository
{
    public interface ISpedicija : IDisposable
    {
        ILogRepository Logovi { get; }
        IVozacRepository Vozaci { get; }

        int Save();
    }
}
