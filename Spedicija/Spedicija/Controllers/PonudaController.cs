using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Spedicija.Models;
using Microsoft.Reporting.WebForms;
using Zen.Barcode;
using System.IO;

namespace Spedicija.Controllers
{
    public class PonudaController : Controller
    {
        private uvhszjiy_spedicijaEntities db = new uvhszjiy_spedicijaEntities();

        //
        // GET: /Ponuda/
        [Authorize]
        public ActionResult Index()
        {
            // return View(db.Ponuda.ToList());
			 return View(new List<Ponuda>());
        }


        public static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // 
        public ActionResult IndexAjax(jQueryDataTableParamModel param)
	 {
            var list = db.Ponuda.Include(d => d.Subjekt).Include(c => c.Valuta).ToList();
            IEnumerable<Ponuda> filteredList = list;


            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredList = filteredList.Where(c =>
                                              (
                                                (String.IsNullOrEmpty("" + c.SerijskiBroj) ? "" : "" + c.SerijskiBroj).ToLower().Contains(param.sSearch.ToLower())
                                             || (String.IsNullOrEmpty("" + c.DatumDnevnika) ?   "" : "" + c.DatumDnevnika.Value.ToShortDateString()).ToLower().Contains(param.sSearch.ToLower())
                                             || (String.IsNullOrEmpty("" + c.Subjekt.Naziv) ?   "" : "" + c.Subjekt.Naziv).ToLower().Contains(param.sSearch.ToLower())
                                             || (String.IsNullOrEmpty("" + c.UtovarGrad) ?      "" : "" + c.UtovarGrad + c.UtovarPTT + c.UtovarDrzava).ToLower().Contains(param.sSearch.ToLower())
                                             || (String.IsNullOrEmpty("" + c.IstovarGrad) ?     "" : "" + c.IstovarGrad + c.IstovarPTT + c.IstovarDrzava).ToLower().Contains(param.sSearch.ToLower())
                                             || (String.IsNullOrEmpty("" + c.CijenaPrevoza) ?   "" : "" + c.CijenaPrevoza + c.Valuta.OznakaValute).ToLower().Contains(param.sSearch.ToLower())
                                                ));

            }

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);

            Func<Ponuda, object> orderingFunction = c =>
            {
                switch (sortColumnIndex)
                {
                                    case 0: return c.SerijskiBroj.Length == 10 ? c.SerijskiBroj.Replace("-", "-0") : c.SerijskiBroj;
				 					case 1: return c.DatumDnevnika.Value.ToShortDateString();
									case 2: return c.Subjekt.Naziv;
									case 3: return c.UtovarPTT + " " + c.UtovarGrad + " / " + c.UtovarDrzava ;
									case 4: return c.IstovarPTT + " " + c.IstovarGrad + " / " + c.IstovarDrzava;
									case 5: return c.CijenaPrevoza + " " + c.Valuta.OznakaValute;
								   default: return c.IdDnevnik;
                }
			};

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredList = filteredList.OrderBy(orderingFunction);
            else
                filteredList = filteredList.OrderByDescending(orderingFunction);

            var displayedList = filteredList.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedList select new object[] {
                c.SerijskiBroj,
                c.DatumDnevnika.Value.ToShortDateString(),
                c.Subjekt.Naziv,
                c.UtovarPTT + " " + c.UtovarGrad  + " / " + c.UtovarDrzava,
                c.IstovarPTT + " " + c.IstovarGrad  + " / " + c.IstovarDrzava,
                c.CijenaPrevoza + " " + c.Valuta.OznakaValute,
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
        // GET: /Ponuda/Details/5
        [Authorize]
        public ActionResult Details(int id = 0)
        {
            Ponuda ponuda = db.Ponuda.Find(id);
            if (ponuda == null)
            {
                return HttpNotFound();
            }
            return View(ponuda);
        }

        //
        // GET: /Ponuda/Create
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.IdNarucioca = new SelectList(db.Subjekt, "IdSubjekt", "Naziv");
            ViewBag.IdValuta = new SelectList(db.Valuta, "IdValuta", "OznakaValute");
            ViewBag.IdSubjekt = new SelectList(db.Subjekt, "IdSubjekt", "Naziv");
            ViewBag.IdValutaPrevoznika = new SelectList(db.Valuta, "IdValuta", "OznakaValute");

            ViewBag.UsloviPrevoza = db.UsloviPrevoza.ToList();


            return View();
        }

