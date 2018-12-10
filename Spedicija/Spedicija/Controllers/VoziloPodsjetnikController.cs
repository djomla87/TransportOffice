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
    public class VoziloPodsjetnikController : Controller
    {
        private uvhszjiy_spedicijaEntities db = new uvhszjiy_spedicijaEntities();

        //
        // GET: /VoziloPodsjetnik/

        public ActionResult Index()
        {
            var vozilopodsjetnik = db.VoziloPodsjetnik.Include(v => v.Vozilo);
            return View(vozilopodsjetnik.ToList());
        }

		// 
	 public ActionResult IndexAjax(jQueryDataTableParamModel param)
	 {
		 var list = db.VoziloPodsjetnik.Include(v => v.Vozilo).ToList();
         IEnumerable<VoziloPodsjetnik> filteredList;

				var FIdVoziloPodsjetnik  = Convert.ToString(Request["sSearch_0"]).ToLower();
				var FIdVozilo  = Convert.ToString(Request["sSearch_1"]).ToLower();
				var FVrsta  = Convert.ToString(Request["sSearch_2"]).ToLower();
				var FDatumIzrade  = Convert.ToString(Request["sSearch_3"]).ToLower();
				var FDatumPodsjetnika  = Convert.ToString(Request["sSearch_4"]).ToLower();
		
        filteredList = list.Where(c =>	
				  (String.IsNullOrEmpty("" + c.IdVoziloPodsjetnik ) ? "" :  "" +  c.IdVoziloPodsjetnik).ToLower().Contains(FIdVoziloPodsjetnik) 
				 &&   (String.IsNullOrEmpty("" + c.IdVozilo ) ? "" :  "" +  c.IdVozilo).ToLower().Contains(FIdVozilo) 
				 &&   (String.IsNullOrEmpty("" + c.Vrsta ) ? "" :  "" +  c.Vrsta).ToLower().Contains(FVrsta) 
				 &&   (String.IsNullOrEmpty("" + c.DatumIzrade ) ? "" :  "" +  c.DatumIzrade).ToLower().Contains(FDatumIzrade) 
				 &&   (String.IsNullOrEmpty("" + c.DatumPodsjetnika ) ? "" :  "" +  c.DatumPodsjetnika).ToLower().Contains(FDatumPodsjetnika) 
		                                  );


            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredList = filteredList.Where(c =>  
											  (String.IsNullOrEmpty("" + c.IdVoziloPodsjetnik ) ? "" :  "" +  c.IdVoziloPodsjetnik).ToLower().Contains(param.sSearch.ToLower()) 
		
											 ||   (String.IsNullOrEmpty("" + c.IdVozilo ) ? "" :  "" +  c.IdVozilo).ToLower().Contains(param.sSearch.ToLower()) 
		
											 ||   (String.IsNullOrEmpty("" + c.Vrsta ) ? "" :  "" +  c.Vrsta).ToLower().Contains(param.sSearch.ToLower()) 
		
											 ||   (String.IsNullOrEmpty("" + c.DatumIzrade ) ? "" :  "" +  c.DatumIzrade).ToLower().Contains(param.sSearch.ToLower()) 
		
											 ||   (String.IsNullOrEmpty("" + c.DatumPodsjetnika ) ? "" :  "" +  c.DatumPodsjetnika).ToLower().Contains(param.sSearch.ToLower()) 
		
																	);

            }

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);

            Func<VoziloPodsjetnik, object> orderingFunction = c =>
            {
                switch (sortColumnIndex)
                {
				 					case 0: return c.IdVoziloPodsjetnik;
									case 1: return c.IdVozilo;
									case 2: return c.Vrsta;
									case 3: return c.DatumIzrade;
									case 4: return c.DatumPodsjetnika;
									default: return c.IdVoziloPodsjetnik;
                }
			};

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredList = filteredList.OrderBy(orderingFunction);
            else
                filteredList = filteredList.OrderByDescending(orderingFunction);

            var displayedList = filteredList.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedList select new object[] { 
			 				c.IdVoziloPodsjetnik,
							c.IdVozilo,
							c.Vrsta,
							String.IsNullOrEmpty("" + c.DatumIzrade) ? "" :   DateTime.Parse(c.DatumIzrade.ToString()).ToString("dd.MM.yyyy"),
							String.IsNullOrEmpty("" + c.DatumPodsjetnika) ? "" :   DateTime.Parse(c.DatumPodsjetnika.ToString()).ToString("dd.MM.yyyy"),
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
        // GET: /VoziloPodsjetnik/Details/5

        public ActionResult Details(int id = 0)
        {
            VoziloPodsjetnik vozilopodsjetnik = db.VoziloPodsjetnik.Find(id);
            if (vozilopodsjetnik == null)
            {
                return HttpNotFound();
            }
            return View(vozilopodsjetnik);
        }

        //
        // GET: /VoziloPodsjetnik/Create

        public ActionResult Create(int id )
        {
            ViewBag.IdVozilo = id;
            return View();
        }

        //
        // POST: /VoziloPodsjetnik/Create

        [HttpPost]
        public ActionResult Create(VoziloPodsjetnik vozilopodsjetnik)
        {
            if (ModelState.IsValid)
            {
                db.VoziloPodsjetnik.Add(vozilopodsjetnik);
                db.SaveChanges();

                return RedirectToAction("Details", "Vozilo", new { id = vozilopodsjetnik.IdVozilo });
            }

            ViewBag.IdVozilo = new SelectList(db.Vozilo, "IdVozilo", "TipVozila", vozilopodsjetnik.IdVozilo);
            return View(vozilopodsjetnik);
        }

        //
        // GET: /VoziloPodsjetnik/Edit/5

        public ActionResult Edit(int id = 0)
        {
            VoziloPodsjetnik vozilopodsjetnik = db.VoziloPodsjetnik.Find(id);
            if (vozilopodsjetnik == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdVozilo = new SelectList(db.Vozilo, "IdVozilo", "TipVozila", vozilopodsjetnik.IdVozilo);
            return View(vozilopodsjetnik);
        }

        //
        // POST: /VoziloPodsjetnik/Edit/5

        [HttpPost]
        public ActionResult Edit(VoziloPodsjetnik vozilopodsjetnik)
        {
            if (ModelState.IsValid)
            {
                db.Entry(vozilopodsjetnik).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "Vozilo", new { id = vozilopodsjetnik.IdVozilo });
            }
            ViewBag.IdVozilo = new SelectList(db.Vozilo, "IdVozilo", "TipVozila", vozilopodsjetnik.IdVozilo);
            return View(vozilopodsjetnik);
        }

        //
        // GET: /VoziloPodsjetnik/Delete/5

        public ActionResult Delete(int id = 0)
        {
            VoziloPodsjetnik vozilopodsjetnik = db.VoziloPodsjetnik.Find(id);
            if (vozilopodsjetnik == null)
            {
                return HttpNotFound();
            }
            return View(vozilopodsjetnik);
        }

        //
        // POST: /VoziloPodsjetnik/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            VoziloPodsjetnik vozilopodsjetnik = db.VoziloPodsjetnik.Find(id);
            db.VoziloPodsjetnik.Remove(vozilopodsjetnik);
            db.SaveChanges();
            return RedirectToAction("Index");
        }







        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}