using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Spedicija.Models;


namespace Spedicija.Controllers
{
    public class DnevnikValuteController : Controller
    {
        private uvhszjiy_spedicijaEntities db = new uvhszjiy_spedicijaEntities();

        //
        // GET: /DnevnikValute/
        [Authorize]
        public ActionResult Index()
        {
            var dnevnikvalute = db.DnevnikValute.Include(d => d.DnevnikPrevoza);
            return View(dnevnikvalute.ToList());
        }

		// 
	 public ActionResult IndexAjax(jQueryDataTableParamModel param)
	 {
		 var list = db.DnevnikValute.Include(d => d.DnevnikPrevoza).ToList();
         IEnumerable<DnevnikValute> filteredList;

				var FIdDnevnikValute  = Convert.ToString(Request["sSearch_0"]).ToLower();
				var FIdDnevnik  = Convert.ToString(Request["sSearch_1"]).ToLower();
				var FDatumAktivnosti  = Convert.ToString(Request["sSearch_2"]).ToLower();
				var FAktivnost  = Convert.ToString(Request["sSearch_3"]).ToLower();
				var FDetalji  = Convert.ToString(Request["sSearch_4"]).ToLower();
		
        filteredList = list.Where(c =>	
				  (String.IsNullOrEmpty("" + c.IdDnevnikValute ) ? "" :  "" +  c.IdDnevnikValute).ToLower().Contains(FIdDnevnikValute) 
				 &&   (String.IsNullOrEmpty("" + c.IdDnevnik ) ? "" :  "" +  c.IdDnevnik).ToLower().Contains(FIdDnevnik) 
				 &&   (String.IsNullOrEmpty("" + c.DatumAktivnosti ) ? "" :  "" +  c.DatumAktivnosti).ToLower().Contains(FDatumAktivnosti) 
				 &&   (String.IsNullOrEmpty("" + c.Aktivnost ) ? "" :  "" +  c.Aktivnost).ToLower().Contains(FAktivnost) 
				 &&   (String.IsNullOrEmpty("" + c.Detalji ) ? "" :  "" +  c.Detalji).ToLower().Contains(FDetalji) 
		                                  );


            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredList = filteredList.Where(c =>  
											  (String.IsNullOrEmpty("" + c.IdDnevnikValute ) ? "" :  "" +  c.IdDnevnikValute).ToLower().Contains(param.sSearch.ToLower()) 
		
											 ||   (String.IsNullOrEmpty("" + c.IdDnevnik ) ? "" :  "" +  c.IdDnevnik).ToLower().Contains(param.sSearch.ToLower()) 
		
											 ||   (String.IsNullOrEmpty("" + c.DatumAktivnosti ) ? "" :  "" +  c.DatumAktivnosti).ToLower().Contains(param.sSearch.ToLower()) 
		
											 ||   (String.IsNullOrEmpty("" + c.Aktivnost ) ? "" :  "" +  c.Aktivnost).ToLower().Contains(param.sSearch.ToLower()) 
		
											 ||   (String.IsNullOrEmpty("" + c.Detalji ) ? "" :  "" +  c.Detalji).ToLower().Contains(param.sSearch.ToLower()) 
		
																	);

            }

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);

            Func<DnevnikValute, object> orderingFunction = c =>
            {
                switch (sortColumnIndex)
                {
				 					case 0: return c.IdDnevnikValute;
									case 1: return c.IdDnevnik;
									case 2: return c.DatumAktivnosti;
									case 3: return c.Aktivnost;
									case 4: return c.Detalji;
									default: return c.IdDnevnikValute;
                }
			};

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredList = filteredList.OrderBy(orderingFunction);
            else
                filteredList = filteredList.OrderByDescending(orderingFunction);

