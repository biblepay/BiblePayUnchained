<%@ Page Title="NFT List" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="NFTList.aspx.cs" Inherits="Unchained.NFTList" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <h2>My NFTs:</h2>

  <br />

    <%=GetNFTList()%>




</asp:Content>
