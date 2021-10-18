<%@ Page Title="Prayer View" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PrayerView.aspx.cs" Inherits="Unchained.PrayerView" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
   

    <!--     <asp:UpdatePanel runat="server" ID="Up2"> </asp:UpdatePanel> -->

    <ContentTemplate>
        <%=GetPrayer() %>
    </ContentTemplate>
 


</asp:Content>
