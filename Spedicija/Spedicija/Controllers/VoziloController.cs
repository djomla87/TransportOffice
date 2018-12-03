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
    public class VoziloController : Controller
    {
        private uvhszjiy_spedicijaEntities db = new uvhszjiy_spedicijaEntities();

        //
        // GET: /Vozilo/
        [Authorize]
        public ActionResult Index()
        {
            // return View(db.Vozilo.ToList());
			 return View(new List<Vozilo>());
        }

		// 
        [Authorize]
	 public ActionResult IndexAjax(jQueryDataTableParamModel param)
	 {
		 var list = db.Vozilo.ToList();
         IEnumerable<Vozilo> filteredList;

				var FIdVozilo  = Convert.ToString(Request["sSearch_0"]).ToLower();
				var FTipVozila  = Convert.ToString(Request["sSearch_1"]).ToLower();
				var FRegistarskiBroj  = Convert.ToString(Request["sSearch_2"]).ToLower();
				var FVrstaVozila  = Convert.ToString(Request["sSearch_3"]).ToLower();
                var FDodatniPodaci = Convert.ToString(Request["sSearch_4"]).ToLower();

        filteredList = list.Where(c =>	
				  (String.IsNullOrEmpty("" + c.IdVozilo ) ? "" :  "" +  c.IdVozilo).ToLower().Contains(FIdVozilo) 
				 &&   (String.IsNullOrEmpty("" + c.TipVozila ) ? "" :  "" +  c.TipVozila).ToLower().Contains(FTipVozila) 
				 &&   (String.IsNullOrEmpty("" + c.RegistarskiBroj ) ? "" :  "" +  c.RegistarskiBroj).ToLower().Contains(FRegistarskiBroj)
                 && (String.IsNullOrEmpty("" + c.VrstaVozila) ? "" : "" + c.VrstaVozila).ToLower().Contains(FVrstaVozila) 
				 &&   (String.IsNullOrEmpty("" + c.DodatniPodaci ) ? "" :  "" +  c.DodatniPodaci).ToLower().Contains(FDodatniPodaci) 

		                                  );


            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredList = filteredList.Where(c =>  
											  (String.IsNullOrEmpty("" + c.IdVozilo ) ? "" :  "" +  c.IdVozilo).ToLower().Contains(param.sSearch.ToLower()) 
		
											 ||   (String.IsNullOrEmpty("" + c.TipVozila ) ? "" :  "" +  c.TipVozila).ToLower().Contains(param.sSearch.ToLower()) 
		
											 ||   (String.IsNullOrEmpty("" + c.RegistarskiBroj ) ? "" :  "" +  c.RegistarskiBroj).ToLower().Contains(param.sSearch.ToLower())

                                              || (String.IsNullOrEmpty("" + c.VrstaVozila) ? "" : "" + c.VrstaVozila).ToLower().Contains(param.sSearch.ToLower())

											 ||   (String.IsNullOrEmpty("" + c.DodatniPodaci ) ? "" :  "" +  c.DodatniPodaci).ToLower().Contains(param.sSearch.ToLower()) 
		

		
																	);

            }

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);

            Func<Vozilo, object> orderingFunction = c =>
            {
                switch (sortColumnIndex)
                {
				 					case 0: return c.IdVozilo;
									case 1: return c.TipVozila;
									case 2: return c.RegistarskiBroj;
									case 3: return c.VrstaVozila;
                                    case 4: return c.DodatniPodaci;

									default: return c.IdVozilo;
                }
			};

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredList = filteredList.OrderBy(orderingFunction);
            else
                filteredList = filteredList.OrderByDescending(orderingFunction);

            var displayedList = filteredList.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedList select new object[] { 
			 				c.IdVozilo,
							c.TipVozila,
							c.RegistarskiBroj,
							c.VrstaVozila,
                            c.DodatniPodaci,

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
        // GET: /Vozilo/Details/5
        [Authorize]
        public ActionResult Details(int id = 0)
        {
            Vozilo vozilo = db.Vozilo.Find(id);
            ViewBag.Dokumenti = db.Dokument.Where(c => c.IdVozilo == id).ToList();
            if (vozilo == null)
            {
                return HttpNotFound();
            }
            return View(vozilo);
        }


        [Authorize]
        public ActionResult GoodsDetail()
        {
           // List<DnevnikPrevoza> Prevozi = db.DnevnikPrevoza.Include(d => d.DnevnikUtovar).Include(d => d.DnevnikIstovar).Where(c => !c.Status.Equals("ISTOVARENO") && c.Vozilo != null).OrderBy(k => k.Vozilo.IdVozilo).OrderByDescending(k => k.SerijskiBroj).ToList();

            List<Vozilo> vozila = db.Vozilo.Where(c => c.VrstaVozila.Equals("Vozilo")).ToList();
            ViewBag.Vozila = vozila;

            return View();
        }

        [Authorize]
        public ActionResult IndexGoodsDetail(jQueryDataTableParamModel param, List<int> Vozila)
        {
            List<int> PodstatusiDostavljeni = new List<int>();
            PodstatusiDostavljeni.Add(1);
            PodstatusiDostavljeni.Add(6);
            PodstatusiDostavljeni.Add(7);

            var list = db.DnevnikPrevoza.Include(d => d.DnevnikUtovar).Include(d => d.DnevnikIstovar).Where(c => (c.ZapisAktivan ?? false) && !c.Status.Equals("ISTOVARENO") && !c.Status.Equals("STOPIRANO") && c.Vozilo != null && !PodstatusiDostavljeni.Contains(c.idStatusDetaljni ?? 0)  ).OrderByDescending(k => k.SerijskiBroj).ToList();
            IEnumerable<DnevnikPrevoza> filteredList;

            try
            {
                List<int> vozila = Request["Vozila"].ToString().Split(',').Select(int.Parse).ToList();
                list = list.Where(c => vozila.Contains(c.Vozilo.IdVozilo)).ToList();
            }
            catch (Exception ex) { }

            

            var FSerijskiBroj = Convert.ToString(Request["sSearch_0"]).ToLower();
            var FVozilo = Convert.ToString(Request["sSearch_1"]).ToLower();
            var FPrikljucno = Convert.ToString(Request["sSearch_2"]).ToLower();
            var FVozac = Convert.ToString(Request["sSearch_3"]).ToLower();
            var FRoba = Convert.ToString(Request["sSearch_4"]).ToLower();
            var Fdatumi = Convert.ToString(Request["sSearch_5"]).ToLower();
            var FRelacija = Convert.ToString(Request["sSearch_6"]).ToLower();
            var FStatus = Convert.ToString(Request["sSearch_7"]).ToLower();
            var FUI = Convert.ToString(Request["sSearch_8"]).ToLower();


            filteredList = list.Where(c =>
                      (String.IsNullOrEmpty("" + c.SerijskiBroj) ? "" : "" + c.SerijskiBroj).ToLower().Contains(FSerijskiBroj)
                     && ((c.Vozilo == null) ? "" : "" + c.Vozilo.TipVozila + " " + c.Vozilo.RegistarskiBroj).ToLower().Contains(FVozilo)
                     && ((c.Vozilo1 == null) ? "" : "" + c.Vozilo1.TipVozila + " " + c.Vozilo1.RegistarskiBroj).ToLower().Contains(FPrikljucno)
                     && ((c.Vozaci == null) ? "" : "" + c.Vozaci.ImeVozaca).ToLower().Contains(FVozac)
                     && (String.IsNullOrEmpty(c.KolicinaRobe + c.VrstaRobe) ? "" : "" + c.KolicinaRobe + " " + c.VrstaRobe).ToLower().Contains(FRoba)
                     && (String.IsNullOrEmpty(c.UtovarGrad + c.IstovarGrad) ? "" : "" + c.UtovarGrad + " - " + c.IstovarGrad).ToLower().Contains(FRelacija)
                     && (String.IsNullOrEmpty(c.Status) ? "" : "" + c.Status).ToLower().Contains(FStatus)
                     && (String.IsNullOrEmpty(c.DatumUtovara + " - " + c.DatumIstovara) ? "" : "" + c.DatumUtovara + " - " + c.DatumIstovara).ToLower().Contains(Fdatumi)
                     && (((c.UtovarDrzava + "").ToLower().Contains("bosn") ? ((c.IstovarDrzava+"").ToLower().Contains("bosn") ? "Unutar BiH" : "Izvoz") : ((c.IstovarDrzava + "").ToLower().Contains("bosn") ? "Uvoz" : "Van BiH")).ToLower().Contains(FUI))

                                              );


            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredList = filteredList.Where(c =>
                        (String.IsNullOrEmpty("" + c.SerijskiBroj) ? "" : "" + c.SerijskiBroj).ToLower().Contains(param.sSearch.ToLower())
                     || ((c.Vozilo == null) ? "" : "" + c.Vozilo.TipVozila + " " + c.Vozilo.RegistarskiBroj).ToLower().Contains(param.sSearch.ToLower())
                     || ((c.Vozilo1 == null) ? "" : "" + c.Vozilo1.TipVozila + " " + c.Vozilo1.RegistarskiBroj).ToLower().Contains(param.sSearch.ToLower())
                     || ((c.Vozaci == null) ? "" : "" + c.Vozaci.ImeVozaca).ToLower().Contains(param.sSearch.ToLower())
                     || (String.IsNullOrEmpty(c.KolicinaRobe + c.VrstaRobe) ? "" : "" + c.KolicinaRobe + " " + c.VrstaRobe).ToLower().Contains(param.sSearch.ToLower())
                     || (String.IsNullOrEmpty(c.UtovarGrad + c.IstovarGrad) ? "" : "" + c.UtovarGrad + " - " + c.IstovarGrad).ToLower().Contains(param.sSearch.ToLower())
                     || (String.IsNullOrEmpty(c.Status) ? "" : "" + c.Status).ToLower().Contains(param.sSearch.ToLower())
                     || (String.IsNullOrEmpty(c.DatumUtovara + " - " + c.DatumIstovara) ? "" : "" + c.DatumUtovara + " - " + c.DatumIstovara).ToLower().Contains(param.sSearch.ToLower())
                     || (((c.UtovarDrzava + "").ToLower().Contains("bosn") ? ((c.IstovarDrzava + "").ToLower().Contains("bosn") ? "Unutar BiH" : "Izvoz") : ((c.IstovarDrzava + "").ToLower().Contains("bosn") ? "Uvoz" : "Van BiH")).ToLower().Contains(param.sSearch.ToLower()))


                                                                    );

            }

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);

            Func<DnevnikPrevoza, object> orderingFunction = c =>
            {
                switch (sortColumnIndex)
                {

                    case 0: return c.SerijskiBroj;
                    case 1: return (c.Vozilo == null) ? "" : "" + c.Vozilo.TipVozila + " " + c.Vozilo.RegistarskiBroj;
                    case 2: return (c.Vozilo1 == null) ? "" : "" + c.Vozilo1.TipVozila + " " + c.Vozilo1.RegistarskiBroj;
                    case 3: return (c.Vozaci == null) ? "" : "" + c.Vozaci.ImeVozaca;
                    case 4: return String.IsNullOrEmpty(c.KolicinaRobe + c.VrstaRobe) ? "" : "" + c.KolicinaRobe + " " + c.VrstaRobe;
                    case 5: return c.DatumIstovara;
                    case 6: return String.IsNullOrEmpty(c.UtovarGrad + c.IstovarGrad) ? "" : "" + c.UtovarGrad + " - " + c.IstovarGrad;
                    case 7: return String.IsNullOrEmpty(c.Status) ? "" : "" + c.Status;
                    case 8: return ((c.UtovarDrzava + "").ToLower().Contains("bosn") ? ((c.IstovarDrzava + "").ToLower().Contains("bosn") ? "Unutar BiH" : "Izvoz") : ((c.IstovarDrzava + "").ToLower().Contains("bosn") ? "Uvoz" : "Van BiH"));

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
                             (c.Vozilo == null) ? "" : "" + c.Vozilo.TipVozila  + " " + c.Vozilo.RegistarskiBroj,
                             (c.Vozilo1 == null) ? "" : "" + c.Vozilo1.TipVozila + " " + c.Vozilo1.RegistarskiBroj,
                             (c.Vozaci == null) ? "" : "" + c.Vozaci.ImeVozaca,
                             String.IsNullOrEmpty(c.KolicinaRobe + c.VrstaRobe) ? "" : "Količina: " + c.KolicinaRobe + " Vrsta: " + c.VrstaRobe + " Težina u kg:" + c.TezinaRobe + " Dimenzije:" + c.DimenzijeRobe,
                             (c.DatumUtovara.HasValue ? c.DatumUtovara.Value.ToShortDateString() : "") + " - " +
                                   (c.DatumIstovara.HasValue ? c.DatumIstovara.Value.ToShortDateString() : ""),
                             String.IsNullOrEmpty(c.UtovarGrad + c.IstovarGrad) ? "" : "" + c.UtovarGrad + " # " + c.IstovarGrad,
                             String.IsNullOrEmpty(c.Status) ? "" : "" + c.Status,
                            // (c.UtovarDrzava.ToLower().Contains("bosn") ? (c.IstovarDrzava.ToLower().Contains("bosn") ? "Unutar BiH" : "Izvoz" ) : (c.IstovarDrzava.ToLower().Contains("bosn") ? "Uvoz" : "Van BiH"))
                            ((c.UtovarDrzava + "").ToLower().Contains("bosn") ? ((c.IstovarDrzava + "").ToLower().Contains("bosn") ? "Unutar BiH" : "Izvoz") : ((c.IstovarDrzava + "").ToLower().Contains("bosn") ? "Uvoz" : "Van BiH"))
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
        public ActionResult GetDnevnik(String SerijskiBroj)
        {
            int id = db.DnevnikPrevoza.Where(c => c.SerijskiBroj.Equals(SerijskiBroj)).SingleOrDefault().IdDnevnik;

            return RedirectToAction("DetailsTab", "DnevnikPrevoza", new { id = id });
        }


        public JsonResult VratiVozila()
        {
            return Json(db.Vozilo.Select(c => new { Naziv = c.TipVozila + " " + c.RegistarskiBroj }).ToList(), JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /Vozilo/Create
        [Authorize]
        public ActionResult Create()
        {
            List<SelectListItem> vrsta = new List<SelectListItem> {
                new SelectListItem { Text = "Vozilo", Value = "Vozilo" },
                new SelectListItem { Text = "Priključno Vozilo", Value ="Priključno Vozilo" }
            };

            ViewBag.Vrsta = new SelectList(vrsta, "Value", "Text", "");
            return View();
        }

        //
        // POST: /Vozilo/Create

        [HttpPost]
        public ActionResult Create(Vozilo vozilo)
        {
            if (ModelState.IsValid)
            {
                vozilo.ZapisAktivan = true;
                vozilo.DatumZapisa = DateTime.Now.AddHours(7);
                db.Vozilo.Add(vozilo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            List<SelectListItem> vrsta = new List<SelectListItem> {
                new SelectListItem { Text = "Vozilo", Value = "Vozilo" },
                new SelectListItem { Text = "Priključno Vozilo", Value ="Priključno Vozilo" }
            };

            ViewBag.Vrsta = new SelectList(vrsta, "Value", "Text", vozilo.VrstaVozila);
            return View(vozilo);
        }

        //
        // GET: /Vozilo/Edit/5
        [Authorize]
        [SpedicijaAutorizacija(Roles = "admin")]
        public ActionResult Edit(int id = 0)
        {
            Vozilo vozilo = db.Vozilo.Find(id);
            if (vozilo == null)
            {
                return HttpNotFound();
            }

            List<SelectListItem> vrsta = new List<SelectListItem> {
                new SelectListItem { Text = "Vozilo", Value = "Vozilo" },
                new SelectListItem { Text = "Priključno Vozilo", Value ="Priključno Vozilo" }
            };

            ViewBag.Vrsta = new SelectList(vrsta, "Value", "Text", vozilo.VrstaVozila);
            return View(vozilo);
        }

        //
        // POST: /Vozilo/Edit/5

        [HttpPost]
        public ActionResult Edit(Vozilo vozilo)
        {
            if (ModelState.IsValid)
            {
                vozilo.ZapisAktivan = true;
                vozilo.DatumZapisa = DateTime.Now.AddHours(7);
                db.Entry(vozilo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            List<SelectListItem> vrsta = new List<SelectListItem> {
                new SelectListItem { Text = "Vozilo", Value = "Vozilo" },
                new SelectListItem { Text = "Priključno Vozilo", Value ="Priključno Vozilo" }
            };

            ViewBag.Vrsta = new SelectList(vrsta, "Value", "Text", vozilo.VrstaVozila);
            return View(vozilo);
        }

        //
        // GET: /Vozilo/Delete/5
        [Authorize]
        [SpedicijaAutorizacija(Roles = "admin")]
        public ActionResult Delete(int id = 0)
        {
            Vozilo vozilo = db.Vozilo.Find(id);
            if (vozilo == null)
            {
                return HttpNotFound();
            }
            return View(vozilo);
        }

        //
        // POST: /Vozilo/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Vozilo vozilo = db.Vozilo.Find(id);
            db.Vozilo.Remove(vozilo);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase file, int IdVozilo)
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

                dokument.IdVozilo = IdVozilo;
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
            int back = dokument.IdVozilo ?? 0;
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

        public JsonResult AktivniPrevoziVozila(int? IdVozilo, int? IdVozac)
        {
            List<int> PodstatusiDostavljeni = new List<int>();
            PodstatusiDostavljeni.Add(1);
            PodstatusiDostavljeni.Add(6);
            PodstatusiDostavljeni.Add(7);

            var Utovari = db.DnevnikPrevoza.Include(K=>K.RedoslijedUtovarIstovar).Where(c => (c.ZapisAktivan ?? false) && (c.Status.Equals("NIJE UTOVARENO") || c.Status.Equals("UTOVARENO U TRANZITU")) && !PodstatusiDostavljeni.Contains(c.idStatusDetaljni ?? 0)).ToList().
                Select(c => new MapaPrevozaModel
                 {
                    Adresa = c.UtovarPTT + " " + c.UtovarGrad + " " + c.UtovarDrzava,
                    Opis = "<strong>Utovar<br/>Adresa: " + c.UtovarAdresa + " " + c.UtovarPTT + " " + c.UtovarGrad + " " + c.UtovarDrzava + "<br/> Firma: " + c.UtovarFirma + "<br/> Datum: " + (c.DatumUtovara.HasValue ? c.DatumUtovara.Value.ToShortDateString() : "" ) +
                    "</br>" + c.KolicinaRobe + " " + c.DimenzijeRobe + " " + c.VrstaRobe + "</br><a href='/DnevnikPrevoza/DetailsTab/" + c.IdDnevnik + "' target='_blank' >"+c.SerijskiBroj+"</a></strong>",
                    Long = "",
                    Lat = "",
                    Tip = "Utovar1",
                    Status = c.Status,
                    IdDnevnik = c.IdDnevnik,
                    IdDodatniUtovar = null,
                    IdDodatniIstovar = null,
                    Aktivnost = 'U',
                    Header = "Utovar",
                    RedniBroj = c.RedoslijedUtovarIstovar.Count > 0 ? 
                                (c.RedoslijedUtovarIstovar.Where(k=> k.IdDodatniUtovar == null && k.IdDodatniIstovar == null && k.Aktivnost.Equals("U"))).FirstOrDefault().RedniBroj : 0,
                    IdRedoslijed = c.RedoslijedUtovarIstovar.Count > 0 ?
                                (c.RedoslijedUtovarIstovar.Where(k => k.IdDodatniUtovar == null && k.IdDodatniIstovar == null && k.Aktivnost.Equals("U"))).FirstOrDefault().IdRedoslijed : 0,
                    IdVozilo = c.IdVozilo,
                    IdVozac = c.IdVozac

                }).ToList();

            var DodatniUtovari = db.DnevnikUtovar.Include(K => K.DnevnikPrevoza).Include(K => K.RedoslijedUtovarIstovar).
                 Where(c => (c.DnevnikPrevoza.ZapisAktivan ?? false) &&
                 (c.DnevnikPrevoza.Status.Equals("NIJE UTOVARENO") || c.DnevnikPrevoza.Status.Equals("UTOVARENO U TRANZITU")) && !PodstatusiDostavljeni.Contains(c.DnevnikPrevoza.idStatusDetaljni ?? 0)).ToList().
                 Select(c => new MapaPrevozaModel
                 {
                     Adresa = c.PTT + " " + c.Mjesto + " " + c.Drzava,
                     Opis = "<strong>Dodatni Utovar<br/>Adresa: " + c.Adresa + " " + c.PTT + " " + c.Mjesto + " " + c.Drzava + "<br/> Firma: " + c.Firma + "<br/> Datum: " + (c.DatmUtovara.HasValue ? c.DatmUtovara.Value.ToShortDateString() : "") +
                     "</br>" + c.KolicinaRobe + " " + c.DimenzijeRobe + " " + c.VrstaRobe  + "</br><a href='/DnevnikPrevoza/DetailsTab/" + c.IdDnevnik + "' target='_blank' >" + c.DnevnikPrevoza.SerijskiBroj + "</a></strong>",
                     Long = "",
                     Lat = "",
                     Tip = "Utovar",
                     Status = c.DnevnikPrevoza.Status,
                     IdDnevnik = c.IdDnevnik,
                     IdDodatniUtovar = c.IdUtovar,
                     IdDodatniIstovar = null,
                     Aktivnost = 'U',
                     Header = "Dodatni Utovar",
                     RedniBroj = c.RedoslijedUtovarIstovar.Count > 0 ? c.RedoslijedUtovarIstovar.FirstOrDefault().RedniBroj : 0,
                     IdRedoslijed = c.RedoslijedUtovarIstovar.Count > 0 ? c.RedoslijedUtovarIstovar.FirstOrDefault().IdRedoslijed : 0,
                     IdVozilo = c.DnevnikPrevoza.IdVozilo,
                     IdVozac = c.DnevnikPrevoza.IdVozac

                 }).ToList();

            var Istovari = db.DnevnikPrevoza.Include(K => K.RedoslijedUtovarIstovar).Where(c => (c.ZapisAktivan ?? false) && (c.Status.Equals("NIJE UTOVARENO") || c.Status.Equals("UTOVARENO U TRANZITU")) && !PodstatusiDostavljeni.Contains(c.idStatusDetaljni ?? 0)).ToList().
                 Select(c => new MapaPrevozaModel
                 {
                     Adresa = c.IstovarPTT + " " + c.IstovarGrad + " " + c.IstovarDrzava,
                     Opis = "<strong>Istovar</br>Adresa: " + c.IstovarAdresa + " " + c.IstovarPTT+ " " + c.IstovarGrad + " " + c.IstovarDrzava + "<br/> Firma: " + c.IstovarFirma + "<br/> Datum: " + (c.DatumIstovara.HasValue ? c.DatumIstovara.Value.ToShortDateString() : "" ) + 
                     "</br>" + c.IstovarKolicinaRobe + "</br><a href='/DnevnikPrevoza/DetailsTab/" + c.IdDnevnik + "' target='_blank' >" + c.SerijskiBroj + "</a></strong>",
                     Long = "",
                     Lat = "",
                     Tip = "Istovar1",
                     Status = c.Status,
                     IdDnevnik = c.IdDnevnik,
                     IdDodatniUtovar = null,
                     IdDodatniIstovar = null,
                     Aktivnost = 'I',
                     Header = "Istovar",
                     RedniBroj = c.RedoslijedUtovarIstovar.Count > 0 ?
                                (c.RedoslijedUtovarIstovar.Where(k => k.IdDodatniUtovar == null && k.IdDodatniIstovar == null && k.Aktivnost.Equals("I"))).FirstOrDefault().RedniBroj : 0,
                     IdRedoslijed = c.RedoslijedUtovarIstovar.Count > 0 ?
                                (c.RedoslijedUtovarIstovar.Where(k => k.IdDodatniUtovar == null && k.IdDodatniIstovar == null && k.Aktivnost.Equals("I"))).FirstOrDefault().IdRedoslijed : 0,
                     IdVozilo = c.IdVozilo,
                     IdVozac = c.IdVozac
                 }).ToList();

            var DodatniIstovari = db.DnevnikIstovar.Include(K => K.DnevnikPrevoza).Include(K => K.RedoslijedUtovarIstovar).
                 Where(c => (c.DnevnikPrevoza.ZapisAktivan ?? false) &&
                 (c.DnevnikPrevoza.Status.Equals("NIJE UTOVARENO") || c.DnevnikPrevoza.Status.Equals("UTOVARENO U TRANZITU")) && !PodstatusiDostavljeni.Contains(c.DnevnikPrevoza.idStatusDetaljni ?? 0)).ToList().
                 Select(c => new MapaPrevozaModel
                 {
                     Adresa = c.PTT + " " + c.Mjesto + " " + c.Drzava,
                     Opis = "<strong>Dodatni Istovar</br>Adresa: " + c.Adresa + " " + c.PTT + " " + c.Mjesto + " " + c.Drzava +  "<br/> Firma: " + c.Firma + "<br/> Datum: " + (c.DatumIstovara.HasValue ? c.DatumIstovara.Value.ToShortDateString() : "") + 
                     "</br>" + c.KolicinaRobe + "</br><a href='/DnevnikPrevoza/DetailsTab/" + c.IdDnevnik + "' target='_blank' >" + c.DnevnikPrevoza.SerijskiBroj + "</a></strong>",
                     Long = "",
                     Lat = "",
                     Tip = "Istovar",
                     Status = c.DnevnikPrevoza.Status,
                     IdDnevnik = c.IdDnevnik,
                     IdDodatniUtovar = null,
                     IdDodatniIstovar = c.IdIstovar,
                     Aktivnost = 'I',
                     Header = "Dodatni Istovar",
                     RedniBroj = c.RedoslijedUtovarIstovar.Count > 0 ? c.RedoslijedUtovarIstovar.FirstOrDefault().RedniBroj : 0,
                     IdRedoslijed = c.RedoslijedUtovarIstovar.Count > 0 ? c.RedoslijedUtovarIstovar.FirstOrDefault().IdRedoslijed : 0,
                     IdVozilo = c.DnevnikPrevoza.IdVozilo,
                     IdVozac = c.DnevnikPrevoza.IdVozac
                 }).ToList();

            Utovari.AddRange(DodatniUtovari);
            Utovari.AddRange(Istovari);
            Utovari.AddRange(DodatniIstovari);

            if (IdVozac != null)
                Utovari = Utovari.Where(c => c.IdVozac == IdVozac).ToList();

            if (IdVozilo != null)
                Utovari = Utovari.Where(c => c.IdVozilo == IdVozilo).ToList();

            return Json(Utovari.OrderBy(c => c.RedniBroj).ToList(), JsonRequestBehavior.AllowGet);

        }

        public JsonResult SortirajUtovarIstovar(List<SortUImodel> ui)
        {
          
                int rb = 1;
                foreach (SortUImodel sort in ui)
                {
                   

                    if (sort.IdRedoslijed == 0)
                    {
                        RedoslijedUtovarIstovar r = new RedoslijedUtovarIstovar
                        {
                            Aktivnost = sort.Aktivnost,
                            IdDnevnik = sort.IdDnevnik,
                            IdDodatniIstovar = sort.IdDodatniIstovar,
                            IdDodatniUtovar = sort.IdDodatniUtovar,
                            RedniBroj = rb
                        };

                        db.RedoslijedUtovarIstovar.Add(r);
                    }
                    else
                    {
                        RedoslijedUtovarIstovar r = db.RedoslijedUtovarIstovar.Find(sort.IdRedoslijed);
                        r.Aktivnost = sort.Aktivnost;
                        r.IdDnevnik = sort.IdDnevnik;
                        r.IdDodatniIstovar = sort.IdDodatniIstovar;
                        r.IdDodatniUtovar = sort.IdDodatniUtovar;
                        r.RedniBroj = rb;

                        db.Entry(r).State = EntityState.Modified;
                    }

                    rb++;
                }

                db.SaveChanges();
        //    }
        //    catch (Exception ex) { return Json("Greška: " + ex.Message, JsonRequestBehavior.AllowGet); }

            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult Roba()
        {
            return View();
        }

        [Authorize]
        public ActionResult RasporedUtovarIstovar()
        {
            ViewBag.IdVozac = new SelectList(db.Vozaci, "IdVozac", "ImeVozaca");
            ViewBag.IdVozilo = new SelectList(db.Vozilo.Where(c => c.VrstaVozila.Equals("Vozilo")).Select(c => new { IdVozilo = c.IdVozilo, TipVozila = c.TipVozila + " " + c.RegistarskiBroj }), "IdVozilo", "TipVozila");

            return View();
        }


        public JsonResult VratiAktivnaVozilaOsim(int IdVozilo)
        {

            return Json(db.Vozilo.Where(c => (c.ZapisAktivan)).Select(k => new { key = k.IdVozilo, value = k.TipVozila + " " + k.RegistarskiBroj, type = "vozilo" }).ToList(), JsonRequestBehavior.AllowGet);
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}