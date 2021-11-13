<%@ Page Title="Report - Generic" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ReportGeneric.aspx.cs" Inherits="Unchained.ReportGeneric" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <h3>Report - Generic:</h3>
    <br />

    Start Date:
    <asp:TextBox width="200px" type="date" id="txtStartDate" runat="server"></asp:TextBox> 

    <br />
    End Date:
    <asp:TextBox width="200px" type="date" id="txtEndDate" runat="server"></asp:TextBox> 

    <br />
    Table:
    <%=Unchained.UICommon.GetDropDownTable(this, "ddTables", (Session["last_ddTable"] ?? "").ToString()) %>
    <br />

    Select Clause:
    <asp:TextBox width="700px" TextMode="MultiLine"  Rows="3"  ID="txtSelect" runat="server"></asp:TextBox> 

    <br />

    Where Clause:
    <asp:TextBox width="700px" TextMode="MultiLine"  Rows="9"  ID="txtWhere" runat="server"></asp:TextBox> 
    <br />

    Order By Clause:
    <asp:TextBox width="700px" TextMode="MultiLine"  Rows="3"  ID="txtOrderBy" runat="server"></asp:TextBox> 

    <br />

    <asp:Button ID="btnRun" runat="server" Text="Run Report" OnClick="Run_Click" />

    <hr />
     <%=GetReport() %>

 </asp:Content>
