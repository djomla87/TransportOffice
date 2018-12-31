using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Spedicija.Models;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Net.Mail;
using System.Text.RegularExpressions;
using Spedicija.Models.Mobile;
using System.Runtime.Caching;
using System.IO;
using System.IO.Compression;

namespace Spedicija.Controllers
{
    public class DnevnikPrevozaController : Controller
    {
        private uvhszjiy_spedicijaEntities db = new uvhszjiy_spedicijaEntities();

        //
        // GET: /DnevnikPrevoza/
        [Authorize]
        public ActionResult Index()
        {
            AppPodesavanja tip_dnevnika = db.AppPodesavanja.FirstOrDefault(c => c.Vrsta.Equals("tip_dnevnika"));
            if (tip_dnevnika.Vrijednost.Equals("Index"))
                return View(new List<DnevnikPrevoza>());
            else
                return View("Index2");

            
        }

        [Authorize]
        public ActionResult Index2()
        {

            AppPodesavanja tip_dnevnika = db.AppPodesavanja.FirstOrDefault(c => c.Vrsta.Equals("tip_dnevnika"));
            if (tip_dnevnika.Vrijednost.Equals("Index2"))
                return View();
            else
                return View("Index", new List<DnevnikPrevoza>());

        }

       
        public ActionResult SetDeafault(String val)
        {
            AppPodesavanja tip_dnevnika = db.AppPodesavanja.FirstOrDefault(c => c.Vrsta.Equals("tip_dnevnika"));
            tip_dnevnika.Vrijednost = val;
            db.Entry(tip_dnevnika).State = EntityState.Modified;

            db.SaveChanges();

            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        // 
        [Authorize]
	    public ActionResult IndexAjax(jQueryDataTableParamModel param)
	 {


            //Session["VrstaDnevnika"] = "Index";
        
         var list = db.DnevnikPrevoza.Include(d => d.Subjekt).Include(d => d.Subjekt1).Include(d => d.DnevnikUvoznikIzvoznik).Include(d => d.DnevnikUtovar).Include(d => d.DnevnikIstovar).Where(k => (k.ZapisAktivan ?? false)).ToList();

         List<int> dok = db.Dokument.Where(c => c.IdDnevnik.HasValue).Select(c => c.IdDnevnik ?? 0).Distinct().ToList();
         List<int> VozacVozilo = db.DnevnikPrevoza.Where(c => (c.ZapisAktivan ?? false) && (c.Vozilo == null || c.Vozaci == null) && !(c.DrugiPrevoznik ?? false)).Select(c => c.IdDnevnik ).ToList();
         List<int> Parenti = db.DnevnikPrevoza.Where(c => (c.ZapisAktivan ?? false) && c.IdDnevnikParent != null).Select(c => c.IdDnevnikParent ?? 0).ToList();
         List<int> Valute = db.DnevnikPrevoza.Where(c => (!(c.Uplaceno ?? false))).ToList()
               .Where(c => ((c.DatumSlanjaFakture ?? DateTime.Today.AddHours(7)).AddDays(c.ValutaPlacanja ?? 999999) < DateTime.Today.AddHours(7))).Select( c=> c.IdDnevnik ).ToList();

            IEnumerable<DnevnikPrevoza> filteredList;

         var FSerijskiBroj = Convert.ToString(Request["sSearch_0"]).ToLower();
         var FDatumDnevnika = Convert.ToString(Request["sSearch_1"]).ToLower();
         var FIdNarucioca = Convert.ToString(Request["sSearch_2"]).ToLower();
         var FUI = Convert.ToString(Request["sSearch_3"]).ToLower();
         var FUtovar = Convert.ToString(Request["sSearch_4"]).ToLower();
         var FDatumUtovara = Convert.ToString(Request["sSearch_5"]).ToLower();
         var FIstovar = Convert.ToString(Request["sSearch_6"]).ToLower();
         var FDatumIstovara = Convert.ToString(Request["sSearch_7"]).ToLower();
         var FStatus = Convert.ToString(Request["sSearch_11"]).ToLower();


         filteredList = list.Where(c =>
                  (String.IsNullOrEmpty("" + c.SerijskiBroj) ? "" : "" + c.SerijskiBroj).ToLower().Contains(FSerijskiBroj)
                  && (String.IsNullOrEmpty("" + c.DatumDnevnika) ? "" : "" + c.DatumDnevnika).ToLower().Contains(FDatumDnevnika)
                  && ((c.Subjekt1 != null) ? c.Subjekt1.Naziv : "").ToLower().Contains(FIdNarucioca)
                  && ((c.DnevnikUvoznikIzvoznik.Count == 0) ? "" : String.Join("   ", c.DnevnikUvoznikIzvoznik.Select(g => g.Uvoznik + " / " + g.Izvoznik).ToList())).ToLower().Contains(FUI)
                  && (String.IsNullOrEmpty("" + c.UtovarAdresa + c.UtovarFirma + c.UtovarPTT + c.UtovarGrad + String.Join(" ", c.DnevnikUtovar.Select(g => g.Adresa + g.PTT + g.Mjesto))) ? "" : "" + c.UtovarAdresa + c.UtovarFirma + c.UtovarPTT + c.UtovarGrad + String.Join(" ", c.DnevnikUtovar.Select(g => g.Adresa + g.PTT + g.Firma + g.Mjesto ))).ToLower().Contains(FUtovar)
                  && (String.IsNullOrEmpty("" + c.DatumUtovara) ? "" : "" + c.DatumUtovara).ToLower().Contains(FDatumUtovara)
                  && (String.IsNullOrEmpty("" + c.IstovarAdresa + c.IstovarFirma + c.IstovarPTT + c.IstovarGrad + String.Join(" ", c.DnevnikIstovar.Select(g => g.Adresa + g.PTT + g.Mjesto))) ? "" : "" + c.IstovarAdresa + c.IstovarFirma + c.UtovarPTT + c.IstovarGrad + String.Join(" ", c.DnevnikIstovar.Select(g => g.Adresa + g.PTT + g.Firma + g.Mjesto))).ToLower().Contains(FIstovar)
                  && (String.IsNullOrEmpty("" + c.DatumIstovara) ? "" : "" + c.DatumIstovara).ToLower().Contains(FDatumIstovara)
                  && (String.IsNullOrEmpty("" + c.Status) ? "" : "" + c.Status).ToLower().Contains(FStatus)


                                           );

         if (!string.IsNullOrEmpty(param.sSearch))
         {
             filteredList = filteredList.Where(c =>
                 (String.IsNullOrEmpty("" + c.SerijskiBroj) ? "" : "" + c.SerijskiBroj).ToLower().Contains(param.sSearch.ToLower())
              || (String.IsNullOrEmpty("" + c.DatumDnevnika) ? "" : "" + c.DatumDnevnika).ToLower().Contains(param.sSearch.ToLower())
              || ((c.Subjekt1 != null) ? c.Subjekt1.Naziv : "").ToLower().Contains(param.sSearch.ToLower())
              || ((c.DnevnikUvoznikIzvoznik.Count == 0) ? "" : String.Join("   ", c.DnevnikUvoznikIzvoznik.Select(g => g.Uvoznik + " / " + g.Izvoznik).ToList())).ToLower().Contains(param.sSearch.ToLower())
              || (String.IsNullOrEmpty("" + c.UtovarAdresa + c.UtovarFirma + c.UtovarPTT + c.UtovarGrad + String.Join(" ", c.DnevnikUtovar.Select(g => g.Adresa + g.PTT + g.Firma + g.Mjesto))) ? "" : "" + c.UtovarAdresa + c.UtovarFirma + c.UtovarPTT + c.UtovarGrad + String.Join(" ", c.DnevnikUtovar.Select(g => g.Adresa + g.PTT + g.Firma + g.Mjesto))).ToLower().Contains(param.sSearch.ToLower())
              || (String.IsNullOrEmpty("" + c.DatumUtovara) ? "" : "" + c.DatumUtovara).ToLower().Contains(param.sSearch.ToLower())
              || (String.IsNullOrEmpty("" + c.IstovarAdresa + c.IstovarFirma + c.IstovarPTT + c.IstovarGrad + String.Join(" ", c.DnevnikIstovar.Select(g => g.Adresa + g.Firma + g.PTT + g.Mjesto))) ? "" : "" + c.IstovarAdresa + c.IstovarFirma + c.IstovarPTT + c.IstovarGrad + String.Join(" ", c.DnevnikIstovar.Select(g => g.Adresa + g.PTT + g.Firma + g.Mjesto))).ToLower().Contains(param.sSearch.ToLower())
              || (String.IsNullOrEmpty("" + c.DatumIstovara) ? "" : "" + c.DatumIstovara).ToLower().Contains(param.sSearch.ToLower())
              || (String.IsNullOrEmpty("" + c.Status) ? "" : "" + c.Status).ToLower().Contains(param.sSearch.ToLower())
                                                                 );

         }

         var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);

         Func<DnevnikPrevoza, object> orderingFunction = c =>
         {
             switch (sortColumnIndex)
             {
                 // case 0: return c.SerijskiBroj.Length > 10 ? c.SerijskiBroj.Substring(0, 10) : c.SerijskiBroj;
                 case 0: return c.SerijskiBroj.Length > 10 ? c.SerijskiBroj : c.SerijskiBroj+"-9";
                 case 1: return c.DatumDnevnika;
                 case 2: return c.IdNarucioca;
                 case 3: return (c.DnevnikUvoznikIzvoznik.Count == 0) ? "" : c.DnevnikUvoznikIzvoznik.FirstOrDefault().Uvoznik + c.DnevnikUvoznikIzvoznik.FirstOrDefault().Izvoznik;
                 case 4: return c.UtovarAdresa;
                 case 5: return c.DatumUtovara;
                 case 6: return c.UtovarAdresa;
                 case 7: return c.DatumIstovara;
                 case 8: return Valute.Contains(c.IdDnevnik);
                 case 9: return dok.Contains(c.IdDnevnik);
                 case 10: return VozacVozilo.Contains(c.IdDnevnik);
                 case 11: return c.Status.Equals("NIJE UTOVARENO") ? 10 : c.Status.Equals("UTOVARENO U TRANZITU") ? 5 : 1;
                 
                 default: return c.SerijskiBroj.Length > 10 ? c.SerijskiBroj.Substring(0,10) : c.SerijskiBroj ;
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
							String.IsNullOrEmpty("" + c.DatumDnevnika) ? "" :   DateTime.Parse(c.DatumDnevnika.ToString()).ToString("dd.MM.yyyy"),
							(c.Subjekt1 != null ) ? c.Subjekt1.Naziv : "",
							c.DnevnikUvoznikIzvoznik.Count == 0 ? "" :  String.Join("   ",  c.DnevnikUvoznikIzvoznik.Select( g => g.Uvoznik + " / " + g.Izvoznik).ToList()),
                            (c.DnevnikUtovar.Count == 0) ? c.UtovarFirma + ", " + c.UtovarAdresa + ", " + c.UtovarPTT + " " + c.UtovarGrad + ", " + c.UtovarDrzava : "1. " + c.UtovarFirma + ", "+ c.UtovarAdresa + ", "+ c.UtovarPTT + " " + c.UtovarGrad +  ", " + c.UtovarDrzava + System.Environment.NewLine + String.Join(System.Environment.NewLine, c.DnevnikUtovar.Select(g =>  "     "+(c.DnevnikUtovar.ToList().IndexOf(g)+2)+ ". "+ g.Firma + ", " + g.Adresa + ", " + g.PTT + " " +  g.Mjesto + ", " + g.Drzava )),
                            String.IsNullOrEmpty("" + c.DatumUtovara) ? "" :   DateTime.Parse(c.DatumUtovara.ToString()).ToString("dd.MM.yyyy"),
							(c.DnevnikIstovar.Count == 0) ? c.IstovarFirma + ", " + c.IstovarAdresa  + " " + c.IstovarPTT + " " + c.IstovarGrad + " " + c.IstovarDrzava: "1. " + c.IstovarFirma+ ", "+ c.IstovarAdresa + ", " + c.IstovarPTT + " " + c.IstovarGrad + ", " + c.IstovarDrzava + System.Environment.NewLine + String.Join(System.Environment.NewLine, c.DnevnikIstovar.Select(g => "    "+(c.DnevnikIstovar.ToList().IndexOf(g)+2)+ ". "+ g.Firma +", " + g.Adresa + ", " + g.PTT + " " + g.Mjesto + ", " + g.Drzava)),
							String.IsNullOrEmpty("" + c.DatumIstovara) ? "" :   DateTime.Parse(c.DatumIstovara.ToString()).ToString("dd.MM.yyyy"),
                            dok.Contains(c.IdDnevnik) ? "1" : "0",
                            VozacVozilo.Contains(c.IdDnevnik) ? "1" : "0",
                            c.Status,
							c.IdDnevnik,
                            Parenti.Contains(c.IdDnevnik) ? "1" : "0",
                            Valute.Contains(c.IdDnevnik) ? "1" : "0"


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
        public ActionResult IndexAjax2(jQueryDataTableParamModel param)
        {

            var FSerijskiBroj = Convert.ToString(Request["sSearch_0"]).ToLower();
            var FNaziv = Convert.ToString(Request["sSearch_1"]).ToLower();
            var FUVIV = Convert.ToString(Request["sSearch_2"]).ToLower();
            var FUADRESA = Convert.ToString(Request["sSearch_3"]).ToLower();
            var FIADRESA = Convert.ToString(Request["sSearch_4"]).ToLower();
            var FUI = Convert.ToString(Request["sSearch_5"]).ToLower();
            var FVAL = Convert.ToString(Request["sSearch_6"]).ToLower();
            var FUPOZORENJE = Convert.ToString(Request["sSearch_7"]);
            var FSTATUS = Convert.ToString(Request["sSearch_8"]).ToLower();

            var GlobalSearch = string.IsNullOrEmpty(param.sSearch) ? "" : param.sSearch;
            ObjectCache cache = MemoryCache.Default;
            List<DnavnikPrevozaIndexModel> list = null;

            if (!(FSerijskiBroj + FNaziv + FUVIV + FUADRESA + FIADRESA + FUI + FVAL + FSTATUS + FUPOZORENJE + GlobalSearch).Equals(""))
            {
                list = (List<DnavnikPrevozaIndexModel>)cache.Get("DnevnikIndex");
            }

            if (list == null)
            {
                List<int> SviKojiSuParenti = db.DnevnikPrevoza.Where(c => c.IdDnevnikParent.HasValue).Select(c => c.IdDnevnikParent.Value ).ToList();

                list = db.DnevnikPrevoza.Include(d => d.Subjekt).Include(d => d.Subjekt1).Include(d => d.DnevnikUvoznikIzvoznik).Include(d => d.DnevnikUtovar).Include(d => d.DnevnikIstovar).Include(d => d.Dokument).Include(d => d.Vozilo).Include(d => d.Vozaci).
                   Where(k => (k.ZapisAktivan ?? false)).ToList().
                   Select(c => new DnavnikPrevozaIndexModel
                   {
                       SerijskiBroj = c.SerijskiBroj,
                       DatumDnevnika = String.IsNullOrEmpty("" + c.DatumDnevnika) ? "" : DateTime.Parse(c.DatumDnevnika.ToString()).ToString("dd.MM.yyyy"),
                       Subjekt = (c.Subjekt1 != null) ? c.Subjekt1.Naziv : "",
                       UvoznikIzvoznik = c.DnevnikUvoznikIzvoznik.Count == 0 ? "" : String.Join("   ", c.DnevnikUvoznikIzvoznik.Select(g => g.Uvoznik + " / " + g.Izvoznik).ToList()),
                       Utovari = (c.DnevnikUtovar.Count == 0) ? c.UtovarFirma + ", " + c.UtovarAdresa + ", " + c.UtovarPTT + " " + c.UtovarGrad + ", " + c.UtovarDrzava : "1. " + c.UtovarFirma + ", " + c.UtovarAdresa + ", " + c.UtovarPTT + " " + c.UtovarGrad + ", " + c.UtovarDrzava + System.Environment.NewLine + String.Join(System.Environment.NewLine, c.DnevnikUtovar.Select(g => "     " + (c.DnevnikUtovar.ToList().IndexOf(g) + 2) + ". " + g.Firma + ", " + g.Adresa + ", " + g.PTT + " " + g.Mjesto + ", " + g.Drzava)),
                       DatumUtovara = String.IsNullOrEmpty("" + c.DatumUtovara) ? "" : DateTime.Parse(c.DatumUtovara.ToString()).ToString("dd.MM.yyyy"),
                       Istovari = (c.DnevnikIstovar.Count == 0) ? c.IstovarFirma + ", " + c.IstovarAdresa + " " + c.IstovarPTT + " " + c.IstovarGrad + " " + c.IstovarDrzava : "1. " + c.IstovarFirma + ", " + c.IstovarAdresa + ", " + c.IstovarPTT + " " + c.IstovarGrad + ", " + c.IstovarDrzava + System.Environment.NewLine + String.Join(System.Environment.NewLine, c.DnevnikIstovar.Select(g => "    " + (c.DnevnikIstovar.ToList().IndexOf(g) + 2) + ". " + g.Firma + ", " + g.Adresa + ", " + g.PTT + " " + g.Mjesto + ", " + g.Drzava)),
                       DatumIstovara = String.IsNullOrEmpty("" + c.DatumIstovara) ? "" : DateTime.Parse(c.DatumIstovara.ToString()).ToString("dd.MM.yyyy"),
                       Dokument = c.Dokument.Count() > 0 ? "A1" : "A0",
                       VozacVozilo = ((c.Vozilo == null || c.Vozaci == null) && !(c.DrugiPrevoznik ?? false)) ? "B1" : "B0",
                       Status = c.Status,
                       IdDnevnik = c.IdDnevnik.ToString(),
                       Parenti = SviKojiSuParenti.Contains(c.IdDnevnik) ? "1" : "0",
                       Valute = (
                                                   (!(c.Uplaceno ?? false)) && (((c.DatumSlanjaFakture ?? DateTime.Now.AddHours(7)).AddDays(c.ValutaPlacanja ?? 999999) < DateTime.Now.AddHours(7)))
                                               ) ? "1" : ((c.Uplaceno ?? false) ? "2" : "0"),
                       Uplaceno = (c.Uplaceno ?? false),
                       Skladiste = c.idStatusDetaljni  == 1 ? "C1" :  "C0",
                       DrugiPrevoznikTranzit = c.idStatusDetaljni == 6 ? "D1" : "D0" ,
                       PoslataFaktura = c.Status.Equals("ISTOVARENO") ? ((c.DatumSlanjaFakture == null || c.BrojFakture == null) ? "E1" : "E0" ) : "E0",
                       BrzaPosta = c.idStatusDetaljni == 7 ? "F1" : "F0",
                   }).
                    ToList();


                cache.Remove("DnevnikIndex");
                cache.Add("DnevnikIndex", list, DateTime.Now.AddHours(8));

            }
            
            

            /*
            List<int> dok = db.Dokument.Where(c => c.IdDnevnik.HasValue).Select(c => c.IdDnevnik ?? 0).Distinct().ToList();
            List<int> VozacVozilo = db.DnevnikPrevoza.Where(c => (c.ZapisAktivan ?? false) && (c.Vozilo == null || c.Vozaci == null) && !(c.DrugiPrevoznik ?? false)).Select(c => c.IdDnevnik).ToList();
            List<int> Parenti = db.DnevnikPrevoza.Where(c => (c.ZapisAktivan ?? false) && c.IdDnevnikParent != null).Select(c => c.IdDnevnikParent ?? 0).ToList();

            List<int> Valute = db.DnevnikPrevoza.Where(c => (!(c.Uplaceno ?? false))).ToList()
              .Where(c => ((c.DatumSlanjaFakture ?? DateTime.Now.AddHours(7)).AddDays(c.ValutaPlacanja ?? 999999) < DateTime.Now.AddHours(7))).Select(c => c.IdDnevnik).ToList();
              */
    
            IEnumerable<DnavnikPrevozaIndexModel> filteredList = list;

           filteredList = filteredList.Where(c =>
               c.SerijskiBroj.ToLower().Contains(FSerijskiBroj)
            && c.Subjekt.ToLower().Contains(FNaziv)
            && c.UvoznikIzvoznik.ToLower().Contains(FUVIV)
            && c.Utovari.ToLower().Contains(FUADRESA)
            && c.DatumUtovara.ToLower().Contains(FUI)
            && c.Istovari.ToLower().Contains(FIADRESA)
            && c.DatumIstovara.ToLower().Contains(FUI)
            && (c.Valute.Equals("1") ? "istekla" : ((c.Uplaceno) ? "placeno" : "u valuti")).Contains(FVAL)
            && c.Status.ToLower().Contains(FSTATUS)
            && (c.VozacVozilo + c.Dokument + c.Skladiste + c.DrugiPrevoznikTranzit + c.PoslataFaktura + c.BrzaPosta ).Contains(FUPOZORENJE)
            );

            //  Valute.Contains(c.IdDnevnik) ? "1" : ((c.Uplaceno ?? false) ? "0" : "2" )
            // 1 - nije uplacno i istekla valuta
            // 0 - nije uplaceno i nije istekla
            // 2 - uplaceno

            // param.sSearch.ToLower()

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredList = filteredList.Where(c =>
               c.SerijskiBroj.ToLower().Contains(param.sSearch.ToLower())
            || c.Subjekt.ToLower().Contains(param.sSearch.ToLower())
            || c.UvoznikIzvoznik.ToLower().Contains(param.sSearch.ToLower())
            || c.Utovari.ToLower().Contains(param.sSearch.ToLower())
            || c.DatumUtovara.ToLower().Contains(param.sSearch.ToLower())
            || c.Istovari.ToLower().Contains(param.sSearch.ToLower())
            || c.DatumIstovara.ToLower().Contains(param.sSearch.ToLower())
            || (c.Valute.Equals("1") ? "istekla" : ((c.Uplaceno) ? "placeno" : "u valuti")).Contains(param.sSearch.ToLower())
            || c.Status.ToLower().Contains(param.sSearch.ToLower())

 );

            }

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);

            Func<DnavnikPrevozaIndexModel, object> orderingFunction = c =>
            {
                switch (sortColumnIndex)
                {
                    case 0: return c.SerijskiBroj.Length > 10 ? c.SerijskiBroj : c.SerijskiBroj + "-9";  
                    case 1: return c.Subjekt;
                    case 2: return c.UvoznikIzvoznik;
                    case 3: return c.Utovari;
                    case 4: return c.Istovari;
                    case 5: return c.DatumIstovara;
                    case 6: return c.Valute;
                    case 7: return c.Dokument;
                    case 8: return c.Status.Equals("NIJE UTOVARENO") ? 10 : c.Status.Equals("UTOVARENO U TRANZITU") ? 5 : 1;
                    default: return c.IdDnevnik;
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
                                c.SerijskiBroj      ,
                                c.DatumDnevnika     ,
                                c.Subjekt           ,
                                c.UvoznikIzvoznik   ,
                                c.Utovari           ,
                                c.DatumUtovara      ,
                                c.Istovari          ,
                                c.DatumIstovara     ,
                                c.Dokument          ,
                                c.VozacVozilo       ,
                                c.Status            ,
                                c.IdDnevnik         ,
                                c.Parenti           ,
                                c.Valute            ,
                                c.Skladiste         ,
                                c.DrugiPrevoznikTranzit,
                                c.PoslataFaktura,
                                c.BrzaPosta

                            // 1 - nije uplacno i istekla valuta
                            // 0 - nije uplaceno i nije istekla
                            // 2 - uplaceno

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

        /*
           [Authorize]
        public ActionResult IndexAjax2(jQueryDataTableParamModel param)
        {

            var FSerijskiBroj = Convert.ToString(Request["sSearch_0"]).ToLower();
            var FNaziv = Convert.ToString(Request["sSearch_1"]).ToLower();
            var FUVIV = Convert.ToString(Request["sSearch_2"]).ToLower();
            var FUADRESA = Convert.ToString(Request["sSearch_3"]).ToLower();
            var FIADRESA = Convert.ToString(Request["sSearch_4"]).ToLower();
            var FUI = Convert.ToString(Request["sSearch_5"]).ToLower();
            var FVAL = Convert.ToString(Request["sSearch_6"]).ToLower();
            var FSTATUS = Convert.ToString(Request["sSearch_8"]).ToLower();



            var list = db.DnevnikPrevoza.Include(d => d.Subjekt).Include(d => d.Subjekt1).Include(d => d.DnevnikUvoznikIzvoznik).Include(d => d.DnevnikUtovar).Include(d => d.DnevnikIstovar).Where(k => (k.ZapisAktivan ?? false)).ToList();
            List<int> dok = db.Dokument.Where(c => c.IdDnevnik.HasValue).Select(c => c.IdDnevnik ?? 0).Distinct().ToList();
            List<int> VozacVozilo = db.DnevnikPrevoza.Where(c => (c.ZapisAktivan ?? false) && (c.Vozilo == null || c.Vozaci == null) && !(c.DrugiPrevoznik ?? false)).Select(c => c.IdDnevnik).ToList();
            List<int> Parenti = db.DnevnikPrevoza.Where(c => (c.ZapisAktivan ?? false) && c.IdDnevnikParent != null).Select(c => c.IdDnevnikParent ?? 0).ToList();

            List<int> Valute = db.DnevnikPrevoza.Where(c => (!(c.Uplaceno ?? false))).ToList()
              .Where(c => ((c.DatumSlanjaFakture ?? DateTime.Now.AddHours(7)).AddDays(c.ValutaPlacanja ?? 999999) < DateTime.Now.AddHours(7))).Select(c => c.IdDnevnik).ToList();

    
            IEnumerable<DnevnikPrevoza> filteredList = list;

           filteredList = filteredList.Where(c =>
            (String.IsNullOrEmpty("" + c.SerijskiBroj) ? "" : "" + c.SerijskiBroj).ToLower().Contains(FSerijskiBroj)
            && ((c.Subjekt1 != null) ? c.Subjekt1.Naziv : "").ToLower().Contains(FNaziv)
            && ((c.DnevnikUvoznikIzvoznik.Count == 0) ? "" : String.Join("   ", c.DnevnikUvoznikIzvoznik.Select(g => g.Uvoznik + " / " + g.Izvoznik).ToList())).ToLower().Contains(FUVIV)
            && (String.IsNullOrEmpty("" + c.UtovarAdresa + c.UtovarFirma + c.UtovarPTT + c.UtovarGrad + c.UtovarDrzava + String.Join(" ", c.DnevnikUtovar.Select(g => g.Adresa + g.PTT + g.Firma + g.Mjesto + g.Drzava))) ? "" : "" + c.UtovarAdresa + c.UtovarFirma + c.UtovarPTT + c.UtovarGrad + c.UtovarDrzava + String.Join(" ", c.DnevnikUtovar.Select(g => g.Adresa + g.PTT + g.Firma + g.Mjesto + g.Drzava))).ToLower().Contains(FUADRESA)
            && (String.IsNullOrEmpty("" + c.DatumUtovara) ? "" : "" + c.DatumUtovara).ToLower().Contains(FUI)
            && (String.IsNullOrEmpty("" + c.IstovarAdresa + c.IstovarFirma + c.IstovarPTT + c.IstovarGrad + c.IstovarDrzava + String.Join(" ", c.DnevnikIstovar.Select(g => g.Adresa + g.Firma + g.PTT + g.Mjesto + g.Drzava))) ? "" : "" + c.IstovarAdresa + c.IstovarFirma + c.IstovarPTT + c.IstovarGrad + c.IstovarDrzava + String.Join(" ", c.DnevnikIstovar.Select(g => g.Adresa + g.PTT + g.Firma + g.Mjesto + g.Drzava))).ToLower().Contains(FIADRESA)
            && (String.IsNullOrEmpty("" + c.DatumIstovara) ? "" : "" + c.DatumIstovara).ToLower().Contains(FUI)
            && ((Valute.Contains(c.IdDnevnik) ? "istekla" : ((c.Uplaceno ?? false) ? "placeno" : "u valuti"))  ).Contains(FVAL)
            && (String.IsNullOrEmpty("" + c.Status) ? "" : "" + c.Status).ToLower().Contains(FSTATUS)
            );

            //  Valute.Contains(c.IdDnevnik) ? "1" : ((c.Uplaceno ?? false) ? "0" : "2" )
            // 1 - nije uplacno i istekla valuta
            // 0 - nije uplaceno i nije istekla
            // 2 - uplaceno

            if (!string.IsNullOrEmpty(param.sSearch))
            {


                filteredList = filteredList.Where(c =>
    (String.IsNullOrEmpty("" + c.SerijskiBroj) ? "" : "" + c.SerijskiBroj).ToLower().Contains(param.sSearch.ToLower())
 || (String.IsNullOrEmpty("" + c.DatumDnevnika) ? "" : "" + c.DatumDnevnika).ToLower().Contains(param.sSearch.ToLower())
 || ((c.Subjekt1 != null) ? c.Subjekt1.Naziv : "").ToLower().Contains(param.sSearch.ToLower())
 || ((c.DnevnikUvoznikIzvoznik.Count == 0) ? "" : String.Join("   ", c.DnevnikUvoznikIzvoznik.Select(g => g.Uvoznik + " / " + g.Izvoznik).ToList())).ToLower().Contains(param.sSearch.ToLower())
 || (String.IsNullOrEmpty("" + c.UtovarAdresa + c.UtovarFirma + c.UtovarPTT + c.UtovarGrad + c.UtovarDrzava + String.Join(" ", c.DnevnikUtovar.Select(g => g.Adresa + g.PTT + g.Firma + g.Mjesto + g.Drzava))) ? "" : "" + c.UtovarAdresa + c.UtovarFirma + c.UtovarPTT + c.UtovarGrad + c.UtovarDrzava + String.Join(" ", c.DnevnikUtovar.Select(g => g.Adresa + g.PTT + g.Firma + g.Mjesto+g.Drzava))).ToLower().Contains(param.sSearch.ToLower())
 || (String.IsNullOrEmpty("" + c.DatumUtovara) ? "" : "" + c.DatumUtovara).ToLower().Contains(param.sSearch.ToLower())
 || (String.IsNullOrEmpty("" + c.IstovarAdresa + c.IstovarFirma + c.IstovarPTT + c.IstovarGrad +c.IstovarDrzava + String.Join(" ", c.DnevnikIstovar.Select(g => g.Adresa + g.Firma + g.PTT + g.Mjesto + g.Drzava))) ? "" : "" + c.IstovarAdresa + c.IstovarFirma + c.IstovarPTT + c.IstovarGrad + c.IstovarDrzava + String.Join(" ", c.DnevnikIstovar.Select(g => g.Adresa + g.PTT + g.Firma + g.Mjesto + g.Drzava))).ToLower().Contains(param.sSearch.ToLower())
 || (String.IsNullOrEmpty("" + c.DatumIstovara) ? "" : "" + c.DatumIstovara).ToLower().Contains(param.sSearch.ToLower())
 || (String.IsNullOrEmpty("" + c.Status) ? "" : "" + c.Status).ToLower().Contains(param.sSearch.ToLower())
 );

            }

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);

            Func<DnevnikPrevoza, object> orderingFunction = c =>
            {
                switch (sortColumnIndex)
                {
                    case 0: return c.SerijskiBroj.Length > 10 ? c.SerijskiBroj : c.SerijskiBroj + "-9";  
                    case 1: return c.Subjekt1.Naziv;
                    case 2: return (c.DnevnikUvoznikIzvoznik.Count == 0) ? "" : c.DnevnikUvoznikIzvoznik.FirstOrDefault().Uvoznik + c.DnevnikUvoznikIzvoznik.FirstOrDefault().Izvoznik;
                    case 3: return c.UtovarFirma + ", " + c.UtovarAdresa + ", " + c.UtovarPTT + " " + c.UtovarGrad;
                    case 4: return c.IstovarFirma + ", " + c.IstovarAdresa + ", " + c.IstovarPTT + " " + c.IstovarGrad;
                    case 5: return c.DatumIstovara;
                    case 6: return Valute.Contains(c.IdDnevnik) ? "1" : ((c.Uplaceno ?? false) ? "2" : "0");
                    case 7: return dok.Contains(c.IdDnevnik);
                    case 8: return c.Status.Equals("NIJE UTOVARENO") ? 10 : c.Status.Equals("UTOVARENO U TRANZITU") ? 5 : 1;
                    default: return c.IdDnevnik;
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
                            String.IsNullOrEmpty("" + c.DatumDnevnika) ? "" :   DateTime.Parse(c.DatumDnevnika.ToString()).ToString("dd.MM.yyyy"),
                            (c.Subjekt1 != null ) ? c.Subjekt1.Naziv : "",
                            c.DnevnikUvoznikIzvoznik.Count == 0 ? "" :  String.Join("   ",  c.DnevnikUvoznikIzvoznik.Select( g => g.Uvoznik + " / " + g.Izvoznik).ToList()),
                            (c.DnevnikUtovar.Count == 0) ? c.UtovarFirma + ", " + c.UtovarAdresa + ", " + c.UtovarPTT + " " + c.UtovarGrad + ", " + c.UtovarDrzava : "1. " + c.UtovarFirma + ", "+ c.UtovarAdresa + ", "+ c.UtovarPTT + " " + c.UtovarGrad +  ", " + c.UtovarDrzava + System.Environment.NewLine + String.Join(System.Environment.NewLine, c.DnevnikUtovar.Select(g =>  "     "+(c.DnevnikUtovar.ToList().IndexOf(g)+2)+ ". "+ g.Firma + ", " + g.Adresa + ", " + g.PTT + " " +  g.Mjesto + ", " + g.Drzava )),
                            String.IsNullOrEmpty("" + c.DatumUtovara) ? "" :   DateTime.Parse(c.DatumUtovara.ToString()).ToString("dd.MM.yyyy"),
                            (c.DnevnikIstovar.Count == 0) ? c.IstovarFirma + ", " + c.IstovarAdresa  + " " + c.IstovarPTT + " " + c.IstovarGrad + " " + c.IstovarDrzava: "1. " + c.IstovarFirma+ ", "+ c.IstovarAdresa + ", " + c.IstovarPTT + " " + c.IstovarGrad + ", " + c.IstovarDrzava + System.Environment.NewLine + String.Join(System.Environment.NewLine, c.DnevnikIstovar.Select(g => "    "+(c.DnevnikIstovar.ToList().IndexOf(g)+2)+ ". "+ g.Firma +", " + g.Adresa + ", " + g.PTT + " " + g.Mjesto + ", " + g.Drzava)),
                            String.IsNullOrEmpty("" + c.DatumIstovara) ? "" :   DateTime.Parse(c.DatumIstovara.ToString()).ToString("dd.MM.yyyy"),
                            dok.Contains(c.IdDnevnik) ? "1" : "0",
                            VozacVozilo.Contains(c.IdDnevnik) ? "1" : "0",
                            c.Status,
                            c.IdDnevnik,
                            Parenti.Contains(c.IdDnevnik) ? "1" : "0",

                            Valute.Contains(c.IdDnevnik) ? "1" : ((c.Uplaceno ?? false) ? "2" : "0" )

                            // 1 - nije uplacno i istekla valuta
                            // 0 - nije uplaceno i nije istekla
                            // 2 - uplaceno

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
         */



        [Authorize]
        public ActionResult NefakturisaniPrevozi()
        {
            return View(new List<DnevnikPrevoza>());
        }

        [Authorize]
        public ActionResult IndexAjaxNefakturisani(jQueryDataTableParamModel param)
        {
          
            var list = db.DnevnikPrevoza.Include(d => d.Subjekt).Include(d => d.Subjekt1).Where(k => (k.ZapisAktivan ?? false) && !k.Status.Equals("STOPIRANO") && k.BrojFakture == null && k.IdDnevnikParent == null).ToList();
            List<int> dok = db.Dokument.Where(c => c.IdDnevnik.HasValue).Select(c => c.IdDnevnik ?? 0).Distinct().ToList();
            IEnumerable<DnevnikPrevoza> filteredList;

            var FSerijskiBroj = Convert.ToString(Request["sSearch_0"]).ToLower();
            var FDatumDnevnika = Convert.ToString(Request["sSearch_1"]).ToLower();
            var FIdNarucioca = Convert.ToString(Request["sSearch_2"]).ToLower();
            var FBrojNaloga = Convert.ToString(Request["sSearch_3"]).ToLower();
            var FUtovar = Convert.ToString(Request["sSearch_4"]).ToLower();
            var FDatumUtovara = Convert.ToString(Request["sSearch_5"]).ToLower();
            var FIstovar = Convert.ToString(Request["sSearch_6"]).ToLower();
            var FDatumIstovara = Convert.ToString(Request["sSearch_7"]).ToLower();
            var FStatus = Convert.ToString(Request["sSearch_9"]).ToLower();


            filteredList = list.Where(c =>
                     (String.IsNullOrEmpty("" + c.SerijskiBroj) ? "" : "" + c.SerijskiBroj).ToLower().Contains(FSerijskiBroj)
                     && (String.IsNullOrEmpty("" + c.DatumDnevnika) ? "" : "" + c.DatumDnevnika).ToLower().Contains(FDatumDnevnika)
                      && ((c.Subjekt1 != null) ? c.Subjekt1.Naziv : "").ToLower().Contains(FIdNarucioca)
                     && (String.IsNullOrEmpty("" + c.BrojNaloga) ? "" : "" + c.BrojNaloga).ToLower().Contains(FBrojNaloga)
                     && (String.IsNullOrEmpty("" + c.UtovarAdresa + c.UtovarFirma + c.UtovarGrad) ? "" : "" + c.UtovarAdresa + c.UtovarFirma + c.UtovarGrad).ToLower().Contains(FUtovar)
                     && (String.IsNullOrEmpty("" + c.DatumUtovara) ? "" : "" + c.DatumUtovara).ToLower().Contains(FDatumUtovara)
                     && (String.IsNullOrEmpty("" + c.IstovarAdresa + c.IstovarFirma + c.IstovarGrad) ? "" : "" + c.IstovarAdresa + c.IstovarFirma + c.IstovarGrad).ToLower().Contains(FIstovar)
                     && (String.IsNullOrEmpty("" + c.DatumIstovara) ? "" : "" + c.DatumIstovara).ToLower().Contains(FDatumIstovara)
                      && (String.IsNullOrEmpty("" + c.Status) ? "" : "" + c.Status).ToLower().Contains(FStatus)


                                              );

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredList = filteredList.Where(c =>
                    (String.IsNullOrEmpty("" + c.SerijskiBroj) ? "" : "" + c.SerijskiBroj).ToLower().Contains(param.sSearch.ToLower())
                 || (String.IsNullOrEmpty("" + c.DatumDnevnika) ? "" : "" + c.DatumDnevnika).ToLower().Contains(param.sSearch.ToLower())
                 || ((c.Subjekt1 != null) ? c.Subjekt1.Naziv : "").ToLower().Contains(param.sSearch.ToLower())
                 || (String.IsNullOrEmpty("" + c.BrojNaloga) ? "" : "" + c.BrojNaloga).ToLower().Contains(param.sSearch.ToLower())
                 || (String.IsNullOrEmpty("" + c.UtovarAdresa + c.UtovarFirma + c.UtovarGrad) ? "" : "" + c.UtovarAdresa + c.UtovarFirma + c.UtovarGrad).ToLower().Contains(param.sSearch.ToLower())
                 || (String.IsNullOrEmpty("" + c.DatumUtovara) ? "" : "" + c.DatumUtovara).ToLower().Contains(param.sSearch.ToLower())
                 || (String.IsNullOrEmpty("" + c.IstovarAdresa + c.IstovarFirma + c.IstovarGrad) ? "" : "" + c.IstovarAdresa + c.IstovarFirma + c.IstovarGrad).ToLower().Contains(param.sSearch.ToLower())
                 || (String.IsNullOrEmpty("" + c.DatumIstovara) ? "" : "" + c.DatumIstovara).ToLower().Contains(param.sSearch.ToLower())
                 || (String.IsNullOrEmpty("" + c.Status) ? "" : "" + c.Status).ToLower().Contains(param.sSearch.ToLower())
                                                                    );

            }

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);

            Func<DnevnikPrevoza, object> orderingFunction = c =>
            {
                switch (sortColumnIndex)
                {
                    case 0: return c.SerijskiBroj;
                    case 1: return c.DatumDnevnika;
                    case 2: return c.IdNarucioca;
                    case 3: return c.BrojNaloga;
                    case 4: return c.UtovarAdresa;
                    case 5: return c.DatumUtovara;
                    case 6: return c.UtovarAdresa;
                    case 7: return c.DatumIstovara;
                    case 9: return c.Status;
                    default: return c.IdDnevnik;
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
							String.IsNullOrEmpty("" + c.DatumDnevnika) ? "" :   DateTime.Parse(c.DatumDnevnika.ToString()).ToString("dd.MM.yyyy"),
							(c.Subjekt1 != null ) ? c.Subjekt1.Naziv : "",
							c.BrojNaloga,
							c.UtovarFirma + " / " + c.UtovarAdresa + " / " + c.UtovarGrad,
							String.IsNullOrEmpty("" + c.DatumUtovara) ? "" :   DateTime.Parse(c.DatumUtovara.ToString()).ToString("dd.MM.yyyy"),
							c.IstovarFirma + " / " + c.IstovarAdresa  + " / " + c.IstovarGrad,
							String.IsNullOrEmpty("" + c.DatumIstovara) ? "" :   DateTime.Parse(c.DatumIstovara.ToString()).ToString("dd.MM.yyyy"),
                            dok.Contains(c.IdDnevnik) ? "1" : "0",
                            c.Status,
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
        // GET: /DnevnikPrevoza/Details/5
        [Authorize]
        public ActionResult Details(int id = 0)
        {
            return RedirectToAction("DetailsTab", new { id = id });
            /*
            DnevnikPrevoza dnevnikprevoza = db.DnevnikPrevoza.Include(d => d.Subjekt).Include(d => d.Subjekt1).Include(d => d.Vozaci).Include(d => d.Vozilo).Include(d => d.Vozilo1).Include(d => d.Valuta).Include(d => d.Valuta1).Include(d => d.Dokument).Where(k => k.IdDnevnik == id).FirstOrDefault();
         

            decimal suma = (dnevnikprevoza.CijenaPrevoza ?? 0) * (dnevnikprevoza.Valuta.UKM ?? 0) - (dnevnikprevoza.CijenaPrevozaPrevoznika ?? 0) * (dnevnikprevoza.Valuta1 == null ? 0 : dnevnikprevoza.Valuta1.UKM ?? 0);
            decimal troskovi = 0;
            if (db.Troskovi.Where(c => c.IdDnevnik == id).Count() > 0)
            troskovi = db.Troskovi.Where(c => c.IdDnevnik == id).Sum(c => (c.Iznos ?? 0) * (c.Valuta.UKM ?? 0));

            ViewBag.Suma = suma.ToString("0.00");
            ViewBag.Trosak = troskovi.ToString("0.00");

            String korisnik = "";
            var korisnici = db.Log.Where(c => c.PK == id && c.Tabela.Equals("DnevnikPrevoza") && c.Aktivnost.Equals("Added")).ToList();


            if (korisnici.Count == 0) korisnik = "Admin";
            else korisnik = korisnici[0].KorisnickoIme;

            ViewBag.Korisnik = korisnik;

            if (dnevnikprevoza == null)
            {
                return HttpNotFound();
            }
            return View(dnevnikprevoza);
            */
        }


        [Authorize]
        public ActionResult DetailsTab(int id = 0)
        {
            // DnevnikPrevoza dnevnikprevoza = db.DnevnikPrevoza.Find(id);
            DnevnikPrevoza dnevnikprevoza = db.DnevnikPrevoza.Include(d => d.Subjekt).Include(d => d.Subjekt1).Include(d => d.Vozaci).Include(d => d.Vozilo).Include(d => d.Vozilo1).Include(d => d.Valuta).Include(d => d.Valuta1).Include(d => d.Dokument).Where(k => k.IdDnevnik == id).FirstOrDefault();


            decimal suma = (dnevnikprevoza.CijenaPrevoza ?? 0) * (dnevnikprevoza.Valuta.UKM ?? 0) - (dnevnikprevoza.CijenaPrevozaPrevoznika ?? 0) * (dnevnikprevoza.Valuta1 == null ? 0 : dnevnikprevoza.Valuta1.UKM ?? 0);
            decimal troskovi = 0;
            decimal troskoviStvarni = 0;
            if (db.Troskovi.Where(c => c.IdDnevnik == id).Count() > 0)
            {
                var x = db.Troskovi.Where(c => c.IdDnevnik == id);
                troskovi = x.Sum(c => (c.Iznos ?? 0) * (c.Valuta.UKM ?? 0));
                troskoviStvarni = x.Sum(c => (c.StvarniTrosak ?? 0) * (c.Valuta.UKM ?? 0));
            }

           


            List<SumePoddnevnika> lstPoddnevika = db.DnevnikPrevoza.Where(c => c.IdDnevnikParent == id).ToList().Select(c => 
            new SumePoddnevnika { SerijskiBroj = c.SerijskiBroj,
                  mojacijena = (c.CijenaPrevoza ?? 0) * (c.Valuta.UKM ?? 0),
                  drugiprevoznik = (c.CijenaPrevozaPrevoznika ?? 0) * (c.Valuta1 == null ? 0 : c.Valuta1.UKM ?? 0),
                  suma = (c.CijenaPrevoza ?? 0) * (c.Valuta.UKM ?? 0 )  - (c.CijenaPrevozaPrevoznika ?? 0) * ( c.Valuta1 == null ? 0 : c.Valuta1.UKM ?? 0),
                  troskovi = c.Troskovi.Sum(d => (d.Iznos ?? 0) * (d.Valuta.UKM ?? 0)),
                  troskoviStvarni = c.Troskovi.Sum(d => (d.StvarniTrosak ?? 0) * (d.Valuta.UKM ?? 0))
            }).ToList();
            ViewBag.PodDnevnikSume = lstPoddnevika;



            ViewBag.Suma = (suma + lstPoddnevika.Sum(c=> c.suma) + troskovi - troskoviStvarni + lstPoddnevika.Sum(c => c.troskovi) - lstPoddnevika.Sum(c => c.troskoviStvarni)).ToString("0.00");
            ViewBag.Trosak = troskovi.ToString("0.00");
            ViewBag.TrosakStvarni = troskoviStvarni.ToString("0.00");

            String korisnik = "";
            var korisnici = db.Log.Where(c => c.PK == id && c.Tabela.Equals("DnevnikPrevoza") && c.Aktivnost.Equals("Added")).ToList();


            if (korisnici.Count == 0) korisnik = "Admin";
            else korisnik = korisnici[0].KorisnickoIme;

            ViewBag.Korisnik = korisnik;

            var DodatneCarine = dnevnikprevoza.DnevnikCarina.Select(c => new { IzvoznaCarina = c.IzvoznaCarina, UvoznaCarina = c.UvoznaCarina }).ToList();
            DodatneCarine.Insert(0,(new { IzvoznaCarina = dnevnikprevoza.IzvoznaSpedicija, UvoznaCarina = dnevnikprevoza.UvoznaSpedicija }));
            int len1 = DodatneCarine.Count;

            var DodatniUI = dnevnikprevoza.DnevnikUvoznikIzvoznik.Select(c => new { Izvoznik = c.Izvoznik, Uvoznik = c.Uvoznik }).ToList();
            int len2 = DodatniUI.Count;

            List<CarineUvoznikIzvoznik> CARINEUI = DodatneCarine.Select(c => new CarineUvoznikIzvoznik { IzvoznaCarina = "", UvoznaCarina = "", Izvoznik = "", Uvoznik = "" }).Where(c => false).ToList();

            int len = len1 >= len2 ? len1 : len2;

            for (int i = 0; i < len; i++)
            {
                var dc = i < len1 ? DodatneCarine.ElementAt(i) : null;
                var dui = i < len2 ? DodatniUI.ElementAt(i) : null;

                CARINEUI.Add(new CarineUvoznikIzvoznik  {
                    IzvoznaCarina = dc == null ? "" : dc.IzvoznaCarina,
                    UvoznaCarina = dc == null ? "" : dc.UvoznaCarina,
                    Izvoznik = dui == null ? "" : dui.Izvoznik,
                    Uvoznik = dui == null ? "" : dui.Uvoznik
                });
            }

            ViewBag.CARINEUI = CARINEUI;


            var vu = db.VozacUser.Where(c => c.IdVozac == (dnevnikprevoza.IdVozac == null ? 0 : dnevnikprevoza.IdVozac)).SingleOrDefault();
            var kor = vu == null ? null : db.Korisnik.Where(c => c.IdKorisnik == vu.IdUser).SingleOrDefault();

            var korime = kor == null ? "" : kor.KorisnickoIme;

            var serializer = new JavaScriptSerializer();
            var PromjeneStatusa = db.Log.Where(c => c.PK == id && c.Tabela.Equals("DnevnikPrevoza") && c.KorisnickoIme.Equals(korime)).ToList().Select(c =>
           new AndroidChange
           {
               IdLog = c.IdLog,
               Korisnik = c.KorisnickoIme,
               Datum = c.Datum.Value.ToShortDateString() + "  " + c.Datum.Value.ToShortTimeString() + "h",
               Dnevnik = serializer.Deserialize<DnevnikPrevoza>(c.NovaVrijednost).Status,
               Long = serializer.Deserialize<DnevnikPrevoza>(c.NovaVrijednost).LONG,
               Lat = serializer.Deserialize<DnevnikPrevoza>(c.NovaVrijednost).LAT
           }).OrderByDescending(c => c.IdLog ).ToList();

            ViewBag.AndroidChange = PromjeneStatusa;


            var UtovarSped = dnevnikprevoza.DnevnikCarina.ToList();
            var Dutovar = dnevnikprevoza.DnevnikUtovar.ToList();
            var Distovar = dnevnikprevoza.DnevnikIstovar.ToList();

            var UtovariIstovari = dnevnikprevoza.DnevnikUtovar.Select(c => new UtovarSpedicije
            {
                Tip = "Utovar",
                Rb = Dutovar.IndexOf(c) + 2,
                Firma = c.Firma + System.Environment.NewLine + c.Adresa + System.Environment.NewLine + c.PTT + " " + c.Mjesto + " " + c.Drzava,
                Spedicija = Dutovar.IndexOf(c) < UtovarSped.Count() ? UtovarSped[Dutovar.IndexOf(c)].IzvoznaCarina : "",
                Roba = "Količina: " + c.KolicinaRobe + System.Environment.NewLine + "Težina u kg: " + c.TezinaRobe + System.Environment.NewLine + "Vrsta: " + c.VrstaRobe,
                Datum =  c.DatmUtovara.HasValue ? c.DatmUtovara.Value.ToShortDateString() : "",
                Kontakt = c.Kontakt
            }).ToList();

            UtovariIstovari.Insert(0, new UtovarSpedicije
            {
                Tip = "Utovar",
                Rb = 1,
                Firma = dnevnikprevoza.UtovarFirma + System.Environment.NewLine + dnevnikprevoza.UtovarAdresa + System.Environment.NewLine + dnevnikprevoza.UtovarPTT + " " + dnevnikprevoza.UtovarGrad + " " + dnevnikprevoza.UtovarDrzava,
                Spedicija = dnevnikprevoza.IzvoznaSpedicija,
                Roba = "Količina: " + dnevnikprevoza.KolicinaRobe + System.Environment.NewLine + "Težina u kg: " + dnevnikprevoza.TezinaRobe + System.Environment.NewLine + "Vrsta: " + dnevnikprevoza.VrstaRobe,
                Datum = dnevnikprevoza.DatumUtovara.HasValue ? dnevnikprevoza.DatumUtovara.Value.ToShortDateString() : "",
                Kontakt = dnevnikprevoza.UtovarKontakt
            });

            UtovariIstovari.Add(
                new UtovarSpedicije
                {
                    Tip = "Istovar",
                    Rb = 1,
                    Firma = dnevnikprevoza.IstovarFirma + System.Environment.NewLine + dnevnikprevoza.IstovarAdresa + System.Environment.NewLine + dnevnikprevoza.IstovarPTT + " " + dnevnikprevoza.IstovarGrad + " " + dnevnikprevoza.IstovarDrzava ,
                    Spedicija = dnevnikprevoza.UvoznaSpedicija,
                    Roba = dnevnikprevoza.IstovarKolicinaRobe,
                    Datum = dnevnikprevoza.DatumIstovara.HasValue ? dnevnikprevoza.DatumIstovara.Value.ToShortDateString() : "",
                    Kontakt = dnevnikprevoza.IstovarKontakt
                }
                );

            UtovariIstovari.AddRange(
                dnevnikprevoza.DnevnikIstovar.Select(c => new UtovarSpedicije
                {
                    Tip = "Istovar",
                    Rb = Distovar.IndexOf(c) + 2,
                    Firma = c.Firma + System.Environment.NewLine + c.Adresa + System.Environment.NewLine + c.PTT + " " +  c.Mjesto + " " + c.Drzava,
                    Spedicija = Distovar.IndexOf(c) < UtovarSped.Count() ? UtovarSped[Distovar.IndexOf(c)].UvoznaCarina : "",
                    Roba = "Količina: " + c.KolicinaRobe,
                    Datum = c.DatumIstovara.HasValue ? c.DatumIstovara.Value.ToShortDateString() : "",
                    Kontakt = c.Kontakt
                }).ToList()
                );


            ViewBag.UtovariIstovari = UtovariIstovari;
            ViewBag.PodredjeniDnevnici = db.DnevnikPrevoza.Where(c => c.IdDnevnikParent == id).ToList();
            ViewBag.SBNadredjenog = dnevnikprevoza.IdDnevnikParent == null ? "" : db.DnevnikPrevoza.Find(dnevnikprevoza.IdDnevnikParent).SerijskiBroj;

            if (dnevnikprevoza == null)
            {
                return HttpNotFound();
            } 
            return View(dnevnikprevoza);
        }


        //
        // GET: /DnevnikPrevoza/Create
        [Authorize]
        public ActionResult Create(int idParent = 0)
        {
            return RedirectToAction("CreateTab", new { idParent = idParent });

            /*
            String SerijskiBroj = "";
            String godina = DateTime.Today.Year.ToString() + "-";
            var sb = db.DnevnikPrevoza.Where(c => (c.ZapisAktivan ?? false) && c.SerijskiBroj.StartsWith(godina)).ToList();

                

            if (sb.Count() == 0)
                SerijskiBroj = godina + "00001";
            else
            {
                var broj = sb.Select(c => new { SerijskiBroj = Convert.ToInt32(c.SerijskiBroj.Replace(godina, "")) }).ToList();
                SerijskiBroj = "0000" + (broj.Max(c => c.SerijskiBroj) + 1);
                SerijskiBroj = godina + SerijskiBroj.Substring(SerijskiBroj.Length - 5, 5);
            }

            ViewBag.DnevnikCarina = new List<DnevnikCarina>();
            ViewBag.DnevnikIstovar = new List<DnevnikIstovar>();
            ViewBag.DnevnikUtovar = new List<DnevnikUtovar>();
            ViewBag.DnevnikUvoznikIzvoznik = new List<DnevnikUvoznikIzvoznik>();

            List<SelectListItem> items = new List<SelectListItem>
            {
                new SelectListItem(){ Value = "Uvozno Carinjenje",  Text  = "Uvozno Carinjenje"},
                new SelectListItem(){ Value = "Izvozno Carinjenje",  Text  = "Izvozno Carinjenje"},
                new SelectListItem(){ Value = "Auto Dan",  Text  = "Auto Dan"},
                 new SelectListItem(){Value = "Rent a Car",  Text  = "Rent a Car"},
                new SelectListItem(){ Value = "Tranzitni Dokument T1",  Text  = "Tranzitni Dokument T1"},
                 new SelectListItem(){Value = "Carinski terminal",  Text  = "Carinski terminal"}
            };

            ViewBag.Vrsta = items;


            ViewBag.IdNar = 0;
            ViewBag.SerijskiBroj = SerijskiBroj;
            ViewBag.IdSubjekt = new SelectList(db.Subjekt, "IdSubjekt", "Naziv");
            ViewBag.IdNarucioca = new SelectList(db.Subjekt, "IdSubjekt", "Naziv");
            ViewBag.IdValuta = new SelectList(db.Valuta, "IdValuta", "OznakaValute");
            ViewBag.IdValutaPrevoznika = new SelectList(db.Valuta, "IdValuta", "OznakaValute");
            ViewBag.IdVozac = new SelectList(db.Vozaci, "IdVozac", "ImeVozaca");
            ViewBag.IdVozilo = new SelectList(db.Vozilo.Where(c => c.VrstaVozila.Equals("Vozilo")).Select(c => new { IdVozilo = c.IdVozilo, TipVozila = c.TipVozila + " " + c.RegistarskiBroj }), "IdVozilo", "TipVozila");
            ViewBag.IdPrikljucno = new SelectList(db.Vozilo.Where(c => c.VrstaVozila.Equals("Priključno Vozilo")).Select(c => new { IdVozilo = c.IdVozilo, TipVozila = c.TipVozila + " " + c.RegistarskiBroj }), "IdVozilo", "TipVozila");
            return View();

            */
        }



        [Authorize]
        public ActionResult CreateTab(int idParent = 0)
        {
            String SerijskiBroj =  SerijskiBrojGenerator.Broj(idParent);

            /*
            String godina = DateTime.Today.Year.ToString() + "-";
            var sb = db.DnevnikPrevoza.Where(c => (c.ZapisAktivan ?? false) && c.SerijskiBroj.StartsWith(godina)).ToList();



            if (sb.Count() == 0)
                SerijskiBroj = godina + "00001";
            else
            {
                var broj = sb.Select(c => new { SerijskiBroj = Convert.ToInt32(c.SerijskiBroj.Replace(godina, "")) }).ToList();
                SerijskiBroj = "0000" + (broj.Max(c => c.SerijskiBroj) + 1);
                SerijskiBroj = godina + SerijskiBroj.Substring(SerijskiBroj.Length - 5, 5);
            }
            */



            ViewBag.DnevnikCarina = new List<DnevnikCarina>();
            ViewBag.DnevnikIstovar = new List<DnevnikIstovar>();
            ViewBag.DnevnikUtovar = new List<DnevnikUtovar>();
            ViewBag.DnevnikUvoznikIzvoznik = new List<DnevnikUvoznikIzvoznik>();

            /*
            List<SelectListItem> items = new List<SelectListItem>
            {
                new SelectListItem(){ Value = "Uvozno Carinjenje",  Text  = "Uvozno Carinjenje"},
                new SelectListItem(){ Value = "Izvozno Carinjenje",  Text  = "Izvozno Carinjenje"},
                new SelectListItem(){ Value = "Auto Dan",  Text  = "Auto Dan"},
                new SelectListItem(){ Value = "Rent a Car",  Text  = "Rent a Car"},
                new SelectListItem(){ Value = "Tranzitni Dokument T1",  Text  = "Tranzitni Dokument T1"},
                new SelectListItem(){ Value = "Carinski terminal",  Text  = "Carinski terminal"}
            };
            */


            ViewBag.Vrsta = new SelectList(db.TipUsluge, "Naziv", "Naziv");


            ViewBag.IdNar = 0;
            ViewBag.SerijskiBroj = SerijskiBroj;
            ViewBag.IdPonuda = new SelectList(db.Ponuda.Select(c=> new { IdDnevnik = c.IdDnevnik, Naziv = c.SerijskiBroj + " [ Za " + c.Subjekt.Naziv + " na destinaciju:  " + c.IstovarPTT + " " + c.IstovarGrad + " " + c.IstovarDrzava + " ]"}).OrderByDescending(k => k.IdDnevnik), "IdDnevnik", "Naziv");
            ViewBag.IdSubjekt = new SelectList(db.Subjekt, "IdSubjekt", "Naziv");
            ViewBag.IdNarucioca = new SelectList(db.Subjekt, "IdSubjekt", "Naziv");
            ViewBag.IdValuta = new SelectList(db.Valuta, "IdValuta", "OznakaValute");
            ViewBag.IdValutaPrevoznika = new SelectList(db.Valuta, "IdValuta", "OznakaValute");
            ViewBag.IdVozac = new SelectList(db.Vozaci, "IdVozac", "ImeVozaca");
            ViewBag.IdStatusDetaljni = new SelectList(db.StatusRobe, "IdStatusRobe", "Naziv");
            ViewBag.IdVozilo = new SelectList(db.Vozilo.Where(c => c.VrstaVozila.Equals("Vozilo")).Select(c => new { IdVozilo = c.IdVozilo, TipVozila = c.TipVozila + " " + c.RegistarskiBroj }), "IdVozilo", "TipVozila");
            ViewBag.IdPrikljucno = new SelectList(db.Vozilo.Where(c => c.VrstaVozila.Equals("Priključno Vozilo")).Select(c => new { IdVozilo = c.IdVozilo, TipVozila = c.TipVozila + " " + c.RegistarskiBroj }), "IdVozilo", "TipVozila");
            ViewBag.IdDnevnikParent = idParent == 0 ? null : ""+idParent; // new SelectList(db.DnevnikPrevoza.Where(c => (c.ZapisAktivan ?? false)).OrderByDescending(c => c.IdDnevnik).Take(50), "IdDnevnik", "SerijskiBroj", idParent);
            return View(new DnevnikPrevoza());
        }

        //
        // POST: /DnevnikPrevoza/Create

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(DnevnikPrevoza dnevnikprevoza)
        {
            if (Request["_DrugiPrevoznik"] != null)
            {
                if (Request["_DrugiPrevoznik"].ToString().Equals("on"))
                    dnevnikprevoza.DrugiPrevoznik = true;
                else dnevnikprevoza.DrugiPrevoznik = false;
            }
            else dnevnikprevoza.DrugiPrevoznik = false;

            if (Request["_KompletanUtovar"] != null)
            {
                if (Request["_KompletanUtovar"].ToString().Equals("on"))
                    dnevnikprevoza.KompletanUtovar = true;
                else dnevnikprevoza.KompletanUtovar = false;
            }
            else dnevnikprevoza.KompletanUtovar = false;

            if (Request["_SaPDV"] != null)
            {
                if (Request["_SaPDV"].ToString().Equals("on"))
                    dnevnikprevoza.SaPDV = true;
                else dnevnikprevoza.SaPDV = false;
            }
            else dnevnikprevoza.SaPDV = false;


            if (db.DnevnikPrevoza.Where(c=> (c.ZapisAktivan ?? false)   &&  c.SerijskiBroj.Equals(dnevnikprevoza.SerijskiBroj) ).Count() > 0 )
                dnevnikprevoza.SerijskiBroj = SerijskiBrojGenerator.Broj(dnevnikprevoza.IdDnevnikParent ?? 0);


            dnevnikprevoza.DnevnikUtovar = dnevnikprevoza.DnevnikUtovar.Where(c => c.Firma != null || c.Mjesto != null || c.Adresa != null).ToList();
            dnevnikprevoza.DnevnikIstovar = dnevnikprevoza.DnevnikIstovar.Where(c => c.Firma != null || c.Mjesto != null || c.Adresa != null).ToList();
            dnevnikprevoza.DnevnikCarina = dnevnikprevoza.DnevnikCarina.Where(c => c.IzvoznaCarina != null || c.UvoznaCarina != null).ToList();
            dnevnikprevoza.DnevnikUvoznikIzvoznik = dnevnikprevoza.DnevnikUvoznikIzvoznik.Where(c => c.Izvoznik != null || c.Uvoznik != null).ToList();
            dnevnikprevoza.Troskovi = dnevnikprevoza.Troskovi.Where(c => c.Iznos != null).ToList();

            dnevnikprevoza.GostPristup = RandomString(25);
            dnevnikprevoza.Uplaceno = false;

            if (ModelState.IsValid)
            {
               
                dnevnikprevoza.ZapisAktivan = true;
                dnevnikprevoza.DatumZapisa = DateTime.Now.AddHours(7);
                dnevnikprevoza.Status = "NIJE UTOVARENO";
                db.DnevnikPrevoza.Add(dnevnikprevoza);

                db.SaveChanges();

                AndroidTask at = new AndroidTask
                {
                    DatumZapisa = DateTime.Now.AddHours(7),
                    IdDnevnik = dnevnikprevoza.IdDnevnik,
                    AdminPregled = true,
                    VozacPregled = false
                };

                db.AndroidTask.Add(at);
                db.SaveChanges();

                DodajGradove(dnevnikprevoza.UtovarGrad);
                DodajGradove(dnevnikprevoza.IstovarGrad);

                return RedirectToAction("Index");
            }


            ViewBag.SerijskiBroj = dnevnikprevoza.SerijskiBroj;
            ViewBag.IdSubjekt = new SelectList(db.Subjekt, "IdSubjekt", "Naziv", dnevnikprevoza.IdSubjekt);
            ViewBag.IdNarucioca = new SelectList(db.Subjekt, "IdSubjekt", "Naziv", dnevnikprevoza.IdNarucioca);
            ViewBag.IdValuta = new SelectList(db.Valuta, "IdValuta", "OznakaValute", dnevnikprevoza.IdValuta);
            ViewBag.IdValutaPrevoznika = new SelectList(db.Valuta, "IdValuta", "OznakaValute", dnevnikprevoza.IdValutaPrevoznika);
            ViewBag.IdVozac = new SelectList(db.Vozaci, "IdVozac", "ImeVozaca", dnevnikprevoza.IdVozac);
            ViewBag.IdStatusDetaljni = new SelectList(db.StatusRobe, "IdStatusRobe", "Naziv");
            ViewBag.IdVozilo = new SelectList(db.Vozilo.Where(c => c.VrstaVozila.Equals("Vozilo")).Select(c => new { IdVozilo = c.IdVozilo, TipVozila = c.TipVozila + " " + c.RegistarskiBroj }), "IdVozilo", "TipVozila", dnevnikprevoza.IdVozilo);
            ViewBag.IdPrikljucno = new SelectList(db.Vozilo.Where(c => c.VrstaVozila.Equals("Priključno Vozilo")).Select(c => new { IdVozilo = c.IdVozilo, TipVozila = c.TipVozila + " " + c.RegistarskiBroj }), "IdVozilo", "TipVozila", dnevnikprevoza.IdPrikljucnoVozilo);
            ViewBag.IdDnevnikParent = dnevnikprevoza.IdDnevnikParent; // new SelectList(db.DnevnikPrevoza.Where(c => (c.ZapisAktivan ?? false)).OrderByDescending(c => c.IdDnevnik).Take(50), "IdDnevnik", "SerijskiBroj", dnevnikprevoza.IdDnevnikParent);

            return View(dnevnikprevoza);
        }

        private void DodajGradove(string g1)
        {
            try
            {
                String grad1 = g1.Split('-')[0].Trim();
                String drzava = g1.Split('-')[1].Trim();

                if (grad1 != null)
                {
                    var g = db.Grad.Where(c => c.Grad1.Equals(grad1)).ToList();

                    if (g.Count() == 0)
                    {
                        Grad grad = new Grad();
                        grad.Grad1 = grad1;
                        grad.ISO = drzava;
                        grad.Drzava = "NN";
                        db.Grad.Add(grad);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex) { }
        }

        private void AzurirajGradove(string old, String novi)
        {
            try
            {
                String grad = old.Split('-')[0].Trim();
                String drzava = old.Split('-')[1].Trim();

                String grad1 = novi.Split('-')[0].Trim();
                String drzava1 = novi.Split('-')[1].Trim();

                if (grad != null && grad1 != null)
                {
                    var g = db.Grad.Where(c => c.Grad1.Equals(grad) && c.Drzava.Equals("NN")).ToList();
                    var n = db.Grad.Where(c => c.Grad1.Equals(grad1) && !c.Drzava.Equals("NN")).ToList();

                    if (g.Count() > 0)
                    {
                        if (n.Count() == 0)
                        {
                            Grad gr = g.FirstOrDefault();
                            gr.Grad1 = grad1;
                            gr.ISO = drzava1;
                            gr.Drzava = "NN";
                            db.Entry(gr).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        if (n.Count() == 0)
                        {
                            Grad gr = new Grad();
                            gr.Grad1 = grad1;
                            gr.ISO = drzava1;
                            gr.Drzava = "NN";
                            db.Grad.Add(gr);
                            db.SaveChanges();
                        }
                    }

                }
            }
            catch (Exception ex) { }
        }
        //
        // GET: /DnevnikPrevoza/Edit/5

        [Authorize]
        public ActionResult Edit(int id = 0)
        {

            return RedirectToAction("EditTab", new { id = id });
            /*
            DnevnikPrevoza dnevnikprevoza = db.DnevnikPrevoza.Find(id);
            if (dnevnikprevoza == null)
            {
                return HttpNotFound();
            }

             List<SelectListItem> items = new List<SelectListItem>
            {
                new SelectListItem(){ Value = "Uvozno Carinjenje",  Text  = "Uvozno Carinjenje"},
                new SelectListItem(){ Value = "Izvozno Carinjenje",  Text  = "Izvozno Carinjenje"},
                new SelectListItem(){ Value = "Auto Dan",  Text  = "Auto Dan"},
                 new SelectListItem(){ Value = "Rent a Car",  Text  = "Rent a Car"},
                new SelectListItem(){ Value = "Tranzitni Dokument T1",  Text  = "Tranzitni Dokument T1"},
                 new SelectListItem(){ Value = "Carinski terminal",  Text  = "Carinski terminal"}
            };


            List<SelectList> ListaTroskova = new List<SelectList>();
            List<SelectList> ListaValuta = new List<SelectList>();
            foreach (var item in dnevnikprevoza.Troskovi)
            {
                ListaTroskova.Add(new SelectList(items.Select(x => new { Vrsta = x.Value, Text = x.Text }).ToList(), "Vrsta", "Text", item.Vrsta));
                ListaValuta.Add(new SelectList(db.Valuta, "IdValuta", "OznakaValute", item.IdValuta));
            }
            ViewBag.ListaTroskova = ListaTroskova;
            ViewBag.ListaValuta = ListaValuta;

            ViewBag.IdSubjektSL = new SelectList(db.Subjekt, "IdSubjekt", "Naziv", dnevnikprevoza.IdSubjekt);
            ViewBag.IdNaruciocaSL = new SelectList(db.Subjekt, "IdSubjekt", "Naziv", dnevnikprevoza.IdNarucioca);
            ViewBag.IdValutaSL = new SelectList(db.Valuta, "IdValuta", "OznakaValute", dnevnikprevoza.IdValuta);
            ViewBag.IdValutaPrevoznikaSL = new SelectList(db.Valuta, "IdValuta", "OznakaValute", dnevnikprevoza.IdValutaPrevoznika);
            ViewBag.IdVozacSL = new SelectList(db.Vozaci, "IdVozac", "ImeVozaca", dnevnikprevoza.IdVozac);
            ViewBag.IdVoziloSL = new SelectList(db.Vozilo.Where(c => c.VrstaVozila.Equals("Vozilo")).Select(c => new { IdVozilo = c.IdVozilo, TipVozila = c.TipVozila + " " + c.RegistarskiBroj }), "IdVozilo", "TipVozila", dnevnikprevoza.IdVozilo);
            ViewBag.IdPrikljucnoSL = new SelectList(db.Vozilo.Where(c => c.VrstaVozila.Equals("Priključno Vozilo")).Select(c => new { IdVozilo = c.IdVozilo, TipVozila = c.TipVozila + " " + c.RegistarskiBroj }), "IdVozilo", "TipVozila", dnevnikprevoza.IdPrikljucnoVozilo);
            return View(dnevnikprevoza);
            */
        }

        [Authorize]
        public ActionResult EditTab(int id = 0)
        {
            DnevnikPrevoza dnevnikprevoza = db.DnevnikPrevoza.Find(id);
            if (dnevnikprevoza == null)
            {
                return HttpNotFound();
            }

            List<SelectListItem> items = 

                 db.TipUsluge.ToList().Select(c => new SelectListItem { Value=c.Naziv, Text = c.Naziv }).ToList();
        


            List<SelectList> ListaTroskova = new List<SelectList>();
            List<SelectList> ListaValuta = new List<SelectList>();
            foreach (var item in dnevnikprevoza.Troskovi)
            {
                ListaTroskova.Add(new SelectList(items.Select(x => new { Vrsta = x.Value, Text = x.Text }).ToList(), "Vrsta", "Text", item.Vrsta));
                ListaValuta.Add(new SelectList(db.Valuta, "IdValuta", "OznakaValute", item.IdValuta));
            }
            ViewBag.ListaTroskova = ListaTroskova;
            ViewBag.ListaValuta = ListaValuta;

            ViewBag.IdPonudaSL = new SelectList(db.Ponuda.Select(c => new { IdDnevnik = c.IdDnevnik, Naziv = c.SerijskiBroj + " [ Za " + c.Subjekt.Naziv + " na destinaciju:  " + c.IstovarPTT + " " + c.IstovarGrad + " " + c.IstovarDrzava + " ]" }), "IdDnevnik", "Naziv", dnevnikprevoza.IdPonuda);
            ViewBag.IdSubjektSL = new SelectList(db.Subjekt, "IdSubjekt", "Naziv", dnevnikprevoza.IdSubjekt);
            ViewBag.IdNaruciocaSL = new SelectList(db.Subjekt, "IdSubjekt", "Naziv", dnevnikprevoza.IdNarucioca);
            ViewBag.IdValutaSL = new SelectList(db.Valuta, "IdValuta", "OznakaValute", dnevnikprevoza.IdValuta);
            ViewBag.IdValutaPrevoznikaSL = new SelectList(db.Valuta, "IdValuta", "OznakaValute", dnevnikprevoza.IdValutaPrevoznika);
            ViewBag.IdVozacSL = new SelectList(db.Vozaci, "IdVozac", "ImeVozaca", dnevnikprevoza.IdVozac);
            ViewBag.IdStatusDetaljniSL = new SelectList(db.StatusRobe, "IdStatusRobe", "Naziv", dnevnikprevoza.idStatusDetaljni);
            ViewBag.IdVoziloSL = new SelectList(db.Vozilo.Where(c => c.VrstaVozila.Equals("Vozilo")).Select(c => new { IdVozilo = c.IdVozilo, TipVozila = c.TipVozila + " " + c.RegistarskiBroj }), "IdVozilo", "TipVozila", dnevnikprevoza.IdVozilo);
            ViewBag.IdPrikljucnoSL = new SelectList(db.Vozilo.Where(c => c.VrstaVozila.Equals("Priključno Vozilo")).Select(c => new { IdVozilo = c.IdVozilo, TipVozila = c.TipVozila + " " + c.RegistarskiBroj }), "IdVozilo", "TipVozila", dnevnikprevoza.IdPrikljucnoVozilo);
          //  ViewBag.IdDnevnikParentSL = new SelectList(db.DnevnikPrevoza.Where(c => (c.ZapisAktivan ?? false)).OrderByDescending(c => c.IdDnevnik).Take(50), "IdDnevnik", "SerijskiBroj");
            return View(dnevnikprevoza);
        }
        //
        // POST: /DnevnikPrevoza/Edit/5

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(DnevnikPrevoza dnevnikprevoza)
        {
            if (Request["_DrugiPrevoznik"] != null)
            {
                if (Request["_DrugiPrevoznik"].ToString().Equals("on"))
                    dnevnikprevoza.DrugiPrevoznik = true;
                else dnevnikprevoza.DrugiPrevoznik = false;
            }
            else dnevnikprevoza.DrugiPrevoznik = false;

            if (Request["_KompletanUtovar"] != null)
            {
                if (Request["_KompletanUtovar"].ToString().Equals("on"))
                    dnevnikprevoza.KompletanUtovar = true;
                else dnevnikprevoza.KompletanUtovar = false;
            }
            else dnevnikprevoza.KompletanUtovar = false;

            if (Request["_SaPDV"] != null)
            {
                if (Request["_SaPDV"].ToString().Equals("on"))
                    dnevnikprevoza.SaPDV = true;
                else dnevnikprevoza.SaPDV = false;
            }
            else dnevnikprevoza.SaPDV = false;


            dnevnikprevoza.DnevnikUtovar = dnevnikprevoza.DnevnikUtovar.Where(c => (c.Firma != null || c.Mjesto != null || c.Adresa != null) || c.IdUtovar != 0).ToList();
            dnevnikprevoza.DnevnikIstovar = dnevnikprevoza.DnevnikIstovar.Where(c => (c.Firma != null || c.Mjesto != null || c.Adresa != null) || c.IdIstovar != 0).ToList();
            dnevnikprevoza.DnevnikCarina = dnevnikprevoza.DnevnikCarina.Where(c => (c.IzvoznaCarina != null || c.UvoznaCarina != null) || c.IdCarina != 0).ToList();
            dnevnikprevoza.DnevnikUvoznikIzvoznik = dnevnikprevoza.DnevnikUvoznikIzvoznik.Where(c => (c.Izvoznik != null || c.Uvoznik != null) || c.IdUvoznikIzvoznik != 0).ToList();
            dnevnikprevoza.Troskovi = dnevnikprevoza.Troskovi.Where(c => c.Iznos != null || c.IdTroskovi != 0).ToList();


            foreach (DnevnikUtovar du in dnevnikprevoza.DnevnikUtovar)
                {
                    if (du.Firma == null && du.Mjesto == null && du.Adresa == null)
                        db.DnevnikUtovar.Remove(db.DnevnikUtovar.Find(du.IdUtovar));
                    else if (du.IdUtovar == 0)
                        db.DnevnikUtovar.Add(du);
                    else
                        db.Entry(du).State = EntityState.Modified;

                    db.SaveChanges();
                }

            foreach (DnevnikIstovar di in dnevnikprevoza.DnevnikIstovar)
                {
                    if (di.Firma == null && di.Mjesto == null && di.Adresa == null)
                        db.DnevnikIstovar.Remove(db.DnevnikIstovar.Find(di.IdIstovar));
                    else if (di.IdIstovar == 0)
                        db.DnevnikIstovar.Add(di);
                    else
                        db.Entry(di).State = EntityState.Modified;

                    db.SaveChanges();
                }


            foreach (DnevnikCarina dc in dnevnikprevoza.DnevnikCarina)
                {
                    if (dc.UvoznaCarina == null && dc.IzvoznaCarina == null)
                            db.DnevnikCarina.Remove(db.DnevnikCarina.Find(dc.IdCarina));
                    else if (dc.IdCarina == 0 && (dc.UvoznaCarina != null || dc.IzvoznaCarina != null))
                            db.DnevnikCarina.Add(dc);
                    else
                            db.Entry(dc).State = EntityState.Modified;

                        db.SaveChanges();

                }

                foreach (DnevnikUvoznikIzvoznik ui in dnevnikprevoza.DnevnikUvoznikIzvoznik)
                {
                    if (ui.Uvoznik == null && ui.Izvoznik == null)
                        db.DnevnikUvoznikIzvoznik.Remove(db.DnevnikUvoznikIzvoznik.Find(ui.IdUvoznikIzvoznik));
                    else if (ui.IdUvoznikIzvoznik == 0)
                        db.DnevnikUvoznikIzvoznik.Add(ui);
                    else
                        db.Entry(ui).State = EntityState.Modified;

                    db.SaveChanges();
                }


            foreach (Troskovi t in dnevnikprevoza.Troskovi)
                {
                    if (t.Iznos == null )
                        db.Troskovi.Remove(db.Troskovi.Find(t.IdTroskovi));
                    else if (t.IdTroskovi == 0)
                        db.Troskovi.Add(t);
                    else
                        db.Entry(t).State = EntityState.Modified;

                    db.SaveChanges();
                }


            if (ModelState.IsValid)
                {
                    DnevnikPrevoza old = db.DnevnikPrevoza.AsNoTracking().Single(c => c.IdDnevnik == dnevnikprevoza.IdDnevnik);


                    dnevnikprevoza.DatumZapisa = DateTime.Now.AddHours(7);
                    dnevnikprevoza.ZapisAktivan = true;
                    db.Entry(dnevnikprevoza).State = EntityState.Modified;
                    db.SaveChanges();

                    AzurirajGradove(old.IstovarGrad, dnevnikprevoza.IstovarGrad);
                    AzurirajGradove(old.UtovarGrad, dnevnikprevoza.UtovarGrad);




                    return RedirectToAction("Index");
                }

            
            ViewBag.IdSubjekt = new SelectList(db.Subjekt, "IdSubjekt", "Naziv", dnevnikprevoza.IdSubjekt);
            ViewBag.IdNarucioca = new SelectList(db.Subjekt, "IdSubjekt", "Naziv", dnevnikprevoza.IdNarucioca);
            ViewBag.IdValuta = new SelectList(db.Valuta, "IdValuta", "OznakaValute", dnevnikprevoza.IdValuta);
            ViewBag.IdValutaPrevoznika = new SelectList(db.Valuta, "IdValuta", "OznakaValute", dnevnikprevoza.IdValutaPrevoznika);
            ViewBag.IdVozac = new SelectList(db.Vozaci, "IdVozac", "ImeVozaca", dnevnikprevoza.IdVozac);
            ViewBag.IdStatusDetaljniSL = new SelectList(db.StatusRobe, "IdStatusRobe", "Naziv", dnevnikprevoza.idStatusDetaljni);
            ViewBag.IdVozilo = new SelectList(db.Vozilo.Where(c => c.VrstaVozila.Equals("Vozilo")).Select(c => new { IdVozilo = c.IdVozilo, TipVozila = c.TipVozila + " " + c.RegistarskiBroj }), "IdVozilo", "TipVozila", dnevnikprevoza.IdVozilo);
            ViewBag.IdPrikljucno = new SelectList(db.Vozilo.Where(c => c.VrstaVozila.Equals("Priključno Vozilo")).Select(c => new { IdVozilo = c.IdVozilo, TipVozila = c.TipVozila + " " + c.RegistarskiBroj }), "IdVozilo", "TipVozila", dnevnikprevoza.IdPrikljucnoVozilo);
           // ViewBag.IdDnevnikParentSL = new SelectList(db.DnevnikPrevoza.Where(c => (c.ZapisAktivan ?? false)).OrderByDescending(c => c.IdDnevnik).Take(50), "IdDnevnik", "SerijskiBroj");

            return View(dnevnikprevoza);
        }

        //
        // GET: /DnevnikPrevoza/Delete/5
        [Authorize]
        [SpedicijaAutorizacija(Roles = "admin")]
        public ActionResult Delete(int id = 0)
        {
            DnevnikPrevoza dnevnikprevoza = db.DnevnikPrevoza.Find(id);
            if (dnevnikprevoza == null)
            {
                return HttpNotFound();
            }
            return View(dnevnikprevoza);
        }

        //
        // POST: /DnevnikPrevoza/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            DnevnikPrevoza dnevnikprevoza = db.DnevnikPrevoza.Find(id);
            dnevnikprevoza.ZapisAktivan = false;
            dnevnikprevoza.IdDnevnikParent = null;
            db.Entry(dnevnikprevoza).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        [Authorize]
        public ActionResult Copy(int id=0)
        {
            
            var originalEntity = db.DnevnikPrevoza.Include(d => d.DnevnikUvoznikIzvoznik).Include(d => d.DnevnikUtovar).Include(d => d.DnevnikIstovar)
                .Include(d => d.DnevnikCarina).Include(d => d.Troskovi).AsNoTracking().FirstOrDefault(e => e.IdDnevnik == id);

            String SerijskiBroj = SerijskiBrojGenerator.Broj();

            /*
            String godina = DateTime.Today.Year.ToString() + "-";
            var sb = db.DnevnikPrevoza.Where(c => (c.ZapisAktivan ?? false) && c.SerijskiBroj.StartsWith(godina)).ToList();



            if (sb.Count() == 0)
                SerijskiBroj = godina + "00001";
            else
            {
                var broj = sb.Select(c => new { SerijskiBroj = Convert.ToInt32(c.SerijskiBroj.Replace(godina, "")) }).ToList();
                SerijskiBroj = "0000" + (broj.Max(c => c.SerijskiBroj) + 1);
                SerijskiBroj = godina + SerijskiBroj.Substring(SerijskiBroj.Length - 5, 5);
            }
            */

            originalEntity.SerijskiBroj = SerijskiBroj;
            originalEntity.BrojFakture = "";
            originalEntity.DatumSlanjaFakture = null;
            originalEntity.GostPristup = RandomString(25);
            originalEntity.DatumZapisa = DateTime.Today.AddHours(7);
            originalEntity.Status = "NIJE UTOVARENO";
            originalEntity.DatumDnevnika = null;
            originalEntity.DatumIstovara = null;
            originalEntity.DatumUtovara = null;
            //originalEntity.ValutaPlacanja = null;
            originalEntity.Uplaceno = null;

            

            db.DnevnikPrevoza.Add(originalEntity);
            db.SaveChanges();

            AndroidTask at = new AndroidTask
            {
                DatumZapisa = DateTime.Now.AddHours(7),
                IdDnevnik = originalEntity.IdDnevnik,
                AdminPregled = true,
                VozacPregled = false
            };

            db.AndroidTask.Add(at);
            db.SaveChanges();

            return RedirectToAction("Edit", new { id = originalEntity.IdDnevnik }); 
            

           
        }

 
        public JsonResult AjaxGradovi(String prefix)
        {
            var gradovi = db.Grad.Select(c => new { Grad1 = c.Grad1 + " - " + c.ISO }).Distinct().Where(c => c.Grad1.Contains(prefix)).GroupBy(g => g.Grad1).Select(group => group.FirstOrDefault()).Take(25).ToList();
            var result = from c in gradovi
                         select new { 
                              c.Grad1
                          };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AjaxDrzava(String prefix)
        {
            var gradovi = db.Drzava.Select(c => new { Drzava = c.Naziv }).Where(c => c.Drzava.Contains(prefix)).Distinct().Take(25).ToList();
            var result = from c in gradovi
                         select new
                         {
                             c.Drzava
                         };

            return Json(result, JsonRequestBehavior.AllowGet);
        }


       
        public JsonResult AjaxAdresa(String prefix)
        {
            var adrese = (db.DnevnikPrevoza.OrderByDescending(c => c.IdDnevnik).Select(c => new { Adresa = c.IstovarAdresa }).Where(c => c.Adresa.Contains(prefix)).
                Union(db.DnevnikPrevoza.OrderByDescending(c => c.IdDnevnik).Select(c => new { Adresa = c.UtovarAdresa }).Where(c => c.Adresa.Contains(prefix))).
                Union(db.DnevnikUtovar.OrderByDescending(c => c.IdDnevnik).Select(a => new { Adresa = a.Adresa }).Where(c => c.Adresa.Contains(prefix))).
                Union(db.DnevnikIstovar.OrderByDescending(c => c.IdDnevnik).Select(a => new { Adresa = a.Adresa }).Where(c => c.Adresa.Contains(prefix)))).Distinct()
                .Take(50);

            var result = from c in adrese
                         select new
                         {
                             c.Adresa
                         };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        
        public JsonResult AjaxFirme(String prefix)
        {
            var firme = (
            db.Subjekt.Where(c => c.Naziv.Contains(prefix)).OrderByDescending(c => c.IdSubjekt).Select(c => new { Firma = c.Naziv }).
                        Union(db.DnevnikPrevoza.Where(c => (c.ZapisAktivan ?? false) && c.UtovarFirma.Contains(prefix)).OrderByDescending(c => c.IdDnevnik).Select(k => new { Firma = k.UtovarFirma })).
                        Union(db.DnevnikPrevoza.Where(c => (c.ZapisAktivan ?? false) && c.IstovarFirma.Contains(prefix)).OrderByDescending(c => c.IdDnevnik).Select(k => new { Firma = k.IstovarFirma })).
                        Union(db.DnevnikUtovar.Where(a => a.Firma.Contains(prefix)).OrderByDescending(c => c.IdDnevnik).Select(a => new { Firma = a.Firma })).
                        Union(db.DnevnikIstovar.Where(a => a.Firma.Contains(prefix)).OrderByDescending(c => c.IdDnevnik).Select(a => new { Firma = a.Firma }))).Distinct().Take(50).ToList();

            var result = from c in firme
                         select new
                         {
                             c.Firma
                         };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

      
        public JsonResult AjaxSpedicija(String prefix)
        {
            var adrese = (db.DnevnikPrevoza.OrderByDescending(c => c.IdDnevnik).Select(c => new { Spedicija = c.IzvoznaSpedicija }).Where(c => c.Spedicija.Contains(prefix)).
                Union(db.DnevnikPrevoza.OrderByDescending(c => c.IdDnevnik).Select(c => new { Spedicija = c.UvoznaSpedicija }).Where(c => c.Spedicija.Contains(prefix))).
                Union(db.DnevnikCarina.OrderByDescending(c => c.IdDnevnik).Select(c => new { Spedicija = c.UvoznaCarina }).Where(c => c.Spedicija.Contains(prefix))).
                Union(db.DnevnikCarina.OrderByDescending(c => c.IdDnevnik).Select(c => new { Spedicija = c.IzvoznaCarina }).Where(c => c.Spedicija.Contains(prefix)))).Distinct().ToList().Take(50);
                               
            var result = from c in adrese
                         select new
                         {
                             c.Spedicija
                         };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

      
        public JsonResult AjaxUvoznik(String prefix)
        {
            var adrese = (db.DnevnikUvoznikIzvoznik.OrderByDescending(c => c.IdDnevnik).Select(c => new { Uvoznik = c.Uvoznik }).Where(c => c.Uvoznik.Contains(prefix)).
                Union(db.Subjekt.Where(c => c.Naziv.Contains(prefix)).Select(c => new { Uvoznik = c.Naziv })))
                .Distinct().Take(50);
            

            var result = from c in adrese
                         select new
                         {
                             c.Uvoznik
                         };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

     
        public JsonResult AjaxIzvoznik(String prefix)
        {
            var adrese = (db.DnevnikUvoznikIzvoznik.OrderByDescending(c => c.IdDnevnik).Select(c => new { Izvoznik = c.Izvoznik }).Where(c => c.Izvoznik.Contains(prefix)).
                 Union(db.Subjekt.Where(c => c.Naziv.Contains(prefix)).Select(c => new { Izvoznik = c.Naziv })))
                .Distinct().Take(50);
            var result = from c in adrese
                         select new
                         {
                             c.Izvoznik
                         };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        
        [Authorize]
        public JsonResult AjaxRegistracijaPrevoznika(String prefix)
        {
            var adrese = db.DnevnikPrevoza.OrderByDescending(c => c.IdDnevnik).Select(c => new { RegPrevoznika = c.RegistracijaVozilaPrevoznika }).Where(c => c.RegPrevoznika.Contains(prefix)).Distinct().Take(10);
            var result = from c in adrese
                         select new
                         {
                             c.RegPrevoznika
                         };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult AjaxVozacPrevoznika(String prefix)
        {
            var adrese = db.DnevnikPrevoza.Select(c => new { VozacPrevoznika = c.VozacPrevoznika }).Where(c => c.VozacPrevoznika.Contains(prefix)).Distinct().Take(10);
            var result = from c in adrese
                         select new
                         {
                             c.VozacPrevoznika
                         };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AjaxVratiAdresuFirme(String Firma)
        {
            var Adresa = db.Subjekt.Where(c => c.Naziv.Equals(Firma)).OrderByDescending(c => c.IdSubjekt).FirstOrDefault();


            var result = new { Grad = Adresa == null ? "" : Adresa.Grad, Adresa = Adresa == null ? "" : Adresa.Adresa };

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public JsonResult AjaxVratiDrzavuZaGrad(String Grad)
        {
           var DrzavaISO = db.Grad.Where(c => Grad.Contains(c.Grad1) && Grad.Contains(c.ISO)).FirstOrDefault();
            var Drzava = DrzavaISO == null ? "" : db.Drzava.Where(c => c.ISO.Equals(DrzavaISO.ISO)).SingleOrDefault().Naziv;

            var result = new { Drzava = Drzava };

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public JsonResult AjaxVratiZaPTT(String PTT)
        {
            var rez = db.DnevnikPrevoza.Where(c => c.UtovarPTT.Equals(PTT)).OrderByDescending(c => c.IdDnevnik).Select(c => new { Grad = c.UtovarGrad, Drzava = c.UtovarDrzava }).FirstOrDefault();

            if (rez == null)
            rez =     db.DnevnikPrevoza.Where(c => c.IstovarPTT.Equals(PTT)).OrderByDescending(c => c.IdDnevnik).Select(c => new { Grad = c.IstovarGrad, Drzava = c.IstovarDrzava }).FirstOrDefault();


            return Json(rez, JsonRequestBehavior.AllowGet);
        }


        public JsonResult AjaxPTT(String prefix) {

            var ptt = (
                db.DnevnikPrevoza.OrderByDescending(c => c.IdDnevnik).Select(c => new { PTT = c.UtovarPTT }).Where(c => c.PTT.Contains(prefix)).
               Union(db.DnevnikPrevoza.OrderByDescending(c => c.IdDnevnik).Select(c => new { PTT = c.IstovarPTT }).Where(c => c.PTT.Contains(prefix)))
               )
               .Distinct().Take(10);


            var result = from c in ptt
                         select new
                         {
                             c.PTT
                         };
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public JsonResult AjaxPodaciOFirmi(String Firma)
        {

            var subjekt = db.Subjekt.Where(c => c.Naziv.Equals(Firma)).OrderByDescending(c => c.IdSubjekt).FirstOrDefault();

            if (subjekt != null)
            {
                string ptt = new String(subjekt.Grad.Where(Char.IsDigit).ToArray());
                string grad = Regex.Replace(subjekt.Grad, @"[\d-]", string.Empty);

                var DrzavaISO = db.Grad.Where(c => grad.Contains(c.Grad1) && grad.Contains(c.ISO) && !c.Drzava.Equals("NN")).FirstOrDefault();
                var Drzava = DrzavaISO == null ? "" : db.Drzava.Where(c => c.ISO.Equals(DrzavaISO.ISO)).SingleOrDefault().Naziv;

                var FirmaPodaci = new {
                    Adresa = subjekt.Adresa,
                    PTT = String.IsNullOrEmpty(subjekt.PTT) ? ptt : subjekt.PTT,
                    Grad = ptt.Equals("") ?  subjekt.Grad : subjekt.Grad.Replace(ptt, "").Trim(),
                    Drzava = String.IsNullOrEmpty(subjekt.Drzava) ? Drzava : subjekt.Drzava,
                    Kontakt = subjekt.KontaktOsoba
                };

                return Json(FirmaPodaci, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var firma = (db.DnevnikPrevoza.Where(c => (c.ZapisAktivan ?? false) && c.UtovarFirma.Equals(Firma)).Select(c => new { IdDnevnik = c.IdDnevnik, Adresa = c.UtovarAdresa, PTT = c.UtovarPTT, Grad = c.UtovarGrad, Drzava = c.UtovarDrzava, Kontakt = c.UtovarKontakt }).
                    Union(db.DnevnikPrevoza.Where(c => (c.ZapisAktivan ?? false) && c.IstovarFirma.Equals(Firma)).Select(c => new { IdDnevnik = c.IdDnevnik, Adresa = c.IstovarAdresa, PTT = c.IstovarPTT, Grad = c.IstovarGrad, Drzava = c.IstovarDrzava, Kontakt = c.IstovarKontakt})).
                    Union(db.DnevnikUtovar.Where(c => c.Firma.Equals(Firma)).Select(c => new { IdDnevnik = c.IdDnevnik, Adresa = c.Adresa, PTT = c.PTT, Grad = c.Mjesto, Drzava = c.Drzava, Kontakt = c.Kontakt })).
                    Union(db.DnevnikIstovar.Where(c => c.Firma.Equals(Firma)).Select(c => new { IdDnevnik = c.IdDnevnik, Adresa = c.Adresa, PTT = c.PTT, Grad = c.Mjesto, Drzava = c.Drzava, Kontakt = c.Kontakt }))).OrderByDescending(c => c.IdDnevnik).FirstOrDefault();

                if (firma != null)
                {

                    if (!String.IsNullOrEmpty(firma.PTT))
                    {
                        var FirmaPodaci1 = new
                        {
                            Adresa = firma.Adresa,
                            PTT = firma.PTT,
                            Grad = firma.Grad,
                            Drzava = firma.Drzava,
                            Kontakt = firma.Kontakt
                        };

                        return Json(FirmaPodaci1, JsonRequestBehavior.AllowGet);
                    }

                    string ptt = new String(firma.Grad.Where(Char.IsDigit).ToArray()) + "";
                    string grad = Regex.Replace(firma.Grad, @"[\d-]", string.Empty) + "";

                    var DrzavaISO = db.Grad.Where(c => firma.Grad.Contains(c.Grad1) && firma.Grad.Contains(c.ISO) ).FirstOrDefault();
                    var Drzava = DrzavaISO == null ? "" : db.Drzava.Where(c => c.ISO.Equals(DrzavaISO.ISO)).SingleOrDefault().Naziv;

                    var FirmaPodaci = new {
                        Adresa = firma.Adresa,
                        PTT = String.IsNullOrEmpty(firma.PTT) ? ptt : firma.PTT,
                        Grad = ptt.Equals("") ? firma.Grad.Trim() : firma.Grad.Replace(ptt, "").Trim(),
                        Drzava = Drzava,
                        Kontakt = firma.Kontakt
                    };

                    return Json(FirmaPodaci, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var FirmaPodaci = new { Adresa = "", PTT = "", Grad = "", Drzava = "", Kontakt =  "" };
                    return Json(FirmaPodaci, JsonRequestBehavior.AllowGet);
                }
   
            }

        }

        /*
        public JsonResult AjaxPodaciOKontaktu(String Firma)
        {
            var dnevnik = db.DnevnikPrevoza.Where(c => c.UtovarFirma.Equals)
        }
        */

        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase file, int IdDnevnik)
        {

            for (int i = 0; i < Request.Files.Count; i++)
            {

                string fn = Request.Files[i].FileName;
                string path = Server.MapPath("~/Dokumenti/" + fn);
                while (System.IO.File.Exists(path))
                {
                    fn = "novi_" + fn;
                    path = Server.MapPath("~/Dokumenti/" + fn);

                }
                Request.Files[i].SaveAs(path);

                Dokument dokument = new Dokument();

                dokument.IdDnevnik = IdDnevnik;
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
        public ActionResult Calendar()
        {
            return View();
        }

        [SpedicijaAutorizacija(Roles = "admin")]
        public JsonResult AjaxCalendar(String start, String end)
        {



            String[] array_start = start.Split('-');
            String[] array_end = end.Split('-');
            DateTime date_start = new DateTime(Convert.ToInt32(array_start[0]), Convert.ToInt32(array_start[1]), Convert.ToInt32(array_start[2]));
            DateTime date_end = new DateTime(Convert.ToInt32(array_end[0]), Convert.ToInt32(array_end[1]), Convert.ToInt32(array_end[2]));


            var utovar = db.DnevnikPrevoza.Where(c => (c.ZapisAktivan ?? false) && c.DatumUtovara <= date_end && c.DatumUtovara >= date_start).ToList();
            var u = utovar.Select(c => new 
            {
                id = c.IdDnevnik.ToString(),
                url = "/DnevnikPrevoza/Edit/" + c.IdDnevnik,
                title = "UTOVAR: " + c.UtovarFirma + ", " + c.UtovarAdresa + ", " + c.UtovarGrad,
                start = c.DatumUtovara.Value.Year + "-" +
                        (c.DatumUtovara.Value.Month < 10 ? ("0" + c.DatumUtovara.Value.Month) : "" + c.DatumUtovara.Value.Month) + "-" +
                        (c.DatumUtovara.Value.Day < 10 ? ("0" + c.DatumUtovara.Value.Day) : "" + c.DatumUtovara.Value.Day) +
                        "T00:00:00"
            });

            var istovar = db.DnevnikPrevoza.Where(c => (c.ZapisAktivan ?? false) && c.DatumIstovara <= date_end && c.DatumIstovara >= date_start).ToList();
            var i = istovar.Select(c => new
            {
                id = c.IdDnevnik.ToString(),
                url = "/DnevnikPrevoza/Edit/" + c.IdDnevnik,
                title = "ISTOVAR: " + c.UtovarFirma + ", " + c.UtovarAdresa + ", " + c.UtovarGrad,
                start = c.DatumIstovara.Value.Year + "-" +
                        (c.DatumIstovara.Value.Month < 10 ? ("0" + c.DatumIstovara.Value.Month) : "" + c.DatumIstovara.Value.Month) + "-" +
                        (c.DatumIstovara.Value.Day < 10 ? ("0" + c.DatumIstovara.Value.Day) : "" + c.DatumIstovara.Value.Day) +
                        "T00:00:00"
            });

            var rez = i.Union(u).ToList();

            return Json(rez, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult ObavezeDanas()
        {
            var obaveze = db.DnevnikPrevoza.Where(c => (c.ZapisAktivan ?? false) && ((c.DatumUtovara.HasValue && c.DatumUtovara == DateTime.Today) && (c.DatumIstovara.HasValue && c.DatumIstovara == DateTime.Today))).ToList().Select
                (c => new { IdDnevnik = c.IdDnevnik, Datum = c.DatumUtovara == DateTime.Today.AddHours(7) ? c.DatumUtovara.Value.ToShortDateString() : c.DatumIstovara.Value.ToShortDateString(), 
                    Tip = c.DatumUtovara == DateTime.Today.AddHours(7) ? "Utovar: "+c.UtovarFirma+", "+c.UtovarGrad : "Istovar: "+c.IstovarFirma+", "+c.IstovarGrad }).ToList();
            return Json(obaveze, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [SpedicijaAutorizacija(Roles = "admin")]
        public ActionResult DeleteDokument(int id = 0)
        {

            Dokument dokument = db.Dokument.Find(id);
            int back = dokument.IdDnevnik ?? 0;
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


        [Authorize]
        [SpedicijaAutorizacija(Roles = "admin")]
        public JsonResult DeleteDokumentAjax(int id = 0)
        {

            Dokument dokument = db.Dokument.Find(id);
            string path = Server.MapPath("~/Dokumenti/" + dokument.Naziv);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
                Dokument dok = db.Dokument.Where(c => c.Putanja.Equals(path)).FirstOrDefault();
                db.Dokument.Remove(dok);
                db.SaveChanges();
            }

            return Json("OK", JsonRequestBehavior.AllowGet);
        }


        [Authorize]
        public JsonResult VratiDokumenteAjax(int id = 0)
        {

            DnevnikPrevoza d = db.DnevnikPrevoza.Find(id);

            var rez = d.Dokument.Select( c => new { IdDokument = c.IdDokument, IdDnevnik = c.IdDnevnik, Naziv = c.Naziv, Putanja = c.Putanja, Tip = c.Tip, Velicina = c.Velicina } ).ToList();

            return Json(rez, JsonRequestBehavior.AllowGet);
        }


        [Authorize]
        public ActionResult PromjenaStatusa(int id, int Status)
        { 
            DnevnikPrevoza dnevnik = db.DnevnikPrevoza.Find(id);

            if (Status != -1)
            {   
                dnevnik.Status = Status == 0 ? "NIJE UTOVARENO" : Status == 1 ? "UTOVARENO U TRANZITU" : Status == 2 ? "ISTOVARENO" : "STOPIRANO";
                if (String.IsNullOrEmpty(dnevnik.GostPristup))
                    dnevnik.GostPristup = RandomString(25);

                db.Entry(dnevnik).State = EntityState.Modified;
                db.SaveChanges();
            }

            var lst = db.DnevnikPrevoza.ToList().Where(c => c.IdDnevnik == id).Select(c => new { DatUtovara = c.DatumUtovara.HasValue ? c.DatumUtovara.Value.ToShortDateString() : "", Status = c.Status, Naziv = c.Subjekt1.Naziv.Replace("&", "and"), IstovarFirma = c.IstovarFirma.Replace("&", "and"), IstovarGrad = c.IstovarPTT + " " + c.IstovarGrad + " " + c.IstovarDrzava, IstovarAdresa = c.IstovarAdresa.Replace("&", "and"), UtovarFirma = c.UtovarFirma.Replace("&", "and"), UtovarGrad = c.UtovarPTT + " " + c.UtovarGrad + " " + c.UtovarDrzava, UtovarAdresa = c.UtovarAdresa.Replace("&", "and"), Email = c.Subjekt1.Email, SerijskiBroj = c.SerijskiBroj, BrojNaloga = c.BrojNaloga, Guest = c.GostPristup, Uvoznik = String.Join(", ", c.DnevnikUvoznikIzvoznik.Select(r => r.Uvoznik)).Replace("&", "and"), Izvoznik = String.Join(", ", c.DnevnikUvoznikIzvoznik.Select(r => r.Izvoznik)).Replace("&", "and"), DatIstovara = c.DatumIstovara.HasValue ? c.DatumIstovara.Value.ToShortDateString() : "" }).ToList();

            lst.AddRange(dnevnik.DnevnikIstovar.ToList().Select(c => new { DatUtovara = "", Status = dnevnik.Status, Naziv = dnevnik.Subjekt1.Naziv.Replace("&", "and"), IstovarFirma = (c.Firma == null) ? "" : c.Firma.Replace("&", "and"), IstovarGrad = (c.Mjesto == null) ? "" : c.PTT + " " + c.Mjesto.Replace("&", "and") + " " + c.Drzava, IstovarAdresa = (c.Adresa == null) ? "" : c.Adresa.Replace("&", "and"), UtovarFirma = "", UtovarGrad = "", UtovarAdresa = "", Email = (dnevnik.Subjekt1 == null) ? "" : dnevnik.Subjekt1.Email, SerijskiBroj = dnevnik.SerijskiBroj + "/1", BrojNaloga = dnevnik.BrojNaloga, Guest = dnevnik.GostPristup, Uvoznik = String.Join(", ", dnevnik.DnevnikUvoznikIzvoznik.Select(r => r.Uvoznik)).Replace("&", "and"), Izvoznik = String.Join(", ", dnevnik.DnevnikUvoznikIzvoznik.Select(r => r.Izvoznik)).Replace("&", "and"), DatIstovara = c.DatumIstovara.HasValue ? c.DatumIstovara.Value.ToShortDateString() : "" }));
            lst.AddRange(dnevnik.DnevnikUtovar.ToList().Select(c => new { DatUtovara = "", Status = dnevnik.Status, Naziv = dnevnik.Subjekt1.Naziv.Replace("&", "and"), IstovarFirma = "", IstovarGrad = "", IstovarAdresa = "", UtovarFirma = (c.Firma == null) ? "" : c.Firma.Replace("&", "and"), UtovarGrad = (c.Mjesto == null) ? "" : c.PTT + " " + c.Mjesto.Replace("&", "and") + " " + c.Drzava, UtovarAdresa = (c.Adresa == null) ? "" : c.Adresa.Replace("&", "and"), Email = (dnevnik.Subjekt1 == null) ? "" : dnevnik.Subjekt1.Email, SerijskiBroj = dnevnik.SerijskiBroj + "/1", BrojNaloga = dnevnik.BrojNaloga, Guest = dnevnik.GostPristup, Uvoznik = String.Join(", ", dnevnik.DnevnikUvoznikIzvoznik.Select(r => r.Uvoznik)).Replace("&", "and"), Izvoznik = String.Join(", ", dnevnik.DnevnikUvoznikIzvoznik.Select(r => r.Izvoznik)).Replace("&", "and"), DatIstovara = "" }));

            return Json(lst, JsonRequestBehavior.AllowGet);
        }


        public ActionResult StopirajPrevoz(int id)
        {
            DnevnikPrevoza dnevnik = db.DnevnikPrevoza.Find(id);

            if (!dnevnik.Status.Equals("STOPIRANO"))
            {
                dnevnik.Status = "STOPIRANO";
            }
            else
            {
                dnevnik.Status = "NIJE UTOVARENO";
            }

            db.Entry(dnevnik).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("DetailsTab", new { id = id });
        }

        [HttpGet]
        [Authorize]
        [SpedicijaAutorizacija(Roles = "admin")]
        public ActionResult IzracunZarada()
        {
            var dnevniciZaIzbor = db.DnevnikPrevoza.Where(c => false).ToList();
            Session["Report"] = new List<DnevnikPrevoza>() ;
            Session["DodatniTroskovi"] = new List<DodatniTroskoviIzracun>();
            ViewBag.troskovi = "0";
            ViewBag.dnevnice = "0";
            ViewBag.cijenaPrevoza = "0";
            ViewBag.cijenaDrugogPrevoznika = "0";
            ViewBag.sum = "0";
            ViewBag.Od = String.Empty;
            ViewBag.Do = String.Empty;
            return View(dnevniciZaIzbor);
        }

        [HttpPost]
        public ActionResult IzracunZarada(DodatniTroskoviIzracun dodatniTroskovi, int add = 0, DateTime? Od = null, DateTime? Do = null, int IdDnevnik = 0 )
        { 
           
            DnevnikPrevoza dp = db.DnevnikPrevoza.Find(IdDnevnik);

            if (Session["Report"] == null)
            {
                if (add == 1)
                {
                    List<DnevnikPrevoza> list = new List<DnevnikPrevoza>();
                    if (dp != null) list.Add(dp);
                    Session["Report"] = list;
                }
            }
            else if (((List<DnevnikPrevoza>)Session["Report"]).Count() == 0)
            {
                if (add == 1)
                {
                    List<DnevnikPrevoza> list = new List<DnevnikPrevoza>();
                    if (dp != null) list.Add(dp);
                    Session["Report"] = list;
                }
            }
            else
            {
                if (dp != null)
                {
                    if (add == 1) ((List<DnevnikPrevoza>)Session["Report"]).Add(dp);
                    else (Session["Report"]) = ((List<DnevnikPrevoza>)Session["Report"]).Where(c => c.IdDnevnik != IdDnevnik).ToList();
                }
            }




            var lstIdDok = ((List<DnevnikPrevoza>)Session["Report"]).Select(c => c.IdDnevnik ).ToList();

            var dnevniciZaIzbor = db.DnevnikPrevoza.Where(c => (c.ZapisAktivan ?? false) && ((c.DatumUtovara >= Od && c.DatumUtovara <= Do) || (c.DatumUtovara >= Od && c.DatumUtovara <= Do))).
                 ToList().Where(c => !lstIdDok.Contains(c.IdDnevnik)).ToList(); //Except(((List<DnevnikPrevoza>)Session["Report"])).ToList();

           

            Decimal troskovi = ((List<DnevnikPrevoza>)Session["Report"]).Sum(c => c.Troskovi.Sum(g => g.Iznos * g.Valuta.UKM)) ?? 0;

            Decimal dnevnice = (from dt in db.DnevnicaDetail
                          join vd in db.VozacDnevnica on dt.IdVozacDnevnica equals vd.IdVozacDnevnica
                          join v in db.Vozaci on vd.IdVozac equals v.IdVozac
                          join d in db.DnevnikPrevoza on v.IdVozac equals d.IdVozac
                          where lstIdDok.Contains(d.IdDnevnik) && dt.Datum >= d.DatumUtovara && dt.Datum <= d.DatumIstovara
                          select dt.Dnevnica).Sum() ?? 0;

            Decimal cijenaPrevoza = ((List<DnevnikPrevoza>)Session["Report"]).Sum(c => c.CijenaPrevoza ?? 0 * (c.Valuta == null ? 1 : c.Valuta.UKM ?? 1) );
            Decimal cijenaDrugogPrevoznika = ((List<DnevnikPrevoza>)Session["Report"]).Sum(c => ((c.CijenaPrevozaPrevoznika ?? 0) * (c.Valuta1 == null ? 1 : c.Valuta1.UKM ?? 1)));

            Decimal dodatni = (Session["DodatniTroskovi"] as List<DodatniTroskoviIzracun>).Sum(c => c.IznosTroskova);

            var MaxList = (Session["DodatniTroskovi"] as List<DodatniTroskoviIzracun>).Select(c => c.IznosTroskova).ToList();
            MaxList.Add(cijenaPrevoza);
            MaxList.Add(cijenaDrugogPrevoznika);
            MaxList.Add(troskovi);
            MaxList.Add(dnevnice);
            MaxList.Add(dodatniTroskovi.IznosTroskova);
            Decimal max = MaxList.Max();


            if (!String.IsNullOrEmpty(dodatniTroskovi.DodatniTrosak))
            {              
                dodatniTroskovi.postotak = cijenaPrevoza == 0 ? 0 : (int)(dodatniTroskovi.IznosTroskova / max * 100);
                (Session["DodatniTroskovi"] as List<DodatniTroskoviIzracun>).Add(dodatniTroskovi);
                dodatni = (Session["DodatniTroskovi"] as List<DodatniTroskoviIzracun>).Sum(c => c.IznosTroskova);
            }

            Decimal sum = cijenaPrevoza - cijenaDrugogPrevoznika + troskovi - dnevnice - dodatni;


            ViewBag.troskovi = troskovi.ToString("0.00");
            ViewBag.dnevnice = dnevnice.ToString("0.00");
            ViewBag.cijenaPrevoza = cijenaPrevoza.ToString("0.00");
            ViewBag.cijenaDrugogPrevoznika = cijenaDrugogPrevoznika.ToString("0.00");
            ViewBag.sum = sum.ToString("0.00");
            ViewBag.Od = Od;
            ViewBag.Do = Do;

            ViewBag.cijenaPrevozaP = max == 0 ? 0 : (int)(cijenaPrevoza / max * 100);
            ViewBag.cijenaDrugogPrevoznikaP = max == 0 ? 0 : (int)(cijenaDrugogPrevoznika / max * 100);
            ViewBag.dnevniceP = max == 0 ? 0 : (int)(dnevnice / max * 100);
            ViewBag.troskoviP = max == 0 ? 0 : (int)(troskovi / max * 100);
            ViewBag.sumP = max == 0 ? 0 : (int)(sum / max * 100);
 

            return View(dnevniciZaIzbor);
        }

        [HttpPost]
        public ActionResult SacuvajIzracun(String NazivIzracuna)
        {

            IzracunZarada izracun = new IzracunZarada();
            izracun.Naziv = NazivIzracuna;
            izracun.DatumZapisa = DateTime.Today.AddHours(7);
            izracun.ZapisAktivan = true;
            db.IzracunZarada.Add(izracun);
            db.SaveChanges();

            foreach (var item in Session["Report"] as List<DnevnikPrevoza>)
            {
                IzracunDnevnik id = new IzracunDnevnik();
                id.IdDnevnik = item.IdDnevnik;
                id.IdIzracunZarada = izracun.IdIzracunZarada;
                db.IzracunDnevnik.Add(id);
                db.SaveChanges();
            }

            foreach (var item in Session["DodatniTroskovi"] as List<DodatniTroskoviIzracun>)
            {
                IzracunTrosak it = new IzracunTrosak();
                it.DodatniTrosak = item.DodatniTrosak;
                it.IznosTroskova = item.IznosTroskova;
                it.Postotak = item.postotak;
                it.IdIzracunZarada = izracun.IdIzracunZarada;

                db.IzracunTrosak.Add(it);
                db.SaveChanges();
            }

            return RedirectToAction("IzracunZarada");
        }

        [Authorize]
        public ActionResult SacuvaniIzracuni()
        {

            var list = db.IzracunZarada.Where(c => c.ZapisAktivan ?? false).Take(100).ToList().OrderByDescending(c => c.DatumZapisa).ToList();

            return View(list);
        }

        [ValidateInput(false)]
        public ActionResult AjaxNotesAdd(String text)
        {
            Notes notes = new Notes();
            notes.Datum = DateTime.Today.AddHours(7);
            notes.Tekst = text;
            notes.Korisnik = User.Identity.Name;
            db.Notes.Add(notes);
            db.SaveChanges();

            var list = db.Notes.Where(c => c.Datum == DateTime.Today && c.Korisnik.Equals(User.Identity.Name)).ToList().OrderByDescending(c => c.IdNotes).Select(c => new { IdNotes = c.IdNotes, Datum = c.Datum.ToShortDateString(), Tekst = c.Tekst, Korisnik = c.Korisnik });
            
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        public ActionResult AjaxNotesEdit(int IdNotes, String text)
        {
            Notes notes = db.Notes.Find(IdNotes);

            if (SpedicijaAutorizacija.ImaRolu("admin") || notes.Korisnik.Equals(User.Identity.Name))
            {
                notes.Tekst = text;
                notes.Datum = DateTime.Today.AddHours(7);

                db.Entry(notes).State = EntityState.Modified;
                db.SaveChanges();

                return Json("Ažuriranje uspješno!", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Nemate pravo da mjenjate zapis", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AjaxNotesDelete(int IdNotes)
         {
             Notes notes = db.Notes.Find(IdNotes);

             if (SpedicijaAutorizacija.ImaRolu("admin") || notes.Korisnik.Equals(User.Identity.Name))
             {
                 db.Notes.Remove(notes);
                 db.SaveChanges();

                 return Json("Zapis izbrisan!", JsonRequestBehavior.AllowGet);
             }
             else
             {
                 return Json("Nemate pravo da obrišete ovaj zapis", JsonRequestBehavior.AllowGet);
             }
         }

        public ActionResult AjaxNotesPoDatumu(DateTime datum, String korisnik)
        {
           // var rola = db.Korisnik.Single(c => c.KorisnickoIme.Equals(User.Identity.Name)).Role;
            Session["NotesIzbor"] = korisnik;

            if (korisnik.Equals("svi")) //&& rola.Equals("admin"))
            {
                var list = db.Notes.Where(c => c.Datum == datum).ToList().OrderByDescending(c => c.IdNotes).Select(c => new { IdNotes = c.IdNotes, Datum = c.Datum.ToShortDateString(), Tekst = c.Tekst, Korisnik = c.Korisnik }).Take(100);
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var list = db.Notes.Where(c => c.Datum == datum && c.Korisnik.Equals(User.Identity.Name)).ToList().OrderByDescending(c => c.IdNotes).Select(c => new { IdNotes = c.IdNotes, Datum = c.Datum.ToShortDateString(), Tekst = c.Tekst, Korisnik = c.Korisnik }).Take(100);
                return Json(list, JsonRequestBehavior.AllowGet);
            }

            
        }

        public ActionResult AjaxNotesPoId(int IdNotes)
        {

            var list = db.Notes.Where(c => c.IdNotes == IdNotes).ToList().Select(c => new { IdNotes = c.IdNotes, Datum = c.Datum.ToShortDateString(), Tekst = c.Tekst, Korisnik = c.Korisnik }).FirstOrDefault();

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AjaxNotesSvi(String korisnik)
        {
           // var rola = db.Korisnik.Single(c => c.KorisnickoIme.Equals(User.Identity.Name)).Role;
            Session["NotesIzbor"] = korisnik;

            if (korisnik.Equals("svi") )// && rola.Equals("admin"))
            {
                var list = db.Notes.ToList().OrderByDescending(c => c.IdNotes).Select(c => new { IdNotes = c.IdNotes, Datum = c.Datum.ToShortDateString(), Tekst = c.Tekst, Korisnik = c.Korisnik }).Take(100);
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var list = db.Notes.Where(c => c.Korisnik.Equals(User.Identity.Name)).ToList().OrderByDescending(c => c.IdNotes).Select(c => new { IdNotes = c.IdNotes, Datum = c.Datum.ToShortDateString(), Tekst = c.Tekst, Korisnik = c.Korisnik }).Take(100);
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            
        }

        public ActionResult PrikaziIzracun(int id = 0)
        {

            IzracunZarada IZ = db.IzracunZarada.Find(id);

            if (IZ != null)
            {
                Session["Report"] = db.DnevnikPrevoza.Where(c => c.IzracunDnevnik.Any(k => k.IdIzracunZarada == id)).ToList();
                Session["DodatniTroskovi"] = IZ.IzracunTrosak.ToList().Select(c => new DodatniTroskoviIzracun { DodatniTrosak = c.DodatniTrosak, IznosTroskova = c.IznosTroskova, postotak = c.Postotak }).ToList();
            }

            var lstIdDok = ((List<DnevnikPrevoza>)Session["Report"]).Select(c => c.IdDnevnik).ToList();

            Decimal troskovi = ((List<DnevnikPrevoza>)Session["Report"]).Sum(c => c.Troskovi.Sum(g => g.Iznos * g.Valuta.UKM)) ?? 0;

            Decimal dnevnice = (from dt in db.DnevnicaDetail
                                join vd in db.VozacDnevnica on dt.IdVozacDnevnica equals vd.IdVozacDnevnica
                                join v in db.Vozaci on vd.IdVozac equals v.IdVozac
                                join d in db.DnevnikPrevoza on v.IdVozac equals d.IdVozac
                                where lstIdDok.Contains(d.IdDnevnik) && dt.Datum >= d.DatumUtovara && dt.Datum <= d.DatumIstovara
                                select dt.Dnevnica).Sum() ?? 0;

            Decimal cijenaPrevoza = ((List<DnevnikPrevoza>)Session["Report"]).Sum(c => c.CijenaPrevoza ?? 0 * (c.Valuta == null ? 1 : c.Valuta.UKM ?? 1));
            Decimal cijenaDrugogPrevoznika = ((List<DnevnikPrevoza>)Session["Report"]).Sum(c => ((c.CijenaPrevozaPrevoznika ?? 0) * (c.Valuta1 == null ? 1 : c.Valuta1.UKM ?? 1)));

            Decimal dodatni = (Session["DodatniTroskovi"] as List<DodatniTroskoviIzracun>).Sum(c => c.IznosTroskova);

            var MaxList = (Session["DodatniTroskovi"] as List<DodatniTroskoviIzracun>).Select(c => c.IznosTroskova).ToList();
            MaxList.Add(cijenaPrevoza);
            MaxList.Add(cijenaDrugogPrevoznika);
            MaxList.Add(troskovi);
            MaxList.Add(dnevnice);
            Decimal max = MaxList.Max();

            Decimal sum = cijenaPrevoza - cijenaDrugogPrevoznika + troskovi - dnevnice - dodatni;


            ViewBag.troskovi = troskovi.ToString("0.00");
            ViewBag.dnevnice = dnevnice.ToString("0.00");
            ViewBag.cijenaPrevoza = cijenaPrevoza.ToString("0.00");
            ViewBag.cijenaDrugogPrevoznika = cijenaDrugogPrevoznika.ToString("0.00");
            ViewBag.sum = sum.ToString("0.00");
            ViewBag.Od = "";
            ViewBag.Do = "";

            ViewBag.cijenaPrevozaP = max == 0 ? 0 : (int)(cijenaPrevoza / max * 100);
            ViewBag.cijenaDrugogPrevoznikaP = max == 0 ? 0 : (int)(cijenaDrugogPrevoznika / max * 100);
            ViewBag.dnevniceP = max == 0 ? 0 : (int)(dnevnice / max * 100);
            ViewBag.troskoviP = max == 0 ? 0 : (int)(troskovi / max * 100);
            ViewBag.sumP = max == 0 ? 0 : (int)(sum / max * 100);


            return View("IzracunZarada", new List<DnevnikPrevoza>());
        }

        public ActionResult PrikaziIzracunDelete(int id = 0)
        {

            IzracunZarada IZ = db.IzracunZarada.Find(id);

            if (IZ != null)
            {
                IZ.ZapisAktivan = false;
                db.Entry(IZ).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("SacuvaniIzracuni");
            
        }

        public JsonResult AjaxRemoveTrosak(String trosak)
        {
            Session["DodatniTroskovi"] = (Session["DodatniTroskovi"] as List<DodatniTroskoviIzracun>).Where(c => !c.DodatniTrosak.Equals(trosak)).ToList();

            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        public ActionResult DnevnikLog(int id = 0)
        {
             var serializer = new JavaScriptSerializer();
            //var result = serializer.DeserializeObject(s);

             List<DnevnikViewModel> list = db.Log.Where(c => c.Tabela.Equals("DnevnikPrevoza") && c.PK == id).ToList().
                 Select(d => new DnevnikViewModel { Staro = serializer.Deserialize<DnevnikPrevoza>(d.StaraVrijednost), Novo = serializer.Deserialize<DnevnikPrevoza>(d.NovaVrijednost), Log = d }).OrderByDescending(c => c.Log.Datum).
               //  Where(c => c.Staro != null && ( c.Staro.CijenaPrevoza != c.Novo.CijenaPrevoza || c.Staro.SaPDV != c.Novo.SaPDV || c.Novo.CijenaPrevozaPrevoznika != c.Staro.CijenaPrevozaPrevoznika)).
                 ToList();



             return View(list);
        }

        public ActionResult AzurirajDnevnikLog(int id = 0, String Status = "", bool Pregledano = false)
        {
            Log log = db.Log.Find(id);
            log.StatusPromjene = Status;
            log.Pregledano = Pregledano;
            db.Entry(log).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("DnevnikLog", new { id = log.PK });
        }

        public JsonResult KriticnePromjeneLog()
        {
            var logovi = db.Log.Where(c => c.StatusPromjene.Equals("Kritično") && !(c.Pregledano ?? true)).ToList().
                Select(d => new { IdDnevnik = d.PK, Datum = d.Datum.Value.ToShortDateString() + " " + d.Datum.Value.ToLongTimeString(), KorisnickoIme = d.KorisnickoIme, IdLog = d.IdLog }).ToList();
            return Json(logovi, JsonRequestBehavior.AllowGet);

        }

        [AllowAnonymous]
        public ActionResult GuestDetail(String s = "")
        {
            DnevnikPrevoza dp = db.DnevnikPrevoza.Where(c => c.GostPristup.Equals(s)).FirstOrDefault();
            return View(dp);
        }

        [Authorize]
        public ActionResult IstekleValute(int id = 0)
        {
          /* var list = db.DnevnikPrevoza.Include(d => d.Subjekt1).ToList()
                .Where(c => !(c.Uplaceno ?? false) && ((c.DatumSlanjaFakture ?? DateTime.Today).AddDays(c.ValutaPlacanja ?? 999999) < DateTime.Today)).ToList();
            */
           DnevnikPrevoza dp = db.DnevnikPrevoza.Where(c => c.IdDnevnik == id).FirstOrDefault();

           ViewBag.Narucioc = dp == null ? new Subjekt() : dp.Subjekt1;
           ViewBag.Upozorenja = dp == null ? new List<DnevnikValute>() : dp.DnevnikValute.OrderBy(c => c.DatumAktivnosti).ToList();
           ViewBag.IdDnevnik = id;
           ViewBag.SerijskiBroj = dp == null ? "" : dp.SerijskiBroj;
           ViewBag.Status = dp == null ? "" : (dp.Uplaceno ?? false) ? "NAPLAĆENO" : "NIJE NAPLAĆENO";

           return View();
        }

        public JsonResult BrojIsteklihValuta()
        {
            int br = db.DnevnikPrevoza.ToList().Where(c => !(c.Uplaceno ?? false) && ((c.DatumSlanjaFakture ?? DateTime.Today.AddHours(7)).AddDays(c.ValutaPlacanja ?? 999999) < DateTime.Today.AddHours(7))).Count();

            return Json(br, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DetaljniStatus(int id)
        {

            var lstStatusa = db.StatusRobe.Select(c => new { c.IdStatusRobe, c.Naziv }).ToList();
            var dnevnik = db.DnevnikPrevoza.Find(id);

            var rez = new { lstStatusa, dnevnik.idStatusDetaljni, dnevnik.StatusDetaljniOpis };

            return Json(rez, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Faktura(int id = 0)
        {
            DnevnikPrevoza d = db.DnevnikPrevoza.Find(id);

            if (d == null)
                return HttpNotFound();

            List<FakturaView> fakturaList = new List<FakturaView>();

            int Rb = 1;
            String SerijskiBroj = d.SerijskiBroj;
            String Opis = "Prevoz na relaciji: " + System.Environment.NewLine + d.UtovarPTT + " " + d.UtovarGrad + " " + d.UtovarDrzava + " - " + d.IstovarPTT + " " + d.IstovarGrad + " " + d.IstovarDrzava +
                          System.Environment.NewLine + "Roba: " + d.VrstaRobe + " Težina: " + d.TezinaRobe + " Kg Dimenzije: " + d.DimenzijeRobe;
            Decimal CijenaBezPdv = (d.CijenaPrevoza ?? 0) - (d.IznosPDV ?? 0);
            Decimal CijenaBezPdvBAM = ((d.CijenaPrevoza ?? 0) - (d.IznosPDV ?? 0)) * (d.Valuta.UKM ?? 0);
            Decimal Pdv = (d.IznosPDV ?? 0);
            Decimal PdvBAM = (d.IznosPDV ?? 0) * (d.Valuta.UKM ?? 0);
            String  Valuta = d.Valuta.OznakaValute;

            var OsnovniPrevoz = new FakturaView {
                Rb = Rb,
                SerijskiBroj = SerijskiBroj,
                Opis = Opis,
                CijenaBezPdv = CijenaBezPdv,
                CijenaBezPdvBAM = CijenaBezPdvBAM,
                Pdv = Pdv,
                PdvBAM = PdvBAM,
                Valuta = Valuta
            };

            int r = 1;
            var DodatneUslugeOsnovniPrevoz = d.Troskovi.Select(c => new FakturaView
            {
                Rb = ++r,
                SerijskiBroj = c.DnevnikPrevoza.SerijskiBroj,
                Opis = "Dodatna usluga: " +  c.Vrsta,
                CijenaBezPdv = (c.Iznos ?? 0),
                CijenaBezPdvBAM = (c.Iznos ?? 0) * (c.Valuta.UKM ?? 0),
                Pdv = 0,
                PdvBAM = 0,
                Valuta = c.Valuta.OznakaValute
            }).ToList();

            fakturaList.Add(OsnovniPrevoz);
            fakturaList.AddRange(DodatneUslugeOsnovniPrevoz);

            var PodDnevnici = db.DnevnikPrevoza.Where(c => c.IdDnevnikParent == id).ToList();

            foreach (var d1 in PodDnevnici)
            {
                FakturaView PodPrevoz = new FakturaView
                {
                    Rb = ++r,
                    SerijskiBroj = d1.SerijskiBroj,
                    Opis = "Prevoz na relaciji: " + System.Environment.NewLine + d1.UtovarPTT + " " + d1.UtovarGrad + " " + d1.UtovarDrzava + " - " + d1.IstovarPTT + " " + d1.IstovarGrad + " " + d1.IstovarDrzava +
                          System.Environment.NewLine + "Roba: " + d1.VrstaRobe + " Težina: " + d1.TezinaRobe + " Kg Dimenzije: " + d1.DimenzijeRobe,
                    CijenaBezPdv = (d1.CijenaPrevoza ?? 0) - (d1.IznosPDV ?? 0),
                    CijenaBezPdvBAM = ((d1.CijenaPrevoza ?? 0) - (d1.IznosPDV ?? 0)) * (d1.Valuta.UKM ?? 0),
                    Pdv = (d1.IznosPDV ?? 0),
                    PdvBAM = (d1.IznosPDV ?? 0) * (d1.Valuta.UKM ?? 0),
                    Valuta = d1.Valuta.OznakaValute
                };

                fakturaList.Add(PodPrevoz);

                var DodatneUslugePodPrevoz = d1.Troskovi.Select(c => new FakturaView
                {
                    Rb = ++r,
                    SerijskiBroj = c.DnevnikPrevoza.SerijskiBroj,
                    Opis = "Dodatna usluga: " + c.Vrsta,
                    CijenaBezPdv = (c.Iznos ?? 0),
                    CijenaBezPdvBAM = (c.Iznos ?? 0) * (c.Valuta.UKM ?? 0),
                    Pdv = 0,
                    PdvBAM = 0,
                    Valuta = c.Valuta.OznakaValute
                }).ToList();

                fakturaList.AddRange(DodatneUslugePodPrevoz);
            }

            var rez = new FakturaMaster
            {
                Narucioc = d.Subjekt1,
                SerijskiBroj = d.SerijskiBroj,
                DatumFakture = (d.DatumSlanjaFakture.HasValue ? d.DatumSlanjaFakture.Value.ToShortDateString() : "" ),
                ValutaPlacanja = (d.ValutaPlacanja ?? 0) + " dana",
                Vozac = (d.Vozaci == null) ? "" : d.Vozaci.ImeVozaca + " (tel: " + d.Vozaci.Telefon + ")",
                Vozilo = (d.Vozilo == null) ? "" :  d.Vozilo.TipVozila + " (reg: " + d.Vozilo.RegistarskiBroj + ")",
                Finansije = fakturaList
            };

            ViewBag.BEZPDV = rez.Finansije.Sum(c => c.CijenaBezPdvBAM).ToString("0.00"); 
            ViewBag.PDV = rez.Finansije.Sum(c => c.PdvBAM).ToString("0.00"); 
            ViewBag.SUMA = rez.Finansije.Sum(c => (c.CijenaBezPdvBAM + c.PdvBAM)).ToString("0.00");

            return View(rez);
        }

        [ValidateInput(false)]
        public JsonResult SacuvajPodesavanja(int id, int idStatus, String text) {

            try
            {
                DnevnikPrevoza dp = db.DnevnikPrevoza.Find(id);

                dp.idStatusDetaljni = idStatus;
                dp.StatusDetaljniOpis = text;

                db.Entry(dp).State = EntityState.Modified;
                db.SaveChanges();

                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public ActionResult ShareDokument() {


            var vozila = (from a in db.Vozilo
                          join b in db.Dokument on a.IdVozilo equals b.IdVozilo
                          where a.ZapisAktivan
                          select new ShareDoc {
                              IdDokument = b.IdDokument,
                              Ko = a.TipVozila + " " + a.RegistarskiBroj,
                              Putanja = b.Putanja,
                              Naziv = b.Naziv
                          }).ToList();

            var vozaci = (from a in db.Vozaci
                          join b in db.Dokument on a.IdVozac equals b.IdVozac
                          where (a.ZapisAktivan ?? false)
                          select new ShareDoc
                          {
                              IdDokument = b.IdDokument,
                              Ko = a.ImeVozaca,
                              Putanja = b.Putanja,
                              Naziv = b.Naziv
                          }).ToList();

            var dnevnici = db.Dokument.Include(k => k.DnevnikPrevoza).Where(c => (c.DnevnikPrevoza.ZapisAktivan ?? false)).Select(c => new {
                IdDnevnik = c.IdDnevnik,
                Relacija = c.DnevnikPrevoza.SerijskiBroj + " ["+ c.DnevnikPrevoza.UtovarGrad + " - " + c.DnevnikPrevoza.IstovarGrad + "]"
            }).ToList();



            ViewBag.Vozila = vozila;
            ViewBag.Vozaci = vozaci;
            ViewBag.Dnevnici = new SelectList(dnevnici,"IdDnevnik", "Relacija");

            return View();
        }

        public JsonResult GetDocumentsOfDnevnici()
        {

            var a = Request["Lista[]"];
            List<int> Dnevnici = new List<int>();

            if (a != null)
                Dnevnici = a.Split(',').Select(c => Convert.ToInt32(c)).ToList();


            var dokumenti = db.Dokument.Include(k=>k.DnevnikPrevoza).Where(c => Dnevnici.Contains(c.IdDnevnik ?? 0)).Select(c =>  new {
                IdDkoument = c.IdDokument,
                Ko = c.DnevnikPrevoza.SerijskiBroj,
                Naziv = c.Naziv,
                Putanja = c.Putanja
            }).OrderByDescending(c => c.Ko).ToList();

            return Json(dokumenti, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetShareLink()
        {

            var a = Request.Params["dok[]"];
            
            if (a != null)
            {
                var dok = a.Split(',').Select(c => Convert.ToInt32(c)).ToList();

                Random random = new Random();
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                String r = new string(Enumerable.Repeat(chars, 10)
                  .Select(s => s[random.Next(10)]).ToArray());

                foreach (int i in dok)
                {
                    DokumentShare ds = new DokumentShare { IdDokument = i, Kod = r };
                    db.DokumentShare.Add(ds);
                }

                db.SaveChanges();

                return Json(AppSettings.GetSettings()["domain_name"] + "/DnevnikPrevoza/DijeljeniDokumenti?key=" + r, JsonRequestBehavior.AllowGet);
            }

           return Json("#ERROR", JsonRequestBehavior.AllowGet);
        }

        public ActionResult DijeljeniDokumenti(String key)
        {
            List<DokumentShare> lst = db.DokumentShare.Where(c => c.Kod.Equals(key)).ToList();

            if (lst.Count == 0)
                return HttpNotFound();

            //////int CurrentFileID = Convert.ToInt32(FileID);  
            using (var memoryStream = new MemoryStream())
            {
                using (var ziparchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    for (int i = 0; i < lst.Count; i++)
                    {
                        ziparchive.CreateEntryFromFile(lst[i].Dokument.Putanja, lst[i].Dokument.Naziv);
                    }
                }
                return File(memoryStream.ToArray(), "application/zip", "Attachments.zip");
            }

        }

        public JsonResult GetUpdateStatus(int id, String status, String token, int mail, int ? mail2, int ? PodStatus, String LONG, String LAT)
        {

            try
            {
                Korisnik korisnik = db.Korisnik.Where(c => c.Token.Equals(token)).SingleOrDefault();
                DnevnikPrevoza dnevnik = db.DnevnikPrevoza.Find(id);

                if (korisnik != null && dnevnik != null)
                {

                    String _status = status.Contains("utovareno") ? "NIJE UTOVARENO" : (status.Contains("transportu") ? "UTOVARENO U TRANZITU" : "ISTOVARENO");

                    // Ako je drugi prevoznik u pitanju i vozac salje da je istovareno, ne setuj status ves DrugiPrevoznikStatus
                    if ((dnevnik.DrugiPrevoznik ?? false))
                    {
                        if (_status.Equals("ISTOVARENO"))
                        {
                            dnevnik.DrugiPrevoznikStatus = "ISTOVARENO";
                            dnevnik.Status = "UTOVARENO U TRANZITU";
                            dnevnik.idStatusDetaljni = 6;
                        }
                        else
                        {
                            dnevnik.Status = _status;
                        }
                    }

                    if (!(dnevnik.DrugiPrevoznik ?? false)) {

                        if ((PodStatus ?? 0) != 0)
                        {
                            dnevnik.DrugiPrevoznikStatus = "ISTOVARENO";
                            dnevnik.idStatusDetaljni = PodStatus;
                            dnevnik.Status = "UTOVARENO U TRANZITU";
                        }
                        else
                        {
                            dnevnik.Status = _status;
                        }

                    }
                    

                    dnevnik.LAT = LAT;
                    dnevnik.LONG = LONG;
                    db.Entry(dnevnik).State = EntityState.Modified;
                    db.SaveChanges();

                    var log = db.Log.Where(c => c.PK == id && c.Tabela.Equals("DnevnikPrevoza")).OrderByDescending(c => c.IdLog).FirstOrDefault();
                if (log != null)
                {
                    log.KorisnickoIme = korisnik.KorisnickoIme;
                    db.Entry(log).State = EntityState.Modified;
                    db.SaveChanges();
                }
                    AndroidTask at = db.AndroidTask.Where(c => c.IdDnevnik == id).SingleOrDefault();
                    if (korisnik.Role.Contains("admin"))
                        at.AdminPregled = true;
                    else
                    {
                        at.VozacPregled = true;
                        at.AdminPregled = false;
                    }
                    db.Entry(at).State = EntityState.Modified;
                    db.SaveChanges();
                

                    MailMessage _mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient("ml01.anaxanet.com");
                    // GMtel2017.
                    _mail.From = new MailAddress("info@gmtel-office.com", "GMTEL OFFICE");

                    //_mail.To.Add("info@gmtellogistics.com");
                    //_mail.Bcc.Add("m.todorovic87@gmail.com");



                    _mail.IsBodyHtml = true;

                    String text = "<div>";
                    text += "<img src='http://gmtel-office.com/Content/images/Logo.png'>";
                    text += "<h2>" + (status.Contains("transportu") ? "Vaša roba je utovarena i nalazi se u transportu! " : "Vaša roba je istovarena na destinaciji!") + "</h2>";
                    text += "<h4>Detalji transporta :</h4>";
                    text += "</div>";
                    text += "<div>";
                    text += "<p>Serijski broj prevoza: </p>";
                    text += "<p>" + dnevnik.SerijskiBroj + "</p>";
                    text += "<p>Utovar: </p>";
                    text += "<p>" + dnevnik.UtovarGrad + ", " + dnevnik.UtovarAdresa + ", " + dnevnik.UtovarFirma + ", " + (dnevnik.DatumUtovara.HasValue ? dnevnik.DatumUtovara.Value.ToShortDateString() : "") + "</p>";
                    text += "<p>Istovar: </p>";
                    text += "<p>" + dnevnik.IstovarGrad + ", " + dnevnik.IstovarAdresa + ", " + dnevnik.IstovarFirma + ", " + (dnevnik.DatumIstovara.HasValue ? dnevnik.DatumIstovara.Value.ToShortDateString() : "") + "</p>";


                    _mail.Subject = (status.Contains("transportu") ? "VAŠA ROBA JE UTOVARENA" : "VAŠA ROBA JE ISTOVARENA");
                    _mail.Body = text;

                    if(mail == 1 || ((mail2.HasValue) ? (mail2.Value == 1) : false) )
                    {
                       // if (korisnik.Role.Contains("admin"))
                      //  {
                            String subjectEmail = "";
                            if (dnevnik.Subjekt1 != null)
                                subjectEmail = "" + dnevnik.Subjekt1.Email;

                        _mail.To.Add("info@gmtellogistics.com");                          // _mail.To.Add("info@gmtellogistics.com");

                        // ako nije u pitanju drugi prev onda saljem mail
                        if ((!(dnevnik.DrugiPrevoznik ?? false))  && (PodStatus ?? 0) == 0  )
                        {
                            if ((mail2.HasValue) ? (mail2.Value == 1) : false)
                            {
                                if (!subjectEmail.Equals(""))
                                {
                                    if (subjectEmail.Contains(";"))
                                    {
                                        String[] niz = subjectEmail.Split(';');
                                        foreach (String s in niz)
                                        {
                                            if (!s.Equals(""))
                                                _mail.To.Add(s);
                                        }
                                    }
                                    else
                                        _mail.To.Add(subjectEmail);
                                }
                            }
                        }
                            _mail.Bcc.Add("m.todorovic87@gmail.com");
                            

                            var lst = new[] {
                        new {
                        DatUtovara = dnevnik.DatumUtovara.HasValue ? dnevnik.DatumUtovara.Value.ToShortDateString() : "", Status = dnevnik.Status,
                        Naziv = dnevnik.Subjekt1.Naziv.Replace("&", "and"), IstovarFirma = dnevnik.IstovarFirma.Replace("&", "and"),
                        IstovarGrad = dnevnik.IstovarPTT + " " + dnevnik.IstovarGrad + " " + dnevnik.IstovarDrzava, IstovarAdresa = dnevnik.IstovarAdresa.Replace("&", "and"),
                        UtovarFirma = dnevnik.UtovarFirma.Replace("&", "and"), UtovarGrad = dnevnik.UtovarPTT + " " + dnevnik.UtovarGrad + " " + dnevnik.UtovarDrzava,
                        UtovarAdresa = dnevnik.UtovarAdresa.Replace("&", "and"), Email = dnevnik.Subjekt1.Email, SerijskiBroj = dnevnik.SerijskiBroj,
                        BrojNaloga = dnevnik.BrojNaloga, Guest = dnevnik.GostPristup,
                        Uvoznik = String.Join(", ", dnevnik.DnevnikUvoznikIzvoznik.Select(r => r.Uvoznik)).Replace("&", "and"),
                        Izvoznik = String.Join(", ", dnevnik.DnevnikUvoznikIzvoznik.Select(r => r.Izvoznik)).Replace("&", "and"),
                        DatIstovara = dnevnik.DatumIstovara.HasValue ? dnevnik.DatumIstovara.Value.ToShortDateString() : ""
                        }
                    }.ToList();

                            lst.AddRange(dnevnik.DnevnikIstovar.ToList().Select(c => new { DatUtovara = "", Status = dnevnik.Status, Naziv = dnevnik.Subjekt1.Naziv.Replace("&", "and"), IstovarFirma = (c.Firma == null) ? "" : c.Firma.Replace("&", "and"), IstovarGrad = (c.Mjesto == null) ? "" : c.PTT + " " + c.Mjesto.Replace("&", "and") + " " + c.Drzava, IstovarAdresa = (c.Adresa == null) ? "" : c.Adresa.Replace("&", "and"), UtovarFirma = (dnevnik.UtovarFirma == null) ? "" : dnevnik.UtovarFirma.Replace("&", "and"), UtovarGrad = dnevnik.UtovarGrad, UtovarAdresa = (dnevnik.UtovarAdresa == null) ? "" : dnevnik.UtovarAdresa.Replace("&", "and"), Email = (dnevnik.Subjekt1 == null) ? "" : dnevnik.Subjekt1.Email, SerijskiBroj = dnevnik.SerijskiBroj + "/1", BrojNaloga = dnevnik.BrojNaloga, Guest = dnevnik.GostPristup, Uvoznik = String.Join(", ", dnevnik.DnevnikUvoznikIzvoznik.Select(r => r.Uvoznik)).Replace("&", "and"), Izvoznik = String.Join(", ", dnevnik.DnevnikUvoznikIzvoznik.Select(r => r.Izvoznik)).Replace("&", "and"), DatIstovara = c.DatumIstovara.HasValue ? c.DatumIstovara.Value.ToShortDateString() : "" }));

                            int i = 0;
                            var StatusSr = "";
                            var StatusEn = "";
                            var Naslov = "";
                            var SubjectSr = "";
                            var SubjectEN = "";
                            var gostiKod = "";
                            var istovariSR = "";
                            var istovariEN = "";
                            var UtovarSR = "";
                            var UtovarEN = "";
                            var UISR = "";
                            var UIEN = "";
                            var DATDOSTAVESR = "";
                            var DATDOSTAVEEN = "";
                            foreach (var dataitem in lst.ToList())
                            {
                                if (i == 0)
                                {

                                    if (dataitem.Status == "ISTOVARENO")
                                    {
                                        StatusSr = "DOSTAVLJENO";
                                        StatusEn = "DELIVERED";
                                    }
                                    else if (dataitem.Status == "UTOVARENO U TRANZITU")
                                    {
                                        StatusSr = "UTOVARENO - U TRANZITU";
                                        StatusEn = "PICKED UP - IN TRANSIT";
                                    }
                                    else
                                    {
                                        StatusSr = "Zakazan utovar " + dataitem.DatUtovara;
                                        StatusEn = "Scheduled pick up " + dataitem.DatUtovara;
                                    }

                                    Naslov = "Notification about shipment no. " + dataitem.SerijskiBroj + " / Izvještaj o statusu pošiljke br. " + dataitem.SerijskiBroj;
                                    SubjectSr = "Poštovani," + "<br/><br/>" + "Obavještenje o statusu pošiljke: " + StatusSr + "<br/>" + "Naručioc prevoza: " + dataitem.Naziv.Replace("&", "i") + "<br/>" + "Broj Naloga za utovar:" + dataitem.BrojNaloga; // + System.Environment.NewLine + "Detalji Utovara: " + dataitem.UtovarFirma.Replace("&", "i") + " / " + dataitem.UtovarGrad + System.Environment.NewLine + "Uvoznik: " + (dataitem.Uvoznik + "").Replace("&", "i") + System.Environment.NewLine + "Izvoznik: " + (dataitem.Izvoznik + "").Replace("&", "i");
                                    SubjectEN = "<br/><br/><br/><br/>Greetings," + "<br/><br/>" + "Notification of shipment status: " + StatusEn + "<br/>" + "Transport order recived from: " + dataitem.Naziv.Replace("&", "and") + "<br/>" + "Transport order no:" + dataitem.BrojNaloga; // + System.Environment.NewLine + "Loading details: " + dataitem.UtovarFirma.Replace("&", "i") + " / " + dataitem.UtovarGrad + System.Environment.NewLine + "Importer: " + (dataitem.Uvoznik + "").Replace("&", "i") + System.Environment.NewLine + "Exporter: " + (dataitem.Izvoznik + "").Replace("&", "i");
                                    gostiKod = dataitem.Guest;

                                    UISR = "<br/>" + "Uvoznik: " + (dataitem.Uvoznik + "").Replace("&", "i") + "<br/>" + "Izvoznik: " + (dataitem.Izvoznik + "").Replace("&", "i");
                                    UIEN = "<br/>" + "Importer: " + (dataitem.Uvoznik + "").Replace("&", "i") + "<br/>" + "Exporter: " + (dataitem.Izvoznik + "").Replace("&", "i");
                                }

                                if (!dataitem.IstovarFirma.Equals(""))
                                {
                                    istovariSR += "<br/>" + "Detalji Istovara: " + dataitem.IstovarFirma.Replace("&", "i") + " / " + dataitem.IstovarGrad;
                                    istovariEN += "<br/>" + "Delivery details: " + dataitem.IstovarFirma.Replace("&", "i") + " / " + dataitem.IstovarGrad;
                                }

                                if (!dataitem.UtovarFirma.Equals(""))
                                {
                                    UtovarSR += "<br/>" + "Detalji Utovara: " + dataitem.UtovarFirma.Replace("&", "i") + " / " + dataitem.UtovarGrad;
                                    UtovarEN += "<br/>" + "Loading details: " + dataitem.UtovarFirma.Replace("&", "i") + " / " + dataitem.UtovarGrad;
                                }

                                if (StatusSr.Equals("UTOVARENO - U TRANZITU"))
                                {
                                    DATDOSTAVESR += "<br/>" + "Planirana Dostava: " + dataitem.DatIstovara;
                                    DATDOSTAVEEN += "<br/>" + "Projected delivery: " + dataitem.DatIstovara;
                                }

                            }

                            _mail.Subject = Naslov;
                            text = "<div>";
                            text += "<img src='http://gmtel-office.com/Content/images/Logo.png'>";
                            text += "</div>";
                            text += "<div>";

                            text += SubjectSr + UtovarSR + istovariSR + DATDOSTAVESR + UISR;

                            text += "</div>";
                            text += "<div>";
                            text += SubjectEN + UtovarEN + istovariEN + DATDOSTAVEEN + UIEN;
                            text += "</div>";
                            text += "</div>";
                            text += "<br/><br/><br/><br/>Status pošiljke možete pratiti na linku / Shipment status can be folowed on this link:" + "<br/>" + "www.gmtel-office.com/DnevnikPrevoza/GuestDetail?s=" + gostiKod + "<br/><br/>";
                            text += "</div>";
                            _mail.Body = text;


                     //   }
                     //   else
                     //   {
                     //       _mail.To.Add("info@gmtellogistics.com");
                     //       _mail.Bcc.Add("m.todorovic87@gmail.com");

                          
                    //    }

                        SmtpServer.Send(_mail);
                     }

                    return Json(new { response = "OK", message = "OK" }, JsonRequestBehavior.AllowGet);
                }

                else return Json(new { response = "ERROR", message = "Pogrešan Token. Logujte se ponovo" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) {
                return Json(new { response = "ERROR", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }



        }

        public JsonResult AndroidTaskSeen(string token, int VozacVidio, int AdminVidio)
        {
            Korisnik korisnik = db.Korisnik.Where(c => c.Token.Equals(token)).SingleOrDefault();
            VozacUser user = db.VozacUser.Where(c => c.IdUser == korisnik.IdKorisnik).SingleOrDefault();


            if(VozacVidio == 1)
            {

            var taskovi = db.AndroidTask                // your starting point - table in the "from" statement
                .Join(db.DnevnikPrevoza,                // the source table of the inner join
                        task => task.IdDnevnik,         // Select the primary key (the first part of the "on" clause in an sql "join" statement)
                        dnevnik => dnevnik.IdDnevnik,   // Select the foreign key (the second part of the "on" clause)
                        (task, dnevnik) => new { Task = task, Dnevnik = dnevnik }) // selection
                        .Where(c => c.Dnevnik.IdVozac == user.IdVozac && !c.Task.VozacPregled).ToList();


                foreach (var task in taskovi)
                {
                    AndroidTask at = db.AndroidTask.Find(task.Task.IdTask);
                    at.DatumZapisa = DateTime.Now.AddHours(7);
                    at.VozacPregled = true;

                    db.Entry(at).State = EntityState.Modified;
                    db.SaveChanges();
                }

            }


            if (AdminVidio == 1)
            {
                var taskovi = db.AndroidTask.Where(c => !c.AdminPregled).ToList();


                foreach (var task in taskovi)
                {
                    AndroidTask at = db.AndroidTask.Find(task.IdTask);
                    at.DatumZapisa = DateTime.Now.AddHours(7);
                    at.AdminPregled = true;

                    db.Entry(at).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

                return Json(new { response = "OK", message = "OK" }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult NoviTaskovi(String token)
        {
            int Idkorisnik = 0;
            int IdVozac = 0;

            Korisnik k = new Korisnik();
            VozacUser v = new VozacUser();

            try
            {
                k = db.Korisnik.Where(c => c.Token.Equals(token)).SingleOrDefault();
                Idkorisnik = k == null ? 0 : k.IdKorisnik;

                v = db.VozacUser.Where(c => c.IdUser == Idkorisnik).SingleOrDefault();
                IdVozac = v == null ? 0 : v.IdVozac;
            }
            catch (Exception ex)
            {

            }



            if (IdVozac > 0)
            {
                List<Task> Tasks = db.DnevnikPrevoza.Include(d => d.AndroidTask).Where(c => (c.ZapisAktivan ?? false) && c.IdVozac == IdVozac && !c.AndroidTask.FirstOrDefault().VozacPregled).OrderByDescending(c => c.Status).ThenByDescending(c => c.DatumZapisa).Take(25).ToList().
                  Select(c =>
                      new Task
                      {
                          IdTask = c.IdDnevnik,
                          SerijskiBroj = c.SerijskiBroj,
                          Vozilo = (c.Vozilo == null) ? "" : c.Vozilo.TipVozila + " " + c.Vozilo.RegistarskiBroj,
                          Utovar = c.UtovarGrad + ", " + c.UtovarAdresa + ", " + c.UtovarFirma + ", " + (c.DatumUtovara.HasValue ? c.DatumUtovara.Value.ToShortDateString() : ""),
                          Istovar = c.IstovarGrad + ", " + c.IstovarAdresa + ", " + c.IstovarFirma + ", " + (c.DatumIstovara.HasValue ? c.DatumIstovara.Value.ToShortDateString() : ""),
                          Roba = c.VrstaRobe + " " + c.KolicinaRobe + " " + c.DimenzijeRobe,
                          Status = (c.DrugiPrevoznikStatus + "").Equals("ISTOVARENO") ? c.DrugiPrevoznikStatus : c.Status,
                          DatumAzuriranja = c.DatumZapisa.Value.ToShortDateString(),
                          UvoznaSpedicija = c.UvoznaSpedicija + " ",
                          IzvoznaSpedicija = c.IzvoznaSpedicija + " ",
                          Uvoznik = c.DnevnikUvoznikIzvoznik.Count == 0 ? "" : String.Join(", ", c.DnevnikUvoznikIzvoznik.Select(g => g.Uvoznik).ToList()),
                          Izvoznik = c.DnevnikUvoznikIzvoznik.Count == 0 ? "" : String.Join(", ", c.DnevnikUvoznikIzvoznik.Select(g => g.Izvoznik).ToList()),
                          Napomena = c.Napomena + " ",
                          RefBroj = c.ReferentniBrojUtovara + " ",
                          Pregledano = (c.DrugiPrevoznik ?? false) ? (c.Subjekt == null ) ? "" : c.Subjekt.Naziv : ""
                      }).OrderByDescending(g => g.DatumAzuriranja).ToList();

                return Json(Tasks, JsonRequestBehavior.AllowGet);
            }


            if (k != null)
            {
                if (k.Role.Contains("admin"))
                {
                    List<Task> Tasks = db.DnevnikPrevoza.Include(d => d.AndroidTask).Where(c => (c.ZapisAktivan ?? false) && c.IdVozac != null && !c.AndroidTask.FirstOrDefault().AdminPregled).OrderByDescending(c => c.Status).ThenByDescending(c => c.DatumZapisa).Take(25).ToList().
                 Select(c =>
                     new Task
                     {
                         IdTask = c.IdDnevnik,
                         SerijskiBroj = c.SerijskiBroj,
                         Vozilo = (c.Vozilo == null) ? "" : c.Vozilo.TipVozila + " " + c.Vozilo.RegistarskiBroj + " " + (c.Vozaci == null ? "" : c.Vozaci.ImeVozaca),
                         Utovar = c.UtovarGrad + ", " + c.UtovarAdresa + ", " + c.UtovarFirma + ", " + (c.DatumUtovara.HasValue ? c.DatumUtovara.Value.ToShortDateString() : ""),
                         Istovar = c.IstovarGrad + ", " + c.IstovarAdresa + ", " + c.IstovarFirma + ", " + (c.DatumIstovara.HasValue ? c.DatumIstovara.Value.ToShortDateString() : ""),
                         Roba = c.VrstaRobe + " " + c.KolicinaRobe + " " + c.DimenzijeRobe,
                         Status = (c.DrugiPrevoznikStatus + "").Equals("ISTOVARENO") ? c.DrugiPrevoznikStatus : c.Status,
                         DatumAzuriranja = c.DatumZapisa.Value.ToShortDateString(),
                         UvoznaSpedicija = c.UvoznaSpedicija + " ",
                         IzvoznaSpedicija = c.IzvoznaSpedicija + " ",
                         Uvoznik = c.DnevnikUvoznikIzvoznik.Count == 0 ? "" : String.Join(", ", c.DnevnikUvoznikIzvoznik.Select(g => g.Uvoznik).ToList()),
                         Izvoznik = c.DnevnikUvoznikIzvoznik.Count == 0 ? "" : String.Join(", ", c.DnevnikUvoznikIzvoznik.Select(g => g.Izvoznik).ToList()),
                         Napomena = c.Napomena + " ",
                         RefBroj = c.ReferentniBrojUtovara + " ",
                         Pregledano = (c.DrugiPrevoznik ?? false) ? (c.Subjekt == null) ? "" : c.Subjekt.Naziv : ""

                     }).OrderByDescending(g => g.DatumAzuriranja).ToList();

                    return Json(Tasks, JsonRequestBehavior.AllowGet);
                }
            }

            return  Json(new List<Task>(), JsonRequestBehavior.AllowGet);

        }

        public JsonResult FindByGuestCode(String code)
        {
            code = code.Replace("www.gmtel-office.com/DnevnikPrevoza/GuestDetail?s=", "");

            List<Task> Tasks = db.DnevnikPrevoza.Include(d => d.AndroidTask).Where(c => (c.ZapisAktivan ?? false) && c.GostPristup.Equals(code)).ToList().
            Select(c =>
              new Task
              {
                  IdTask = c.IdDnevnik,
                  SerijskiBroj = c.SerijskiBroj,
                  Vozilo = (c.Vozilo == null) ? "" : c.Vozilo.TipVozila + " " + c.Vozilo.RegistarskiBroj,
                  Utovar = c.UtovarGrad + ", " + c.UtovarAdresa + ", " + c.UtovarFirma + ", " + (c.DatumUtovara.HasValue ? c.DatumUtovara.Value.ToShortDateString() : ""),
                  Istovar = c.IstovarGrad + ", " + c.IstovarAdresa + ", " + c.IstovarFirma + ", " + (c.DatumIstovara.HasValue ? c.DatumIstovara.Value.ToShortDateString() : ""),
                  Roba = c.VrstaRobe + " " + c.KolicinaRobe + " " + c.DimenzijeRobe,
                  Status = (c.DrugiPrevoznikStatus + "").Equals("ISTOVARENO") ? c.DrugiPrevoznikStatus : c.Status,
                  DatumAzuriranja = c.DatumZapisa.Value.ToShortDateString(),
                  UvoznaSpedicija = c.UvoznaSpedicija + " ",
                  IzvoznaSpedicija = c.IzvoznaSpedicija + " ",
                  Uvoznik = c.DnevnikUvoznikIzvoznik.Count == 0 ? "" : String.Join(", ", c.DnevnikUvoznikIzvoznik.Select(g => g.Uvoznik).ToList()),
                  Izvoznik = c.DnevnikUvoznikIzvoznik.Count == 0 ? "" : String.Join(", ", c.DnevnikUvoznikIzvoznik.Select(g => g.Izvoznik).ToList()),
                  Napomena = c.Napomena + " ",
                  RefBroj = c.ReferentniBrojUtovara + " ",
                  Pregledano = (c.AndroidTask.Where(d => d.IdDnevnik == c.IdDnevnik).SingleOrDefault() != null ? (c.AndroidTask.Where(d => d.IdDnevnik == c.IdDnevnik).SingleOrDefault().VozacPregled ? "1" : "0") : "1")
            
              }).OrderByDescending(g => g.DatumAzuriranja).ToList();

            return Json(Tasks, JsonRequestBehavior.AllowGet);
        }

        public JsonResult NapraviVozacevTrosak(String token, int IdTrosak, Decimal iznos, String valuta, String tip, String vrstatroska, String napomena, String date, int ? Kartica)
        {

            try
            {
                if (tip.StartsWith("Z"))
                    tip = "ZADUŽENJE";

                DateTime oDate = DateTime.ParseExact(date, "d.M.yyyy HH:mm", null);

                Korisnik korisnik = db.Korisnik.Where(c => c.Token.Equals(token)).SingleOrDefault();
                int IdVozac = db.VozacUser.Where(k => k.IdUser == korisnik.IdKorisnik).SingleOrDefault().IdVozac;

                if (IdTrosak == 0)
                {
                    VozacTroskovi vozacTroskovi = new VozacTroskovi();
                    vozacTroskovi.IdVozac = IdVozac;
                    vozacTroskovi.IdValuta = db.Valuta.Where(c => c.OznakaValute.Equals(valuta)).SingleOrDefault().IdValuta;

                    if (String.IsNullOrEmpty(vrstatroska))
                        vozacTroskovi.IdVrstaTroska = null;
                    else
                        vozacTroskovi.IdVrstaTroska = db.VozacVrstaTroskova.Where(c => c.Naziv.Equals(vrstatroska)).FirstOrDefault().IdVrstaTroska;

                    vozacTroskovi.Iznos = iznos;
                    vozacTroskovi.Napomena = napomena;
                    vozacTroskovi.OdobrenoZaduzenje = false;
                    vozacTroskovi.PrihvacenRashod = false;
                    vozacTroskovi.RazmjenjenoMjenjacnica = false;
                    vozacTroskovi.Tip = tip;
                    vozacTroskovi.Aktivno = true;
                    vozacTroskovi.Datum = oDate;
                    vozacTroskovi.Kartica = Kartica == 1;

                    db.VozacTroskovi.Add(vozacTroskovi);
                    db.SaveChanges();
                }
                else
                {
                    VozacTroskovi vozacTroskovi = db.VozacTroskovi.Find(IdTrosak);
                    vozacTroskovi.IdVozac = IdVozac;
                    vozacTroskovi.IdValuta = db.Valuta.Where(c => c.OznakaValute.Equals(valuta)).SingleOrDefault().IdValuta;

                    if (String.IsNullOrEmpty(vrstatroska))
                        vozacTroskovi.IdVrstaTroska = null;
                    else
                        vozacTroskovi.IdVrstaTroska = db.VozacVrstaTroskova.Where(c => c.Naziv.Equals(vrstatroska)).FirstOrDefault().IdVrstaTroska;

                    vozacTroskovi.Iznos = iznos;
                    vozacTroskovi.Napomena += " Podatak je imao ažuriranje od strane " + korisnik.Ime;
                    vozacTroskovi.OdobrenoZaduzenje = false;
                    vozacTroskovi.PrihvacenRashod = false;
                    vozacTroskovi.RazmjenjenoMjenjacnica = false;
                    vozacTroskovi.Tip = tip;
                    vozacTroskovi.Aktivno = true;
                    vozacTroskovi.Datum = oDate;
                    vozacTroskovi.Kartica = Kartica == 1;

                    db.Entry(vozacTroskovi).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return Json(new { response = "ERROR", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { response = "OK", message = "OK" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListaVozacevihTroskova(String token)
        {

            Korisnik korisnik = db.Korisnik.Where(c => c.Token.Equals(token)).SingleOrDefault();

            int IdVozac = db.VozacUser.Where(k => k.IdUser == korisnik.IdKorisnik).SingleOrDefault().IdVozac;

            var list = db.VozacTroskovi.Where(c => c.Aktivno && c.IdVozac == IdVozac && (!(c.Zakljucano ?? false))).OrderByDescending(c => c.Datum).Take(100).ToList().Select(
                k => new
                {
                    Id = k.IdCozacTroskovi,
                    Iznos = k.Iznos.ToString("0.00") + " " + k.Valuta.OznakaValute,
                    Datum = k.Datum.ToShortDateString() + " " + k.Datum.ToShortTimeString(),
                    Tip = k.Tip,
                    Vrsta = k.IdVrstaTroska == null ? "" : k.VozacVrstaTroskova.Naziv,
                    Kartica = (k.Kartica ?? false) ? 1 : 0
                }
                ).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult IzbrisiTrosakVozaca(String token, int id)
        {
            VozacTroskovi vt = db.VozacTroskovi.Find(id);
            vt.Aktivno = false;

            db.Entry(vt).State = EntityState.Modified;
            db.SaveChanges();

            return Json(new { response = "OK", message = "OK" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RazmjeniNovac(int id, decimal iznos1, string valuta1, decimal iznos2, string valuta2, String date)
        {

            try
            {

                DateTime oDate = DateTime.ParseExact(date, "d.M.yyyy HH:mm", null);

                VozacTroskovi vt = db.VozacTroskovi.Find(id);
                vt.Aktivno = false;
                vt.RazmjenjenoMjenjacnica = true;
                db.Entry(vt).State = EntityState.Modified;
                db.SaveChanges();


                VozacTroskovi vozacTroskovi = new VozacTroskovi();
                vozacTroskovi.IdVozac = vt.IdVozac;
                vozacTroskovi.IdValuta = db.Valuta.Where(c => c.OznakaValute.Equals(valuta1)).SingleOrDefault().IdValuta;
                vozacTroskovi.IdVrstaTroska = vt.IdVrstaTroska;
                vozacTroskovi.Iznos = iznos1;
                vozacTroskovi.Napomena = "Zaduženje posle razmjene novca";
                vozacTroskovi.OdobrenoZaduzenje = vt.OdobrenoZaduzenje;
                vozacTroskovi.PrihvacenRashod = vt.PrihvacenRashod;
                vozacTroskovi.RazmjenjenoMjenjacnica = false;
                vozacTroskovi.Tip = vt.Tip;
                vozacTroskovi.Aktivno = true;
                vozacTroskovi.Datum = oDate;


                if (iznos1 > 0)
                {
                    db.VozacTroskovi.Add(vozacTroskovi);
                    db.SaveChanges();
                }


                if (iznos2 > 0)
                {
                    VozacTroskovi vozacTroskovi1 = new VozacTroskovi();
                    vozacTroskovi1.IdVozac = vt.IdVozac;
                    vozacTroskovi1.IdValuta = db.Valuta.Where(c => c.OznakaValute.Equals(valuta2)).SingleOrDefault().IdValuta;
                    vozacTroskovi1.IdVrstaTroska = vt.IdVrstaTroska;
                    vozacTroskovi1.Iznos = iznos2;
                    vozacTroskovi1.Napomena = "Zaduženje posle razmjene novca";
                    vozacTroskovi1.OdobrenoZaduzenje = vt.OdobrenoZaduzenje;
                    vozacTroskovi1.PrihvacenRashod = vt.PrihvacenRashod;
                    vozacTroskovi1.RazmjenjenoMjenjacnica = false;
                    vozacTroskovi1.Tip = vt.Tip;
                    vozacTroskovi1.Aktivno = true;
                    vozacTroskovi1.Datum = oDate;

                    db.VozacTroskovi.Add(vozacTroskovi1);
                    db.SaveChanges();
                }

                return Json(new { response = "OK", message = "OK" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { response = "NOK", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ListaVrsteTroskova()
        {
            return Json( db.VozacVrstaTroskova.Select(c => new { IdVrstaTroska = c.IdVrstaTroska, Naziv = c.Naziv }).ToList() , JsonRequestBehavior.AllowGet);
        }

        public JsonResult SearchDnevnik(String token)
        {

            int Idkorisnik = 0;
            int IdVozac = 0;

            Korisnik k = new Korisnik();
            VozacUser v = new VozacUser();

            try
            {
                k = db.Korisnik.Where(c => c.Token.Equals(token)).SingleOrDefault();
                Idkorisnik = k == null ? 0 : k.IdKorisnik;

                v = db.VozacUser.Where(c => c.IdUser == Idkorisnik).SingleOrDefault();
                IdVozac = v == null ? 0 : v.IdVozac;
            }
            catch (Exception ex)
            {

            }



            if (IdVozac > 0)
            {


                List<Task> Tasks = db.DnevnikPrevoza.Where(c => (c.ZapisAktivan ?? false) && c.IdVozac != IdVozac && c.IdVozac !=  null)
                  //  .Where(c => c.SerijskiBroj.Contains(query) || c.Vozaci.ImeVozaca.Contains(query))
                    .OrderByDescending(c => c.Status).ThenByDescending(c => c.DatumZapisa).Take(100).ToList().
                  Select(c =>
                      new Task
                      {
                          IdTask = c.IdDnevnik,
                          SerijskiBroj = c.SerijskiBroj + "#" + c.Vozaci.ImeVozaca,
                          Vozilo = (c.Vozilo == null) ? "" : c.Vozilo.TipVozila + " " + c.Vozilo.RegistarskiBroj,
                          Utovar = c.UtovarPTT + " " + c.UtovarGrad + ", " + c.UtovarAdresa + ", " + c.UtovarFirma + ", " + (c.DatumUtovara.HasValue ? c.DatumUtovara.Value.ToShortDateString() : ""),
                          Istovar = c.IstovarPTT + " " + c.IstovarGrad + ", " + c.IstovarAdresa + ", " + c.IstovarFirma + ", " + (c.DatumIstovara.HasValue ? c.DatumIstovara.Value.ToShortDateString() : ""),
                          Roba = c.VrstaRobe + " " + c.KolicinaRobe + " " + c.DimenzijeRobe,
                          Status = (c.DrugiPrevoznikStatus + "").Equals("ISTOVARENO") ? c.DrugiPrevoznikStatus : c.Status,
                          DatumAzuriranja = c.DatumZapisa.Value.ToShortDateString(),
                          UvoznaSpedicija = c.UvoznaSpedicija + " ",
                          IzvoznaSpedicija = c.IzvoznaSpedicija + " ",
                          Uvoznik = c.DnevnikUvoznikIzvoznik.Count == 0 ? "" : String.Join(", ", c.DnevnikUvoznikIzvoznik.Select(g => g.Uvoznik).ToList()),
                          Izvoznik = c.DnevnikUvoznikIzvoznik.Count == 0 ? "" : String.Join(", ", c.DnevnikUvoznikIzvoznik.Select(g => g.Izvoznik).ToList()),
                          Napomena = c.Napomena + " ",
                          RefBroj = c.ReferentniBrojUtovara + " ",
                          Pregledano = (c.DrugiPrevoznik ?? false) ? (c.Subjekt == null) ? "" : c.Subjekt.Naziv : ""
                      }).OrderByDescending(g => g.DatumAzuriranja).ToList();

                return Json(Tasks, JsonRequestBehavior.AllowGet);
            }


            if (k != null)
            {
                if (k.Role.Contains("admin"))
                {
                    List<Task> Tasks = db.DnevnikPrevoza.Where(c => (c.ZapisAktivan ?? false))
                     //   .Where(c => c.SerijskiBroj.Contains(query) || c.Vozaci.ImeVozaca.Contains(query))
                        .OrderByDescending(c => c.Status).ThenByDescending(c => c.DatumZapisa).Take(100).ToList().
                 Select(c =>
                     new Task
                     {
                         IdTask = c.IdDnevnik,
                         SerijskiBroj = c.SerijskiBroj + "#" + c.Vozaci.ImeVozaca,
                         Vozilo = (c.Vozilo == null) ? "" : c.Vozilo.TipVozila + " " + c.Vozilo.RegistarskiBroj + " " + (c.Vozaci == null ? "" : c.Vozaci.ImeVozaca),
                         Utovar = c.UtovarPTT + " " + c.UtovarGrad + ", " + c.UtovarAdresa + ", " + c.UtovarFirma + ", " + (c.DatumUtovara.HasValue ? c.DatumUtovara.Value.ToShortDateString() : ""),
                         Istovar = c.IstovarPTT + " " + c.IstovarGrad + ", " + c.IstovarAdresa + ", " + c.IstovarFirma + ", " + (c.DatumIstovara.HasValue ? c.DatumIstovara.Value.ToShortDateString() : ""),
                         Roba = c.VrstaRobe + " " + c.KolicinaRobe + " " + c.DimenzijeRobe,
                         Status = (c.DrugiPrevoznikStatus + "").Equals("ISTOVARENO") ? c.DrugiPrevoznikStatus : c.Status,
                         DatumAzuriranja = c.DatumZapisa.Value.ToShortDateString(),
                         UvoznaSpedicija = c.UvoznaSpedicija + " ",
                         IzvoznaSpedicija = c.IzvoznaSpedicija + " ",
                         Uvoznik = c.DnevnikUvoznikIzvoznik.Count == 0 ? "" : String.Join(", ", c.DnevnikUvoznikIzvoznik.Select(g => g.Uvoznik).ToList()),
                         Izvoznik = c.DnevnikUvoznikIzvoznik.Count == 0 ? "" : String.Join(", ", c.DnevnikUvoznikIzvoznik.Select(g => g.Izvoznik).ToList()),
                         Napomena = c.Napomena + " ",
                         RefBroj = c.ReferentniBrojUtovara + " ",
                         Pregledano = (c.DrugiPrevoznik ?? false) ? c.Subjekt.Naziv : ""
                     }).OrderByDescending(g => g.DatumAzuriranja).ToList();

                    return Json(Tasks, JsonRequestBehavior.AllowGet);
                }
            }



            return  Json(new List<Task>(), JsonRequestBehavior.AllowGet);

        }





        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}