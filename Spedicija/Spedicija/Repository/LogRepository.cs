using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Spedicija.Repository
{
    public class LogRepository : Repository<Log>, ILogRepository
    {
        public LogRepository(uvhszjiy_spedicijaEntities context)
            : base(context)
        { }


    }
}