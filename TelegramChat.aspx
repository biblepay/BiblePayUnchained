<%@ Page Title="Telegram Chat" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="TelegramChat.aspx.cs" Inherits="Unchained.TelegramChat" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <script src="https://code.jquery.com/jquery-2.2.4.min.js"></script>

        <script src="Scripts/BBPTelegramAPIJavascript/dist/telegramApi.js"></script>

    <script src="Scripts/BBPTelegramAPIJavascript/BBPTelegramIntegration.js"></script>


    
    <br />

    <%=GetTelegram()%>
    

</asp:Content>
