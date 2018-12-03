using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Spedicija.Models.Mobile;
using System.Runtime.InteropServices;

namespace Spedicija.Controllers
{
    public class TasksController : ApiController
    {
        private uvhszjiy_spedicijaEntities db = new uvhszjiy_spedicijaEntities();

        // GET api/Tasks

        public IEnumerable<Task> GetDnevnikPrevozas(String token)
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
            catch (Exception ex) {

            }

         
           
            if (IdVozac > 0)
            {


                List<Task> Tasks = db.DnevnikPrevoza.Where(c => (c.ZapisAktivan ?? false) && c.IdVozac == IdVozac).OrderByDescending(c => c.Status).ThenByDescending(c => c.DatumZapisa).Take(30).ToList().
                  Select(c =>
                      new Task
                      {
                          IdTask = c.IdDnevnik,
                          SerijskiBroj = c.SerijskiBroj,
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

                return Tasks;
            }


            if (k != null)
            {
                if (k.Role.Contains("admin"))
                {
                    List<Task> Tasks = db.DnevnikPrevoza.Where(c => (c.ZapisAktivan ?? false)).OrderByDescending(c => c.Status).ThenByDescending(c => c.DatumZapisa).Take(50).ToList().
                 Select(c =>
                     new Task
                     {
                         IdTask = c.IdDnevnik,
                         SerijskiBroj = c.SerijskiBroj,
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

                    return Tasks;
                }
            }



            return new List<Task>();

        }



        /*
        // GET api/Tasks/5
        public DnevnikPrevoza GetDnevnikPrevoza(int id)
        {
            DnevnikPrevoza dnevnikprevoza = db.DnevnikPrevoza.Find(id);
            if (dnevnikprevoza == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return dnevnikprevoza;
        }

        // PUT api/Tasks/5
        public HttpResponseMessage PutDnevnikPrevoza(int id, DnevnikPrevoza dnevnikprevoza)
        {
            if (ModelState.IsValid && id == dnevnikprevoza.IdDnevnik)
            {
                db.Entry(dnevnikprevoza).State = EntityState.Modified;

                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // POST api/Tasks
        public HttpResponseMessage PostDnevnikPrevoza(DnevnikPrevoza dnevnikprevoza)
        {
            if (ModelState.IsValid)
            {
                db.DnevnikPrevoza.Add(dnevnikprevoza);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, dnevnikprevoza);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = dnevnikprevoza.IdDnevnik }));
                return response;
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // DELETE api/Tasks/5
        public HttpResponseMessage DeleteDnevnikPrevoza(int id)
        {
            DnevnikPrevoza dnevnikprevoza = db.DnevnikPrevoza.Find(id);
            if (dnevnikprevoza == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.DnevnikPrevoza.Remove(dnevnikprevoza);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.OK, dnevnikprevoza);
        }
        */
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}