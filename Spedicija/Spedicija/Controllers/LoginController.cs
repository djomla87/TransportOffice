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

namespace Spedicija.Controllers
{
    public class LoginController : ApiController
    {
        private uvhszjiy_spedicijaEntities db = new uvhszjiy_spedicijaEntities();

        static string getMd5Hash(string input)
        {
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



        // GET api/Login
        /*
        public IEnumerable<Korisnik> GetKorisniks()
        {
          
            return db.Korisnik.AsEnumerable();
        }
        */


        // GET api/Login/5
        public Token GetKorisnik(String Username, String Password)
        {
            String passwordHash = getMd5Hash(Password);

            List<Korisnik> korisnik = db.Korisnik.Where(i => 
                i.KorisnickoIme.ToLower().Equals(Username.ToLower()) &&
                i.Lozinka.Equals(passwordHash) && 
                (i.ZapisAktivan ?? false)).ToList();

            if (korisnik.Count > 0)
            { 
                Random random = new Random();
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                String code = new string(Enumerable.Repeat(chars, 50)
                .Select(s => s[random.Next(s.Length)]).ToArray());

                korisnik[0].Token = code;

                 db.Entry(korisnik[0]).State = EntityState.Modified;
                 db.SaveChanges();

                VozacUser v = new VozacUser();
                int idkorisnka = korisnik[0].IdKorisnik;
                v = db.VozacUser.Where(c => c.IdUser == idkorisnka).SingleOrDefault();
                int IdVozac = (v == null ? 0 : v.IdVozac);


                return new Token { 
                    Code = code, 
                    VrijemeKreiranjaTokena = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString(), 
                    Korisnik = korisnik[0].Ime, 
                    Aktivan = "OK", 
                    KorisnickoIme = korisnik[0].KorisnickoIme,
                    Rola = korisnik[0].Role,
                    IdVozac = IdVozac
                };
            }

            return new Token();
        }


        /*
        // PUT api/Login/5
        public HttpResponseMessage PutKorisnik(int id, Korisnik korisnik)
        {
            if (ModelState.IsValid && id == korisnik.IdKorisnik)
            {
                db.Entry(korisnik).State = EntityState.Modified;

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

        // POST api/Login
        public HttpResponseMessage PostKorisnik(Korisnik korisnik)
        {
            if (ModelState.IsValid)
            {
                db.Korisnik.Add(korisnik);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, korisnik);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = korisnik.IdKorisnik }));
                return response;
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // DELETE api/Login/5
        public HttpResponseMessage DeleteKorisnik(int id)
        {
            Korisnik korisnik = db.Korisnik.Find(id);
            if (korisnik == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.Korisnik.Remove(korisnik);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.OK, korisnik);
        }
        */
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}