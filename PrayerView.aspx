<%@ Page Title="Prayer View" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PrayerView.aspx.cs" Inherits="Unchained.PrayerView" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
   

  <asp:UpdatePanel runat="server" ID="Up7">
    <ContentTemplate>
        <%=GetPrayer() %>
    </ContentTemplate>
  </asp:UpdatePanel>
 


</asp:Content>
