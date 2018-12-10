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
    public class KorisnikUpitController : Controller
    {
        private uvhszjiy_spedicijaEntities db = new uvhszjiy_spedicijaEntities();

        //
        // GET: /KorisnikUpit/


        [Authorize]
        public ActionResult Index()
        {
            // return View(db.KorisnikUpit.ToList());
			 return View(new List<KorisnikUpit>());
        }

		// 
         [Authorize]
	 public ActionResult IndexAjax(jQueryDataTableParamModel param)
	 {
		 var list = db.KorisnikUpit.ToList();
         IEnumerable<KorisnikUpit> filteredList;

				var FUtovar  = Convert.ToString(Request["sSearch_0"]).ToLower();
				var FIstovar  = Convert.ToString(Request["sSearch_1"]).ToLower();
				var FKolicinaRobe  = Convert.ToString(Request["sSearch_2"]).ToLower();
				var FVrijednostRobe  = Convert.ToString(Request["sSearch_3"]).ToLower();
				var FVrstaRobe  = Convert.ToString(Request["sSearch_4"]).ToLower();
				var FDatumUtovara  = Convert.ToString(Request["sSearch_5"]).ToLower();
				var FDatumIstovara  = Convert.ToString(Request["sSearch_6"]).ToLower();
				var FImeKorisnika  = Convert.ToString(Request["sSearch_7"]).ToLower();
				var FEmail  = Convert.ToString(Request["sSearch_8"]).ToLower();
				var FTelefon  = Convert.ToString(Request["sSearch_9"]).ToLower();
		
        filteredList = list.Where(c =>	
                      (String.IsNullOrEmpty("" + c.Utovar ) ? "" :  "" +  c.Utovar).ToLower().Contains(FUtovar) 
				 &&   (String.IsNullOrEmpty("" + c.Istovar ) ? "" :  "" +  c.Istovar).ToLower().Contains(FIstovar) 
				 &&   (String.IsNullOrEmpty("" + c.KolicinaRobe ) ? "" :  "" +  c.KolicinaRobe).ToLower().Contains(FKolicinaRobe) 
				 &&   (String.IsNullOrEmpty("" + c.VrijednostRobe ) ? "" :  "" +  c.VrijednostRobe).ToLower().Contains(FVrijednostRobe) 
				 &&   (String.IsNullOrEmpty("" + c.VrstaRobe ) ? "" :  "" +  c.VrstaRobe).ToLower().Contains(FVrstaRobe) 
				 &&   (String.IsNullOrEmpty("" + c.DatumUtovara ) ? "" :  "" +  c.DatumUtovara).ToLower().Contains(FDatumUtovara) 
				 &&   (String.IsNullOrEmpty("" + c.DatumIstovara ) ? "" :  "" +  c.DatumIstovara).ToLower().Contains(FDatumIstovara) 
				 &&   (String.IsNullOrEmpty("" + c.ImeKorisnika ) ? "" :  "" +  c.ImeKorisnika).ToLower().Contains(FImeKorisnika) 
				 &&   (String.IsNullOrEmpty("" + c.Email ) ? "" :  "" +  c.Email).ToLower().Contains(FEmail) 
				 &&   (String.IsNullOrEmpty("" + c.Telefon ) ? "" :  "" +  c.Telefon).ToLower().Contains(FTelefon) 
		                                  );


            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredList = filteredList.Where(c =>  
											     (String.IsNullOrEmpty("" + c.Utovar ) ? "" :  "" +  c.Utovar).ToLower().Contains(param.sSearch.ToLower()) 
		
											 ||   (String.IsNullOrEmpty("" + c.Istovar ) ? "" :  "" +  c.Istovar).ToLower().Contains(param.sSearch.ToLower()) 
		
											 ||   (String.IsNullOrEmpty("" + c.KolicinaRobe ) ? "" :  "" +  c.KolicinaRobe).ToLower().Contains(param.sSearch.ToLower()) 
		
											 ||   (String.IsNullOrEmpty("" + c.VrijednostRobe ) ? "" :  "" +  c.VrijednostRobe).ToLower().Contains(param.sSearch.ToLower()) 
		
											 ||   (String.IsNullOrEmpty("" + c.VrstaRobe ) ? "" :  "" +  c.VrstaRobe).ToLower().Contains(param.sSearch.ToLower()) 
		
											 ||   (String.IsNullOrEmpty("" + c.DatumUtovara ) ? "" :  "" +  c.DatumUtovara).ToLower().Contains(param.sSearch.ToLower()) 
		
											 ||   (String.IsNullOrEmpty("" + c.DatumIstovara ) ? "" :  "" +  c.DatumIstovara).ToLower().Contains(param.sSearch.ToLower()) 
		
											 ||   (String.IsNullOrEmpty("" + c.ImeKorisnika ) ? "" :  "" +  c.ImeKorisnika).ToLower().Contains(param.sSearch.ToLower()) 
		
											 ||   (String.IsNullOrEmpty("" + c.Email ) ? "" :  "" +  c.Email).ToLower().Contains(param.sSearch.ToLower()) 
		
											 ||   (String.IsNullOrEmpty("" + c.Telefon ) ? "" :  "" +  c.Telefon).ToLower().Contains(param.sSearch.ToLower()) 
		
																	);

            }

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);

            Func<KorisnikUpit, object> orderingFunction = c =>
            {
                switch (sortColumnIndex)
                {

									case 0: return c.Utovar;
									case 1: return c.Istovar;
									case 2: return c.KolicinaRobe;
									case 3: return c.VrijednostRobe;
									case 4: return c.VrstaRobe;
									case 5: return c.DatumUtovara;
									case 6: return c.DatumIstovara;
									case 7: return c.ImeKorisnika;
									case 8: return c.Email;
									case 9: return c.Telefon;
                                    default: return c.Utovar;
                }
			};

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredList = filteredList.OrderBy(orderingFunction);
            else
                filteredList = filteredList.OrderByDescending(orderingFunction);

            var displayedList = filteredList.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedList select new object[] { 
							c.Utovar,
							c.Istovar,
							c.KolicinaRobe,
							c.VrijednostRobe,
							c.VrstaRobe,
							c.DatumUtovara.HasValue ? c.DatumUtovara.Value.ToShortDateString() : "" ,
							c.DatumIstovara.HasValue ? c.DatumIstovara.Value.ToShortDateString() : "" ,
							c.ImeKorisnika,
							c.Email,
							c.Telefon,
								c.IdKorisnikUpit
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
        // GET: /KorisnikUpit/Details/5
         [Authorize]
        public ActionResult Details(int id = 0)
        {
            KorisnikUpit korisnikupit = db.KorisnikUpit.Find(id);
            if (korisnikupit == null)
            {
                return HttpNotFound();
            }
            return View(korisnikupit);
        }

        //
        // GET: /KorisnikUpit/Create

        public ActionResult Create()
        {
            ViewBag.IdSubjekt = new SelectList(db.Subjekt.OrderBy(c => c.Naziv), "IdSubjekt", "Naziv");
            return View();
        }

        //
        // POST: /KorisnikUpit/Create

        [HttpPost]
        public ActionResult Create(KorisnikUpit korisnikupit)
        {
            if (ModelState.IsValid)
            {

                if (Request["IdSubjekt"] != null)
                {
                    int id = Convert.ToInt32(Request["IdSubjekt"]);

                    Subjekt s = db.Subjekt.Find(id);

                    korisnikupit.ImeKorisnika = s.Naziv;
                    korisnikupit.Telefon = s.Telefon;
                    korisnikupit.Email = s.Email;
                }

                if (Request["Utovar1"] != null && Request["Utovar2"] != null)
                {
                    korisnikupit.Utovar = Request["Utovar1"] + " " + Request["Utovar2"] + " " + korisnikupit.Utovar;
                }

                if (Request["Istovar1"] != null && Request["Istovar2"] != null)
                {
                    korisnikupit.Istovar = Request["Istovar1"] + " " + Request["Istovar2"] + " " + korisnikupit.Istovar;
                }

                if (Request["KolicinaRobe1"] != null)
                {
                    korisnikupit.KolicinaRobe += " " + Request["KolicinaRobe1"];
                }


                db.KorisnikUpit.Add(korisnikupit);
                db.SaveChanges();

              
                    MailMessage mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient("ml01.anaxanet.com");
                    // GMtel2017.
                    mail.From = new MailAddress("info@gmtel-office.com", "GMTEL OFFICE");
                    mail.Subject = "Upit za cijenu prevoza";
                    //mail.To.Add("m.todorovic87@gmail.com");

                    mail.To.Add("info@gmtellogistics.com");
                    mail.Bcc.Add("m.todorovic87@gmail.com");


                    mail.IsBodyHtml = true;

                    String text = "<div>";
                    text += "<img src='http://gmtel-office.com/Content/images/Logo.png'>";

                    text += "<h2>Upit za cijenu prevoza poslat je preko sajta</h2>";
                    text += "</div>";
                    text += "<div>";
                    text += "<p> ImeKorisnika: " + korisnikupit.ImeKorisnika + "</p>";
                    text += "<p> Email: " + korisnikupit.Email + "</p>";
                    text += "<p> Telefon: " + korisnikupit.Telefon + "</p>";

                    text += "<p></p>";

                    text += "<p> Utovar: "+korisnikupit.Utovar+"</p>";
                    text += "<p> Roba spremna za utovar od: " + korisnikupit.DatumUtovara + "</p>";
                    text += "<p> KolicinaRobe: " + korisnikupit.KolicinaRobe + "</p>";
                    text += "<p> VrijednostRobe: " + korisnikupit.VrijednostRobe + "</p>";

                    text += "<p> Istovar: " + korisnikupit.Istovar + "</p>";
                    text += "<p> Željeni rok dostave robe: " + korisnikupit.DatumIstovara + "</p>";
                    text += "<p> Napomena: " + korisnikupit.Napomena + "</p>";
                    text += "</div>";

                    mail.Body = text;
                    SmtpServer.Send(mail);
              

                return RedirectToAction("Login", "Account", new { Message = "OK" });
            }

            return View(korisnikupit);
        }

        //
        // GET: /KorisnikUpit/Edit/5

        public ActionResult Edit(int id = 0)
        {
            KorisnikUpit korisnikupit = db.KorisnikUpit.Find(id);
            if (korisnikupit == null)
            {
                return HttpNotFound();
            }
            return View(korisnikupit);
        }

        //
        // POST: /KorisnikUpit/Edit/5

        [HttpPost]
        public ActionResult Edit(KorisnikUpit korisnikupit)
        {
            if (ModelState.IsValid)
            {
                db.Entry(korisnikupit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(korisnikupit);
        }

        //
        // GET: /KorisnikUpit/Delete/5

        public ActionResult Delete(int id = 0)
        {
            KorisnikUpit korisnikupit = db.KorisnikUpit.Find(id);
            if (korisnikupit == null)
            {
                return HttpNotFound();
            }
            return View(korisnikupit);
        }

        //
        // POST: /KorisnikUpit/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            KorisnikUpit korisnikupit = db.KorisnikUpit.Find(id);
            db.KorisnikUpit.Remove(korisnikupit);
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