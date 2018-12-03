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
                                   
                    ReportViewer2.LocalReport.ReportPath = Server.MapPath("~/Reports/DnevnikStampa.rdlc");
                    ReportViewer2.LocalReport.DataSources.Clear();
                    ReportViewer2.ShowExportControls = true;
                    ReportViewer2.ShowDocumentMapButton = true;
                    ReportViewer2.LocalReport.DisplayName = "DnevnikPrevoza";

                    ReportDataSource Dnevnik  = new ReportDataSource("Dnevnik", db.VratiDnevnikPoId(param).ToList());
                    ReportDataSource Carine = new ReportDataSource("Carine", db.VratiDnevnikCarinaPoIdDnevnik(param).ToList());
                    ReportDataSource Istovari = new ReportDataSource("Istovari", db.VratiDnevnikIStovarPoIdDnevnik(param).ToList());
                    ReportDataSource Utovari = new ReportDataSource("Utovari", db.VratiDnevnikUtovarPoIdDnevnik(param).ToList());
                    ReportDataSource UvoznikIzvoznik = new ReportDataSource("UvoznikIzvoznik", db.VratiDnevnikUIPoIdDnevnik(param).ToList());
                    ReportDataSource Troskovi = new ReportDataSource("Troskovi", db.VratiTroskovePoIdDnevnik(param).ToList());

                    ReportViewer2.LocalReport.EnableExternalImages = true;
                    ReportViewer2.LocalReport.DataSources.Add(Dnevnik);
                    ReportViewer2.LocalReport.DataSources.Add(Carine);
                    ReportViewer2.LocalReport.DataSources.Add(Istovari);
                    ReportViewer2.LocalReport.DataSources.Add(Utovari);
                    ReportViewer2.LocalReport.DataSources.Add(UvoznikIzvoznik);
                    ReportViewer2.LocalReport.DataSources.Add(Troskovi);
                    
                    ReportViewer2.LocalReport.Refresh();
                
                                 
            }
            
        }
    </script>
</head>
<body>
    <form id="form2" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager2" runat="server"></asp:ScriptManager>
        <rsweb:ReportViewer ID="ReportViewer2" runat="server" AsyncRendering="false" SizeToReportContent="true">

        </rsweb:ReportViewer>
    </div>
        </form>
</body>
</html>
