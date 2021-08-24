<%@ Page Title="Town Hall - List Topics" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="TownHallList.aspx.cs" Inherits="Unchained.TownHallList" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
  
    <%=GetTownHallList() %>


</asp:Content>
