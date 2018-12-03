<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<%@ Import Namespace="System.Data.Entity" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta name="viewport" content="width=device-width" />
    <title>Izvještaj Dnevnik Prevoza</title>
    <script runat="server">

        void Page_Load(Object Sender, EventArgs e) {

            if (!IsPostBack)
            {

                Spedicija.uvhszjiy_spedicijaEntities db = new Spedicija.uvhszjiy_spedicijaEntities();

                int param = Convert.ToInt32(@ViewData["IdDnevnik"]);

                ReportViewer3.LocalReport.ReportPath = Server.MapPath("~/Reports/DnevnikStampa_Novi.rdlc");
                ReportViewer3.LocalReport.DataSources.Clear();
                ReportViewer3.ShowExportControls = true;
                ReportViewer3.ShowDocumentMapButton = true;
                ReportViewer3.LocalReport.DisplayName = "DnevnikPrevoza";

                ReportDataSource Dnevnik  = new ReportDataSource("Dnevnik", db.VratiDnevnikPoId(param).ToList());

                var dnevnikprevoza = db.DnevnikPrevoza.Find(param);
                var UtovarSped = dnevnikprevoza.DnevnikCarina.ToList();
                var Dutovar = dnevnikprevoza.DnevnikUtovar.ToList();
                var Distovar = dnevnikprevoza.DnevnikIstovar.ToList();


                var x1 =  db.Troskovi.Include(a => a.Valuta).Where(c => c.IdDnevnik == param).Select(c=> new {IdTroskovi = c.IdTroskovi, IznosTroskaBAM = (c.Iznos * c.Valuta.UKM), iznosTroska = c.Iznos, IznosStvarniTrosakBAM = c.StvarniTrosak * c.Valuta.UKM, StvarniTrosak = c.StvarniTrosak, ValutaTrosak = c.Valuta.OznakaValute, VrstaTrosak = c.Vrsta  }).ToList();
                Decimal Suma1 = x1.Sum(c => (c.IznosTroskaBAM ?? 0));
                Decimal SumaStvarnitrosak = x1.Sum(c => c.IznosStvarniTrosakBAM ?? 0);
                //String valuta1 = x1.Count > 0 ? x1[0].ValutaTrosak : "";

                var t = x1.Select( c => new TroskoviReportView { Trosak = "DA", Opis = "Usluge", Rb = 5, IznosTroskaBAM = (c.IznosTroskaBAM ?? 0).ToString("0.00") + " BAM", iznosTroska = c.iznosTroska + " " + c.ValutaTrosak, VrstaTrosak = c.VrstaTrosak , IdTroskovi = "" }).ToList();
                t.Insert(0,new TroskoviReportView { Trosak = "NE", Opis = "Usluge", Rb = 3, IznosTroskaBAM = ((dnevnikprevoza.CijenaPrevoza ?? 0) * (dnevnikprevoza.Valuta.UKM ?? 0)).ToString("0.00") + " BAM", iznosTroska = dnevnikprevoza.CijenaPrevoza + " " + dnevnikprevoza.Valuta.OznakaValute, VrstaTrosak = "Moja ukupna cijena prevoza", IdTroskovi = (dnevnikprevoza.ValutaPlacanja == null ? "" : dnevnikprevoza.ValutaPlacanja + " dana") });
                t.Insert(0,new TroskoviReportView { Trosak = "NE", Opis = "Usluge", Rb = 2, IznosTroskaBAM = ((dnevnikprevoza.IznosPDV ?? 0) * (dnevnikprevoza.Valuta.UKM ?? 0)).ToString("0.00") + " BAM", iznosTroska = dnevnikprevoza.IznosPDV + " " + dnevnikprevoza.Valuta.OznakaValute, VrstaTrosak = "PDV", IdTroskovi = "" });
                t.Insert(0,new TroskoviReportView { Trosak = "NE", Opis = "Usluge", Rb = 1, IznosTroskaBAM = (((dnevnikprevoza.CijenaPrevoza ?? 0) - (dnevnikprevoza.IznosPDV ?? 0 )) * (dnevnikprevoza.Valuta.UKM ?? 0)).ToString("0.00") + " BAM", iznosTroska = dnevnikprevoza.CijenaPrevoza - (dnevnikprevoza.IznosPDV ?? 0 ) + " " + dnevnikprevoza.Valuta.OznakaValute, VrstaTrosak = "Iznos bez PDV-a", IdTroskovi = "" });

                if(dnevnikprevoza.DrugiPrevoznik ?? false)
                    t.Insert(0,new TroskoviReportView { Trosak = "DA", Opis = "Troškovi", Rb = 60, IznosTroskaBAM = ((dnevnikprevoza.CijenaPrevozaPrevoznika ?? 0) * (dnevnikprevoza.Valuta1 == null ? 0 : dnevnikprevoza.Valuta1.UKM ?? 0)).ToString("0.00") + " BAM" , iznosTroska = dnevnikprevoza.CijenaPrevozaPrevoznika + " " + (dnevnikprevoza.Valuta1 == null ? "" : dnevnikprevoza.Valuta1.OznakaValute), VrstaTrosak = "Cijena Drugog prevoznika", IdTroskovi = "" });

                //t.Insert(0,new TroskoviReportView { Trosak = "DA", Opis = "Troškovi", Rb = 31, iznosTroska = SumaStvarnitrosak + " " + valuta1, VrstaTrosak = "Dodatni Troškovi", IdTroskovi = "" });
                foreach (var a in x1)
                {
                    t.Add(new TroskoviReportView { Trosak = "DA", Opis = "Troškovi", Rb = 61 + x1.IndexOf(a),IznosTroskaBAM = (a.IznosStvarniTrosakBAM ?? 0).ToString("0.00") + " BAM" ,  iznosTroska = a.StvarniTrosak + " " + a.ValutaTrosak, VrstaTrosak = a.VrstaTrosak, IdTroskovi = "" });
                }

                Decimal ZaFakturianje = Suma1 + (dnevnikprevoza.CijenaPrevoza ?? 0) * (dnevnikprevoza.Valuta.UKM ?? 0);
                Decimal ZaTrosak = SumaStvarnitrosak + (dnevnikprevoza.CijenaPrevozaPrevoznika ?? 0) * ((dnevnikprevoza.Valuta1 == null ? 0 : dnevnikprevoza.Valuta1.UKM) ?? 0) ;


                var Poddnevnici = db.DnevnikPrevoza.Where(c => c.IdDnevnikParent == param).ToList();
                var tpoddnevnici = new List<TroskoviReportView>();
                var UIpoddnevnici = new List<UtovarSpedicije>();


                foreach (var item in Poddnevnici)
                {
                    var x =  db.Troskovi.Include(a => a.Valuta).Where(c => c.IdDnevnik == item.IdDnevnik).Select(c=> new {IdTroskovi = c.IdTroskovi, IznosTroskaBAM = (c.Iznos * c.Valuta.UKM), iznosTroska = c.Iznos,  IznosStvarniTrosakBAM = c.StvarniTrosak * c.Valuta.UKM, StvarniTrosak = c.StvarniTrosak, ValutaTrosak = c.Valuta.OznakaValute, VrstaTrosak = c.Vrsta  }).ToList();

                    Decimal Suma = x.Sum(c => c.IznosTroskaBAM ?? 0);
                    Decimal SumaStvarniTrosak = x.Sum(c => c.IznosStvarniTrosakBAM ?? 0);
                   // String valuta = x.Count > 0 ? x[0].ValutaTrosak : "";

                    var t1 = x.Select(c=> new TroskoviReportView {  Trosak = "DA", Opis = "Usluge sa poddnevnika", Rb = 40  + Poddnevnici.IndexOf(item)+1, IznosTroskaBAM = (c.IznosTroskaBAM ?? 0).ToString("0.00") + " BAM" , iznosTroska = (c.iznosTroska ?? 0) + " " + c.ValutaTrosak, VrstaTrosak = c.VrstaTrosak , IdTroskovi = item.SerijskiBroj }).ToList();
                    tpoddnevnici.AddRange(t1);
                    tpoddnevnici.Add(new TroskoviReportView {  Trosak = "NE", Opis = "Usluge sa poddnevnika", Rb = 20 + Poddnevnici.IndexOf(item)+1 , IznosTroskaBAM = ((item.CijenaPrevoza * item.Valuta.UKM) ?? 0).ToString("0.00") + " BAM" , iznosTroska = item.CijenaPrevoza + " " + item.Valuta.OznakaValute, VrstaTrosak = "Moja ukupna cijena prevoza", IdTroskovi = item.SerijskiBroj  });

                    if (item.DrugiPrevoznik ?? false)
                        tpoddnevnici.Add(new TroskoviReportView {  Trosak = "DA", Opis = "Troškovi sa Poddnevnika", Rb = 80 + Poddnevnici.IndexOf(item)+1, IznosTroskaBAM = ((item.CijenaPrevozaPrevoznika ?? 0) * (item.Valuta1 == null ? 0 : (item.Valuta1.UKM ?? 0) )).ToString("0.00") + " BAM" , iznosTroska = item.CijenaPrevozaPrevoznika + " " + (item.Valuta1 == null ? "" : item.Valuta1.OznakaValute), VrstaTrosak = "Cijena Drugog prevoznika", IdTroskovi = item.SerijskiBroj  });

                    //tpoddnevnici.Add(new TroskoviReportView {  Trosak = "DA", Opis = "Troškovi sa Poddnevnika", Rb = 50 + Poddnevnici.IndexOf(item)+1, iznosTroska = SumaStvarniTrosak + " " + valuta, VrstaTrosak = "Dodatne usluge" , IdTroskovi = item.SerijskiBroj  });
                    tpoddnevnici.AddRange(x.Select(c=> new TroskoviReportView {  Trosak = "DA", Opis = "Troškovi sa Poddnevnika", Rb = 81  + Poddnevnici.IndexOf(item)+1, IznosTroskaBAM = (c.IznosStvarniTrosakBAM ?? 0).ToString("0.00") + " BAM" ,iznosTroska = (c.StvarniTrosak ?? 0) + " " + c.ValutaTrosak, VrstaTrosak = c.VrstaTrosak , IdTroskovi = item.SerijskiBroj }).ToList());



                    ZaFakturianje = ZaFakturianje + Suma + (item.CijenaPrevoza ?? 0) * (item.Valuta.UKM ?? 0);
                    ZaTrosak = ZaTrosak + SumaStvarniTrosak +  (item.CijenaPrevozaPrevoznika ?? 0) * (item.Valuta1 == null ? 0 : (item.Valuta1.UKM ?? 0)) ; //Zarada + item.CijenaPrevoza ?? 0 - item.CijenaPrevozaPrevoznika ?? 0;


                    UIpoddnevnici.Add( new UtovarSpedicije
                    {
                        Tip = "Utovar " + item.SerijskiBroj,
                        Rb = Poddnevnici.IndexOf(item)+2,
                        Firma = item.UtovarFirma + System.Environment.NewLine + item.UtovarAdresa + System.Environment.NewLine + item.UtovarPTT + " " + item.UtovarGrad + " " + item.UtovarDrzava ,
                        Spedicija = item.IzvoznaSpedicija,
                        Roba = "Količina: " + item.KolicinaRobe + System.Environment.NewLine + "Težinau kg: " + item.TezinaRobe + System.Environment.NewLine + "Vrsta: " + item.VrstaRobe,
                        Datum = item.DatumUtovara.HasValue ? item.DatumUtovara.Value.ToShortDateString() : "",
                        Kontakt = item.UtovarKontakt
                    });

                    UIpoddnevnici.Add(
                        new UtovarSpedicije
                        {
                            Tip = "Istovar "+ item.SerijskiBroj,
                            Rb = Poddnevnici.IndexOf(item)+2,
                            Firma = item.IstovarFirma + System.Environment.NewLine + item.IstovarAdresa + System.Environment.NewLine + item.IstovarPTT + " " + item.IstovarGrad +" " + item.IstovarDrzava,
                            Spedicija = item.UvoznaSpedicija,
                            Roba = "Količina: " + item.IstovarKolicinaRobe,
                            Datum = item.DatumIstovara.HasValue ? item.DatumIstovara.Value.ToShortDateString() : "",
                            Kontakt = item.IstovarKontakt
                        }
                        );

                }



                t.AddRange(tpoddnevnici);
                t.Add(new TroskoviReportView { Trosak = "NE", Opis = "Za Fakturisanje", Rb = 59, IznosTroskaBAM = ZaFakturianje.ToString("0.00") + " BAM" , VrstaTrosak = "", IdTroskovi = "" });
                t.Add(new TroskoviReportView { Trosak = "NE", Opis = "Ukupni Troškovi", Rb = 99, IznosTroskaBAM = ZaTrosak.ToString("0.00") + " BAM" , VrstaTrosak = "", IdTroskovi = "" });
                t.Add(new TroskoviReportView { Trosak = "NE", Opis = "Zarada", Rb = 100, IznosTroskaBAM = (ZaFakturianje - ZaTrosak).ToString("0.00") + " BAM" , VrstaTrosak = "", IdTroskovi = "" });

                //ReportDataSource Troskovi = new ReportDataSource("Troskovi", t.OrderBy(c => c.Rb).ToList());
                ReportDataSource TroskoviPodDnevnik = new ReportDataSource("TroskoviPodDnevnik", t.OrderBy(c => c.Rb).ToList());




                var UtovarIstovar= dnevnikprevoza.DnevnikUtovar.Select(c => new UtovarSpedicije
                {
                    Tip = "Utovar",
                    Rb =  Dutovar.IndexOf(c) + 2,
                    Firma = c.Firma + System.Environment.NewLine + c.Adresa + System.Environment.NewLine + c.PTT + " " + c.Mjesto + " " + c.Drzava,
                    Spedicija = Dutovar.IndexOf(c) < UtovarSped.Count() ? UtovarSped[Dutovar.IndexOf(c)].IzvoznaCarina : "",
                    Roba = "Količina: " + c.KolicinaRobe + System.Environment.NewLine + "Težina u kg: " + c.TezinaRobe + System.Environment.NewLine + "Vrsta: " + c.VrstaRobe,
                    Datum =  c.DatmUtovara.HasValue ? c.DatmUtovara.Value.ToShortDateString() : "" ,
                    Kontakt = c.Kontakt
                }).ToList();

                UtovarIstovar.Insert(0, new UtovarSpedicije
                {
                    Tip = "Utovar",
                    Rb = 1,
                    Firma = dnevnikprevoza.UtovarFirma + System.Environment.NewLine + dnevnikprevoza.UtovarAdresa + System.Environment.NewLine + dnevnikprevoza.UtovarPTT + " " + dnevnikprevoza.UtovarGrad + " " + dnevnikprevoza.UtovarDrzava ,
                    Spedicija = dnevnikprevoza.IzvoznaSpedicija,
                    Roba = "Količina: " + dnevnikprevoza.KolicinaRobe + System.Environment.NewLine + "Težina u kg: " + dnevnikprevoza.TezinaRobe + System.Environment.NewLine + "Vrsta: " + dnevnikprevoza.VrstaRobe,
                    Datum = dnevnikprevoza.DatumUtovara.HasValue ? dnevnikprevoza.DatumUtovara.Value.ToShortDateString() : "",
                    Kontakt = dnevnikprevoza.UtovarKontakt
                });

                UtovarIstovar.Add(
                    new UtovarSpedicije
                    {
                        Tip = "Istovar",
                        Rb = 1,
                        Firma = dnevnikprevoza.IstovarFirma + System.Environment.NewLine + dnevnikprevoza.IstovarAdresa + System.Environment.NewLine + dnevnikprevoza.IstovarPTT + " " + dnevnikprevoza.IstovarGrad +" " + dnevnikprevoza.IstovarDrzava,
                        Spedicija = dnevnikprevoza.UvoznaSpedicija,
                        Roba = "Količina: " + dnevnikprevoza.IstovarKolicinaRobe,
                        Datum = dnevnikprevoza.DatumIstovara.HasValue ? dnevnikprevoza.DatumIstovara.Value.ToShortDateString() : "",
                        Kontakt = dnevnikprevoza.IstovarKontakt
                    }
                    );

                UtovarIstovar.AddRange(
                    dnevnikprevoza.DnevnikIstovar.Select(c => new UtovarSpedicije
                    {
                        Tip = "Istovar",
                        Rb = Distovar.IndexOf(c) + 2,
                        Firma = c.Firma + System.Environment.NewLine + c.Adresa + System.Environment.NewLine + c.PTT + " " + c.Mjesto + " " + c.Drzava,
                        Spedicija = Distovar.IndexOf(c) < UtovarSped.Count() ? UtovarSped[Distovar.IndexOf(c)].UvoznaCarina : "",
                        Roba = "Količina: " + c.KolicinaRobe,
                        Datum = c.DatumIstovara.HasValue ? c.DatumIstovara.Value.ToShortDateString() : "",
                        Kontakt = c.Kontakt
                    }).ToList()
                    );



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


                UtovarIstovar.AddRange(UIpoddnevnici);
                ReportDataSource UtovarSpedicije = new ReportDataSource("UtovarSpedicije", UtovarIstovar);

                ReportDataSource CarineUvoznikIzvoznik = new ReportDataSource("CarineUvoznikIzvoznik", CARINEUI);

                ReportViewer3.LocalReport.EnableExternalImages = true;
                ReportViewer3.LocalReport.DataSources.Add(Dnevnik);
                ReportViewer3.LocalReport.DataSources.Add(UtovarSpedicije);
                ReportViewer3.LocalReport.DataSources.Add(CarineUvoznikIzvoznik);
                //ReportViewer3.LocalReport.DataSources.Add(Zarade);
                //  ReportViewer3.LocalReport.DataSources.Add(UtovarSpedicijePoddnevnici);
                ReportViewer3.LocalReport.DataSources.Add(TroskoviPodDnevnik);

                ReportViewer3.LocalReport.Refresh();


            }

        }
    </script>
</head>
<body>
    <form id="form2" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager3" runat="server"></asp:ScriptManager>
        <rsweb:ReportViewer ID="ReportViewer3" runat="server" AsyncRendering="false" SizeToReportContent="true">

        </rsweb:ReportViewer>
    </div>
        </form>
</body>
</html>
