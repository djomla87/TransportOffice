using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Spedicija.Models;

using Spedicija.Repository;


namespace Spedicija.Controllers
{
    public class VozaciController : Controller
    {
        private uvhszjiy_spedicijaEntities db = new uvhszjiy_spedicijaEntities();
    //    private Spedicija.Repository.Spedicija sp = new Repository.Spedicija(new uvhszjiy_spedicijaEntities());

        //
        // GET: /Vozaci/
        [Authorize]
        public ActionResult Index()
        {
           
            return View(new List<Vozaci>());
        }

		// 
        [Authorize]
	    public ActionResult IndexAjax(jQueryDataTableParamModel param)
	 {
		 var list = db.Vozaci.Include(v => v.Subjekt).ToList();
         IEnumerable<Vozaci> filteredList;

				var FIdVozac  = Convert.ToString(Request["sSearch_0"]).ToLower();
				var FImeVozaca  = Convert.ToString(Request["sSearch_1"]).ToLower();
				var FTelefon  = Convert.ToString(Request["sSearch_2"]).ToLower();
				var FAdresa  = Convert.ToString(Request["sSearch_3"]).ToLower();
				var FBrojPasosa  = Convert.ToString(Request["sSearch_4"]).ToLower();
				var FJMBG  = Convert.ToString(Request["sSearch_5"]).ToLower();
				var FBrojLK  = Convert.ToString(Request["sSearch_6"]).ToLower();
				var FBrojVozackeDozvole  = Convert.ToString(Request["sSearch_7"]).ToLower();
				var FIdSubjekt  = Convert.ToString(Request["sSearch_8"]).ToLower();
            var FNapomena = Convert.ToString(Request["sSearch_9"]).ToLower();
            var FAktivan = Convert.ToString(Request["sSearch_10"]).ToLower();

            filteredList = list.Where(c =>	
				  (String.IsNullOrEmpty("" + c.IdVozac ) ? "" :  "" +  c.IdVozac).ToLower().Contains(FIdVozac) 
				 &&   (String.IsNullOrEmpty("" + c.ImeVozaca ) ? "" :  "" +  c.ImeVozaca).ToLower().Contains(FImeVozaca) 
				 &&   (String.IsNullOrEmpty("" + c.Telefon ) ? "" :  "" +  c.Telefon).ToLower().Contains(FTelefon) 
				 &&   (String.IsNullOrEmpty("" + c.Adresa ) ? "" :  "" +  c.Adresa).ToLower().Contains(FAdresa) 
				 &&   (String.IsNullOrEmpty("" + c.BrojPasosa ) ? "" :  "" +  c.BrojPasosa).ToLower().Contains(FBrojPasosa) 
				 &&   (String.IsNullOrEmpty("" + c.JMBG ) ? "" :  "" +  c.JMBG).ToLower().Contains(FJMBG) 
				 &&   (String.IsNullOrEmpty("" + c.BrojLK ) ? "" :  "" +  c.BrojLK).ToLower().Contains(FBrojLK) 
				 &&   (String.IsNullOrEmpty("" + c.BrojVozackeDozvole ) ? "" :  "" +  c.BrojVozackeDozvole).ToLower().Contains(FBrojVozackeDozvole) 
				 &&   (String.IsNullOrEmpty("" + c.Subjekt.Naziv ) ? "" :  "" +  c.Subjekt.Naziv).ToLower().Contains(FIdSubjekt)
                  && (String.IsNullOrEmpty("" + c.Napomene) ? "" : "" + c.Napomene).ToLower().Contains(FNapomena)
                 &&   ((c.ZapisAktivan ?? false ) ? "DA" : "NE").ToLower().Contains(FAktivan)
                                          );


            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredList = filteredList.Where(c =>  
											  (String.IsNullOrEmpty("" + c.IdVozac ) ? "" :  "" +  c.IdVozac).ToLower().Contains(param.sSearch.ToLower()) 	
											 ||   (String.IsNullOrEmpty("" + c.ImeVozaca ) ? "" :  "" +  c.ImeVozaca).ToLower().Contains(param.sSearch.ToLower()) 
											 ||   (String.IsNullOrEmpty("" + c.Telefon ) ? "" :  "" +  c.Telefon).ToLower().Contains(param.sSearch.ToLower()) 
											 ||   (String.IsNullOrEmpty("" + c.Adresa ) ? "" :  "" +  c.Adresa).ToLower().Contains(param.sSearch.ToLower()) 
											 ||   (String.IsNullOrEmpty("" + c.BrojPasosa ) ? "" :  "" +  c.BrojPasosa).ToLower().Contains(param.sSearch.ToLower()) 
											 ||   (String.IsNullOrEmpty("" + c.JMBG ) ? "" :  "" +  c.JMBG).ToLower().Contains(param.sSearch.ToLower()) 
											 ||   (String.IsNullOrEmpty("" + c.BrojLK ) ? "" :  "" +  c.BrojLK).ToLower().Contains(param.sSearch.ToLower()) 
											 ||   (String.IsNullOrEmpty("" + c.BrojVozackeDozvole ) ? "" :  "" +  c.BrojVozackeDozvole).ToLower().Contains(param.sSearch.ToLower()) 
											 ||   (String.IsNullOrEmpty("" + c.Subjekt.Naziv ) ? "" :  "" +  c.Subjekt.Naziv).ToLower().Contains(param.sSearch.ToLower())
                                             ||   (String.IsNullOrEmpty("" + c.Napomene) ? "" : "" + c.Napomene).ToLower().Contains(param.sSearch.ToLower())
                                             ||   ((c.ZapisAktivan ?? false) ? "DA" : "NE").ToLower().Contains(param.sSearch.ToLower())
                                                                    );

            }

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);

