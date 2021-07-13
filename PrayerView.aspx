<%@ Page Title="Prayer View" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PrayerView.aspx.cs" Inherits="Unchained.PrayerView" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h3>Prayer - View</h3>

<%=GetPrayer() %>

</asp:Content>
