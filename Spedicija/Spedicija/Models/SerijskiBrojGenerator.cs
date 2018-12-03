using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Spedicija.Models
{
    public static class SerijskiBrojGenerator
    {
      
        public static String Broj(int IdParent = 0)
        {
            uvhszjiy_spedicijaEntities db = new uvhszjiy_spedicijaEntities();


            if (IdParent != 0)
            {
                String s1 = db.DnevnikPrevoza.Find(IdParent).SerijskiBroj;
                int a = db.DnevnikPrevoza.Where(c => c.SerijskiBroj.Contains(s1)).Count();
                return s1 + "-" + a;
            }

            String godina = DateTime.Today.Year.ToString() + "-";
            var max = db.DnevnikPrevoza.Where(c => (c.ZapisAktivan ?? false) && c.IdDnevnikParent == null).OrderByDescending(c => c.IdDnevnik).FirstOrDefault();

            if (max.SerijskiBroj.StartsWith(godina))
            {
                int br = Convert.ToInt32(max.SerijskiBroj.Split('-')[1]) + 1;
                String str = ("0000" + br);
                str = str.Substring(str.Length - 5, 5);

                return godina + str;
            }
            else
                return godina + "00001";

        }

    }
}