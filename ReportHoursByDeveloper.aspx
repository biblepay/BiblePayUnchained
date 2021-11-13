<%@ Page Title="Hours By Developer" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ReportHoursByDeveloper.aspx.cs" Inherits="Unchained.ReportHoursByDeveloper" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <h3>Hours by Developer:</h3>
    <br />

    Start Date:
    <asp:TextBox width="200px" type="date" id="txtStartDate" runat="server"></asp:TextBox> 

    <br />

    End Date:
    <asp:TextBox width="200px" type="date" id="txtEndDate" runat="server"></asp:TextBox> 

    <br />
    Developer:
    <%=Unchained.UICommon.GetDropDownUser(this, "ddDevelopers", "", Unchained.Common.gUser(this).id, true) %>
    <br />
    <br />

    <asp:Button ID="btnRun" runat="server" Text="Run Report" OnClick="Run_Click" />
   
 </asp:Content>