            Func<Vozaci, object> orderingFunction = c =>
            {
                switch (sortColumnIndex)
                {
				 					case 0: return c.IdVozac;
									case 1: return c.ImeVozaca;
									case 2: return c.Telefon;
									case 3: return c.Adresa;
									case 4: return c.BrojPasosa;
									case 5: return c.JMBG;
									case 6: return c.BrojLK;
									case 7: return c.BrojVozackeDozvole;
									case 8: return c.Subjekt.Naziv;
                                    case 9: return c.Napomene;
                                     case 10: return (c.ZapisAktivan ?? false) ? "DA" : "NE";
									default: return c.IdVozac;
                }
			};

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredList = filteredList.OrderBy(orderingFunction);
            else
                filteredList = filteredList.OrderByDescending(orderingFunction);

            var displayedList = filteredList.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedList select new object[] { 
			 				c.IdVozac,
							c.ImeVozaca,
							c.Telefon,
							c.Adresa,
							c.BrojPasosa,
							c.JMBG,
							c.BrojLK,
							c.BrojVozackeDozvole,
							c.Subjekt.Naziv,
                            c.Napomene,
                            (c.ZapisAktivan ?? false) ? "DA" : "NE",

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
        public ActionResult PrevoziVozaca(jQueryDataTableParamModel param, int IdVozac)
        {
            var list = db.DnevnikPrevoza.Where(c => c.IdVozac == IdVozac).ToList();
            IEnumerable<DnevnikPrevoza> filteredList;

            var FOD = Convert.ToString(Request["sSearch_0"]).ToLower();
            var FDO = Convert.ToString(Request["sSearch_1"]).ToLower();
            var FUtovar = Convert.ToString(Request["sSearch_2"]).ToLower();
            var FIstovar = Convert.ToString(Request["sSearch_3"]).ToLower();
            var FVozilo = Convert.ToString(Request["sSearch_4"]).ToLower();

            filteredList = list.Where(c =>
                        ( c.UtovarFirma +" - "+ c.UtovarGrad).ToLower().Contains(FOD)
                     && (c.IstovarFirma + " - " + c.IstovarGrad).ToLower().Contains(FDO)
                     && (c.DatumUtovara.HasValue ? c.DatumUtovara.Value.ToShortDateString() : "").ToLower().Contains(FDO)
                     && (c.DatumIstovara.HasValue ? c.DatumIstovara.Value.ToShortDateString() : "").ToLower().Contains(FDO)
                      && (c.Vozilo == null ? "" : c.Vozilo.VrstaVozila + " " + c.Vozilo.RegistarskiBroj).ToLower().Contains(FDO)
                                              );


            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredList = filteredList.Where(c =>
                        (c.UtovarFirma + " - " + c.UtovarGrad).ToLower().Contains(param.sSearch.ToLower())
                     || (c.IstovarFirma + " - " + c.IstovarGrad).ToLower().Contains(FDO)
                     || (c.DatumUtovara.HasValue ? c.DatumUtovara.Value.ToShortDateString() : "").ToLower().Contains(param.sSearch.ToLower())
                     || (c.DatumIstovara.HasValue ? c.DatumIstovara.Value.ToShortDateString() : "").ToLower().Contains(param.sSearch.ToLower())
                     || (c.Vozilo == null ? "" : c.Vozilo.VrstaVozila + " " + c.Vozilo.RegistarskiBroj).ToLower().Contains(param.sSearch.ToLower())


                                                                    );

            }

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);

