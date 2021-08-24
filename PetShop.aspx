<%@ Page Title="Message Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PetShop.aspx.cs" Inherits="Unchained.PetShop" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <h3>Pet Shop</h3>

<%=RetrievePets()%>
 </asp:Content>
