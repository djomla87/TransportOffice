using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Spedicija.Models;
using Spedicija.Models.Finansije;


namespace Spedicija.Controllers
{
    public class TroskoviController : Controller
    {
        private uvhszjiy_spedicijaEntities db = new uvhszjiy_spedicijaEntities();

        //
        // GET: /Troskovi/
        [Authorize]
        public ActionResult Index()
        {
           
            return View(new List<Troskovi>());
        }

		// 
        [Authorize]
	    public ActionResult IndexAjax(jQueryDataTableParamModel param)
	 {
		 var list = db.Troskovi.Include(t => t.DnevnikPrevoza).Include(t => t.Valuta).ToList();
         IEnumerable<Troskovi> filteredList;

				var FIdTroskovi  = Convert.ToString(Request["sSearch_0"]).ToLower();
				var FIdDnevnik  = Convert.ToString(Request["sSearch_1"]).ToLower();
				var FIznos  = Convert.ToString(Request["sSearch_2"]).ToLower();
				var FIdValuta  = Convert.ToString(Request["sSearch_3"]).ToLower();
				var FVrsta  = Convert.ToString(Request["sSearch_4"]).ToLower();
		
        filteredList = list.Where(c =>	
				  (String.IsNullOrEmpty("" + c.IdTroskovi ) ? "" :  "" +  c.IdTroskovi).ToLower().Contains(FIdTroskovi) 
				 &&   (String.IsNullOrEmpty("" + c.IdDnevnik ) ? "" :  "" +  c.IdDnevnik).ToLower().Contains(FIdDnevnik) 
				 &&   (String.IsNullOrEmpty("" + c.Iznos ) ? "" :  "" +  c.Iznos).ToLower().Contains(FIznos) 
				 &&   (String.IsNullOrEmpty("" + c.IdValuta ) ? "" :  "" +  c.IdValuta).ToLower().Contains(FIdValuta) 
				 &&   (String.IsNullOrEmpty("" + c.Vrsta ) ? "" :  "" +  c.Vrsta).ToLower().Contains(FVrsta) 
		                                  );


            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredList = filteredList.Where(c =>  
											  (String.IsNullOrEmpty("" + c.IdTroskovi ) ? "" :  "" +  c.IdTroskovi).ToLower().Contains(param.sSearch.ToLower()) 
		
											 ||   (String.IsNullOrEmpty("" + c.IdDnevnik ) ? "" :  "" +  c.IdDnevnik).ToLower().Contains(param.sSearch.ToLower()) 
		
											 ||   (String.IsNullOrEmpty("" + c.Iznos ) ? "" :  "" +  c.Iznos).ToLower().Contains(param.sSearch.ToLower()) 
		
											 ||   (String.IsNullOrEmpty("" + c.IdValuta ) ? "" :  "" +  c.IdValuta).ToLower().Contains(param.sSearch.ToLower()) 
		
											 ||   (String.IsNullOrEmpty("" + c.Vrsta ) ? "" :  "" +  c.Vrsta).ToLower().Contains(param.sSearch.ToLower()) 
		
																	);

            }

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);

            Func<Troskovi, object> orderingFunction = c =>
            {
                switch (sortColumnIndex)
                {
				 					case 0: return c.IdTroskovi;
									case 1: return c.IdDnevnik;
									case 2: return c.Iznos;
									case 3: return c.IdValuta;
									case 4: return c.Vrsta;
									default: return c.IdTroskovi;
                }
			};

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredList = filteredList.OrderBy(orderingFunction);
            else
                filteredList = filteredList.OrderByDescending(orderingFunction);

