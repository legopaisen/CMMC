<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportViewer.aspx.cs" Inherits="CMMC.Reports.ReportViewer" %>
<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server"> 
 <script src="<%= ResolveUrl("~/Assets/js/jquery-1.12.0.min.js") %>"></script>
    <title>View Report</title>
    <script type="text/javascript">
        $(document).ready(function () {
         var usertype = '<%= Session["UserType"].ToString()%>';

         if (usertype != '2' && usertype != '8' && usertype != '9' && usertype != '11') //9 - User Administrator, 8 - Audit, 11 - Custom, 2 - System Administrator
         {
             $("a[title='Word']").parent().hide();
             $("a[title='PDF']").parent().hide();
         }
     });
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
     <asp:ScriptManager ID="ScriptManager1" runat="server">
     </asp:ScriptManager>
     <rsweb:ReportViewer ID="rptReportViewer" runat="server" Width="100%" Height="100%" Font-Names="Verdana" Font-Size="8pt" AsyncRendering="False" SizeToReportContent="true" >
     </rsweb:ReportViewer>
    
    </div>
    </form>
</body>
</html>
