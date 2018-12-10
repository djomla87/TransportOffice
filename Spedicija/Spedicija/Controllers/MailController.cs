using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Spedicija.Controllers
{
    public class MailController : Controller
    {
        //
        // GET: /Mail/

        uvhszjiy_spedicijaEntities db = new uvhszjiy_spedicijaEntities();
        public ActionResult Index()
        {

            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("ml01.anaxanet.com");
            // GMtel2017.
            mail.From = new MailAddress("info@gmtel-office.com","GMTEL OFFICE");
           // mail.To.Add("m.todorovic87@gmail.com");

            mail.To.Add("info@gmtellogistics.com");
            mail.Bcc.Add("m.todorovic87@gmail.com");


            DateTime sutra = DateTime.Today.AddDays(1);

            var obaveze = db.DnevnikPrevoza.Where(c => (c.ZapisAktivan ?? false) && ((c.DatumUtovara.HasValue && c.DatumUtovara == DateTime.Today) || (c.DatumIstovara.HasValue && c.DatumIstovara == DateTime.Today) )).ToList().Select
            (c => new
            {
                Link = "http://gmtel-office.com/DnevnikPrevoza/Details/" + c.IdDnevnik ,
                Datum = c.DatumUtovara == DateTime.Today ? c.DatumUtovara.Value.ToShortDateString() : c.DatumIstovara.Value.ToShortDateString(),
                Tip = c.DatumUtovara == DateTime.Today ? "Utovar: " + c.UtovarFirma + ", " + c.UtovarGrad : "Istovar: " + c.IstovarFirma + ", " + c.IstovarGrad
            }).ToList();


            var obavezeSutra =
                db.DnevnikPrevoza.Where(c => (c.ZapisAktivan ?? false) && ((c.DatumUtovara.HasValue && c.DatumUtovara == sutra) || (c.DatumIstovara.HasValue && c.DatumIstovara == sutra))).ToList().Select
            (c => new
            {
                Link = "http://gmtel-office.com/DnevnikPrevoza/Details/" + c.IdDnevnik,
                Datum = c.DatumUtovara == sutra ? c.DatumUtovara.Value.ToShortDateString() : c.DatumIstovara.Value.ToShortDateString(),
                Tip = c.DatumUtovara == sutra ? "Utovar: " + c.UtovarFirma + ", " + c.UtovarGrad : "Istovar: " + c.IstovarFirma + ", " + c.IstovarGrad
            }).ToList();

                


            mail.IsBodyHtml = true;

            String text = "<div>";
            text += "<img src='http://gmtel-office.com/Content/images/Logo.png'>";
            text += "<h2>Pregled današnjih i sutrašnjih obaveza</h2>";
            text += "<h4>Dobro jutro Miha, <br>Ispod se nalazi pregled obaveza za danas</h4>";
            text += "</div>";
            text += "<div>";
            text += "<p></p>";
            text += "<p>";
            
            foreach (var s in obaveze)
            {
                text += "<ul>";
                text += "<li>Datum: " + s.Datum + "</li>";
                text += "<li>" + s.Tip + "</li>";
                text += "<li><a href='" + s.Link + "'>Otvori u aplikaciji</a></li>"; 
                text += "</ul>";
            }
           
            text += "</p>";
            text += "</div>";


            text += "<h4><br>Ispod se nalazi pregled obaveza za sutra</h4>";
            text += "<div>";
            text += "<p></p>";
            text += "<p>";
            foreach (var s in obavezeSutra)
            {
                text += "<ul>";
                text += "<li>Datum: " + s.Datum + "</li>";
                text += "<li>" + s.Tip + "</li>";
                text += "<li><a href='" + s.Link + "'>Otvori u aplikaciji</a></li>";
                text += "</ul>";
            }
            text += "</p>";
            text += "</div>";


            text += "<div>";
            text += "<i>Ova poruka vam je poslat automatski sa gmtel-office.com. Nemojte odgovarati na ovaj mail!</i>";
            text += "</div>";

            mail.Subject = "Pregled dnevnih obaveza - " + DateTime.Today.AddHours(7).ToShortDateString();
            mail.Body = text;

            if (obaveze.Count != 0)
            SmtpServer.Send(mail);

            return View();
        }



        public ActionResult DownloadEmail(int PathToAttachment)
        {
            var message = new MailMessage();

            message.From = new MailAddress("info@gmtel-office.com");
            message.To.Add("info@gmtellogistics.com");
            message.Subject = "Invoice and documents";
            message.Body = "Molimo Vas pogledajte prilog" + System.Environment.NewLine + "Please see the attachment";

            var putanja = db.Dokument.Find(PathToAttachment).Naziv;


            var path = Server.MapPath("~/Dokumenti/" + putanja );
            message.Attachments.Add(new Attachment(path));

            using (var client = new SmtpClient())
            {
                var id = Guid.NewGuid();

                var tempFolder = Path.Combine(Path.GetTempPath(), Assembly.GetExecutingAssembly().GetName().Name);

                tempFolder = Path.Combine(tempFolder, "MailMessageToEMLTemp");

                // create a temp folder to hold just this .eml file so that we can find it easily.
                tempFolder = Path.Combine(tempFolder, id.ToString());

                if (!Directory.Exists(tempFolder))
                {
                    Directory.CreateDirectory(tempFolder);
                }

                client.UseDefaultCredentials = true;
                client.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                client.PickupDirectoryLocation = tempFolder;
                client.Send(message);

                // tempFolder should contain 1 eml file

                var filePath = Directory.GetFiles(tempFolder).Single();

                // stream out the contents - don't need to dispose because File() does it for you
                var fs = new FileStream(filePath, FileMode.Open);
                return File(fs, "application/vnd.ms-outlook", "email.eml");
            }
        }



        //
        // GET: /Mail/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Mail/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Mail/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
               

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Mail/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Mail/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Mail/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Mail/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