            var displayedList = filteredList.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedList select new object[] { 
			 				c.IdTroskovi,
							c.IdDnevnik,
							c.Iznos,
							c.IdValuta,
							c.Vrsta,
								""
            };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = list.Count(),
                iTotalDisplayRecords = filteredList.Count(),
                aaData = result
            },
                        JsonRequestBehavior.AllowGet);

		}


        //
        // GET: /Troskovi/Details/5
        [Authorize]
        public ActionResult Details(int id = 0)
        {
            Troskovi troskovi = db.Troskovi.Find(id);
            if (troskovi == null)
            {
                return HttpNotFound();
            }
            return View(troskovi);
        }

        //
        // GET: /Troskovi/Create
        [Authorize]
        public ActionResult Create(int id = 0)
        {
            ViewBag.IdDnevnik = id;
            ViewBag.IdValuta = new SelectList(db.Valuta, "IdValuta", "OznakaValute");

            /*
            List<SelectListItem> items = new List<SelectListItem>
            {
                new SelectListItem(){ Value = "Uvozno Carinjenje",  Text  = "Uvozno Carinjenje"},
                new SelectListItem(){ Value = "Izvozno Carinjenje",  Text  = "Izvozno Carinjenje"},
                new SelectListItem(){ Value = "Auto Dan",  Text  = "Auto Dan"},
                 new SelectListItem(){ Value = "Rent a Car",  Text  = "Rent a Car"},
                new SelectListItem(){ Value = "Tranzitni Dokument T1",  Text  = "Tranzitni Dokument T1"},
                 new SelectListItem(){ Value = "Carinski terminal",  Text  = "Carinski terminal"}
            };
 */
            ViewBag.Vrsta = new SelectList(db.TipUsluge, "Naziv","Naziv");

            return View();
        }

        //
        // POST: /Troskovi/Create

        [HttpPost]
        public ActionResult Create(Troskovi troskovi)
        {
            if (ModelState.IsValid)
            {
                db.Troskovi.Add(troskovi);
                db.SaveChanges();
                return RedirectToAction("Details", "DnevnikPrevoza", new { id = troskovi.IdDnevnik });
            }

            ViewBag.IdDnevnik =  troskovi.IdDnevnik;
            ViewBag.IdValuta = new SelectList(db.Valuta, "IdValuta", "OznakaValute");

            /*
            List<SelectListItem> items = new List<SelectListItem>
            {
                new SelectListItem(){ Value = "Uvozno Carinjenje",  Text  = "Uvozno Carinjenje" },
                new SelectListItem(){ Value = "Izvozno Carinjenje",  Text  = "Izvozno Carinjenje" },
                new SelectListItem(){ Value = "Auto Dan",  Text  = "Auto Dan"},
                 new SelectListItem(){ Value = "Rent a Car",  Text  = "Rent a Car"},
                new SelectListItem(){ Value = "Tranzitni Dokument T1",  Text  = "Tranzitni Dokument T1" },
                 new SelectListItem(){ Value = "Carinski terminal",  Text  = "Carinski terminal"}
            };
            */

            ViewBag.Vrsta = new SelectList(db.TipUsluge, "Naziv", "Naziv");

            return View(troskovi);
        }

        //
        // GET: /Troskovi/Edit/5
        [Authorize]
        [SpedicijaAutorizacija(Roles = "admin")]
        public ActionResult Edit(int id = 0)
        {
            Troskovi troskovi = db.Troskovi.Find(id);
            if (troskovi == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdDnevnik = new SelectList(db.DnevnikPrevoza, "IdDnevnik", "BrojNaloga", troskovi.IdDnevnik);
            ViewBag.IdValuta = new SelectList(db.Valuta, "IdValuta", "NazivValute", troskovi.IdValuta);
            return View(troskovi);
        }

        //
        // POST: /Troskovi/Edit/5

        [HttpPost]
        public ActionResult Edit(Troskovi troskovi)
        {
            if (ModelState.IsValid)
            {
                db.Entry(troskovi).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdDnevnik = new SelectList(db.DnevnikPrevoza, "IdDnevnik", "BrojNaloga", troskovi.IdDnevnik);
            ViewBag.IdValuta = new SelectList(db.Valuta, "IdValuta", "NazivValute", troskovi.IdValuta);
            return View(troskovi);
        }

        //
        // GET: /Troskovi/Delete/5
        [Authorize]
        [SpedicijaAutorizacija(Roles = "admin")]
        public ActionResult Delete(int id = 0)
        {
            Troskovi troskovi = db.Troskovi.Find(id);
            if (troskovi == null)
            {
                return HttpNotFound();
            }
            return View(troskovi);
        }

        //
        // POST: /Troskovi/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Troskovi troskovi = db.Troskovi.Find(id);
            int ret = Convert.ToInt32(troskovi.IdDnevnik);
            db.Troskovi.Remove(troskovi);
            db.SaveChanges();
            return RedirectToAction("Details", "DnevnikPrevoza", new { id = ret });
        }

        [Authorize]
        [SpedicijaAutorizacija(Roles = "admin")]
        public ActionResult IzvjestajFinansije()
        {
            ViewBag.Vozaci = new SelectList(db.Vozaci, "IdVozac", "ImeVozaca");
            ViewBag.Vozila = new SelectList(db.Vozilo.Select(c => new { IdVozilo = c.IdVozilo, Oznaka = c.TipVozila + " " + c.RegistarskiBroj }), "IdVozilo", "Oznaka");
            ViewBag.DatumOd = DateTime.Today.AddDays(-30).ToShortDateString();
            ViewBag.DatumDo = DateTime.Today.ToShortDateString();

            var selected = new[] { 0 };
            var ture = db.DnevnikPrevoza.Where(c => (c.ZapisAktivan ?? false))
                .ToList()
                .Select(c => new SelectListItem
                {
                    Value = c.IdDnevnik.ToString(),
                    Text = c.SerijskiBroj + " [" + c.UtovarGrad + " - " + c.IstovarGrad + "]",
                    Selected = selected.Contains(c.IdDnevnik)
                }).ToList().OrderByDescending(c => c.Value);

            ViewBag.Ture = ture;
            ViewBag.FinansijeTura = null;
            ViewBag.Poruka = "";
            return View();
        }

        [Authorize]
        [HttpPost]
        [SpedicijaAutorizacija(Roles = "admin")]
        public ActionResult IzvjestajFinansije( int ? IdVozac, int ? IdVozilo, DateTime DatumOd, DateTime DatumDo)
        {

            var a = Request.Params;
            ViewBag.DatumOd = DatumOd.ToShortDateString();
            ViewBag.DatumDo = DatumDo.ToShortDateString();
            ViewBag.Vozaci = new SelectList(db.Vozaci, "IdVozac", "ImeVozaca", IdVozac);
            ViewBag.Vozila = new SelectList(db.Vozilo.Select(c => new { IdVozilo = c.IdVozilo, Oznaka = c.TipVozila + " " + c.RegistarskiBroj }), "IdVozilo", "Oznaka", IdVozilo);

            var selected = new[] { "0" };
            if (Request["Ture"] != null)
                selected = Request["Ture"].ToString().Split(',');

            var ture = db.DnevnikPrevoza.Where(c => (c.ZapisAktivan ?? false))
                .ToList()
                .Select(c => new SelectListItem
                {
                    Value = c.IdDnevnik.ToString(),
                    Text = c.SerijskiBroj + " [" + c.UtovarGrad + " - " + c.IstovarGrad + "]",
                    Selected = selected.Contains(c.IdDnevnik.ToString())
                }).ToList().OrderByDescending(c => c.Value);

            ViewBag.Ture = ture;


            var dnevnici = selected.Select(c => Convert.ToInt32(c)).ToList();

            var Podaci = db.DnevnikPrevoza.Include(k=> k.Vozaci).Include(k => k.Vozilo)
                .Where(c => (c.ZapisAktivan ?? false) && c.DatumDnevnika <= DatumDo && c.DatumDnevnika >= DatumOd).ToList();

            if (IdVozac.HasValue)
                Podaci = Podaci.Where(c => c.IdVozac == IdVozac).ToList();
            if (IdVozilo.HasValue)
                Podaci = Podaci.Where(c => c.IdVozilo == IdVozilo).ToList();
            if (Request["Ture"] != null)
            Podaci = Podaci.Where(c => dnevnici.Contains(c.IdDnevnik)).ToList();

            List<Finansija> Lst = new List<Finansija>();
            List<Finansija> LstVozac = new List<Finansija>();

            Lst = Podaci.Select(c => new Finansija {
                RB = 1,
                SerijskiBroj = c.SerijskiBroj + " [" + c.UtovarGrad + " - " + c.IstovarGrad + "]",
                Vozilo = (c.IdVozilo == null) ? "" : c.Vozilo.TipVozila + " " + c.Vozilo.RegistarskiBroj,
                Vozac = (c.IdVozac == null) ? "" : c.Vozaci.ImeVozaca,
                Opis = "Cijena Prevoza",
                Prihod = (( c.CijenaPrevoza * c.Valuta.UKM ) ?? 0 ),
                Rashod = 0,
                Dobit = ((c.CijenaPrevoza * c.Valuta.UKM) ?? 0)
            }).ToList();

            Lst.AddRange(
                Podaci.SelectMany(p => p.Troskovi).Select(c => new Finansija
                {
                    RB = 2,
                    SerijskiBroj = c.DnevnikPrevoza.SerijskiBroj + " [" + c.DnevnikPrevoza.UtovarGrad + " - " + c.DnevnikPrevoza.IstovarGrad + "]",
                    Vozilo = (c.DnevnikPrevoza.IdVozilo == null) ? "" : c.DnevnikPrevoza.Vozilo.TipVozila + " " + c.DnevnikPrevoza.Vozilo.RegistarskiBroj,
                    Vozac = (c.DnevnikPrevoza.IdVozac == null) ? "" : c.DnevnikPrevoza.Vozaci.ImeVozaca,
                    Opis = c.Vrsta,
                    Prihod = ((c.Iznos * c.Valuta.UKM) ?? 0),
                    Rashod = ((c.StvarniTrosak * c.Valuta.UKM) ?? 0),
                    Kartica = 0,
                    Dobit = ((c.Iznos * c.Valuta.UKM) ?? 0) - ((c.StvarniTrosak * c.Valuta.UKM) ?? 0)
                }).ToList()
                );

            LstVozac = Podaci.SelectMany(p => p.Troskovi).Where(c => (c.Vrsta.Equals("Plata vozača") || c.Vrsta.Equals("Dnevnice Vozača") || c.Vrsta.Equals("Vozač zadržao ostatak od avansa"))).Select(c => new Finansija
            {
                RB = 1,
                SerijskiBroj = c.DnevnikPrevoza.SerijskiBroj + " [" + c.DnevnikPrevoza.UtovarGrad + " - " + c.DnevnikPrevoza.IstovarGrad + "]",
                Vozilo = (c.DnevnikPrevoza.IdVozilo == null) ? "" : c.DnevnikPrevoza.Vozilo.TipVozila + " " + c.DnevnikPrevoza.Vozilo.RegistarskiBroj,
                Vozac = (c.DnevnikPrevoza.IdVozac == null) ? "" : c.DnevnikPrevoza.Vozaci.ImeVozaca,
                Opis = c.Vrsta,
                Prihod = ((c.StvarniTrosak * c.Valuta.UKM) ?? 0),
                Rashod = 0,
                Kartica = 0,
                Dobit = ((c.StvarniTrosak * c.Valuta.UKM) ?? 0)
            }).ToList();



            Lst.AddRange(
                Podaci.SelectMany(p => p.VozacTroskovi).Where(c => c.Tip.Equals("RASHOD")).Select(c => new Finansija
                {
                    RB =            3,
                    SerijskiBroj =  c.DnevnikPrevoza.SerijskiBroj + " [" + c.DnevnikPrevoza.UtovarGrad + " - " + c.DnevnikPrevoza.IstovarGrad + "]",
                    Vozilo =        (c.DnevnikPrevoza.IdVozilo == null) ? "" : c.DnevnikPrevoza.Vozilo.TipVozila + " " + c.DnevnikPrevoza.Vozilo.RegistarskiBroj,
                    Vozac =         (c.DnevnikPrevoza.IdVozac == null) ? "" : c.DnevnikPrevoza.Vozaci.ImeVozaca,
                    Opis =          c.VozacVrstaTroskova.Naziv,
                    Prihod =        0,
                    Rashod =        (c.Kartica ?? false) ? 0 : ((c.Iznos * c.Valuta.UKM) ?? 0),
                    Kartica =       (c.Kartica ?? false) ? ((c.Iznos * c.Valuta.UKM) ?? 0) : 0,
                    Dobit =         (c.Kartica ?? false) ? 0 : -1 * ((c.Iznos * c.Valuta.UKM) ?? 0)
                }).ToList()
                );

            LstVozac.AddRange(
               Podaci.SelectMany(p => p.VozacTroskovi).Select(c => new Finansija
               {
                   RB = c.Tip.Equals("ZADUŽENJE") ? 2 : 3,
                   SerijskiBroj = c.DnevnikPrevoza.SerijskiBroj + " [" + c.DnevnikPrevoza.UtovarGrad + " - " + c.DnevnikPrevoza.IstovarGrad + "]",
                   Vozilo = (c.DnevnikPrevoza.IdVozilo == null) ? "" : c.DnevnikPrevoza.Vozilo.TipVozila + " " + c.DnevnikPrevoza.Vozilo.RegistarskiBroj,
                   Vozac = (c.DnevnikPrevoza.IdVozac == null) ? "" : c.DnevnikPrevoza.Vozaci.ImeVozaca,
                   Opis = c.Tip.Equals("ZADUŽENJE") ? "Avans" : c.VozacVrstaTroskova.Naziv,
                   Prihod   = c.Tip.Equals("ZADUŽENJE") ? ((c.Iznos * c.Valuta.UKM) ?? 0) : 0 ,
                   Rashod   = c.Tip.Equals("RASHOD") ? ((c.Kartica ?? false) ? 0 : ((c.Iznos * c.Valuta.UKM) ?? 0)) : 0,
                   Kartica  = c.Tip.Equals("RASHOD") ? ((c.Kartica ?? false) ? ((c.Iznos * c.Valuta.UKM) ?? 0) : 0) : 0,
                   Dobit    = c.Tip.Equals("RASHOD") ? ((c.Kartica ?? false) ? 0 : -1 * ((c.Iznos * c.Valuta.UKM) ?? 0)) : ((c.Iznos * c.Valuta.UKM) ?? 0)
               }).ToList()
               );


            ViewBag.FinansijeTura = Lst.OrderBy(c => c.SerijskiBroj).ThenBy(c => c.RB).ToList();
            ViewBag.FinansijeVozac = LstVozac.OrderBy(c => c.SerijskiBroj).ThenBy(c => c.RB).ToList();

            ViewBag.Poruka = "Izvještaj o zaradi za period: " + DatumOd.ToShortDateString() + " - " + DatumDo.ToShortDateString() + "<br> Parametri izvještaja: <br>" +
                "Vozač: "  + (IdVozac.HasValue ? db.Vozaci.Find(IdVozac).ImeVozaca : "SVI VOZAČI") + ", <br>" +
                "Vozilo: " + (IdVozilo.HasValue ? db.Vozilo.Find(IdVozilo).RegistarskiBroj : "SVA VOZILA");

            return View();
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}