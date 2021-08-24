<%@ Page Title="Prayer Blog" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="Media.aspx.cs" Inherits="Unchained.Media" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
 
    
  <asp:UpdatePanel runat="server" ID="Up3">
    <ContentTemplate>
      <%=GetVideo() %>
    </ContentTemplate>
  </asp:UpdatePanel>
 
</asp:Content>
