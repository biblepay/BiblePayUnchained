<%@ Page Title="Message Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Status.aspx.cs" Inherits="Unchained.Status" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <h3>Sidechain Status:</h3>

    <%=GetStatus() %>
 </asp:Content>
