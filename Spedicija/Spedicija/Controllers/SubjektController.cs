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
    public class SubjektController : Controller
    {
        private uvhszjiy_spedicijaEntities db = new uvhszjiy_spedicijaEntities();

        //
        // GET: /Subjekt/
        [Authorize]
        public ActionResult Index()
        {
            // return View(db.Subjekt.ToList());
			 return View(new List<Subjekt>());
        }

		// 
        [Authorize]
	     public ActionResult IndexAjax(jQueryDataTableParamModel param)
	 {
		 var list = db.Subjekt.Where(c => c.ZapisAktivan).ToList();
         IEnumerable<Subjekt> filteredList;

				var FIdSubjekt  = Convert.ToString(Request["sSearch_0"]).ToLower();
				var FNaziv  = Convert.ToString(Request["sSearch_1"]).ToLower();
				var FAdresa  = Convert.ToString(Request["sSearch_2"]).ToLower();
				var FTelefon  = Convert.ToString(Request["sSearch_3"]).ToLower();
				var FEmail  = Convert.ToString(Request["sSearch_4"]).ToLower();
				var FKontaktOsoba  = Convert.ToString(Request["sSearch_5"]).ToLower();
				var FTimocom  = Convert.ToString(Request["sSearch_6"]).ToLower();
				var FDatumKreiranja  = Convert.ToString(Request["sSearch_7"]).ToLower();
                var FPIB = Convert.ToString(Request["sSearch_8"]).ToLower();
		
        filteredList = list.Where(c =>	
				  (String.IsNullOrEmpty("" + c.RedniBroj) ? "" :  "" +  c.RedniBroj).ToLower().Contains(FIdSubjekt) 
				 &&   (String.IsNullOrEmpty("" + c.Naziv ) ? "" :  "" +  c.Naziv).ToLower().Contains(FNaziv) 
				 &&   (String.IsNullOrEmpty("" + c.Adresa ) ? "" :  "" +  c.Adresa).ToLower().Contains(FAdresa) 
				 &&   (String.IsNullOrEmpty("" + c.Telefon ) ? "" :  "" +  c.Telefon).ToLower().Contains(FTelefon) 
				 &&   (String.IsNullOrEmpty("" + c.Email ) ? "" :  "" +  c.Email).ToLower().Contains(FEmail) 
				 &&   (String.IsNullOrEmpty("" + c.KontaktOsoba ) ? "" :  "" +  c.KontaktOsoba).ToLower().Contains(FKontaktOsoba) 
				 &&   (String.IsNullOrEmpty("" + c.Timocom ) ? "" :  "" +  c.Timocom).ToLower().Contains(FTimocom) 
				 &&   (String.IsNullOrEmpty("" + c.DatumKreiranja ) ? "" :  "" +  c.DatumKreiranja).ToLower().Contains(FDatumKreiranja)
                 && (String.IsNullOrEmpty("" + c.PIB) ? "" : "" + c.PIB).ToLower().Contains(FPIB) 
		                                  );


            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredList = filteredList.Where(c =>  
											  (String.IsNullOrEmpty("" + c.RedniBroj) ? "" :  "" +  c.RedniBroj).ToLower().Contains(param.sSearch.ToLower()) 
											 ||   (String.IsNullOrEmpty("" + c.Naziv ) ? "" :  "" +  c.Naziv).ToLower().Contains(param.sSearch.ToLower()) 
											 ||   (String.IsNullOrEmpty("" + c.Adresa ) ? "" :  "" +  c.Adresa).ToLower().Contains(param.sSearch.ToLower()) 
											 ||   (String.IsNullOrEmpty("" + c.Telefon ) ? "" :  "" +  c.Telefon).ToLower().Contains(param.sSearch.ToLower()) 
											 ||   (String.IsNullOrEmpty("" + c.Email ) ? "" :  "" +  c.Email).ToLower().Contains(param.sSearch.ToLower()) 
											 ||   (String.IsNullOrEmpty("" + c.KontaktOsoba ) ? "" :  "" +  c.KontaktOsoba).ToLower().Contains(param.sSearch.ToLower()) 
											 ||   (String.IsNullOrEmpty("" + c.Timocom ) ? "" :  "" +  c.Timocom).ToLower().Contains(param.sSearch.ToLower()) 
											 ||   (String.IsNullOrEmpty("" + c.DatumKreiranja ) ? "" :  "" +  c.DatumKreiranja).ToLower().Contains(param.sSearch.ToLower())
                                             ||   (String.IsNullOrEmpty("" + c.PIB) ? "" : "" + c.PIB).ToLower().Contains(param.sSearch.ToLower()) 
																	);

            }

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);

            Func<Subjekt, object> orderingFunction = c =>
            {
                switch (sortColumnIndex)
                {
				 					case 0: return c.RedniBroj;
									case 1: return c.Naziv;
									case 2: return c.Adresa;
									case 3: return c.Telefon;
									case 4: return c.Email;
									case 5: return c.KontaktOsoba;
									case 6: return c.Timocom;
									case 7: return c.DatumKreiranja;
                                    case 8: return c.PIB; 
									default: return c.RedniBroj;
                }
			};

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredList = filteredList.OrderBy(orderingFunction);
            else
                filteredList = filteredList.OrderByDescending(orderingFunction);

            var displayedList = filteredList.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedList select new object[] { 
			 				c.RedniBroj,
							c.Naziv,
							c.Adresa + " - " + c.PTT + " " + c.Grad,
							c.Telefon,
							c.Email,
							c.KontaktOsoba,
							c.Timocom,
							String.IsNullOrEmpty("" + c.DatumKreiranja) ? "" :   DateTime.Parse(c.DatumKreiranja.ToString()).ToString("dd.MM.yyyy"),
                            c.PIB,
								c.IdSubjekt
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
        // GET: /Subjekt/Details/5
        [Authorize]
        public ActionResult Details(int id = 0)
        {
            Subjekt subjekt = db.Subjekt.Find(id);
            if (subjekt == null)
            {
                return HttpNotFound();
            }
            return View(subjekt);
        }

        //
        // GET: /Subjekt/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Subjekt/Create

        [HttpPost]
        public ActionResult Create(Subjekt subjekt)
        {

            int dupli = db.Subjekt.Where(c => c.PIB.Equals(subjekt.PIB)).Count();
            if (dupli > 0) ModelState.AddModelError("PIB", "Ovaj PIB već postoji u sistemu");

            try
            {
                int RB = db.Subjekt.Max(c => c.RedniBroj ?? 0);
                subjekt.RedniBroj = RB + 1;
            }
            catch (Exception ex)
            {
                subjekt.RedniBroj = 1;
            }

            if (ModelState.IsValid)
            {
                subjekt.DatumKreiranja = DateTime.Today.AddHours(7);
                subjekt.DatumZapisa = DateTime.Now.AddHours(7);
                subjekt.ZapisAktivan = true;

                db.Subjekt.Add(subjekt);
                db.SaveChanges();


                List<KorisnikNalog> kn = db.KorisnikNalog.Where(c => !c.IdSubjekt.HasValue && c.Naziv.Equals(subjekt.Naziv)).ToList();

                foreach (KorisnikNalog k in kn)
                {
                    k.IdSubjekt = subjekt.IdSubjekt;
                    db.Entry(k).State = EntityState.Modified;
                }

                db.SaveChanges();


                return RedirectToAction("Index");
            }

            return View(subjekt);
        }

        //
        // GET: /Subjekt/Edit/5
        [Authorize]
        [SpedicijaAutorizacija(Roles = "admin")]
        public ActionResult Edit(int id = 0)
        {
            Subjekt subjekt = db.Subjekt.Find(id);
            if (subjekt == null)
            {
                return HttpNotFound();
            }
            return View(subjekt);
        }

        //
        // POST: /Subjekt/Edit/5

        [HttpPost]
        public ActionResult Edit(Subjekt subjekt)
        {
            if (ModelState.IsValid)
            {
                subjekt.DatumZapisa = DateTime.Now.AddHours(7);
                subjekt.ZapisAktivan = true;

                db.Entry(subjekt).State = EntityState.Modified;
                db.SaveChanges();


                return RedirectToAction("Index");
            }
            return View(subjekt);
        }

        //
        // GET: /Subjekt/Delete/5
        [Authorize]
        [SpedicijaAutorizacija(Roles = "admin")]
        public ActionResult Delete(int id = 0)
        {
            Subjekt subjekt = db.Subjekt.Find(id);
            if (subjekt == null)
            {
                return HttpNotFound();
            }
            return View(subjekt);
        }

        //
        // POST: /Subjekt/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Subjekt subjekt = db.Subjekt.Find(id);
            db.Subjekt.Remove(subjekt);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize]
        public JsonResult Search(String searchString)
        {

            if (!String.IsNullOrEmpty(searchString))
            {
                var stranke = db.Subjekt.Where(c => c.ZapisAktivan).Select(c => new { IdSubjekt = c.IdSubjekt, Naziv = c.Naziv, KontaktOsoba = c.KontaktOsoba == null ? "" : c.KontaktOsoba, Timocom = c.Timocom, Telefon = c.Telefon }).ToList().Where(
                    c => Convert.ToString(c.Naziv ).ToLower().Contains(searchString.ToLower()) ||
                         Convert.ToString(c.KontaktOsoba == null ? "" : c.KontaktOsoba).ToLower().Contains(searchString.ToLower()) ||
                         Convert.ToString(c.Timocom == null ? "" : c.Timocom).ToLower().Contains(searchString.ToLower())
                         );

                TempData["contactSearch"] = searchString;

                return Json(stranke, JsonRequestBehavior.AllowGet);
            }
            else return Json(null, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult Card(int id = 0)
        {
            if (TempData["contactSearch"] != null)
                ViewBag.contactSearch = TempData["contactSearch"].ToString();
            else
                ViewBag.contactSearch = "";

            
            Subjekt subjekt = db.Subjekt.Include(d => d.DnevnikPrevoza).Include(d => d.DnevnikPrevoza1).Where(c => c.IdSubjekt == id).FirstOrDefault();
            if (id == 0) subjekt = db.Subjekt.FirstOrDefault();

            return View(subjekt);
        }

        [Authorize]
        public JsonResult MailoviSubjekata()
        {
           var lst = db.Subjekt.Where(c => c.ZapisAktivan && !((c.Email + "").Equals(""))).Select(c => new { Email = c.Email }).ToList();
           
            string MailingLista = "";

            foreach (var item in lst)
            {
                MailingLista += item.Email + ";";
            }
            
            return Json(MailingLista, JsonRequestBehavior.AllowGet);

        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}