<%@ Page Title="Prayer Blog" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="ListView.aspx.cs" Inherits="Unchained.ListView" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
  
    <%=GetList()%>


</asp:Content>
