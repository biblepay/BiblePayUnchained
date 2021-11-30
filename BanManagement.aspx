<%@ Page Title="Ban Management" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="BanManagement.aspx.cs" Inherits="Unchained.BanManagement" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <h5>Content Flagged as Inappropriate (Within the last 180 days) that is not Reviewed:</h5>
    <small>
 
    </small>

     <asp:UpdatePanel runat="server" ID="UpdatePanel1">
     <ContentTemplate>


        <%=GetBanReport()%>
     </ContentTemplate>
     </asp:UpdatePanel>
         


</asp:Content>
