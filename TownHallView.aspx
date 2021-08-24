<%@ Page Title="Topic - View Thread" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TownHallView.aspx.cs" Inherits="Unchained.TownHallView" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
   
<%=GetTownHallThread() %>

</asp:Content>
