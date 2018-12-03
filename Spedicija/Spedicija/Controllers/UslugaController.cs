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
    public class UslugaController : Controller
    {
        private uvhszjiy_spedicijaEntities db = new uvhszjiy_spedicijaEntities();

        //
        // GET: /Usluga/
        [Authorize]
        public ActionResult Index()
        {
            // return View(db.TipUsluge.ToList());
			 return View(new List<TipUsluge>());
        }

		// 
	 public ActionResult IndexAjax(jQueryDataTableParamModel param)
	 {
		 var list = db.TipUsluge.ToList();
         IEnumerable<TipUsluge> filteredList;

				var FIdTipUsluge  = Convert.ToString(Request["sSearch_0"]).ToLower();
				var FNaziv  = Convert.ToString(Request["sSearch_1"]).ToLower();
		
        filteredList = list.Where(c =>	
				  (String.IsNullOrEmpty("" + c.IdTipUsluge ) ? "" :  "" +  c.IdTipUsluge).ToLower().Contains(FIdTipUsluge) 
				 &&   (String.IsNullOrEmpty("" + c.Naziv ) ? "" :  "" +  c.Naziv).ToLower().Contains(FNaziv) 
		                                  );


            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredList = filteredList.Where(c =>  
											  (String.IsNullOrEmpty("" + c.IdTipUsluge ) ? "" :  "" +  c.IdTipUsluge).ToLower().Contains(param.sSearch.ToLower()) 
		
											 ||   (String.IsNullOrEmpty("" + c.Naziv ) ? "" :  "" +  c.Naziv).ToLower().Contains(param.sSearch.ToLower()) 
		
																	);

            }

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);

            Func<TipUsluge, object> orderingFunction = c =>
            {
                switch (sortColumnIndex)
                {
				 					case 0: return c.IdTipUsluge;
									case 1: return c.Naziv;
									default: return c.IdTipUsluge;
                }
			};

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredList = filteredList.OrderBy(orderingFunction);
            else
                filteredList = filteredList.OrderByDescending(orderingFunction);

            var displayedList = filteredList.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedList select new object[] { 
			 				c.IdTipUsluge,
							c.Naziv,
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
        // GET: /Usluga/Details/5
        [Authorize]
        public ActionResult Details(int id = 0)
        {
            TipUsluge tipusluge = db.TipUsluge.Find(id);
            if (tipusluge == null)
            {
                return HttpNotFound();
            }
            return View(tipusluge);
        }

        //
        // GET: /Usluga/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Usluga/Create

        [HttpPost]
        public ActionResult Create(TipUsluge tipusluge)
        {
            if (ModelState.IsValid)
            {
                db.TipUsluge.Add(tipusluge);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tipusluge);
        }

        //
        // GET: /Usluga/Edit/5
        [Authorize]
        public ActionResult Edit(int id = 0)
        {
            TipUsluge tipusluge = db.TipUsluge.Find(id);
            if (tipusluge == null)
            {
                return HttpNotFound();
            }
            return View(tipusluge);
        }

        //
        // POST: /Usluga/Edit/5

        [HttpPost]
        public ActionResult Edit(TipUsluge tipusluge)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tipusluge).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tipusluge);
        }

        //
        // GET: /Usluga/Delete/5
        [Authorize]
        public ActionResult Delete(int id = 0)
        {
            TipUsluge tipusluge = db.TipUsluge.Find(id);
            if (tipusluge == null)
            {
                return HttpNotFound();
            }
            return View(tipusluge);
        }

        //
        // POST: /Usluga/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            TipUsluge tipusluge = db.TipUsluge.Find(id);

            db.TipUsluge.Remove(tipusluge);
            db.SaveChanges();
            return RedirectToAction("Index");
        }



        public JsonResult VratiUsluge()
        {
            return Json(db.TipUsluge.ToList(), JsonRequestBehavior.AllowGet);
        }



        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}