        //
        // POST: /Ponuda/Create

        [HttpPost]
        public ActionResult Create(Ponuda ponuda, int [] _UslovPrevoza)
        {
            if (ModelState.IsValid)
            {

                if (Request["_SaPDV"] != null)
                {
                    if (Request["_SaPDV"].ToString().Equals("on"))
                        ponuda.SaPDV = true;
                    else ponuda.SaPDV = false;
                }
                else ponuda.SaPDV = false;

                if (Request["_DrugiPrevoznik"] != null)
                {
                    if (Request["_DrugiPrevoznik"].ToString().Equals("on"))
                        ponuda.DrugiPrevoznik = true;
                    else ponuda.DrugiPrevoznik = false;
                }
                else ponuda.DrugiPrevoznik = false;


                ponuda.ZapisAktivan = true;
                ponuda.DatumZapisa = DateTime.Now.AddHours(7);

                String SB = ponuda.DatumDnevnika.Value.ToString("yyyyMMdd");
                int BR = db.Ponuda.Where(c => c.SerijskiBroj.Contains(SB)).Count() + 1;

                ponuda.SerijskiBroj = SB + "-" + BR;
                ponuda.GostPristup = RandomString(25);
                db.Ponuda.Add(ponuda);
                db.SaveChanges();

                using (System.Drawing.Image imagen = BarcodeDrawFactory.GetSymbology(BarcodeSymbology.CodeQr).Draw(AppSettings.GetSettings()["domain_name"]+"/Ponuda/ReportPonuda2/" +ponuda.IdDnevnik, 50, 2))
                {
                    String path = Server.MapPath("~/BARCODE/" + ponuda.SerijskiBroj + ".png");
                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);

                    imagen.Save(path, System.Drawing.Imaging.ImageFormat.Png);
                }

                


                List<PonudaUsloviPrevoza> lpu = new List<PonudaUsloviPrevoza>();
                foreach (int i in _UslovPrevoza)
                {
                    db.PonudaUsloviPrevoza.Add((new PonudaUsloviPrevoza { IdPonuda = ponuda.IdDnevnik, IdUsloviPrevoza = i })); 
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ponuda);
        }

        //
        // GET: /Ponuda/Edit/5
        [Authorize]
        public ActionResult Edit(int id = 0)
        {

            Ponuda ponuda = db.Ponuda.Find(id);

            if (ponuda == null)
            {
                return HttpNotFound();
            }

            List<int> IdUsloviP = ponuda.PonudaUsloviPrevoza.Select(c =>  c.IdUsloviPrevoza ).ToList();
            ViewBag.IdNaruciocaSL = new SelectList(db.Subjekt, "IdSubjekt", "Naziv", ponuda.IdNarucioca);
            ViewBag.IdValutaSL = new SelectList(db.Valuta, "IdValuta", "OznakaValute", ponuda.IdValuta);

            ViewBag.IdSubjektSL = new SelectList(db.Subjekt, "IdSubjekt", "Naziv", ponuda.IdSubjekt);
            ViewBag.IdValutaPrevoznikaSL = new SelectList(db.Valuta, "IdValuta", "OznakaValute", ponuda.IdValutaPrevoznika);

            ViewBag.UsloviPrevoza = db.UsloviPrevoza.ToList().Select(c => new UsloviPrevoza {  IdUslov = c.IdUslov, Tekst = c.Tekst, Aktivan = IdUsloviP.Contains(c.IdUslov) }).ToList();


            return View(ponuda);
        }

        //
        // POST: /Ponuda/Edit/5