            var displayedList = filteredList.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedList select new object[] { 
			 				c.IdDnevnikValute,
							c.IdDnevnik,
							String.IsNullOrEmpty("" + c.DatumAktivnosti) ? "" :   DateTime.Parse(c.DatumAktivnosti.ToString()).ToString("dd.MM.yyyy"),
							c.Aktivnost,
							c.Detalji,
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


        [Authorize]
        public ActionResult DnevnikIstekleValute(jQueryDataTableParamModel param)
     {

         var list = db.DnevnikPrevoza.Include(d => d.Subjekt1).Where(c => (!(c.Uplaceno ?? false))).ToList()
                .Where(c => ((c.DatumSlanjaFakture ?? DateTime.Today.AddHours(7)).AddDays(c.ValutaPlacanja ?? 999999) < DateTime.Today.AddHours(7))).ToList();


         IEnumerable<DnevnikPrevoza> filteredList;

         var FSerijskiBroj = Convert.ToString(Request["sSearch_0"]).ToLower();
         var FSubjekt = Convert.ToString(Request["sSearch_1"]).ToLower();
         var FUI = Convert.ToString(Request["sSearch_2"]).ToLower();
         var FCijenaPrevoza = Convert.ToString(Request["sSearch_3"]).ToLower();
         var FDatumSlanjaFakture = Convert.ToString(Request["sSearch_4"]).ToLower();
         var FValutaPlacanja = Convert.ToString(Request["sSearch_5"]).ToLower();
         var FValutaIstekla = Convert.ToString(Request["sSearch_6"]).ToLower();

         filteredList = list.Where(c =>
                     (c.SerijskiBroj).ToLower().Contains(FSerijskiBroj)
                  && (c.DatumSlanjaFakture.Value.ToString("dd.MM.yyyy")).ToLower().Contains(FDatumSlanjaFakture)
                  && (c.Subjekt1.Naziv).ToLower().Contains(FSubjekt)
                  && (c.UtovarGrad+c.IstovarGrad).ToLower().Contains(FUI)
                  && (c.CijenaPrevoza.ToString()).ToLower().Contains(FCijenaPrevoza)
                  && (c.ValutaPlacanja.ToString()).ToLower().Contains(FValutaPlacanja)
                  && (c.DatumSlanjaFakture.Value.AddDays(c.ValutaPlacanja ?? 0).ToString("dd.MM.yyyy")).ToLower().Contains(FValutaIstekla)
                  
                                           );


         if (!string.IsNullOrEmpty(param.sSearch))
         {
             filteredList = filteredList.Where(c =>
                                            (c.SerijskiBroj).ToLower().Contains(param.sSearch)
                  || (c.DatumSlanjaFakture.Value.ToString("dd.MM.yyyy")).ToLower().Contains(param.sSearch)
                  || (c.Subjekt1.Naziv).ToLower().Contains(param.sSearch)
                  || (c.UtovarGrad + c.IstovarGrad).ToLower().Contains(param.sSearch)
                  || (c.CijenaPrevoza.ToString()).ToLower().Contains(param.sSearch)
                  || (c.ValutaPlacanja.ToString()).ToLower().Contains(param.sSearch)
                  || (c.DatumSlanjaFakture.Value.AddDays(c.ValutaPlacanja ?? 0).ToString("dd.MM.yyyy")).ToLower().Contains(param.sSearch)

                                                                 );

         }

         var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);

         Func<DnevnikPrevoza, object> orderingFunction = c =>
         {
             switch (sortColumnIndex)
             {
                 case 0: return c.SerijskiBroj;
                 case 1: return c.Subjekt1.Naziv;
                 case 2: return c.UtovarGrad + c.IstovarGrad;
                 case 3: return c.CijenaPrevoza + " " + c.Valuta.OznakaValute;
                 case 4: return c.DatumSlanjaFakture.Value.ToString("yyyy-MM-dd");
                 case 5: return c.ValutaPlacanja;
                 case 6: return c.DatumSlanjaFakture.Value.AddDays(c.ValutaPlacanja ?? 0).ToString("yyyy-MM-dd");
                 default: return c.SerijskiBroj;
             }
         };

         var sortDirection = Request["sSortDir_0"]; // asc or desc
         if (sortDirection == "asc")
             filteredList = filteredList.OrderBy(orderingFunction);
         else
             filteredList = filteredList.OrderByDescending(orderingFunction);

         var displayedList = filteredList.Skip(param.iDisplayStart).Take(param.iDisplayLength);
         var result = from c in displayedList
                      select new object[] { 
			 				c.SerijskiBroj,
							c.Subjekt1.Naziv,
							c.UtovarGrad + " / " + c.IstovarGrad,
							c.CijenaPrevoza + " " + c.Valuta.OznakaValute,
                            c.DatumSlanjaFakture.Value.ToString("dd.MM.yyyy"),
                            c.ValutaPlacanja,
                            c.DatumSlanjaFakture.Value.AddDays(c.ValutaPlacanja ?? 0).ToString("dd.MM.yyyy"),
							c.IdDnevnik
            };
         return Json(new
         {
             position = 25,
             sEcho = param.sEcho,
             iTotalRecords = list.Count(),
             iTotalDisplayRecords = filteredList.Count(),
             aaData = result
         },
                     JsonRequestBehavior.AllowGet);

     }


    

     public JsonResult PageNumber(int id = 0)
     {
         var list = db.DnevnikPrevoza.Where(c => (!(c.Uplaceno ?? false))).Select(c => new {c.IdDnevnik, c.DatumSlanjaFakture, c.ValutaPlacanja }).ToList()
                 .Where(c => ((c.DatumSlanjaFakture ?? DateTime.Today.AddHours(7)).AddDays(c.ValutaPlacanja ?? 999999) < DateTime.Today.AddHours(7)))
               .OrderBy(c => ((c.DatumSlanjaFakture ?? DateTime.Today.AddHours(7)).AddDays(c.ValutaPlacanja ?? 999999))).ToList();

         
         int index = (id == 0  || list.Where(c => c.IdDnevnik == id).Count() == 0)  ? 0 : list.IndexOf(list.Single(c => c.IdDnevnik == id));
         index = index - index % 10;



         return Json( index , JsonRequestBehavior.AllowGet);
     }
        //
        // GET: /DnevnikValute/Details/5
        [Authorize]
        public ActionResult Details(int id = 0)
        {
            DnevnikValute dnevnikvalute = db.DnevnikValute.Find(id);
            if (dnevnikvalute == null)
            {
                return HttpNotFound();
            }
            return View(dnevnikvalute);
        }

