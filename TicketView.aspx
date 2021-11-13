<%@ Page Title="Ticket View" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TicketView.aspx.cs" Inherits="Unchained.TicketView" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
   
     

    <asp:UpdatePanel runat="server" ID="Up2">
    <ContentTemplate>
        <%=GetTicket() %>
    </ContentTemplate>
  </asp:UpdatePanel>
 


</asp:Content>
