<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<%@ Import Namespace="System.Data.Entity" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta name="viewport" content="width=device-width" />
    <title>ReportNalog</title>
    <script runat="server">

            void Page_Load(Object Sender, EventArgs e) {

                if (!IsPostBack)
                {

                    Spedicija.uvhszjiy_spedicijaEntities db = new Spedicija.uvhszjiy_spedicijaEntities();

                    int param = Convert.ToInt32(@ViewData["IdDnevnik"]);
                    String param1 = (@ViewData["Korisnik"]).ToString();
                    String jezik = (@ViewData["Jezik"]).ToString();


                    var DodatniUtovar = db.DnevnikUtovar.Where(c => c.IdDnevnik == param).ToList();
                    String du = "";
                    String fu = "";

                    foreach (var s in DodatniUtovar)
                    {
                        du += System.Environment.NewLine + (DodatniUtovar.IndexOf(s) + 2) + ". " + s.Adresa + ", " + s.PTT + " " + s.Mjesto + " " + s.Drzava;
                        fu += System.Environment.NewLine + (DodatniUtovar.IndexOf(s) + 2) + ". " + s.Firma;
                    }

                    var DodatniIstovar = db.DnevnikIstovar.Where(c => c.IdDnevnik == param).ToList();
                    String di = "";
                    String fi = "";

                    foreach (var s in DodatniIstovar)
                    {
                        di += System.Environment.NewLine + (DodatniIstovar.IndexOf(s) + 2) + ". " + s.Adresa + ", " + s.PTT + " " + s.Mjesto + " " + s.Drzava;
                        fi += System.Environment.NewLine + (DodatniIstovar.IndexOf(s) + 2) + ". " + s.Firma;
                    }


                    var UvoznikIzvoznik = db.DnevnikUvoznikIzvoznik.Where(c => c.IdDnevnik == param).ToList();
                    String uvoznik = "";
                    String izvoznik = "";

                    foreach (var item in UvoznikIzvoznik)
                    {
                        uvoznik += (UvoznikIzvoznik.IndexOf(item) + 1) + ". " + item.Uvoznik + System.Environment.NewLine;
                        izvoznik += (UvoznikIzvoznik.IndexOf(item) + 1) + ". " + item.Izvoznik + System.Environment.NewLine;
                    }

                    var NalogZaUtovar = db.DnevnikPrevoza.Where(c => c.IdDnevnik == param).ToList().Select(
                         c => new
                         {
                             // c.DnevnikUtovar.Select(g => g.Mjesto)
                             BrojNaloga = c.SerijskiBroj,
                             Naziv = c.Subjekt == null ? "" : c.Subjekt.Naziv,
                             Adresa = c.Subjekt == null ? "" : c.Subjekt.Adresa + ", " + c.Subjekt.PTT + " " + c.Subjekt.Grad + " " + c.Subjekt.Drzava,
                             Timocom = c.Subjekt == null ? "" : c.Subjekt.Timocom,
                             Izvoznik = c.DnevnikUvoznikIzvoznik.Count() == 0 ? "" : izvoznik, //String.Join(", ", c.DnevnikUvoznikIzvoznik.Select(g => g.Izvoznik)), //c.UtovarFirma,
                             Napomena = c.Napomena,

                             Utovar = String.IsNullOrEmpty(c.PrevoznikUtovarFirma) ?
                                        (du.Equals("") ? c.UtovarAdresa + ", " + c.UtovarPTT + " " + c.UtovarGrad + " " + c.UtovarDrzava : "1. " + c.UtovarAdresa + ", " + c.UtovarGrad + " " + c.UtovarDrzava + du) :
                                        (c.PrevoznikUtovarAdresa + ", " + c.PrevoznikUtovarPTT + " " + c.PrevoznikUtovarGrad + " " + c.PrevoznikUtovarDrzava),

                             UtovarFirma = String.IsNullOrEmpty(c.PrevoznikUtovarFirma) ?
                                        (fu.Equals("") ? c.UtovarFirma : "1. " + c.UtovarFirma + fu) :
                                        (c.PrevoznikUtovarFirma),

                             DatumUtovara = c.DatumUtovara.HasValue ? c.DatumUtovara.Value.ToShortDateString() : "",
                             UtovarKontakt = c.UtovarKontakt,
                             VrtsaRobe = c.VrstaRobe,
                             Brutotezina = c.KolicinaRobe + " " + (String.IsNullOrEmpty(c.TezinaRobe) ? "" : c.TezinaRobe + " kg"),
                             IzvoznaCarina = c.IzvoznaSpedicija,

                             Subjekt = c.DnevnikUvoznikIzvoznik.Count() == 0 ? "" : uvoznik, //c.DnevnikUvoznikIzvoznik.FirstOrDefault().Uvoznik,  //c.IstovarFirma,
                             KontaktOsoba = c.KontaktOsobaPrevoznika,


                             IstovarFirma = String.IsNullOrEmpty(c.PrevoznikIstovarFirma) ?
                                            (fi.Equals("") ? c.IstovarFirma : "1. " + c.IstovarFirma + fi) :
                                            (c.PrevoznikIstovarFirma),

                            IstovatAdresa =  String.IsNullOrEmpty(c.PrevoznikIstovarFirma) ?
                                            (di.Equals("") ? c.IstovarAdresa + ", " + c.IstovarPTT + " " + c.IstovarGrad + " " + c.IstovarDrzava : "1. " + c.IstovarAdresa + ", " + c.IstovarPTT + " " + c.IstovarGrad + " " + c.IstovarDrzava + di) :
                                            (c.PrevoznikIstovarAdresa + ", " + c.PrevoznikIstovarPTT + " " + c.PrevoznikIstovarGrad + " " + c.PrevoznikIstovarDrzava),




                         IStovarkontakt = c.IstovarKontakt,
                         DatumIstovara = c.DatumIstovara.HasValue ? c.DatumIstovara.Value.ToShortDateString() : "",
                         UvoznaSpedicija = c.UvoznaSpedicija,

                         RegBroj = c.RegistracijaVozilaPrevoznika,
                         ImeVozaca = c.VozacPrevoznika,
                         CijenaPrevoza = c.CijenaPrevozaPrevoznika + " " + (c.Valuta1 == null ? "" : c.Valuta1.OznakaValute),
                         Valuta = c.Valuta1 == null ? "" : c.Valuta1.OznakaValute,
                         ValutaPlacanja = c.ValutaPlacanjaPrevoznika,

                         KompletanUtovar = (c.KompletanUtovar ?? false) ? "DA" : "NE",

                         Veza = param1,

                         BarKod = "http://"+ Spedicija.AppSettings.GetSettings()["domain"] +"/BARCODE/" + c.SerijskiBroj + ".png" , 


                         DimenzijaRobe = c.DimenzijeRobe

                             //BarKod = "http://www.barcodes4.me/barcode/c39/" + c.SerijskiBroj + ".png?IsTextDrawn=1"

                    // BarKod =  "http://api-bwipjs.rhcloud.com/?bcid=code128&text=" + c.SerijskiBroj


                    // BarKod =  "https://barcode.tec-it.com/barcode.ashx?translate-esc=off&data=" + c.SerijskiBroj +"&code=Code128&unit=Fit&dpi=96&imagetype=Png&rotation=0&color=000000&bgcolor=FFFFFF&qunit=Mm&quiet=0"
                }).ToList();





                /*
                    var DnevnikPrevoza = db.DnevnikPrevoza.Where(c => c.IdDnevnik == param1).ToList();
                    var Valuta = db.Valuta.Where(c => c.IdValuta == DnevnikPrevoza.FirstOrDefault().IdValuta);
                    var Vozaci = db.Vozaci.Where(c => c.IdVozac == DnevnikPrevoza.FirstOrDefault().IdVozac);
                    var Vozilo = db.Vozilo.Where(c => c.IdVozilo == DnevnikPrevoza.FirstOrDefault().IdVozilo);
                    var Subjekt = db.Subjekt.Where(c => c.IdSubjekt == DnevnikPrevoza.FirstOrDefault().IdNarucioca);
                */
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/Nalog" + jezik + ".rdlc");
                ReportViewer1.LocalReport.EnableExternalImages = true;


                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.ShowExportControls = true;
                ReportViewer1.ShowDocumentMapButton = true;
                ReportViewer1.LocalReport.DisplayName = NalogZaUtovar.FirstOrDefault().BrojNaloga + "-OFFICE-" + (jezik.Equals("SR") ? "NalogZaUtovar" : "Оrder-for-loading");
                ReportDataSource rdc = new ReportDataSource("NalogZaUtovar", NalogZaUtovar);

                ReportViewer1.LocalReport.DataSources.Add(rdc);


                ReportViewer1.LocalReport.Refresh();




            }

        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" AsyncRendering="false" SizeToReportContent="true">

        </rsweb:ReportViewer>
    </div>
        </form>
</body>
</html>
