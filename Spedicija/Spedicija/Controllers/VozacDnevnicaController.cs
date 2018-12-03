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
    public class VozacDnevnicaController : Controller
    {
        private uvhszjiy_spedicijaEntities db = new uvhszjiy_spedicijaEntities();

        //
        // GET: /VozacDnevnica/

        [Authorize]
        public ActionResult Index()
        {
            var vozacdnevnica = db.VozacDnevnica.Include(v => v.Vozaci);
            return View(vozacdnevnica.ToList());
        }

        
		// 
        [Authorize]
        public ActionResult IndexAjax(jQueryDataTableParamModel param, int IdVozac)
	 {
         var list1 = db.VozacDnevnica.Include(c => c.DnevnicaDetail).Where(c => c.IdVozac == IdVozac).ToList().GroupBy(k => new { k.IdVozacDnevnica , k.Mjesec, k.Godina, k.Plata }).Select
             (c => new { IdVozacDnevnica = c.FirstOrDefault().IdVozacDnevnica, Mjesec = c.FirstOrDefault().Mjesec, Godina = c.FirstOrDefault().Godina, Plata = c.FirstOrDefault().Plata, Dnevnice = c.Sum(p => p.DnevnicaDetail.Sum(k => k.Dnevnica)) }).OrderByDescending(c => c.Godina).ThenByDescending(c => c.Mjesec).ToList();


         List<DnevniceViewModel> list = list1.Select(c => new DnevniceViewModel { Dnevnice = c.Dnevnice, Godina = c.Godina, IdVozacDnevnica = c.IdVozacDnevnica, Mjesec = c.Mjesec, Plata = c.Plata }).ToList();
            

         IEnumerable<DnevniceViewModel> filteredList;

				var Fmjesec  = Convert.ToString(Request["sSearch_0"]).ToLower();
				var FGodina   = Convert.ToString(Request["sSearch_1"]).ToLower();
				var Fplata  = Convert.ToString(Request["sSearch_2"]).ToLower();
                var FDnevnice = Convert.ToString(Request["sSearch_3"]).ToLower();
		
        filteredList = list.Where(c =>	
				 // (String.IsNullOrEmpty("" + c.IdVozacDnevnica ) ? "" :  "" +  c.IdVozacDnevnica).ToLower().Contains(FIdVozacDnevnica) 
				 //   (String.IsNullOrEmpty("" + c.IdVozac ) ? "" :  "" +  c.IdVozac).ToLower().Contains(FIdVozac) 
                    (c.Mjesec.ToString()).ToLower().Contains(Fmjesec)
                 && (c.Godina.ToString()).ToLower().Contains(FGodina)
                 && ((c.Plata ?? 0) .ToString("0.00")).ToLower().Contains(Fplata)
                 && ((c.Dnevnice ?? 0).ToString("0.00")).ToLower().Contains(Fplata)
		                                  );


            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredList = filteredList.Where(c =>  
											 // (String.IsNullOrEmpty("" + c.IdVozacDnevnica ) ? "" :  "" +  c.IdVozacDnevnica).ToLower().Contains(param.sSearch.ToLower()) 
		
											 //  (String.IsNullOrEmpty("" + c.IdVozac ) ? "" :  "" +  c.IdVozac).ToLower().Contains(param.sSearch.ToLower()) 
                                              (c.Mjesec.ToString()).ToLower().Contains(param.sSearch.ToLower())
                                             || (c.Godina.ToString()).ToLower().Contains(param.sSearch.ToLower())
                                             || ((c.Plata ?? 0).ToString("0.00")).ToLower().Contains(param.sSearch.ToLower())
                                                || ((c.Dnevnice ?? 0).ToString("0.00")).ToLower().Contains(param.sSearch.ToLower())
																	);

            }

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);

            Func<DnevniceViewModel, object> orderingFunction = c =>
            {
                switch (sortColumnIndex)
                {
				 					//case 0: return c.IdVozacDnevnica;
									
									case 0: return c.Mjesec;
									case 1: return c.Godina;
									case 2: return c.Plata;
                                    case 3: return c.Dnevnice;
									default: return c.IdVozacDnevnica;
                }
			};

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredList = filteredList.OrderBy(orderingFunction);
            else
                filteredList = filteredList.OrderByDescending(orderingFunction);

            var displayedList = filteredList.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedList select new object[] { 
			 				//c.IdVozacDnevnica,
                            c.Mjesec,
                            c.Godina,
							(c.Plata ?? 0).ToString("0.00"),
                            (c.Dnevnice ?? 0).ToString("0.00"),
								c.IdVozacDnevnica
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
        // GET: /VozacDnevnica/Details/5
        [Authorize]
        public ActionResult Details(int id = 0)
        {
            VozacDnevnica dnevnica = db.VozacDnevnica.Include(c => c.DnevnicaDetail).Where(c => c.IdVozacDnevnica == id).FirstOrDefault();

            var detalji = dnevnica.DnevnicaDetail.ToList();

            decimal SumDnevnice115  = detalji.Where(c => c.Datum.Day <= 15).Sum(c => c.Dnevnica ?? 0);
            decimal SumDnevnice1630 = detalji.Where(c => c.Datum.Day > 15).Sum(c => c.Dnevnica ?? 0);
            decimal SumDnevnice     = SumDnevnice115 + SumDnevnice1630;

            decimal SumNocenje115   = detalji.Where(c => c.Datum.Day <= 15).Sum(c => c.Nocenje ?? 0);
            decimal SumNocenje1630  = detalji.Where(c => c.Datum.Day > 15).Sum(c => c.Nocenje ?? 0);
            decimal SumNocenje      = SumNocenje115 + SumNocenje1630;

            decimal SumTroskovi115  = detalji.Where(c => c.Datum.Day <= 15).Sum(c => c.OstaliTroskovi ?? 0);
            decimal SumTroskovi1630 = detalji.Where(c => c.Datum.Day > 15).Sum(c => c.OstaliTroskovi ?? 0);
            decimal SumTroskovi     = SumTroskovi115 + SumTroskovi1630;

            decimal SumUkupno115    = detalji.Where(c => c.Datum.Day <= 15).Sum(c => c.Ukupno ?? 0);
            decimal SumUkupno1630   = detalji.Where(c => c.Datum.Day > 15).Sum(c => c.Ukupno ?? 0);
            decimal SumUkupno       = SumUkupno115 + SumUkupno1630;

            decimal Sum115          = SumDnevnice115 + SumNocenje115 + SumTroskovi115 + SumUkupno115;
            decimal Sum1630         = SumDnevnice1630 + SumNocenje1630 + SumTroskovi1630 + SumUkupno1630;
            decimal Sum             = Sum115 + Sum1630;

            ViewBag.SumDnevnice115  = SumDnevnice115 ;
            ViewBag.SumDnevnice1630 = SumDnevnice1630;
            ViewBag.SumDnevnice     = SumDnevnice    ;
    
            ViewBag.SumNocenje115   = SumNocenje115  ;
            ViewBag.SumNocenje1630  = SumNocenje1630 ;
            ViewBag.SumNocenje      = SumNocenje     ;

            ViewBag.SumTroskovi115  = SumTroskovi115 ;
            ViewBag.SumTroskovi1630 = SumTroskovi1630;
            ViewBag.SumTroskovi     = SumTroskovi    ;

            ViewBag.SumUkupno115    = SumUkupno115   ;
            ViewBag.SumUkupno1630   = SumUkupno1630  ;
            ViewBag.SumUkupno       = SumUkupno      ;

            ViewBag.Sum115          = Sum115         ;
            ViewBag.Sum1630         = Sum1630        ;
            ViewBag.Sum             = Sum            ;






















            if (dnevnica == null)
            {
                return HttpNotFound();
            }
            return View(dnevnica);
        }

        //
        // GET: /VozacDnevnica/Create
        [Authorize]
        public ActionResult Create(String mjesec, int id = 0)
        {
            ViewBag.IdVozac = id;
            ViewBag.Godina = mjesec.Split('.')[1].ToString();
            ViewBag.Mjesec = mjesec.Split('.')[0].ToString();
            return View();
        }

        //
        // POST: /VozacDnevnica/Create

        [HttpPost]
        public ActionResult Create(List<DnevnicaDetail> dnevnice, int IdVozac, decimal Plata = 0)
        {
            var a = Request.Params;

            if (ModelState.IsValid)
            {

                VozacDnevnica vd = new VozacDnevnica();
                vd.Godina = dnevnice[0].Datum.Year;
                vd.Mjesec = dnevnice[0].Datum.Month;
                vd.IdVozac = IdVozac;
                vd.IdValuta = 1;
                vd.Plata = Plata;
                db.VozacDnevnica.Add(vd);
                db.SaveChanges();

                foreach (DnevnicaDetail d in dnevnice)
                {
                    d.IdVozacDnevnica = vd.IdVozacDnevnica;
                    d.Dnevnica = d.Dnevnica == null ? 0 : d.Dnevnica;
                    d.Nocenje = d.Nocenje == null ? 0 : d.Nocenje;
                    d.OstaliTroskovi = d.OstaliTroskovi == null ? 0 : d.OstaliTroskovi;
                    d.Ukupno = d.Dnevnica + d.Nocenje + d.OstaliTroskovi;

                    db.DnevnicaDetail.Add(d);
                }
                db.SaveChanges();

                return RedirectToAction("Details", "Vozaci", new { id = IdVozac });
            }

            ViewBag.IdVozac = IdVozac;
            ViewBag.Godina = dnevnice[0].Datum.Year;
            ViewBag.Mjesec = dnevnice[0].Datum.Month;
            return View(dnevnice);
        }

        //
        // GET: /VozacDnevnica/Edit/5
        [Authorize]
        [SpedicijaAutorizacija(Roles = "admin")]
        public ActionResult Edit(int id = 0)
        {
            List<DnevnicaDetail> vozacdnevnica = db.DnevnicaDetail.Where(c => c.IdVozacDnevnica == id).ToList();
            if (vozacdnevnica == null)
            {
                return HttpNotFound();
            }


            ViewBag.Plata = db.VozacDnevnica.Single(c => c.IdVozacDnevnica == id).Plata;
            return View(vozacdnevnica);
        }

        //
        // POST: /VozacDnevnica/Edit/5

        [HttpPost]
        public ActionResult Edit(List<DnevnicaDetail> dnevnice, decimal Plata = 0)
        {
            if (ModelState.IsValid)
            {
                VozacDnevnica vd = db.VozacDnevnica.Find(dnevnice[0].IdVozacDnevnica);
                vd.Plata = Plata;
                db.Entry(vd).State = EntityState.Modified;
                db.SaveChanges();


                foreach (DnevnicaDetail d in dnevnice)
                {
                    d.IdVozacDnevnica = vd.IdVozacDnevnica;
                    d.Dnevnica = d.Dnevnica == null ? 0 : d.Dnevnica;
                    d.Nocenje = d.Nocenje == null ? 0 : d.Nocenje;
                    d.OstaliTroskovi = d.OstaliTroskovi == null ? 0 : d.OstaliTroskovi;
                    d.Ukupno = d.Dnevnica + d.Nocenje + d.OstaliTroskovi;

                    db.Entry(d).State = EntityState.Modified;
                }
                db.SaveChanges();

                return RedirectToAction("Details", "Vozaci", new { id = vd.IdVozac });
            }


            ViewBag.Plata = Plata;
            return View(dnevnice);
        }

        //
        // GET: /VozacDnevnica/Delete/5
        [Authorize]
        [SpedicijaAutorizacija(Roles = "admin")]
        public ActionResult Delete(int id = 0)
        {
            VozacDnevnica vozacdnevnica = db.VozacDnevnica.Find(id);
            if (vozacdnevnica == null)
            {
                return HttpNotFound();
            }
            return View(vozacdnevnica);
        }

        //
        // POST: /VozacDnevnica/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            VozacDnevnica vozacdnevnica = db.VozacDnevnica.Find(id);
            List<DnevnicaDetail> detail = db.DnevnicaDetail.Where(c => c.IdVozacDnevnica == vozacdnevnica.IdVozacDnevnica).ToList();
            int back = vozacdnevnica.IdVozac;

            foreach (DnevnicaDetail dt in detail)
            {
                db.DnevnicaDetail.Remove(dt);
            }
            db.SaveChanges();

            db.VozacDnevnica.Remove(vozacdnevnica);
            db.SaveChanges();
            return RedirectToAction("Details", "Vozaci", new { id = back });
        }


        public String MjesecPostoji(String mjesec, int IdVozac)
        {

            int m = Convert.ToInt32(mjesec.Split('.')[0]);
            int g = Convert.ToInt32(mjesec.Split('.')[1]);

            var dnevnica = db.VozacDnevnica.Where(c => c.IdVozac == IdVozac && c.Mjesec == m && c.Godina == g).ToList();
            
            return dnevnica.Count() == 0 ? "true" : "false";
        }

        [Authorize]
        [SpedicijaAutorizacija(Roles = "admin")]
        public JsonResult DnevnicaPlaceno(int IdVozacDnevnica, int period, int placeno)
        {
            var detail = db.DnevnicaDetail.Where(c => c.IdVozacDnevnica == IdVozacDnevnica).ToList();

            if (period == 0)
            {
                DnevnicaDetail dd = detail.Single(c => c.Datum.Day == 1);
                dd.Isplaceno = placeno == 1;
                db.Entry(dd).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                DnevnicaDetail dd = detail.Single(c => c.Datum.Day == 16);
                dd.Isplaceno = placeno == 1;
                db.Entry(dd).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}