        //
        // GET: /DnevnikValute/Create
        [Authorize]
        public ActionResult Create(int id)
        {
            ViewBag.IdDnevnik = id;


            var a = new [] {
             new { Aktivnost = "Telefonski Poziv", Vrijednost = "Telefonski Poziv" },
              new { Aktivnost = "Usmeno upozorenje", Vrijednost = "Usmeno upozorenje" },
               new { Aktivnost = "Poslat Email", Vrijednost = "Poslat Email" }
            };

            ViewBag.AktivnostiSL = new SelectList(a, "Aktivnost", "Vrijednost");
            return View();
        }

        //
        // POST: /DnevnikValute/Create

        [HttpPost]
        public ActionResult Create(DnevnikValute dnevnikvalute)
        {
            if (ModelState.IsValid)
            {
                db.DnevnikValute.Add(dnevnikvalute);
                db.SaveChanges();
                int dnevnik = dnevnikvalute.IdDnevnik;
                return RedirectToAction("IstekleValute", "DnevnikPrevoza", new { id = dnevnik });
            }

            ViewBag.IdDnevnik = new SelectList(db.DnevnikPrevoza, "IdDnevnik", "BrojNaloga", dnevnikvalute.IdDnevnik);
            return View(dnevnikvalute);
        }

        //
        // GET: /DnevnikValute/Edit/5
        [Authorize]
        public ActionResult Edit(int id = 0)
        {
            DnevnikValute dnevnikvalute = db.DnevnikValute.Find(id);
            if (dnevnikvalute == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdDnevnik = new SelectList(db.DnevnikPrevoza, "IdDnevnik", "BrojNaloga", dnevnikvalute.IdDnevnik);
            return View(dnevnikvalute);
        }

        //
        // POST: /DnevnikValute/Edit/5

        [HttpPost]
        public ActionResult Edit(DnevnikValute dnevnikvalute)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dnevnikvalute).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdDnevnik = new SelectList(db.DnevnikPrevoza, "IdDnevnik", "BrojNaloga", dnevnikvalute.IdDnevnik);
            return View(dnevnikvalute);
        }

        //
        // GET: /DnevnikValute/Delete/5
        [Authorize]
        public ActionResult Delete(int id = 0)
        {
            DnevnikValute dnevnikvalute = db.DnevnikValute.Find(id);
            if (dnevnikvalute == null)
            {
                return HttpNotFound();
            }
            return View(dnevnikvalute);
        }

        //
        // POST: /DnevnikValute/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            DnevnikValute dnevnikvalute = db.DnevnikValute.Find(id);
            int dnevnik = dnevnikvalute.IdDnevnik;
            db.DnevnikValute.Remove(dnevnikvalute);
            db.SaveChanges();

            return RedirectToAction("IstekleValute", "DnevnikPrevoza", new { id = dnevnik });
        }


        public ActionResult SetStatus(int id, int Status)
        {
            //DnevnikValute dv = db.DnevnikValute.Find(id);
            DnevnikPrevoza dp = db.DnevnikPrevoza.Find(id);

            dp.Uplaceno = Status == 1;
            db.Entry(dp).State = EntityState.Modified;


            DnevnikValute dvnew = new DnevnikValute { IdDnevnik = dp.IdDnevnik, Aktivnost = "Postavka Statsa " + (Status == 1 ? "NAPLAĆENO" : "NIJE NAPLAĆENO"), DatumAktivnosti = DateTime.Now.AddHours(7), Detalji = "Automatsko aplikativno dodavanje zapisa" };
            db.DnevnikValute.Add(dvnew);

            db.SaveChanges();

            return RedirectToAction("IstekleValute", "DnevnikPrevoza", new { id = dp.IdDnevnik });

        }

        public JsonResult SetStatusJson(int id, int Status)
        {
            //DnevnikValute dv = db.DnevnikValute.Find(id);
            DnevnikPrevoza dp = db.DnevnikPrevoza.Find(id);

            dp.Uplaceno = Status == 1;
            db.Entry(dp).State = EntityState.Modified;


            DnevnikValute dvnew = new DnevnikValute { IdDnevnik = dp.IdDnevnik, Aktivnost = "Postavka Statsa " + (Status == 1 ? "NAPLAĆENO" : "NIJE NAPLAĆENO"), DatumAktivnosti = DateTime.Now.AddHours(7), Detalji = "Automatsko aplikativno dodavanje zapisa" };
            db.DnevnikValute.Add(dvnew);

            db.SaveChanges();

            return Json(Status, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetStatus(int id) {

            DnevnikPrevoza dp = db.DnevnikPrevoza.Find(id);


            String status = "";

            if ((!(dp.Uplaceno ?? false)) && ((dp.DatumSlanjaFakture ?? DateTime.Today.AddHours(7)).AddDays(dp.ValutaPlacanja ?? 999999)) < DateTime.Today.AddHours(7))
                status = "0";
            else status = "1";

            return Json(status, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}