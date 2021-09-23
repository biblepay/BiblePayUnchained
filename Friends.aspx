<%@ Page Title="My Friends" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="Friends.aspx.cs" Inherits="Unchained.Friends" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

   
    <%=GetFriends()%>
    

</asp:Content>
