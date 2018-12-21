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
    public class VozacTroskoviController : Controller
    {
        private uvhszjiy_spedicijaEntities db = new uvhszjiy_spedicijaEntities();

        //
        // GET: /VozacTroskovi/
        [Authorize]
        public ActionResult Index()
        {
            var vozactroskovi = db.VozacTroskovi.Include(v => v.Valuta).Include(v => v.VozacVrstaTroskova);
            return View(vozactroskovi.ToList());
        }

        // 
        [Authorize]
        public ActionResult IndexAjax(jQueryDataTableParamModel param)
	 {
		 var list = db.VozacTroskovi.Include(v => v.Valuta).Include(v => v.VozacVrstaTroskova).ToList();
         IEnumerable<VozacTroskovi> filteredList;

				var FIdCozacTroskovi  = Convert.ToString(Request["sSearch_0"]).ToLower();
				var FIznos  = Convert.ToString(Request["sSearch_1"]).ToLower();
				var FIdValuta  = Convert.ToString(Request["sSearch_2"]).ToLower();
				var FTip  = Convert.ToString(Request["sSearch_3"]).ToLower();
				var FIdVrstaTroska  = Convert.ToString(Request["sSearch_4"]).ToLower();
				var FOdobrenoZaduzenje  = Convert.ToString(Request["sSearch_5"]).ToLower();
				var FPrihvacenRashod  = Convert.ToString(Request["sSearch_6"]).ToLower();
				var FRazmjenjenoMjenjacnica  = Convert.ToString(Request["sSearch_7"]).ToLower();
				var FDatum  = Convert.ToString(Request["sSearch_8"]).ToLower();
				var FNapomena  = Convert.ToString(Request["sSearch_9"]).ToLower();
				var FAktivno  = Convert.ToString(Request["sSearch_10"]).ToLower();
				var FIdVozac  = Convert.ToString(Request["sSearch_11"]).ToLower();
		
        filteredList = list.Where(c =>	
				  (String.IsNullOrEmpty("" + c.IdCozacTroskovi ) ? "" :  "" +  c.IdCozacTroskovi).ToLower().Contains(FIdCozacTroskovi) 
				 &&   (String.IsNullOrEmpty("" + c.Iznos ) ? "" :  "" +  c.Iznos).ToLower().Contains(FIznos) 
				 &&   (String.IsNullOrEmpty("" + c.IdValuta ) ? "" :  "" +  c.IdValuta).ToLower().Contains(FIdValuta) 
				 &&   (String.IsNullOrEmpty("" + c.Tip ) ? "" :  "" +  c.Tip).ToLower().Contains(FTip) 
				 &&   (String.IsNullOrEmpty("" + c.IdVrstaTroska ) ? "" :  "" +  c.IdVrstaTroska).ToLower().Contains(FIdVrstaTroska) 
				 &&   (String.IsNullOrEmpty("" + c.OdobrenoZaduzenje ) ? "" :  "" +  c.OdobrenoZaduzenje).ToLower().Contains(FOdobrenoZaduzenje) 
				 &&   (String.IsNullOrEmpty("" + c.PrihvacenRashod ) ? "" :  "" +  c.PrihvacenRashod).ToLower().Contains(FPrihvacenRashod) 
				 &&   (String.IsNullOrEmpty("" + c.RazmjenjenoMjenjacnica ) ? "" :  "" +  c.RazmjenjenoMjenjacnica).ToLower().Contains(FRazmjenjenoMjenjacnica) 
				 &&   (String.IsNullOrEmpty("" + c.Datum ) ? "" :  "" +  c.Datum).ToLower().Contains(FDatum) 
				 &&   (String.IsNullOrEmpty("" + c.Napomena ) ? "" :  "" +  c.Napomena).ToLower().Contains(FNapomena) 
				 &&   (String.IsNullOrEmpty("" + c.Aktivno ) ? "" :  "" +  c.Aktivno).ToLower().Contains(FAktivno) 
				 &&   (String.IsNullOrEmpty("" + c.IdVozac ) ? "" :  "" +  c.IdVozac).ToLower().Contains(FIdVozac) 
		                                  );


            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredList = filteredList.Where(c =>  
											  (String.IsNullOrEmpty("" + c.IdCozacTroskovi ) ? "" :  "" +  c.IdCozacTroskovi).ToLower().Contains(param.sSearch.ToLower()) 
		
											 ||   (String.IsNullOrEmpty("" + c.Iznos ) ? "" :  "" +  c.Iznos).ToLower().Contains(param.sSearch.ToLower()) 
		
											 ||   (String.IsNullOrEmpty("" + c.IdValuta ) ? "" :  "" +  c.IdValuta).ToLower().Contains(param.sSearch.ToLower()) 
		
											 ||   (String.IsNullOrEmpty("" + c.Tip ) ? "" :  "" +  c.Tip).ToLower().Contains(param.sSearch.ToLower()) 
		
											 ||   (String.IsNullOrEmpty("" + c.IdVrstaTroska ) ? "" :  "" +  c.IdVrstaTroska).ToLower().Contains(param.sSearch.ToLower()) 
		
											 ||   (String.IsNullOrEmpty("" + c.OdobrenoZaduzenje ) ? "" :  "" +  c.OdobrenoZaduzenje).ToLower().Contains(param.sSearch.ToLower()) 
		
											 ||   (String.IsNullOrEmpty("" + c.PrihvacenRashod ) ? "" :  "" +  c.PrihvacenRashod).ToLower().Contains(param.sSearch.ToLower()) 
		
											 ||   (String.IsNullOrEmpty("" + c.RazmjenjenoMjenjacnica ) ? "" :  "" +  c.RazmjenjenoMjenjacnica).ToLower().Contains(param.sSearch.ToLower()) 
		
											 ||   (String.IsNullOrEmpty("" + c.Datum ) ? "" :  "" +  c.Datum).ToLower().Contains(param.sSearch.ToLower()) 
		
											 ||   (String.IsNullOrEmpty("" + c.Napomena ) ? "" :  "" +  c.Napomena).ToLower().Contains(param.sSearch.ToLower()) 
		
											 ||   (String.IsNullOrEmpty("" + c.Aktivno ) ? "" :  "" +  c.Aktivno).ToLower().Contains(param.sSearch.ToLower()) 
		
											 ||   (String.IsNullOrEmpty("" + c.IdVozac ) ? "" :  "" +  c.IdVozac).ToLower().Contains(param.sSearch.ToLower()) 
		
																	);

            }

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);

            Func<VozacTroskovi, object> orderingFunction = c =>
            {
                switch (sortColumnIndex)
                {
				 					case 0: return c.IdCozacTroskovi;
									case 1: return c.Iznos;
									case 2: return c.IdValuta;
									case 3: return c.Tip;
									case 4: return c.IdVrstaTroska;
									case 5: return c.OdobrenoZaduzenje;
									case 6: return c.PrihvacenRashod;
									case 7: return c.RazmjenjenoMjenjacnica;
									case 8: return c.Datum;
									case 9: return c.Napomena;
									case 10: return c.Aktivno;
									case 11: return c.IdVozac;
									default: return c.IdCozacTroskovi;
                }
			};

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredList = filteredList.OrderBy(orderingFunction);
            else
                filteredList = filteredList.OrderByDescending(orderingFunction);

            var displayedList = filteredList.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedList select new object[] { 
			 				c.IdCozacTroskovi,
							c.Iznos,
							c.IdValuta,
							c.Tip,
							c.IdVrstaTroska,
							c.OdobrenoZaduzenje,
							c.PrihvacenRashod,
							c.RazmjenjenoMjenjacnica,
							String.IsNullOrEmpty("" + c.Datum) ? "" :   DateTime.Parse(c.Datum.ToString()).ToString("dd.MM.yyyy"),
							c.Napomena,
							c.Aktivno,
							c.IdVozac,
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
        public ActionResult TroskoviVozaca(int id, int ? IdDnevnik)
        {
            DateTime OD = Session["TroskoviOd"] != null ? (DateTime)Session["TroskoviOd"] : DateTime.Now.AddHours(8).AddDays(-30);
            DateTime DO = Session["TroskoviDo"] != null ? (DateTime)Session["TroskoviDo"] : DateTime.Now.AddHours(8);
            String Zakljucano = Session["TroskoviZakljucano"] != null ? Session["TroskoviZakljucano"].ToString() : "";

            bool daliPrikazatiZakljucane = (Zakljucano.Equals("") ? false : true);

            var vozactroskovi =
                IdDnevnik == null ?
                db.VozacTroskovi.Include(v => v.Valuta).Include(v => v.VozacVrstaTroskova).
                Where(c => c.IdVozac == id && c.Datum >= OD && c.Datum <= DO).ToList()
                :
                db.VozacTroskovi.Include(v => v.Valuta).Include(v => v.VozacVrstaTroskova).
                Where(c => c.IdVozac == id && c.IdDnevnik == IdDnevnik.Value).ToList();

            if (!daliPrikazatiZakljucane)
                vozactroskovi = vozactroskovi.Where(c => !(c.Zakljucano ?? false)).ToList();

            ViewBag.DatumOd = OD.ToShortDateString();
            ViewBag.DatumDo = DO.ToShortDateString();
            ViewBag.Vozac = db.Vozaci.Find(id).ImeVozaca;
            ViewBag.IdVozac = id;
            ViewBag.Zakljucano = Zakljucano;

            List<DnevnikPrevoza> listDnevnika =
            IdDnevnik == null ?
            db.DnevnikPrevoza.Where(c => c.IdVozac == id && (c.ZapisAktivan ?? false) &&
            (
            (c.DatumUtovara >= OD && c.DatumIstovara <= DO) ||
            (c.DatumUtovara <= DO && c.DatumIstovara >= DO) ||
            (c.DatumUtovara <= OD && c.DatumIstovara >= OD) ||
            (c.DatumUtovara <= OD && c.DatumIstovara >= DO)
            )
            ).ToList()
            :
             db.DnevnikPrevoza.Where(c => c.IdVozac == id && c.IdDnevnik == IdDnevnik.Value).ToList();

            ViewBag.listDnevnika = listDnevnika;

            String Napomene = db.Vozaci.Find(id).Napomene;
            ViewBag.Napomene = String.IsNullOrEmpty(Napomene) ? "" : Napomene;
            ViewBag.IdDnevnik = new SelectList(db.DnevnikPrevoza.Where(c => (c.ZapisAktivan ?? false)).
               Select(c => new { IdDnevnik = c.IdDnevnik, Text = c.SerijskiBroj + " [" + c.UtovarGrad + " - " + c.IstovarGrad + "]" }), "IdDnevnik", "Text", IdDnevnik);


            return View(vozactroskovi.OrderBy(c => c.IdCozacTroskovi).ToList());
        }

        [HttpPost]
        public ActionResult TroskoviVozaca(int IdVozac, DateTime DatumOd, DateTime DatumDo, int ? IdDnevnik)
        {
            DateTime OD = DatumOd;
            DateTime DO = DatumDo.AddHours(23).AddMinutes(59).AddSeconds(59);

            Session["TroskoviOd"] = OD;
            Session["TroskoviDo"] = DO;
            Session["TroskoviZakljucano"] = Request["Zakljucano"] != null ? "checked" : "";
          //  string a = Request["Zakljucano"].ToString();

            bool daliPrikazatiZakljucane = Request["Zakljucano"] == null ? false : (Request["Zakljucano"].ToString().Equals("") ? false : true);

           

            var vozactroskovi =
             IdDnevnik == null ?
             db.VozacTroskovi.Include(v => v.Valuta).Include(v => v.VozacVrstaTroskova).
             Where(c => c.IdVozac == IdVozac && c.Datum >= OD && c.Datum <= DO).ToList()
             :
             db.VozacTroskovi.Include(v => v.Valuta).Include(v => v.VozacVrstaTroskova).
             Where(c => c.IdVozac == IdVozac && c.IdDnevnik == IdDnevnik.Value).ToList();

            if (!daliPrikazatiZakljucane)
                vozactroskovi = vozactroskovi.Where(c => !(c.Zakljucano ?? false)).ToList();


            ViewBag.DatumOd = OD.ToShortDateString();
            ViewBag.DatumDo = DO.ToShortDateString();
            ViewBag.Zakljucano = Session["TroskoviZakljucano"].ToString();

            ViewBag.Vozac = db.Vozaci.Find(IdVozac).ImeVozaca;
            ViewBag.IdVozac = IdVozac;
            ViewBag.IdDnevnik = new SelectList(db.DnevnikPrevoza.Where(c => (c.ZapisAktivan ?? false)).
               Select(c => new { IdDnevnik = c.IdDnevnik, Text = c.SerijskiBroj + " [" + c.UtovarGrad + " - " + c.IstovarGrad + "]" }), "IdDnevnik", "Text", IdDnevnik);

            List<DnevnikPrevoza> listDnevnika =
           IdDnevnik == null ?
           db.DnevnikPrevoza.Where(c => c.IdVozac == IdVozac && (c.ZapisAktivan ?? false) &&
           (
           (c.DatumUtovara >= OD && c.DatumIstovara <= DO) ||
           (c.DatumUtovara <= DO && c.DatumIstovara >= DO) ||
           (c.DatumUtovara <= OD && c.DatumIstovara >= OD) ||
           (c.DatumUtovara <= OD && c.DatumIstovara >= DO)
           )
           ).ToList()
           :
            db.DnevnikPrevoza.Where(c => c.IdVozac == IdVozac && c.IdDnevnik == IdDnevnik.Value).ToList();

            ViewBag.listDnevnika = listDnevnika;
            String Napomene = db.Vozaci.Find(IdVozac).Napomene;
            ViewBag.Napomene = String.IsNullOrEmpty(Napomene) ? "" : Napomene;

            return View("TroskoviVozaca", vozactroskovi.OrderBy(c => c.IdCozacTroskovi).ToList());
        }

        [Authorize]
        public JsonResult AzurirajStatuseTroskova(int id, int? ZO, int? PR, int? RM, int? AK)
        {
            VozacTroskovi trosak = db.VozacTroskovi.Find(id);

            if (ZO != null)
                trosak.OdobrenoZaduzenje = ((ZO.Value) == 1);

            if (PR != null)
                trosak.PrihvacenRashod = ((PR.Value) == 1);

            if (RM != null)
                trosak.RazmjenjenoMjenjacnica = ((RM.Value) == 1);

            if (AK != null)
                trosak.Aktivno = ((AK.Value) == 1);

            db.Entry(trosak).State = EntityState.Modified;
            db.SaveChanges();

            String suma = db.VozacTroskovi.Where(c => c.IdVozac == trosak.IdVozac && c.Aktivno).Sum(c =>  (c.Tip.Equals("RASHOD") ? (-1) : 1 ) *  c.Iznos * (c.Valuta.UKM ?? 0)).ToString("0.00");

            return Json(suma, JsonRequestBehavior.AllowGet);
        }


        public JsonResult Zakljucavanje(List<int> zakljucani, List<int> nisuZakljucani)
        {

            var lstZaZakljucati = zakljucani == null ? null : db.VozacTroskovi.Where(c => zakljucani.Contains(c.IdCozacTroskovi));
            var lstZaOtkljucati = nisuZakljucani == null ? null : db.VozacTroskovi.Where(c => nisuZakljucani.Contains(c.IdCozacTroskovi));

            if (lstZaZakljucati != null)
            foreach (var item in lstZaZakljucati)
            {
                item.Zakljucano = true;
                db.Entry(item).State = EntityState.Modified;
            }

            if (lstZaOtkljucati != null)
                foreach (var item in lstZaOtkljucati)
            {
                item.Zakljucano = false;
                db.Entry(item).State = EntityState.Modified;
            }

            db.SaveChanges();


            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /VozacTroskovi/Details/5
        [Authorize]
        public ActionResult Details(int id = 0)
        {
            VozacTroskovi vozactroskovi = db.VozacTroskovi.Find(id);
            if (vozactroskovi == null)
            {
                return HttpNotFound();
            }
            return View(vozactroskovi);
        }

        //
        // GET: /VozacTroskovi/Create
        [Authorize]
        public ActionResult Create(int id, int ? idDnevnik)
        {
            ViewBag.IdValuta = new SelectList(db.Valuta, "IdValuta", "OznakaValute");
            ViewBag.IdVrstaTroska = new SelectList(db.VozacVrstaTroskova, "IdVrstaTroska", "Naziv");
            ViewBag.IdVozac = id;

            ViewBag.IdDnevnik = new SelectList(db.DnevnikPrevoza.Where(c => (c.ZapisAktivan ?? false)).
                Select(c => new { IdDnevnik = c.IdDnevnik, Text = c.SerijskiBroj + " [" + c.UtovarGrad + " - " + c.IstovarGrad + "]" }), "IdDnevnik", "Text", idDnevnik );

            List<SelectListItem> items = new List<SelectListItem>
            {
                new SelectListItem(){ Value = "ZADUŽENJE",  Text  = "ZADUŽENJE"},
                new SelectListItem(){ Value = "RASHOD",  Text  = "RASHOD"},

            };

            ViewBag.Tip = new SelectList(items, "Value", "Text");

            return View();
        }

        //
        // POST: /VozacTroskovi/Create

        [HttpPost]
        public ActionResult Create(VozacTroskovi vozactroskovi)
        {

            vozactroskovi.Aktivno = true;
            vozactroskovi.OdobrenoZaduzenje = true;
            vozactroskovi.PrihvacenRashod = true;
            vozactroskovi.PrihvacenRashod = true;
            vozactroskovi.RazmjenjenoMjenjacnica = false;
            

            if (ModelState.IsValid)
            {
                db.VozacTroskovi.Add(vozactroskovi);
                db.SaveChanges();
                return RedirectToAction("TroskoviVozaca", new { id = vozactroskovi.IdVozac });
            }

            ViewBag.IdValuta = new SelectList(db.Valuta, "IdValuta", "OznakaValute", vozactroskovi.IdValuta);
            ViewBag.IdVrstaTroska = new SelectList(db.VozacVrstaTroskova, "IdVrstaTroska", "Naziv", vozactroskovi.IdVrstaTroska);
            ViewBag.IdVozac = vozactroskovi.IdVozac;

            List<SelectListItem> items = new List<SelectListItem>
            {
                new SelectListItem(){ Value = "ZADUŽENJE",  Text  = "ZADUŽENJE"},
                new SelectListItem(){ Value = "RASHOD",  Text  = "RASHOD"},

            };

            ViewBag.Tip = new SelectList(items, "Value", "Text", vozactroskovi.Tip);
            ViewBag.IdDnevnik = new SelectList(db.DnevnikPrevoza.Where(c => (c.ZapisAktivan ?? false)).
               Select(c => new { IdDnevnik = c.IdDnevnik, Text = c.SerijskiBroj + " [" + c.UtovarGrad + " - " + c.IstovarGrad + "]" }), "IdDnevnik", "Text", vozactroskovi.IdDnevnik);

            return View(vozactroskovi);
        }

        //
        // GET: /VozacTroskovi/Edit/5
        [Authorize]
        public ActionResult Edit(int id = 0)
        {
            VozacTroskovi vozactroskovi = db.VozacTroskovi.Find(id);
            if (vozactroskovi == null)
            {
                return HttpNotFound();
            }

            List<SelectListItem> items = new List<SelectListItem>
            {
                new SelectListItem(){ Value = "ZADUŽENJE",  Text  = "ZADUŽENJE"},
                new SelectListItem(){ Value = "RASHOD",  Text  = "RASHOD"},

            };

            ViewBag.IdValuta = new SelectList(db.Valuta, "IdValuta", "OznakaValute", vozactroskovi.IdValuta);
            ViewBag.IdVrstaTroska = new SelectList(db.VozacVrstaTroskova, "IdVrstaTroska", "Naziv", vozactroskovi.IdVrstaTroska);
            ViewBag.Tip = new SelectList(items, "Value", "Text", vozactroskovi.Tip);
            ViewBag.IdDnevnikSL = new SelectList(db.DnevnikPrevoza.Where(c => (c.ZapisAktivan ?? false)).
             Select(c => new { IdDnevnik = c.IdDnevnik, Text = c.SerijskiBroj + " [" + c.UtovarGrad + " - " + c.IstovarGrad + "]" }), "IdDnevnik", "Text", vozactroskovi.IdDnevnik);

            return View(vozactroskovi);
        }

        //
        // POST: /VozacTroskovi/Edit/5

        [HttpPost]
        public ActionResult Edit(VozacTroskovi vozactroskovi)
        {
            if (ModelState.IsValid)
            {
                db.Entry(vozactroskovi).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("TroskoviVozaca", new {id = vozactroskovi.IdVozac });
            }
            ViewBag.IdValuta = new SelectList(db.Valuta, "IdValuta", "OznakaValute", vozactroskovi.IdValuta);
            ViewBag.IdVrstaTroska = new SelectList(db.VozacVrstaTroskova, "IdVrstaTroska", "Naziv", vozactroskovi.IdVrstaTroska);
            ViewBag.IdDnevnikSL = new SelectList(db.DnevnikPrevoza.Where(c => (c.ZapisAktivan ?? false)).
             Select(c => new { IdDnevnik = c.IdDnevnik, Text = c.SerijskiBroj + " [" + c.UtovarGrad + " - " + c.IstovarGrad + "]" }), "IdDnevnik", "Text", vozactroskovi.IdDnevnik);

            return View(vozactroskovi);
        }

        //
        // GET: /VozacTroskovi/Delete/5
        [Authorize]
        public ActionResult Delete(int id = 0)
        {
            VozacTroskovi vozactroskovi = db.VozacTroskovi.Find(id);
            if (vozactroskovi == null)
            {
                return HttpNotFound();
            }
            return View(vozactroskovi);
        }

        //
        // POST: /VozacTroskovi/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            VozacTroskovi vozactroskovi = db.VozacTroskovi.Find(id);
            int IdVozac = vozactroskovi.IdVozac;
            db.VozacTroskovi.Remove(vozactroskovi);
            db.SaveChanges();

            return RedirectToAction("TroskoviVozaca", new { id = IdVozac });

        }

        public ActionResult AjaxDelete(int id)
        {
            VozacTroskovi vozactroskovi = db.VozacTroskovi.Find(id);
           
            db.VozacTroskovi.Remove(vozactroskovi);
            db.SaveChanges();

            return Json("OK", JsonRequestBehavior.AllowGet);

        }





        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}