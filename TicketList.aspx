<%@ Page Title="Ticket List" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="TicketList.aspx.cs" Inherits="Unchained.TicketList" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
  
    <%=GetTicketList() %>


</asp:Content>
