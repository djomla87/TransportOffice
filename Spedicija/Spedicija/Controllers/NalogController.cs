using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Spedicija.Models;
using System.Drawing;
using Zen.Barcode;
using System.IO;



namespace Spedicija.Controllers
{
    public class NalogController : Controller
    {
        private uvhszjiy_spedicijaEntities db = new uvhszjiy_spedicijaEntities();

        //
        // GET: /Nalog/
        [Authorize]
        public ActionResult Index()
        {
            var nalogzautovar = db.NalogZaUtovar.Include(n => n.DnevnikPrevoza).Include(n => n.Subjekt);
            return View(nalogzautovar.ToList());
        }

		// 
        [Authorize]
	 public ActionResult IndexAjax(jQueryDataTableParamModel param)
	 {
		 var list = db.NalogZaUtovar.Include(n => n.DnevnikPrevoza).Include(n => n.Subjekt).ToList();
         IEnumerable<NalogZaUtovar> filteredList;

				var FIdNalog  = Convert.ToString(Request["sSearch_0"]).ToLower();
				var FBrojNaloga  = Convert.ToString(Request["sSearch_1"]).ToLower();
				var FIdSubjekt  = Convert.ToString(Request["sSearch_2"]).ToLower();
				var FIzvoznik  = Convert.ToString(Request["sSearch_3"]).ToLower();
				var FNapomena  = Convert.ToString(Request["sSearch_4"]).ToLower();
				var FVrstaRobe  = Convert.ToString(Request["sSearch_5"]).ToLower();
				var FBrutoTezina  = Convert.ToString(Request["sSearch_6"]).ToLower();
				var FIzvoznaCarina  = Convert.ToString(Request["sSearch_7"]).ToLower();
				var FIdDnevnik  = Convert.ToString(Request["sSearch_8"]).ToLower();
		
        filteredList = list.Where(c =>	
				  (String.IsNullOrEmpty("" + c.IdNalog ) ? "" :  "" +  c.IdNalog).ToLower().Contains(FIdNalog) 
				 &&   (String.IsNullOrEmpty("" + c.BrojNaloga ) ? "" :  "" +  c.BrojNaloga).ToLower().Contains(FBrojNaloga) 
				 &&   (String.IsNullOrEmpty("" + c.IdSubjekt ) ? "" :  "" +  c.IdSubjekt).ToLower().Contains(FIdSubjekt) 
				 &&   (String.IsNullOrEmpty("" + c.Izvoznik ) ? "" :  "" +  c.Izvoznik).ToLower().Contains(FIzvoznik) 
				 &&   (String.IsNullOrEmpty("" + c.Napomena ) ? "" :  "" +  c.Napomena).ToLower().Contains(FNapomena) 
				 &&   (String.IsNullOrEmpty("" + c.VrstaRobe ) ? "" :  "" +  c.VrstaRobe).ToLower().Contains(FVrstaRobe) 
				 &&   (String.IsNullOrEmpty("" + c.BrutoTezina ) ? "" :  "" +  c.BrutoTezina).ToLower().Contains(FBrutoTezina) 
				 &&   (String.IsNullOrEmpty("" + c.IzvoznaCarina ) ? "" :  "" +  c.IzvoznaCarina).ToLower().Contains(FIzvoznaCarina) 
				 &&   (String.IsNullOrEmpty("" + c.IdDnevnik ) ? "" :  "" +  c.IdDnevnik).ToLower().Contains(FIdDnevnik) 
		                                  );


            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredList = filteredList.Where(c =>  
											  (String.IsNullOrEmpty("" + c.IdNalog ) ? "" :  "" +  c.IdNalog).ToLower().Contains(param.sSearch.ToLower()) 
		
											 ||   (String.IsNullOrEmpty("" + c.BrojNaloga ) ? "" :  "" +  c.BrojNaloga).ToLower().Contains(param.sSearch.ToLower()) 
		
											 ||   (String.IsNullOrEmpty("" + c.IdSubjekt ) ? "" :  "" +  c.IdSubjekt).ToLower().Contains(param.sSearch.ToLower()) 
		
											 ||   (String.IsNullOrEmpty("" + c.Izvoznik ) ? "" :  "" +  c.Izvoznik).ToLower().Contains(param.sSearch.ToLower()) 
		
											 ||   (String.IsNullOrEmpty("" + c.Napomena ) ? "" :  "" +  c.Napomena).ToLower().Contains(param.sSearch.ToLower()) 
		
											 ||   (String.IsNullOrEmpty("" + c.VrstaRobe ) ? "" :  "" +  c.VrstaRobe).ToLower().Contains(param.sSearch.ToLower()) 
		
											 ||   (String.IsNullOrEmpty("" + c.BrutoTezina ) ? "" :  "" +  c.BrutoTezina).ToLower().Contains(param.sSearch.ToLower()) 
		
											 ||   (String.IsNullOrEmpty("" + c.IzvoznaCarina ) ? "" :  "" +  c.IzvoznaCarina).ToLower().Contains(param.sSearch.ToLower()) 
		
											 ||   (String.IsNullOrEmpty("" + c.IdDnevnik ) ? "" :  "" +  c.IdDnevnik).ToLower().Contains(param.sSearch.ToLower()) 
		
																	);

            }

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);