        [HttpPost]
        public ActionResult Edit(Ponuda ponuda, int[] _UslovPrevoza)
        {
            if (ModelState.IsValid)
            {

                if (Request["_SaPDV"] != null)
                {
                    if (Request["_SaPDV"].ToString().Equals("on"))
                        ponuda.SaPDV = true;
                    else ponuda.SaPDV = false;
                }
                else ponuda.SaPDV = false;

                if (Request["_DrugiPrevoznik"] != null)
                {
                    if (Request["_DrugiPrevoznik"].ToString().Equals("on"))
                        ponuda.DrugiPrevoznik = true;
                    else ponuda.DrugiPrevoznik = false;
                }
                else ponuda.DrugiPrevoznik = false;


                ponuda.ZapisAktivan = true;
                ponuda.DatumZapisa = DateTime.Now.AddHours(7);

                db.Entry(ponuda).State = EntityState.Modified;
                db.SaveChanges();



                foreach (PonudaUsloviPrevoza pu in db.PonudaUsloviPrevoza.Where(c => c.IdPonuda == ponuda.IdDnevnik).ToList())
                {
                    db.PonudaUsloviPrevoza.Remove(pu);
                }
                db.SaveChanges();


                List<PonudaUsloviPrevoza> lpu = new List<PonudaUsloviPrevoza>();
                foreach (int i in _UslovPrevoza)
                {
                    db.PonudaUsloviPrevoza.Add((new PonudaUsloviPrevoza { IdPonuda = ponuda.IdDnevnik, IdUsloviPrevoza = i }));
                }

                db.SaveChanges();



                return RedirectToAction("Index");
            }
            return View(ponuda);
        }

        //
        // GET: /Ponuda/Delete/5
        [Authorize]
        public ActionResult Delete(int id = 0)
        {
            Ponuda ponuda = db.Ponuda.Find(id);
            if (ponuda == null)
            {
                return HttpNotFound();
            }
            return View(ponuda);
        }

        //
        // POST: /Ponuda/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Ponuda ponuda = db.Ponuda.Find(id);

            foreach (PonudaUsloviPrevoza pu in db.PonudaUsloviPrevoza.Where(c => c.IdPonuda == ponuda.IdDnevnik).ToList())
            {
                db.PonudaUsloviPrevoza.Remove(pu);
            }
            db.SaveChanges();

            db.Ponuda.Remove(ponuda);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult ReportPonuda(int id)
        {
            ViewData["IdDnevnik"] = id;
            return View();
        }
        [Authorize]
        public ActionResult ReportPonuda2(int id)
        {
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/Ponuda.rdlc");
            ReportDataSource reportDataSource = new ReportDataSource("Ponuda", db.Ponuda.Where(c => c.IdDnevnik == id).ToList().Select(
                                    c => new {
                                        IdDnevnik = c.IdDnevnik,
                                        Subjekt = c.Subjekt.Naziv,
                                        Datum = c.DatumDnevnika.Value.ToShortDateString(),
                                        Utovar = c.UtovarPTT + " " + c.UtovarGrad + " " + c.UtovarDrzava,
                                        Istovar = c.IstovarPTT + " " + c.IstovarGrad + " " + c.IstovarDrzava,
                                        VrstaRobe = c.VrstaRobe,
                                        KolicinaRobe = c.KolicinaRobe,
                                        DimenzijeRobe = c.DimenzijeRobe,
                                        Cijena = c.CijenaPrevoza + " " + c.Valuta.OznakaValute,
                                        PDV = c.IznosPDV == null ? 0 : c.IznosPDV.Value,
                                        ValutaPlacanja = c.ValutaPlacanja,
                                        Tezina = c.TezinaRobe,
                                        BarCode = "http://"+ AppSettings.GetSettings()["domain"] + "/BARCODE/" + c.SerijskiBroj + ".png",
                                        SerijskiBroj = c.SerijskiBroj,
                                        Napomena = c.Napomena
                                    }
                    ).ToList());

            localReport.DataSources.Add(reportDataSource);


            ReportDataSource reportDataSource2 = new ReportDataSource("Uslovi", db.PonudaUsloviPrevoza.Where(c => c.IdPonuda == id).ToList().Select(
                                    c => new {
                                       Tekst = c.UsloviPrevoza.Tekst 
                                    }
                    ).ToList());

            localReport.DataSources.Add(reportDataSource2);

            localReport.EnableExternalImages = true;
            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension;

            //The DeviceInfo settings should be changed based on the reportType
            //http://msdn.microsoft.com/en-us/library/ms155397.aspx
            string deviceInfo =
            "<DeviceInfo>" +
            "  <OutputFormat>PDF</OutputFormat>" +
            "</DeviceInfo>";

            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            //Render the report
            renderedBytes = localReport.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);


