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
    public class KorisnikController : Controller
    {
        private uvhszjiy_spedicijaEntities db = new uvhszjiy_spedicijaEntities();

        //
        // GET: /Korisnik/
        [Authorize]
        public ActionResult Index()
        {
            // return View(db.Korisnik.ToList());
			 return View(new List<Korisnik>());
        }

		// 
        [Authorize]
        [SpedicijaAutorizacija(Roles = "admin")]
	    public ActionResult IndexAjax(jQueryDataTableParamModel param)
	 {
		 var list = db.Korisnik.ToList();
         IEnumerable<Korisnik> filteredList;

				var FIdKorisnik  = Convert.ToString(Request["sSearch_0"]).ToLower();
				var FKorisnickoIme  = Convert.ToString(Request["sSearch_1"]).ToLower();
				
				var FDatumKreiranja  = Convert.ToString(Request["sSearch_2"]).ToLower();
				var FPoslednjaPrijava  = Convert.ToString(Request["sSearch_3"]).ToLower();
				var FIPAdresa  = Convert.ToString(Request["sSearch_4"]).ToLower();
				var FZapisAktivan  = Convert.ToString(Request["sSearch_5"]).ToLower();
				var FRole  = Convert.ToString(Request["sSearch_6"]).ToLower();
		
        filteredList = list.Where(c =>	
				      (String.IsNullOrEmpty("" + c.IdKorisnik ) ? "" :  "" +  c.IdKorisnik).ToLower().Contains(FIdKorisnik) 
				 &&   (String.IsNullOrEmpty("" + c.KorisnickoIme ) ? "" :  "" +  c.KorisnickoIme).ToLower().Contains(FKorisnickoIme) 
				 &&   (String.IsNullOrEmpty("" + c.DatumKreiranja ) ? "" :  "" +  c.DatumKreiranja).ToLower().Contains(FDatumKreiranja) 
				 &&   (String.IsNullOrEmpty("" + c.PoslednjaPrijava ) ? "" :  "" +  c.PoslednjaPrijava).ToLower().Contains(FPoslednjaPrijava) 
				 &&   (String.IsNullOrEmpty("" + c.IPAdresa ) ? "" :  "" +  c.IPAdresa).ToLower().Contains(FIPAdresa) 
				 &&   ((c.ZapisAktivan ?? false) ? "DA" : "NE" ).ToLower().Contains(FZapisAktivan)
                 && (String.IsNullOrEmpty("" + c.Ime) ? "" : "" + c.Ime).ToLower().Contains(FRole) 
		                                  );


            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredList = filteredList.Where(c =>  
											      (String.IsNullOrEmpty("" + c.IdKorisnik )         ? "" :  "" +  c.IdKorisnik).ToLower().Contains(param.sSearch.ToLower()) 
											 ||   (String.IsNullOrEmpty("" + c.KorisnickoIme )      ? "" :  "" +  c.KorisnickoIme).ToLower().Contains(param.sSearch.ToLower()) 
											
											 ||   (String.IsNullOrEmpty("" + c.DatumKreiranja )     ? "" :  "" +  c.DatumKreiranja).ToLower().Contains(param.sSearch.ToLower()) 
											 ||   (String.IsNullOrEmpty("" + c.PoslednjaPrijava )   ? "" :  "" +  c.PoslednjaPrijava).ToLower().Contains(param.sSearch.ToLower()) 
											 ||   (String.IsNullOrEmpty("" + c.IPAdresa )           ? "" :  "" +  c.IPAdresa).ToLower().Contains(param.sSearch.ToLower())
                                             ||   ((c.ZapisAktivan ?? false) ? "DA" : "NE").ToLower().Contains(param.sSearch.ToLower())
                                             || (String.IsNullOrEmpty("" + c.Ime) ? "" : "" + c.Ime).ToLower().Contains(param.sSearch.ToLower()) 
		
																	);

            }

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);

            Func<Korisnik, object> orderingFunction = c =>
            {
                switch (sortColumnIndex)
                {
				 					case 0: return c.IdKorisnik;
									case 1: return c.KorisnickoIme;
									case 2: return c.DatumKreiranja;
									case 3: return c.PoslednjaPrijava;
									case 4: return c.IPAdresa;
									case 5: return c.ZapisAktivan;
                                    case 6: return c.Ime;
									default: return c.IdKorisnik;
                }
			};

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredList = filteredList.OrderBy(orderingFunction);
            else
                filteredList = filteredList.OrderByDescending(orderingFunction);

            var displayedList = filteredList.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedList select new object[] { 
			 				c.IdKorisnik,
							c.KorisnickoIme,
							String.IsNullOrEmpty("" + c.DatumKreiranja) ? "" :   DateTime.Parse(c.DatumKreiranja.ToString()).ToString("dd.MM.yyyy"),
							String.IsNullOrEmpty("" + c.PoslednjaPrijava) ? "" :   DateTime.Parse(c.PoslednjaPrijava.ToString()).ToString("dd.MM.yyyy"),
							c.IPAdresa,
							(c.ZapisAktivan ?? false) ? "DA" : "NE" ,
							c.Ime,
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


        static string getMd5Hash(string input)
     {
         input = input == null ? "" : input;
         // Create a new instance of the MD5CryptoServiceProvider object.

         System.Security.Cryptography.MD5CryptoServiceProvider md5Hasher = new System.Security.Cryptography.MD5CryptoServiceProvider();

         // Convert the input string to a byte array and compute the hash. 

         byte[] data = md5Hasher.ComputeHash(System.Text.Encoding.Default.GetBytes(input));

         // Create a new Stringbuilder to collect the bytes 
         // and create a string.
         System.Text.StringBuilder sBuilder = new System.Text.StringBuilder();

         // Loop through each byte of the hashed data  
         // and format each one as a hexadecimal string. 
         for (int i = 0; i < data.Length; i++)
         {
             sBuilder.Append(data[i].ToString("x2"));
         }

         // Return the hexadecimal string. 
         return sBuilder.ToString();
     }

        //
        // GET: /Korisnik/Details/5
        [Authorize]
        public ActionResult Details(String name)
        {
            Korisnik korisnik = db.Korisnik.Single(c => c.KorisnickoIme.Equals(name));
            if (korisnik == null)
            {
                return HttpNotFound();
            }
            return View(korisnik);
        }

        //
        // GET: /Korisnik/Create
        [Authorize]
        [SpedicijaAutorizacija(Roles = "admin")]
        public ActionResult Create()
        {
            SelectListItem [] sl = new SelectListItem [] {
               new SelectListItem { Value = "admin", Text="admin"},
               new SelectListItem { Value = "user", Text = "user"}
            };


            ViewBag.RoleSL = new SelectList(sl, "Value", "Text");
            return View();
        }

        //
        // POST: /Korisnik/Create

        [HttpPost]
        public ActionResult Create(Korisnik korisnik)
        {
            if (db.Korisnik.Where(c => c.KorisnickoIme.Equals(korisnik.KorisnickoIme)).Count() > 0)
                ModelState.AddModelError("KorisnickoIme", "Korisničko ime je već zauzeto");

            if (ModelState.IsValid)
            {
                korisnik.DatumKreiranja = DateTime.Today.AddHours(7);
                korisnik.Lozinka = getMd5Hash(korisnik.Lozinka);

                db.Korisnik.Add(korisnik);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            SelectListItem[] sl = new SelectListItem[] {
               new SelectListItem { Value = "admin", Text="admin"},
               new SelectListItem { Value = "user", Text = "user"}
            };


            ViewBag.RoleSL = new SelectList(sl, "Value", "Text");
            return View(korisnik);
        }

        //
        // GET: /Korisnik/Edit/5
        [Authorize]
        [SpedicijaAutorizacija(Roles = "admin")]
        public ActionResult Edit(String name)
        {

            Korisnik korisnik = db.Korisnik.Single(c => c.KorisnickoIme.Equals(name));
            if (korisnik == null)
            {
                return HttpNotFound();
            }

            SelectListItem[] sl = new SelectListItem[] {
               new SelectListItem { Value = "admin", Text="admin"},
               new SelectListItem { Value = "user", Text = "user"}
            };


            ViewBag.RoleSL = new SelectList(sl, "Value", "Text");
            return View(korisnik);
        }

        //
        // POST: /Korisnik/Edit/5

        [HttpPost]
        public ActionResult Edit(Korisnik korisnik)
        {
          
            if (ModelState.IsValid)
            {
                if (Request["_ZapisAktivan"] != null)
                {
                    if (Request["_ZapisAktivan"].ToString().Equals("on"))
                        korisnik.ZapisAktivan = true;
                }
                else korisnik.ZapisAktivan = false;

                korisnik.Lozinka =  korisnik.Lozinka == null ? null : getMd5Hash(korisnik.Lozinka);
                db.Entry(korisnik).State = EntityState.Modified;
                db.Entry(korisnik).Property(e => e.Lozinka).IsModified = !(korisnik.Lozinka == null);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            SelectListItem[] sl = new SelectListItem[] {
               new SelectListItem { Value = "admin", Text="admin"},
               new SelectListItem { Value = "user", Text = "user"}
            };


            ViewBag.RoleSL = new SelectList(sl, "Value", "Text");
            return View(korisnik);
        }

        //
        // GET: /Korisnik/Delete/5
        [Authorize]
        [SpedicijaAutorizacija(Roles = "admin")]
        public ActionResult Delete(int id = 0)
        {
            Korisnik korisnik = db.Korisnik.Find(id);
            if (korisnik == null)
            {
                return HttpNotFound();
            }
            return View(korisnik);
        }

        //
        // POST: /Korisnik/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Korisnik korisnik = db.Korisnik.Find(id);
            db.Korisnik.Remove(korisnik);
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