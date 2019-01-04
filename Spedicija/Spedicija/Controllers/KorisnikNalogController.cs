using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Spedicija.Models;
using System.Net.Mail;


namespace Spedicija.Controllers
{
    public class KorisnikNalogController : Controller
    {
        private uvhszjiy_spedicijaEntities db = new uvhszjiy_spedicijaEntities();

        //
        // GET: /KorisnikNalog/
          [Authorize]
        public ActionResult Index()
        {
            // return View(db.KorisnikNalog.ToList());
			 return View(new List<KorisnikNalog>());
        }

		// 
          [Authorize]
	 public ActionResult IndexAjax(jQueryDataTableParamModel param)
	 {
		 var list = db.KorisnikNalog.ToList();
         IEnumerable<KorisnikNalog> filteredList;

				var FNaziv  = Convert.ToString(Request["sSearch_0"]).ToLower();
				var FTelefon  = Convert.ToString(Request["sSearch_1"]).ToLower();
				var FEmail  = Convert.ToString(Request["sSearch_2"]).ToLower();
				var FKontaktOsoba  = Convert.ToString(Request["sSearch_3"]).ToLower();
				var FPIB  = Convert.ToString(Request["sSearch_4"]).ToLower();
				
				var FMjestoUtovara  = Convert.ToString(Request["sSearch_5"]).ToLower();
				var FDatumUtovara  = Convert.ToString(Request["sSearch_6"]).ToLower();
				
				var FMjestoIstovara  = Convert.ToString(Request["sSearch_7"]).ToLower();
				var FDatumIstovara  = Convert.ToString(Request["sSearch_8"]).ToLower();
				
				var FDatumZahtjeva  = Convert.ToString(Request["sSearch_9"]).ToLower();
		
        filteredList = list.Where(c =>	
				 (String.IsNullOrEmpty("" + c.Naziv ) ? "" :  "" +  c.Naziv).ToLower().Contains(FNaziv) 

				 &&   (String.IsNullOrEmpty("" + c.Telefon ) ? "" :  "" +  c.Telefon).ToLower().Contains(FTelefon) 
				 &&   (String.IsNullOrEmpty("" + c.Email ) ? "" :  "" +  c.Email).ToLower().Contains(FEmail) 
				 &&   (String.IsNullOrEmpty("" + c.KontaktOsoba ) ? "" :  "" +  c.KontaktOsoba).ToLower().Contains(FKontaktOsoba) 
				
				 &&   (String.IsNullOrEmpty("" + c.PIB ) ? "" :  "" +  c.PIB).ToLower().Contains(FPIB) 


				 &&   (String.IsNullOrEmpty("" + c.MjestoUtovara ) ? "" :  "" +  c.MjestoUtovara).ToLower().Contains(FMjestoUtovara) 
				 &&   (String.IsNullOrEmpty("" + c.DatumUtovara ) ? "" :  "" +  c.DatumUtovara).ToLower().Contains(FDatumUtovara) 

				 &&   (String.IsNullOrEmpty("" + c.MjestoIstovara ) ? "" :  "" +  c.MjestoIstovara).ToLower().Contains(FMjestoIstovara) 
				 &&   (String.IsNullOrEmpty("" + c.DatumIstovara ) ? "" :  "" +  c.DatumIstovara).ToLower().Contains(FDatumIstovara) 

				 &&   (String.IsNullOrEmpty("" + c.DatumZahtjeva ) ? "" :  "" +  c.DatumZahtjeva).ToLower().Contains(FDatumZahtjeva) 
		                                  );


            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredList = filteredList.Where(c =>  
											   (String.IsNullOrEmpty("" + c.Naziv ) ? "" :  "" +  c.Naziv).ToLower().Contains(param.sSearch.ToLower()) 
		
											
											 ||   (String.IsNullOrEmpty("" + c.Telefon ) ? "" :  "" +  c.Telefon).ToLower().Contains(param.sSearch.ToLower()) 
		
											 ||   (String.IsNullOrEmpty("" + c.Email ) ? "" :  "" +  c.Email).ToLower().Contains(param.sSearch.ToLower()) 
		
											 ||   (String.IsNullOrEmpty("" + c.KontaktOsoba ) ? "" :  "" +  c.KontaktOsoba).ToLower().Contains(param.sSearch.ToLower()) 

											 ||   (String.IsNullOrEmpty("" + c.PIB ) ? "" :  "" +  c.PIB).ToLower().Contains(param.sSearch.ToLower()) 
		
											
											 ||   (String.IsNullOrEmpty("" + c.MjestoUtovara ) ? "" :  "" +  c.MjestoUtovara).ToLower().Contains(param.sSearch.ToLower()) 
		
											 ||   (String.IsNullOrEmpty("" + c.DatumUtovara ) ? "" :  "" +  c.DatumUtovara).ToLower().Contains(param.sSearch.ToLower()) 
		
											
											
											 ||   (String.IsNullOrEmpty("" + c.MjestoIstovara ) ? "" :  "" +  c.MjestoIstovara).ToLower().Contains(param.sSearch.ToLower()) 
		
											 ||   (String.IsNullOrEmpty("" + c.DatumIstovara ) ? "" :  "" +  c.DatumIstovara).ToLower().Contains(param.sSearch.ToLower()) 
		
											
											 ||   (String.IsNullOrEmpty("" + c.DatumZahtjeva ) ? "" :  "" +  c.DatumZahtjeva).ToLower().Contains(param.sSearch.ToLower()) 
		
																	);

            }

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);

