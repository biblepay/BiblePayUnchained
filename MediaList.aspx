<%@ Page Title="Media List" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="MediaList.aspx.cs" Inherits="Unchained.MediaList"%>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <h3>Media List:</h3>

    <br /><br /><br />




    <div><%=GetMediaList()%></div>




</asp:Content>
