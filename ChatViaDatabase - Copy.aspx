<%@ Page Title="Chat" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ChatViaDatabaseBK.aspx.cs" Inherits="Unchained.ChatViaDatabaseBK" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
   

  <asp:UpdatePanel runat="server" ID="Up7">
    <ContentTemplate>
        <%=GetChatPanel() %>
    </ContentTemplate>
  </asp:UpdatePanel>
 


</asp:Content>
