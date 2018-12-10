using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Spedicija
{
    public class AppSettings
    {

        public Dictionary<String, String> settings { get; private set; }
        private static AppSettings instance;

        private AppSettings() { }

        public static Dictionary<String, String> GetSettings() {

            if (instance == null)
            {
                uvhszjiy_spedicijaEntities db = new uvhszjiy_spedicijaEntities();

                Dictionary<String, String> s = new Dictionary<string, string>();
                var Podesavanja = db.AppPodesavanja.ToList();
                foreach (var item in Podesavanja )
                {
                    s.Add(item.Vrsta, item.Vrijednost);
                }

                instance = new AppSettings();
                instance.settings = s;
            }

            return instance.settings;
        }

    }
}
