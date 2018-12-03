<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<%@ Import Namespace="System.Data.Entity" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta name="viewport" content="width=device-width" />
    <title>Ponuda</title>
    <script runat="server">

        void Page_Load(Object Sender, EventArgs e) {

            if (!IsPostBack)
            {

                Spedicija.uvhszjiy_spedicijaEntities db = new Spedicija.uvhszjiy_spedicijaEntities();

                int param = Convert.ToInt32(@ViewData["IdDnevnik"]);

                ReportViewer4.LocalReport.ReportPath = Server.MapPath("~/Reports/Ponuda.rdlc");
                ReportViewer4.LocalReport.DataSources.Clear();
                ReportViewer4.ShowExportControls = true;
                ReportViewer4.ShowDocumentMapButton = true;
                ReportViewer4.LocalReport.DisplayName = "Ponuda";

                ReportDataSource Ponuda  = new ReportDataSource("Ponuda", db.Ponuda.Where(c => c.IdDnevnik == param).ToList().Select(
                    c => new {
                        IdDnevnik	 = c.IdDnevnik,
                        Subjekt		= c.Subjekt.Naziv,
                        Datum			= c.DatumDnevnika.Value.ToShortDateString(),
                        Utovar 		= c.UtovarPTT + " " + c.UtovarGrad + " " + c.UtovarDrzava,
                        Istovar 		= c.IstovarPTT + " " + c.IstovarGrad + " " + c.IstovarDrzava,
                        VrstaRobe		= c.VrstaRobe,
                        KolicinaRobe	= c.KolicinaRobe,
                        DimenzijeRobe 	= c.DimenzijeRobe,
                        Cijena			= c.CijenaPrevoza + " " + c.Valuta.NazivValute,
                        PDV			=  c.IznosPDV == 0 ? 0 : c.IznosPDV.Value ,
                        ValutaPlacanja = c.ValutaPlacanja,
                        Tezina			= c.TezinaRobe
                    }
                    ).ToList());


                ReportViewer4.LocalReport.EnableExternalImages = true;
                ReportViewer4.LocalReport.DataSources.Add(Ponuda);


                ReportViewer4.LocalReport.Refresh();


            }

        }
    </script>
</head>
<body>
    <form id="form4" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager4" runat="server"></asp:ScriptManager>
        <rsweb:ReportViewer ID="ReportViewer4" runat="server" AsyncRendering="false" SizeToReportContent="true">

        </rsweb:ReportViewer>
    </div>
        </form>
</body>
</html>