            Func<DnevnikPrevoza, object> orderingFunction = c =>
            {
                switch (sortColumnIndex)
                {
                    case 0: return c.UtovarFirma;
                    case 1: return c.IstovarFirma;
                    case 2: return c.DatumUtovara;
                    case 3: return c.DatumIstovara;
                    case 4: return c.Vozilo.VrstaVozila;
                    default: return c.DatumUtovara;
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
			 				c.UtovarFirma + " - " + c.UtovarGrad,
                            c.IstovarFirma + " - " + c.IstovarGrad,
                            c.DatumUtovara.HasValue ? c.DatumUtovara.Value.ToShortDateString() : "",
                            c.DatumIstovara.HasValue ? c.DatumIstovara.Value.ToShortDateString() : "",
                            c.Vozilo == null ? "" : c.Vozilo.VrstaVozila + " " + c.Vozilo.RegistarskiBroj,
						    c.IdDnevnik
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
        // GET: /Vozaci/Details/5
        [Authorize]
        public ActionResult Details(int id = 0)
        {
            Vozaci vozaci = db.Vozaci.Find(id);
            ViewBag.Dokumenti = db.Dokument.Where(c => c.IdVozac == id).ToList();
            ViewBag.KorisnickoIme = (from a in db.Korisnik
                                    join b in db.VozacUser on a.IdKorisnik equals b.IdUser
                                    where b.IdVozac == id
                                    select a.KorisnickoIme).SingleOrDefault();


            if (vozaci == null)
            {
                return HttpNotFound();
            }
            return View(vozaci);
        }

        //
        // GET: /Vozaci/Create
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.IdSubjekt = new SelectList(db.Subjekt, "IdSubjekt", "Naziv");
            return View();
        }

        //
        // POST: /Vozaci/Create

        [HttpPost]
        public ActionResult Create(Vozaci vozaci)
        {

            String username = "";
            if (Request["KorisnickoIme"] != null)
                username = Request["KorisnickoIme"].ToString();

            Korisnik k = db.Korisnik.Where(c => c.KorisnickoIme.Equals(username)).SingleOrDefault();

            if (k == null && !username.Equals(""))
                ModelState.AddModelError("KorisnickoIme", "Ne postoji unešeno korisničko ime");

            if (ModelState.IsValid)
            {
                vozaci.ZapisAktivan = true;
                db.Vozaci.Add(vozaci);

               
                    if (!username.Equals(""))
                    {                   
                        VozacUser vu = new VozacUser();
                        vu.IdUser = k.IdKorisnik;
                        vu.IdVozac = vozaci.IdVozac;

                        db.VozacUser.Add(vu);
                        
                     }
                

                db.SaveChanges();

                return RedirectToAction("Index");
            }




            var subjekti = db.Subjekt.Where(c => c.ZapisAktivan);

            ViewBag.IdSubjekt = new SelectList(subjekti, "IdSubjekt", "Naziv", vozaci.IdSubjekt);
            return View(vozaci);
        }

        //
        // GET: /Vozaci/Edit/5
        [Authorize]
        [SpedicijaAutorizacija(Roles = "admin")]
        public ActionResult Edit(int id = 0)
        {
            Vozaci vozaci = db.Vozaci.Find(id);
            if (vozaci == null)
            {
                return HttpNotFound();
            }
            var subjekti = db.Subjekt.Where(c => c.ZapisAktivan);
            ViewBag.IdSubjekt = new SelectList(subjekti, "IdSubjekt", "Naziv", vozaci.IdSubjekt);


            ViewBag.KorisnickoIme = (from a in db.Korisnik
                                     join b in db.VozacUser on a.IdKorisnik equals b.IdUser
                                     where b.IdVozac == id
                                     select a.KorisnickoIme).SingleOrDefault();



            return View(vozaci);
        }

        //
        // POST: /Vozaci/Edit/5

        [HttpPost]
        public ActionResult Edit(Vozaci vozaci)
        {

            String username = "";
            if (Request["KorisnickoIme"] != null)
                username = Request["KorisnickoIme"].ToString();

            Korisnik k = db.Korisnik.Where(c => c.KorisnickoIme.Equals(username)).SingleOrDefault();

            if (k == null && !username.Equals(""))
                ModelState.AddModelError("KorisnickoIme", "Ne postoji unešeno korisničko ime");


            if (ModelState.IsValid)
            {

                if (Request["_ZapisAktivan"] != null)
                {
                    if (Request["_ZapisAktivan"].ToString().Equals("on"))
                        vozaci.ZapisAktivan = true;
                    else
                        vozaci.ZapisAktivan = false;
                }
                else vozaci.ZapisAktivan = false;

                db.Entry(vozaci).State = EntityState.Modified;

                if (!username.Equals(""))
                {

                    VozacUser vu = db.VozacUser.Where(c => c.IdVozac == vozaci.IdVozac).SingleOrDefault();
                    if (vu == null)
                    {
                        vu = new VozacUser();
                        vu.IdUser = k.IdKorisnik;
                        vu.IdVozac = vozaci.IdVozac;

                        db.VozacUser.Add(vu);
                    }
                    else
                    {
                        vu.IdUser = k.IdKorisnik;
                        db.Entry(vu).State = EntityState.Modified;
                    }

                   
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            var subjekti = db.Subjekt.Where(c => c.ZapisAktivan);
            ViewBag.IdSubjekt = new SelectList(subjekti, "IdSubjekt", "Naziv", vozaci.IdSubjekt);
            return View(vozaci);
        }

        //
        // GET: /Vozaci/Delete/5
        [Authorize]
        [SpedicijaAutorizacija(Roles = "admin")]
        public ActionResult Delete(int id = 0)
        {
            Vozaci vozaci = db.Vozaci.Find(id);
            if (vozaci == null)
            {
                return HttpNotFound();
            }
            return View(vozaci);
        }

        //
        // POST: /Vozaci/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Vozaci vozaci = db.Vozaci.Find(id);
            db.Vozaci.Remove(vozaci);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase file, int IdVozac)
        {

            for (int i = 0; i < Request.Files.Count; i++)
            {

                string fn = Request.Files[i].FileName;
                string path = Server.MapPath("~/Dokumenti/" + fn);
                while (System.IO.File.Exists(path))
                {
                    fn = "novi_" + fn;
                    path = Server.MapPath("~/Dokumenti/" + fn);
                    // System.IO.File.Delete(path);
                    // Dokument dok = db.Dokument.Where(c => c.Putanja.Equals(path)).FirstOrDefault();
                    // db.Dokument.Remove(dok);
                    // db.SaveChanges();
                }
                Request.Files[i].SaveAs(path);

                Dokument dokument = new Dokument();

                dokument.IdVozac = IdVozac;
                dokument.Putanja = path;
                dokument.Naziv = path.Substring(path.LastIndexOf('\\') + 1, path.Length - path.LastIndexOf('\\') - 1);
                dokument.Tip = path.Substring(path.LastIndexOf('.') + 1, path.Length - path.LastIndexOf('.') - 1);
                dokument.Velicina = (new System.IO.FileInfo(path).Length / 1000).ToString("0.00");
                db.Dokument.Add(dokument);
                db.SaveChanges();

            }
            return Json("OK", JsonRequestBehavior.AllowGet);

        }


        [Authorize]
        [SpedicijaAutorizacija(Roles = "admin")]
        public ActionResult DeleteDokument(int id = 0)
        {

            Dokument dokument = db.Dokument.Find(id);
            int back = dokument.IdVozac ?? 0;
            string path = Server.MapPath("~/Dokumenti/" + dokument.Naziv);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
                Dokument dok = db.Dokument.Where(c => c.Putanja.Equals(path)).FirstOrDefault();
                db.Dokument.Remove(dok);
                db.SaveChanges();
            }

            return RedirectToAction("Details", new { id = back });
        }



        public JsonResult VratiAktivneVozaceOsim(int IdVozac) {

            return Json(db.Vozaci.Where(c => (c.ZapisAktivan ?? false)).Select(k => new { key = k.IdVozac, value = k.ImeVozaca } ).ToList(), JsonRequestBehavior.AllowGet);
        }


        public JsonResult PromjeniVozaca(int IdDnevnik, int IdVozilo, int IdVozac)
        {
            try
            {
                DnevnikPrevoza dp = db.DnevnikPrevoza.Find(IdDnevnik);
                AndroidTask at = db.AndroidTask.FirstOrDefault(c => c.IdDnevnik == IdDnevnik);

                IDnevnikPrevozaVozac idpv = new IDnevnikPrevozaVozac {
                    IdDnevnik = IdDnevnik,
                    IdVozilo = dp.IdVozilo,
                    DatumDo = DateTime.Now.AddHours(7),
                    DatumOd = (dp.DatumDnevnika ?? DateTime.MinValue),
                    IdVozac = dp.IdVozac ?? 1,
                    DatumZapisa = DateTime.Now.AddHours(7)
                };
                db.IDnevnikPrevozaVozac.Add(idpv);

                dp.IdVozac = IdVozac;
                dp.IdVozilo = IdVozilo;
                dp.DatumZapisa = DateTime.Now.AddHours(7);
                db.Entry(dp).State = EntityState.Modified;

                at.VozacPregled = false;
                db.Entry(at).State = EntityState.Modified;

                db.SaveChanges();
            }
            catch (Exception ex) {
                return Json(new { response = "ERROR", message = ex.Message } , JsonRequestBehavior.AllowGet);
            }

            return Json(new { response = "OK", message = "OK" }, JsonRequestBehavior.AllowGet);
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}