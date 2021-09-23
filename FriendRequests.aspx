<%@ Page Title="Friend Requests" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="FriendRequests.aspx.cs" Inherits="Unchained.FriendRequests" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

   
    <%=GetFriendRequests()%>
    

</asp:Content>
