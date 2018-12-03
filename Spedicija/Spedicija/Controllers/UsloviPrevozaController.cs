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
    public class UsloviPrevozaController : Controller
    {
        private uvhszjiy_spedicijaEntities db = new uvhszjiy_spedicijaEntities();

        //
        // GET: /UsloviPrevoza/
        [Authorize]
        public ActionResult Index()
        {
            // return View(db.UsloviPrevoza.ToList());
			 return View(new List<UsloviPrevoza>());
        }

		// 
	 public ActionResult IndexAjax(jQueryDataTableParamModel param)
	 {
		 var list = db.UsloviPrevoza.ToList();
         IEnumerable<UsloviPrevoza> filteredList;

				var FIdUslov  = Convert.ToString(Request["sSearch_0"]).ToLower();
				var FTekst  = Convert.ToString(Request["sSearch_1"]).ToLower();
				var FAktivan  = Convert.ToString(Request["sSearch_2"]).ToLower();

            filteredList = list;


            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredList = filteredList.Where(c =>  
											  (String.IsNullOrEmpty("" + c.IdUslov ) ? "" :  "" +  c.IdUslov).ToLower().Contains(param.sSearch.ToLower()) 		
											 ||   (String.IsNullOrEmpty("" + c.Tekst ) ? "" :  "" +  c.Tekst).ToLower().Contains(param.sSearch.ToLower()) 	
		
																	);

            }

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);

            Func<UsloviPrevoza, object> orderingFunction = c =>
            {
                switch (sortColumnIndex)
                {
				 					case 0: return c.IdUslov;
									case 1: return c.Tekst;
									case 2: return c.Aktivan;
									default: return c.IdUslov;
                }
			};

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredList = filteredList.OrderBy(orderingFunction);
            else
                filteredList = filteredList.OrderByDescending(orderingFunction);

            var displayedList = filteredList.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedList select new object[] { 
			 				c.IdUslov,
							c.Tekst,
							c.Aktivan ? "DA" : "NE",
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
        // GET: /UsloviPrevoza/Details/5
        [Authorize]
        public ActionResult Details(int id = 0)
        {
            UsloviPrevoza usloviprevoza = db.UsloviPrevoza.Find(id);
            if (usloviprevoza == null)
            {
                return HttpNotFound();
            }
            return View(usloviprevoza);
        }

        //
        // GET: /UsloviPrevoza/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /UsloviPrevoza/Create

        [HttpPost]
        public ActionResult Create(UsloviPrevoza usloviprevoza)
        {
            if (ModelState.IsValid)
            {

                if (Request["_Aktivan"] != null)
                {
                    if (Request["_Aktivan"].ToString().Equals("on"))
                        usloviprevoza.Aktivan = true;
                    else usloviprevoza.Aktivan = false;
                }
                else usloviprevoza.Aktivan = false;

                db.UsloviPrevoza.Add(usloviprevoza);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(usloviprevoza);
        }

        //
        // GET: /UsloviPrevoza/Edit/5
        [Authorize]
        public ActionResult Edit(int id = 0)
        {
            UsloviPrevoza usloviprevoza = db.UsloviPrevoza.Find(id);
            if (usloviprevoza == null)
            {
                return HttpNotFound();
            }
            return View(usloviprevoza);
        }

        //
        // POST: /UsloviPrevoza/Edit/5

        [HttpPost]
        public ActionResult Edit(UsloviPrevoza usloviprevoza)
        {
            if (ModelState.IsValid)
            {

                if (Request["_Aktivan"] != null)
                {
                    if (Request["_Aktivan"].ToString().Equals("on"))
                        usloviprevoza.Aktivan = true;
                    else usloviprevoza.Aktivan = false;
                }
                else usloviprevoza.Aktivan = false;

                db.Entry(usloviprevoza).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(usloviprevoza);
        }

        //
        // GET: /UsloviPrevoza/Delete/5
        [Authorize]
        public ActionResult Delete(int id = 0)
        {
            UsloviPrevoza usloviprevoza = db.UsloviPrevoza.Find(id);
            if (usloviprevoza == null)
            {
                return HttpNotFound();
            }
            return View(usloviprevoza);
        }

        //
        // POST: /UsloviPrevoza/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            UsloviPrevoza usloviprevoza = db.UsloviPrevoza.Find(id);


            foreach (PonudaUsloviPrevoza i in db.PonudaUsloviPrevoza.Where(c => c.IdUsloviPrevoza == id).ToList())
            {
                db.PonudaUsloviPrevoza.Remove(i);
            }
            db.SaveChanges();

            db.UsloviPrevoza.Remove(usloviprevoza);
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