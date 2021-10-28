<%@ Page Title="Reports" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Reports.aspx.cs" Inherits="Unchained.Reports" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <h3>Reports:</h3>
    <hr />

    <asp:LinkButton ID="linkHours" runat="server" Text="Hours by Developer" OnClick="HoursByDeveloper_Click"></asp:LinkButton>


 </asp:Content>