            Response.AddHeader("content-disposition", "attachment; filename=" + db.Ponuda.Find(id).SerijskiBroj + ".pdf");
            return File(renderedBytes, mimeType );



        }
        [Authorize]
        public ActionResult ReportPonudaAll()
        {
            LocalReport localReport = new LocalReport();
            localReport.ReportPath = Server.MapPath("~/Reports/PonudaAll.rdlc");


            ReportDataSource reportDataSource = new ReportDataSource("PonudaAll", db.Ponuda.Include(d => d.DnevnikPrevoza).OrderByDescending(c => c.IdDnevnik).ToList().Select(
                                    c => new {
                                       PonudaZa = c.Subjekt.Naziv,
                                       Dostava = c.IstovarPTT + " " + c.IstovarGrad,
                                       Drzava = c.IstovarDrzava,
                                       Dimenzije = c.DimenzijeRobe,
                                       Tezina = c.TezinaRobe,
                                       DrugiPrevoznik = ( c.DnevnikPrevoza.Count() > 0 ) ? ((c.DnevnikPrevoza.FirstOrDefault().Subjekt1 != null) ? c.DnevnikPrevoza.FirstOrDefault().Subjekt1.Naziv : "") : "",
                                       DrugiPrevoznikCijena = (c.DnevnikPrevoza.Count() > 0) ? 
                                                            ((c.DnevnikPrevoza.FirstOrDefault().CijenaPrevozaPrevoznika ?? 0) +" "+ (c.DnevnikPrevoza.FirstOrDefault().Valuta1 == null ? "" : c.DnevnikPrevoza.FirstOrDefault().Valuta1.OznakaValute)) 
                                                            : "0",
                                       DrugiPrevoznikCijenaKM = ((c.DnevnikPrevoza.Count() > 0 ) ? 
                                                    (
                                                    (c.DnevnikPrevoza.FirstOrDefault().CijenaPrevozaPrevoznika ?? 0) * 
                                                    (c.DnevnikPrevoza.FirstOrDefault().Valuta1 == null ? 0 : c.DnevnikPrevoza.FirstOrDefault().Valuta1.UKM ?? 0)
                                                    ) 
                                                    : 0).ToString("0.00"),
                                       MojaCijena = (c.CijenaPrevoza ?? 0 ) + " " + (c.Valuta == null ? "" : c.Valuta.OznakaValute),
                                       MojaCijenaKM = ((c.CijenaPrevoza ?? 0) * (c.Valuta == null ? 0 : c.Valuta.UKM ?? 0)).ToString("0.00"),
                                       Zarada = (((c.CijenaPrevoza ?? 0) * (c.Valuta == null ? 0 : c.Valuta.UKM ?? 0)) -
                                                ((c.DnevnikPrevoza.Count() > 0) ?
                                                    (
                                                    (c.DnevnikPrevoza.FirstOrDefault().CijenaPrevozaPrevoznika ?? 0) *
                                                    (c.DnevnikPrevoza.FirstOrDefault().Valuta1 == null ? 0 : c.DnevnikPrevoza.FirstOrDefault().Valuta1.UKM ?? 0)
                                                    )
                                                    : 0)).ToString("0.00"),
                                       Link = "http://" + AppSettings.GetSettings()["domain"] + "/Ponuda/ReportPonuda2/" + c.IdDnevnik
                                    }
                    ).ToList());

            localReport.DataSources.Add(reportDataSource);


            localReport.EnableExternalImages = true;
            localReport.EnableHyperlinks = true;
            string reportType = "Excel";
            string mimeType;
            string encoding;
            string fileNameExtension;

            //The DeviceInfo settings should be changed based on the reportType
            //http://msdn.microsoft.com/en-us/library/ms155397.aspx
            string deviceInfo =
            "<DeviceInfo>" +
            "  <OutputFormat>XLSX</OutputFormat>" +
            "</DeviceInfo>";

            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            //Render the report
            renderedBytes = localReport.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);
            //Response.AddHeader("content-disposition", "attachment; filename=NorthWindCustomers." + fileNameExtension);
            return File(renderedBytes, mimeType);
        }
        [Authorize]
        public ActionResult CreateFromPonuda(int id, int idParent = 0)
        {
            Ponuda p = db.Ponuda.Find(id);

            DnevnikPrevoza d = new DnevnikPrevoza();
            d.DatumDnevnika = p.DatumDnevnika;
            d.IdNarucioca = p.IdNarucioca;
            d.CijenaPrevoza = p.CijenaPrevoza;
            d.IdValuta = p.IdValuta;
            d.IznosPDV = p.IznosPDV;
            d.SaPDV = p.SaPDV;
            d.DimenzijeRobe = p.DimenzijeRobe;
            d.KolicinaRobe = p.KolicinaRobe;
            d.TezinaRobe = p.TezinaRobe;
            d.VrstaRobe = p.VrstaRobe;
            d.UtovarDrzava = p.UtovarDrzava;
            d.UtovarGrad = p.UtovarGrad;
            d.UtovarPTT = p.UtovarPTT;
            d.IstovarDrzava = p.IstovarDrzava;
            d.IstovarGrad = p.IstovarGrad;
            d.IstovarPTT = p.IstovarPTT;
            d.IdPonuda = p.IdDnevnik;
            d.IdSubjekt = p.IdSubjekt;
            d.DrugiPrevoznik = p.DrugiPrevoznik;
            d.CijenaPrevozaPrevoznika = p.CijenaPrevozaPrevoznika;
            d.IdValutaPrevoznika = p.IdValutaPrevoznika;
            d.ValutaPlacanjaPrevoznika = p.ValutaPlacanjaPrevoznika;
            d.PrevoznikUtovarGrad = p.PrevoznikUtovarGrad;
            d.PrevoznikUtovarDrzava = p.PrevoznikUtovarDrzava;
            d.PrevoznikIstovarGrad = p.PrevoznikIstovarGrad;
            d.PrevoznikIstovarDrzava = p.PrevoznikIstovarDrzava;

            String SerijskiBroj = SerijskiBrojGenerator.Broj(idParent);

            ViewBag.DnevnikCarina = new List<DnevnikCarina>();
            ViewBag.DnevnikIstovar = new List<DnevnikIstovar>();
            ViewBag.DnevnikUtovar = new List<DnevnikUtovar>();

            var dUI = new List<DnevnikUvoznikIzvoznik>();
            ViewBag.DnevnikUvoznikIzvoznik = dUI;

            ViewBag.Vrsta = new SelectList(db.TipUsluge, "Naziv", "Naziv");

            ViewBag.IdNar = d.IdNarucioca;
            ViewBag.SerijskiBroj = SerijskiBroj;
            ViewBag.IdPonuda = new SelectList(db.Ponuda.Select(c => new { IdDnevnik = c.IdDnevnik, Naziv = c.SerijskiBroj + " [ Za " + c.Subjekt.Naziv + " na destinaciju:  " + c.IstovarPTT + " " + c.IstovarGrad + " " + c.IstovarDrzava + " ]" }).OrderByDescending(k => k.IdDnevnik), "IdDnevnik", "Naziv", d.IdPonuda);
            ViewBag.IdSubjekt = new SelectList(db.Subjekt, "IdSubjekt", "Naziv", d.IdSubjekt);
            ViewBag.IdNarucioca = new SelectList(db.Subjekt, "IdSubjekt", "Naziv", d.IdNarucioca);
            ViewBag.IdValuta = new SelectList(db.Valuta, "IdValuta", "OznakaValute",d.IdValuta);
            ViewBag.IdValutaPrevoznika = new SelectList(db.Valuta, "IdValuta", "OznakaValute", d.IdValutaPrevoznika);
            ViewBag.IdVozac = new SelectList(db.Vozaci, "IdVozac", "ImeVozaca");
            ViewBag.IdStatusDetaljni = new SelectList(db.StatusRobe, "IdStatusRobe", "Naziv");
            ViewBag.IdVozilo = new SelectList(db.Vozilo.Where(c => c.VrstaVozila.Equals("Vozilo")).Select(c => new { IdVozilo = c.IdVozilo, TipVozila = c.TipVozila + " " + c.RegistarskiBroj }), "IdVozilo", "TipVozila");
            ViewBag.IdPrikljucno = new SelectList(db.Vozilo.Where(c => c.VrstaVozila.Equals("Priključno Vozilo")).Select(c => new { IdVozilo = c.IdVozilo, TipVozila = c.TipVozila + " " + c.RegistarskiBroj }), "IdVozilo", "TipVozila");
            // ViewBag.IdDnevnikParent = null; // new SelectList(db.DnevnikPrevoza.Where(c => (c.ZapisAktivan ?? false)).OrderByDescending(c => c.IdDnevnik).Take(50), "IdDnevnik", "SerijskiBroj");
            ViewBag.IdDnevnikParent = idParent == 0 ? null : "" + idParent;

            // return View(dp);
            return View("~/Views/DnevnikPrevoza/CreateTab.cshtml", d);



        }


        public JsonResult PodundaIzDnevnika(String GradOd, String GradDo)
        {

            if (GradOd.Contains("-"))
            {
                int index = GradOd.IndexOf('-');
                GradOd = GradOd.Substring(0, index).Trim();
            }

            if (GradDo.Contains("-"))
            {
                int index = GradDo.IndexOf('-');
                GradDo = GradDo.Substring(0, index).Trim();
            }

            var ListaDnevnika = db.DnevnikPrevoza.Where(c => (c.ZapisAktivan ?? false) && c.UtovarGrad.Contains(GradOd) && c.IstovarGrad.Contains(GradDo)).ToList().OrderByDescending(c => c.IdDnevnik).Select(
                c => new {
                    IdDnevnik = c.IdDnevnik,
                    RelacijaOd = c.UtovarPTT + " " + c.UtovarGrad + " " + c.UtovarDrzava,
                    RelacijaDo = c.IstovarPTT + " " + c.IstovarGrad + " " + c.IstovarDrzava,
                    Cijena = c.CijenaPrevoza + " " + c.Valuta.OznakaValute,
                    PDV = c.IznosPDV != null ? c.IznosPDV.Value.ToString("0.00") : "0",
                    CijenaDrugogPrevoznika = ((c.CijenaPrevozaPrevoznika ?? 0) * (c.Valuta1 == null ? 0 : c.Valuta1.UKM ?? 0)).ToString("0.00"),
                    Troskovi = c.Troskovi.Count() > 0 ? (c.Troskovi.Sum(d => (d.Iznos ?? 0) * (d.Valuta.UKM ?? 0))).ToString("0.00") : "0",
                    Roba = "Količina: " + c.KolicinaRobe + System.Environment.NewLine + "Težina: " + c.TezinaRobe + System.Environment.NewLine + "Vrsta: " + c.VrstaRobe,
                    Datum = c.DatumDnevnika.Value.ToShortDateString(),
                }
                ).ToList();
            return Json(ListaDnevnika, JsonRequestBehavior.AllowGet);

            /*
             
            decimal suma = (dnevnikprevoza.CijenaPrevoza ?? 0) * (dnevnikprevoza.Valuta.UKM ?? 0) - (dnevnikprevoza.CijenaPrevozaPrevoznika ?? 0) * (dnevnikprevoza.Valuta1 == null ? 0 : dnevnikprevoza.Valuta1.UKM ?? 0);
            decimal troskovi = 0;
            if (db.Troskovi.Where(c => c.IdDnevnik == id).Count() > 0)
                troskovi = db.Troskovi.Where(c => c.IdDnevnik == id).Sum(c => (c.Iznos ?? 0) * (c.Valuta.UKM ?? 0));

             * */


        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}