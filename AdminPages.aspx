<%@ Page Title="Admin Pages" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AdminPages.aspx.cs" Inherits="Unchained.AdminPages"  ValidateRequest="false" %>


<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h3>Admin Pages</h3>

    <br />


    <a href="ListView.aspx?objecttype=Organization">List Organizations</a>
    <br />

    <a href="OrganizationAdd">Add Organization</a>


</asp:Content>



