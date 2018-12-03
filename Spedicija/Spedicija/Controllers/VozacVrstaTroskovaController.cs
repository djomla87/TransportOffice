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
    public class VozacVrstaTroskovaController : Controller
    {
        private uvhszjiy_spedicijaEntities db = new uvhszjiy_spedicijaEntities();

        //
        // GET: /VozacVrstaTroskova/
        [Authorize]
        public ActionResult Index()
        {
            // return View(db.VozacVrstaTroskova.ToList());
			 return View(new List<VozacVrstaTroskova>());
        }

		// 
	 public ActionResult IndexAjax(jQueryDataTableParamModel param)
	 {
		 var list = db.VozacVrstaTroskova.ToList();
         IEnumerable<VozacVrstaTroskova> filteredList;

				var FIdVrstaTroska  = Convert.ToString(Request["sSearch_0"]).ToLower();
				var FNaziv  = Convert.ToString(Request["sSearch_1"]).ToLower();
		
        filteredList = list.Where(c =>	
				  (String.IsNullOrEmpty("" + c.IdVrstaTroska ) ? "" :  "" +  c.IdVrstaTroska).ToLower().Contains(FIdVrstaTroska) 
				 &&   (String.IsNullOrEmpty("" + c.Naziv ) ? "" :  "" +  c.Naziv).ToLower().Contains(FNaziv) 
		                                  );


            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredList = filteredList.Where(c =>  
											  (String.IsNullOrEmpty("" + c.IdVrstaTroska ) ? "" :  "" +  c.IdVrstaTroska).ToLower().Contains(param.sSearch.ToLower()) 
		
											 ||   (String.IsNullOrEmpty("" + c.Naziv ) ? "" :  "" +  c.Naziv).ToLower().Contains(param.sSearch.ToLower()) 
		
																	);

            }

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);

            Func<VozacVrstaTroskova, object> orderingFunction = c =>
            {
                switch (sortColumnIndex)
                {
				 					case 0: return c.IdVrstaTroska;
									case 1: return c.Naziv;
									default: return c.IdVrstaTroska;
                }
			};

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredList = filteredList.OrderBy(orderingFunction);
            else
                filteredList = filteredList.OrderByDescending(orderingFunction);

            var displayedList = filteredList.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedList select new object[] { 
			 				c.IdVrstaTroska,
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
        // GET: /VozacVrstaTroskova/Details/5
        [Authorize]
        public ActionResult Details(int id = 0)
        {
            VozacVrstaTroskova vozacvrstatroskova = db.VozacVrstaTroskova.Find(id);
            if (vozacvrstatroskova == null)
            {
                return HttpNotFound();
            }
            return View(vozacvrstatroskova);
        }

        //
        // GET: /VozacVrstaTroskova/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /VozacVrstaTroskova/Create

        [HttpPost]
        public ActionResult Create(VozacVrstaTroskova vozacvrstatroskova)
        {
            if (ModelState.IsValid)
            {
                db.VozacVrstaTroskova.Add(vozacvrstatroskova);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(vozacvrstatroskova);
        }

        //
        // GET: /VozacVrstaTroskova/Edit/5
        [Authorize]
        public ActionResult Edit(int id = 0)
        {
            VozacVrstaTroskova vozacvrstatroskova = db.VozacVrstaTroskova.Find(id);
            if (vozacvrstatroskova == null)
            {
                return HttpNotFound();
            }
            return View(vozacvrstatroskova);
        }

        //
        // POST: /VozacVrstaTroskova/Edit/5

        [HttpPost]
        public ActionResult Edit(VozacVrstaTroskova vozacvrstatroskova)
        {
            if (ModelState.IsValid)
            {
                db.Entry(vozacvrstatroskova).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(vozacvrstatroskova);
        }

        //
        // GET: /VozacVrstaTroskova/Delete/5
        [Authorize]
        public ActionResult Delete(int id = 0)
        {
            VozacVrstaTroskova vozacvrstatroskova = db.VozacVrstaTroskova.Find(id);
            if (vozacvrstatroskova == null)
            {
                return HttpNotFound();
            }
            return View(vozacvrstatroskova);
        }

        //
        // POST: /VozacVrstaTroskova/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            VozacVrstaTroskova vozacvrstatroskova = db.VozacVrstaTroskova.Find(id);
            db.VozacVrstaTroskova.Remove(vozacvrstatroskova);
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