            Func<KorisnikNalog, object> orderingFunction = c =>
            {
                switch (sortColumnIndex)
                {
				 					
									case 0: return c.Naziv;
									
									case 1: return c.Telefon;
									case 2: return c.Email;
									case 3: return c.KontaktOsoba;
									
									case 4: return c.PIB;
									
									case 5: return c.MjestoUtovara;
									case 6: return c.DatumUtovara;
									
									case 7: return c.MjestoIstovara;
									case 8: return c.DatumIstovara;
									
									case 9: return c.DatumZahtjeva;
                                   default: return c.Naziv;
                }
			};

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredList = filteredList.OrderBy(orderingFunction);
            else
                filteredList = filteredList.OrderByDescending(orderingFunction);

            var displayedList = filteredList.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedList select new object[] { 
			 				
							c.Naziv,
							
							c.Telefon,
							c.Email,
							c.KontaktOsoba,
							
							c.PIB,
							
							c.MjestoUtovara,
							String.IsNullOrEmpty("" + c.DatumUtovara) ? "" :   DateTime.Parse(c.DatumUtovara.ToString()).ToString("dd.MM.yyyy"),
							
							c.MjestoIstovara,
							String.IsNullOrEmpty("" + c.DatumIstovara) ? "" :   DateTime.Parse(c.DatumIstovara.ToString()).ToString("dd.MM.yyyy"),
							
							String.IsNullOrEmpty("" + c.DatumZahtjeva) ? "" :   DateTime.Parse(c.DatumZahtjeva.ToString()).ToString("dd.MM.yyyy"),
								c.idKorisnikNalog
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
        // GET: /KorisnikNalog/Details/5
          [Authorize]
        public ActionResult Details(int id = 0)
        {
            KorisnikNalog korisniknalog = db.KorisnikNalog.Find(id);
            if (korisniknalog == null)
            {
                return HttpNotFound();
            }
            return View(korisniknalog);
        }

        //
        // GET: /KorisnikNalog/Create
        public ActionResult Create()
        {
            ViewBag.IdSubjekt = new SelectList(db.Subjekt.OrderBy(c => c.Naziv), "IdSubjekt", "Naziv");

            return View();
        }

        //
        // POST: /KorisnikNalog/Create

        [HttpPost]
        public ActionResult Create(KorisnikNalog korisniknalog)
        {
            if (ModelState.IsValid)
            {
                korisniknalog.DatumZahtjeva = DateTime.Now.AddHours(7);
                if (korisniknalog.IdSubjekt.HasValue)
                {
                    Subjekt kn = db.Subjekt.Find(korisniknalog.IdSubjekt);

                    korisniknalog.Adresa = kn.Adresa;
                    korisniknalog.Email = kn.Email;
                    korisniknalog.Grad = kn.Grad;
                    korisniknalog.KontaktOsoba = kn.KontaktOsoba;
                    korisniknalog.Naziv = kn.Naziv;
                    korisniknalog.PIB = kn.PIB;
                    korisniknalog.Telefon = kn.Telefon;
                    korisniknalog.Timocom = kn.Timocom;
                    korisniknalog.JIB = kn.JIB;
                    korisniknalog.PTT = kn.PTT;
                    korisniknalog.Drzava = kn.Drzava;

                }


                db.KorisnikNalog.Add(korisniknalog);
                db.SaveChanges();

                /*
                foreach (KorisnikNalogUtovar knu in korisniknalog.KorisnikNalogUtovar)
                {
                    knu.IdKorisnikNalog = korisniknalog.idKorisnikNalog;
                    db.KorisnikNalogUtovar.Add(knu);
                }

                foreach (KorisnikNalogIstovar kni in korisniknalog.KorisnikNalogIstovar)
                {
                    kni.IdKorisnikNalog = korisniknalog.idKorisnikNalog;
                    db.KorisnikNalogIstovar.Add(kni);
                }
                db.SaveChanges();
                */

                String Posiljaoc = korisniknalog.Naziv == null ? db.Subjekt.Find(korisniknalog.IdSubjekt).Naziv : korisniknalog.Naziv;
              
                    MailMessage mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient("ml01.anaxanet.com");

                    mail.From = new MailAddress(AppSettings.GetSettings()["mail_from"], AppSettings.GetSettings()["company_name"]);
                    mail.Subject = "Dobili ste nalog za utovar!";

                    mail.To.Add(AppSettings.GetSettings()["mail_to"]);
                    mail.Bcc.Add("m.todorovic87@gmail.com");

                    mail.IsBodyHtml = true;

                    String text = "<div>";
                    text += "<img src='http://"+ AppSettings.GetSettings()["domain"] + "/Content/images/Logo.png'>";

                    text += "<h2>" + Posiljaoc + " je poslao Nalog za Utovar.</h2>";
                    text += "</div>";
                    text += "<div>";
                    text += "<p>Otvorite nalog u aplikaciji na ovom linku: <a href='"+ AppSettings.GetSettings()["domain_name"] + "/KorisnikNalog/Details/" + korisniknalog.idKorisnikNalog + "'> " + AppSettings.GetSettings()["domain_name"] + "/KorisnikNalog/Details/" + korisniknalog.idKorisnikNalog +"</a></p>";
                    text += "</div>";

                    mail.Body = text;
                    SmtpServer.Send(mail);

                return RedirectToAction("Login", "Account", new { Message = "OKNALOG" });

            }

            return View(korisniknalog);
        }

        [Authorize]
        public ActionResult CreateSubject(int id)
        {
            KorisnikNalog kn = db.KorisnikNalog.Find(id);

            Subjekt s = new Subjekt();
            s.Adresa = kn.Adresa;
            s.DatumKreiranja = DateTime.Now.AddHours(7);
            s.DatumZapisa = DateTime.Now.AddHours(7);
            s.Email = kn.Email;
            s.Grad = kn.Grad;
            s.KontaktOsoba = kn.KontaktOsoba;
            s.Naziv = kn.Naziv;
            s.PIB = kn.PIB;
            s.Telefon = kn.Telefon;
            s.Timocom = kn.Timocom;
            s.JIB = kn.JIB;
            s.PTT = kn.PTT;
            s.Drzava = kn.Drzava;
            s.ZapisAktivan = true;

            return View("~/Views/Subjekt/Create.cshtml", s);
        }

        [Authorize]
        public ActionResult CreateDnevnik(int id)
        {
            KorisnikNalog kn = db.KorisnikNalog.Find(id);

            DnevnikPrevoza dp = new DnevnikPrevoza();

            dp.DatumDnevnika = kn.DatumZahtjeva;

            if(kn.IdSubjekt.HasValue) 
                dp.IdNarucioca = kn.IdSubjekt;

            dp.ReferentniBrojUtovara = kn.ReferentniBrojUtovara;
            dp.UtovarFirma = kn.FirmaUtovar;
            dp.UtovarAdresa = kn.MjestoUtovara;
            dp.UtovarGrad = kn.Grad1;
            dp.UtovarPTT = kn.PTT1;
            dp.UtovarDrzava = kn.Drzava1;
            dp.IstovarFirma = kn.FirmaIStovar;
            dp.IstovarAdresa = kn.MjestoIstovara;
            dp.IstovarGrad = kn.Grad2;
            dp.IstovarPTT = kn.PTT2;
            dp.IstovarDrzava = kn.Drzava2;
            dp.UtovarKontakt = kn.IzvoznikKontaktOsoba;
            dp.IstovarKontakt = kn.UvoznikKontaktOsoba;
            dp.DatumUtovara = kn.DatumUtovara;
            dp.DatumIstovara = kn.DatumIstovara;
            dp.VrstaRobe = kn.VrstaRobe;
            dp.DimenzijeRobe = kn.Dimenzije;
            dp.TezinaRobe = kn.BrutoTezina;
            dp.KolicinaRobe = kn.VrijednostRobe;
            dp.UvoznaSpedicija = kn.UvoznaCarina;
            dp.IzvoznaSpedicija = kn.IzvoznaCarina;

            dp.DnevnikUvoznikIzvoznik.Add(new DnevnikUvoznikIzvoznik { Uvoznik = kn.Uvoznik, Izvoznik = kn.Izvoznik });

            int utovari = kn.KorisnikNalogUtovar.Count();  // 2
            int istovari = kn.KorisnikNalogIstovar.Count();  // 3
            int max = kn.KorisnikNalogUtovar.Count() > kn.KorisnikNalogIstovar.Count() ? kn.KorisnikNalogUtovar.Count() : kn.KorisnikNalogIstovar.Count();  // 3

            for (int i = 0; i < max; i++)
            {
                var uto = i < utovari ? kn.KorisnikNalogUtovar.ElementAt(i) : null;
                var ist = i < istovari ? kn.KorisnikNalogIstovar.ElementAt(i) : null;

                dp.DnevnikCarina.Add(new DnevnikCarina {

                    IzvoznaCarina = uto == null ? "" : uto.IzvoznaCarina,
                    UvoznaCarina = ist == null ? "" : ist.UvoznaCarina
                    
                });
            }

            dp.DnevnikUtovar = kn.KorisnikNalogUtovar.Select(c => new DnevnikUtovar {
                Adresa = c.Adresa,
                DatmUtovara = c.DatmUtovara,
                DimenzijeRobe = c.DimenzijeRobe,
                Drzava = c.Drzava,
                Firma = c.Firma,
                KolicinaRobe = c.KolicinaRobe,
                Kontakt = c.Kontakt,
                Mjesto = c.Mjesto,
                PTT = c.PTT,
                ReferentniBrojUtovara = c.ReferentniBrojUtovara,
                TezinaRobe = c.TezinaRobe,
                VrstaRobe = c.VrstaRobe
            }).ToList();

            dp.DnevnikIstovar = kn.KorisnikNalogIstovar.Select(c => new DnevnikIstovar
            {
                Adresa = c.Adresa,
                DatumIstovara = c.DatumIstovara,
                Drzava = c.Drzava,
                Firma = c.Firma,
                KolicinaRobe = c.KolicinaRobe,
                Kontakt = c.Kontakt,
                Mjesto = c.Mjesto,
                PTT = c.PTT
            }).ToList();

            String SerijskiBroj = SerijskiBrojGenerator.Broj();

            ViewBag.DnevnikCarina = new List<DnevnikCarina>();
            ViewBag.DnevnikIstovar = new List<DnevnikIstovar>();
            ViewBag.DnevnikUtovar = new List<DnevnikUtovar>();

            var dUI = new List<DnevnikUvoznikIzvoznik>();
            dUI.Add(new DnevnikUvoznikIzvoznik { Uvoznik = kn.Uvoznik, Izvoznik = kn.Izvoznik });
            ViewBag.DnevnikUvoznikIzvoznik = dUI;



            ViewBag.Vrsta = new SelectList(db.TipUsluge, "Naziv", "Naziv");

            ViewBag.IdNar = dp.IdNarucioca;
            ViewBag.SerijskiBroj = SerijskiBroj;
            ViewBag.IdPonuda = new SelectList(db.Ponuda.Select(c => new { IdDnevnik = c.IdDnevnik, Naziv = c.SerijskiBroj + " [ Za " + c.Subjekt.Naziv + " na destinaciju:  " + c.IstovarPTT + " " + c.IstovarGrad + " " + c.IstovarDrzava + " ]" }).OrderByDescending(k => k.IdDnevnik), "IdDnevnik", "Naziv");
            ViewBag.IdSubjekt = new SelectList(db.Subjekt, "IdSubjekt", "Naziv", dp.IdSubjekt);
            ViewBag.IdNarucioca = new SelectList(db.Subjekt, "IdSubjekt", "Naziv", dp.IdNarucioca);
            ViewBag.IdValuta = new SelectList(db.Valuta, "IdValuta", "OznakaValute");
            ViewBag.IdValutaPrevoznika = new SelectList(db.Valuta, "IdValuta", "OznakaValute");
            ViewBag.IdVozac = new SelectList(db.Vozaci, "IdVozac", "ImeVozaca");
            ViewBag.IdStatusDetaljni = new SelectList(db.StatusRobe, "IdStatusRobe", "Naziv");
            ViewBag.IdVozilo = new SelectList(db.Vozilo.Where(c => c.VrstaVozila.Equals("Vozilo")).Select(c => new { IdVozilo = c.IdVozilo, TipVozila = c.TipVozila + " " + c.RegistarskiBroj }), "IdVozilo", "TipVozila");
            ViewBag.IdPrikljucno = new SelectList(db.Vozilo.Where(c => c.VrstaVozila.Equals("Priključno Vozilo")).Select(c => new { IdVozilo = c.IdVozilo, TipVozila = c.TipVozila + " " + c.RegistarskiBroj }), "IdVozilo", "TipVozila");
            ViewBag.IdDnevnikParent = null; // new SelectList(db.DnevnikPrevoza.Where(c => (c.ZapisAktivan ?? false)).OrderByDescending(c => c.IdDnevnik).Take(50), "IdDnevnik", "SerijskiBroj");


            // return View(dp);
            return View("~/Views/DnevnikPrevoza/CreateTab.cshtml", dp);

        }


        //
        // GET: /KorisnikNalog/Edit/5
          [Authorize]
        public ActionResult Edit(int id = 0)
        {
            KorisnikNalog korisniknalog = db.KorisnikNalog.Find(id);
            if (korisniknalog == null)
            {
                return HttpNotFound();
            }
            return View(korisniknalog);
        }

        //
        // POST: /KorisnikNalog/Edit/5

        [HttpPost]
        public ActionResult Edit(KorisnikNalog korisniknalog)
        {
            if (ModelState.IsValid)
            {
                db.Entry(korisniknalog).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(korisniknalog);
        }

        //
        // GET: /KorisnikNalog/Delete/5
          [Authorize]
        public ActionResult Delete(int id = 0)
        {
            KorisnikNalog korisniknalog = db.KorisnikNalog.Find(id);
            if (korisniknalog == null)
            {
                return HttpNotFound();
            }
            return View(korisniknalog);
        }

        //
        // POST: /KorisnikNalog/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            KorisnikNalog korisniknalog = db.KorisnikNalog.Find(id);
            db.KorisnikNalog.Remove(korisniknalog);
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