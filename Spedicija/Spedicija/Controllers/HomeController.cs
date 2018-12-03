using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Spedicija.Models;
using System.Data.Entity;

namespace Spedicija.Controllers
{
    
   
    public class HomeController : Controller
    {
        private uvhszjiy_spedicijaEntities db = new uvhszjiy_spedicijaEntities();

        [Authorize]
        public ActionResult Index(String search)
        {


            SearchViewModel sv = new SearchViewModel();

            sv.Subjekt = db.Subjekt.Where(k => k.ZapisAktivan && (k.Naziv.Contains(search) || k.KontaktOsoba.Contains(search) || k.Timocom.Contains(search) )).ToList();
            sv.Dnevnik = db.DnevnikPrevoza.Include(c => c.Subjekt1).Include(c => c.Subjekt).Where(k => (k.ZapisAktivan ?? false) && (k.BrojNaloga.Contains(search) || k.SerijskiBroj.Contains(search) || k.Subjekt.Naziv.Contains(search) || 
                k.Subjekt1.Naziv.Contains(search) || k.UtovarFirma.Contains(search) || k.UtovarGrad.Contains(search) || k.IstovarFirma.Contains(search) ||
                k.IstovarGrad.Contains(search))).ToList();

            sv.Vozaci = db.Vozaci.Include(c => c.Subjekt).Where(k => (k.ZapisAktivan ?? false) && (k.Subjekt.Naziv.Contains(search) || k.ImeVozaca.Contains(search) || k.JMBG.Contains(search))).ToList();

            sv.Vozila = db.Vozilo.Where(k => (k.ZapisAktivan) && (k.TipVozila.Contains(search) || k.RegistarskiBroj.Contains(search))).ToList();

            sv.Dokument = db.Dokument.Include(c => c.DnevnikPrevoza).Where(c => (c.DnevnikPrevoza.ZapisAktivan ?? false) && c.Naziv.Contains(search)).ToList();

            sv.Nalozi = db.NalogZaUtovar.Where(c => c.Subjekt.Naziv.Contains(search) || c.BrojNaloga.Contains(search) || c.DnevnikPrevoza.BrojNaloga.Contains(search)).ToList();

            return View(sv);
        }

        [Authorize]
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        [Authorize]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        public ActionResult AuthorizationFailed()
        {
            return View();
        }

    }
}