            Func<NalogZaUtovar, object> orderingFunction = c =>
            {
                switch (sortColumnIndex)
                {
				 					case 0: return c.IdNalog;
									case 1: return c.BrojNaloga;
									case 2: return c.IdSubjekt;
									case 3: return c.Izvoznik;
									case 4: return c.Napomena;
									case 5: return c.VrstaRobe;
									case 6: return c.BrutoTezina;
									case 7: return c.IzvoznaCarina;
									case 8: return c.IdDnevnik;
									default: return c.IdNalog;
                }
			};

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredList = filteredList.OrderBy(orderingFunction);
            else
                filteredList = filteredList.OrderByDescending(orderingFunction);

            var displayedList = filteredList.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedList select new object[] { 
			 				c.IdNalog,
							c.BrojNaloga,
							c.IdSubjekt,
							c.Izvoznik,
							c.Napomena,
							c.VrstaRobe,
							c.BrutoTezina,
							c.IzvoznaCarina,
							c.IdDnevnik,
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
        // GET: /Nalog/Details/5
        [Authorize]
        public ActionResult Details(int id = 0)
        {
            NalogZaUtovar nalogzautovar = db.NalogZaUtovar.Find(id);
            if (nalogzautovar == null)
            {
                return HttpNotFound();
            }
            return View(nalogzautovar);
        }

        //
        // GET: /Nalog/Create
        [Authorize]
        public ActionResult Create(int id = 0)
        {
            /*
            String godina = "/" + DateTime.Today.Year.ToString().Substring(2, 2);

            var nalozi = db.NalogZaUtovar.Where(c => c.BrojNaloga.EndsWith(godina)).ToList().Select(c => new { Broj = Convert.ToInt32( c.BrojNaloga.Replace(godina,""))});

            string nalog = "";

            if (nalozi.Count() == 0)
                nalog = "1" + godina;
            else
                nalog = (nalozi.Max(c => c.Broj) + 1).ToString() + godina;

            */
            ViewBag.BrojNaloga = db.DnevnikPrevoza.Find(id).SerijskiBroj;
            ViewBag.IdDnevnik = id;
            ViewBag.IdSubjekt = new SelectList(db.Subjekt, "IdSubjekt", "Naziv");
            return View();
        }

        //
        // POST: /Nalog/Create

        [HttpPost]
        public ActionResult Create(NalogZaUtovar nalogzautovar)
        {
            if (ModelState.IsValid)
            {
                db.NalogZaUtovar.Add(nalogzautovar);
                db.SaveChanges();
                return RedirectToAction("Details", "DnevnikPrevoza", new { id = nalogzautovar.IdDnevnik });
            }



            /*
            String godina = "/" + DateTime.Today.Year.ToString().Substring(2, 2);

            var nalozi = db.NalogZaUtovar.Where(c => c.BrojNaloga.EndsWith(godina)).ToList().Select(c => new { Broj = Convert.ToInt32(c.BrojNaloga.Replace(godina, "")) });

            string nalog = "";

            if (nalozi.Count() == 0)
                nalog = "1" + godina;
            else
                nalog = (nalozi.Max(c => c.Broj) + 1).ToString() + godina;

*/
            ViewBag.BrojNaloga = db.DnevnikPrevoza.Find(nalogzautovar.IdDnevnik).SerijskiBroj;
            
            ViewBag.IdDnevnik = nalogzautovar.IdDnevnik;
            ViewBag.IdSubjekt = new SelectList(db.Subjekt, "IdSubjekt", "Naziv");
            return View(nalogzautovar);
        }

        //
        // GET: /Nalog/Edit/5
        [Authorize]
        [SpedicijaAutorizacija(Roles = "admin")]
        public ActionResult Edit(int id = 0)
        {
            NalogZaUtovar nalogzautovar = db.NalogZaUtovar.Find(id);
            if (nalogzautovar == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdDnevnik = new SelectList(db.DnevnikPrevoza, "IdDnevnik", "BrojNaloga", nalogzautovar.IdDnevnik);
            ViewBag.IdSubjekt = new SelectList(db.Subjekt, "IdSubjekt", "Naziv", nalogzautovar.IdSubjekt);
            return View(nalogzautovar);
        }

        //
        // POST: /Nalog/Edit/5

        [HttpPost]
        public ActionResult Edit(NalogZaUtovar nalogzautovar)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nalogzautovar).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "DnevnikPrevoza", new { id = nalogzautovar.IdDnevnik });
            }
            ViewBag.IdDnevnik = new SelectList(db.DnevnikPrevoza, "IdDnevnik", "BrojNaloga", nalogzautovar.IdDnevnik);
            ViewBag.IdSubjekt = new SelectList(db.Subjekt, "IdSubjekt", "Naziv", nalogzautovar.IdSubjekt);
            return View(nalogzautovar);
        }

        //
        // GET: /Nalog/Delete/5
        [Authorize]
        [SpedicijaAutorizacija(Roles = "admin")]
        public ActionResult Delete(int id = 0)
        {
            NalogZaUtovar nalogzautovar = db.NalogZaUtovar.Find(id);
            if (nalogzautovar == null)
            {
                return HttpNotFound();
            }
            return View(nalogzautovar);
        }

        //
        // POST: /Nalog/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            NalogZaUtovar nalogzautovar = db.NalogZaUtovar.Find(id);
            db.NalogZaUtovar.Remove(nalogzautovar);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult Report(int id = 0, String Lan = "SR")
        {
            ViewData["IdDnevnik"] = id;
            ViewData["Korisnik"] = db.Korisnik.Single(c => c.KorisnickoIme.Equals(HttpContext.User.Identity.Name)).Ime;
            ViewData["Jezik"] = Lan;
            

            var dnevnik = db.DnevnikPrevoza.Find(id);
            String SerijskiBroj = dnevnik.SerijskiBroj;

            String kobasica = dnevnik.GostPristup;

            using (System.Drawing.Image imagen = BarcodeDrawFactory.GetSymbology(BarcodeSymbology.CodeQr).Draw(AppSettings.GetSettings()["domain_name"]+"/DnevnikPrevoza/GuestDetail?s=" + kobasica, 50, 2))
         
            {
                String path = Server.MapPath("~/BARCODE/" + SerijskiBroj + ".png");
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);

                imagen.Save(path, System.Drawing.Imaging.ImageFormat.Png);

                ViewData["BarCode"] = path;

            } 


           

            return View();
        }

        [Authorize]
        public ActionResult ReportDnevnik(int id = 0)
        {
            ViewData["IdDnevnik"] = id;

            var dnevnik = db.DnevnikPrevoza.Find(id);
            String SerijskiBroj = dnevnik.SerijskiBroj;

            String kobasica = dnevnik.GostPristup;


            using (System.Drawing.Image imagen = BarcodeDrawFactory.GetSymbology(BarcodeSymbology.CodeQr).Draw(AppSettings.GetSettings()["domain_name"]+"/DnevnikPrevoza/GuestDetail?s=" + kobasica, 50, 2))
            {
                String path = Server.MapPath("~/BARCODE/" + SerijskiBroj + ".png");
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);

                imagen.Save(path, System.Drawing.Imaging.ImageFormat.Png);

                ViewData["BarCode"] = path;

            } 

            return View();
        }

        [Authorize]
        public ActionResult ReportDnevnikNovi(int id = 0)
        {
            ViewData["IdDnevnik"] = id;

            var dnevnik = db.DnevnikPrevoza.Find(id);
            String SerijskiBroj = dnevnik.SerijskiBroj;

            String kobasica = dnevnik.GostPristup;


            using (System.Drawing.Image imagen = BarcodeDrawFactory.GetSymbology(BarcodeSymbology.CodeQr).Draw(AppSettings.GetSettings()["domain_name"]+"/DnevnikPrevoza/GuestDetail?s=" + kobasica, 50, 2))
            {
                String path = Server.MapPath("~/BARCODE/" + SerijskiBroj + ".png");
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);

                imagen.Save(path, System.Drawing.Imaging.ImageFormat.Png);

                ViewData["BarCode"] = path;

            }

            return View();
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}