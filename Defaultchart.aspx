<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Defaultchart.aspx.cs" Inherits="Defaultchart" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <script type="text/javascript" src="https://www.gstatic.com/charts/loader.js"></script>  
    <asp:GridView ID="Ch_Data_Monthly" runat="server" BackColor="White" 
    BorderColor="#3366CC" BorderStyle="None" BorderWidth="1px" CellPadding="4">
    </asp:GridView>

    <asp:Literal ID="ltScriptsDataMonthly" runat="server"></asp:Literal>  
    <div id="Ch_BarChart_Monthly" style="width: 900px; height: 300px;" /> 
    </div>
    </form>
</body>
</html>
