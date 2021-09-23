<%@ Page Title="Sponsor Orphans" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="NFTOrphans.aspx.cs" Inherits="Unchained.NFTOrphans" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h3> Orphan Sponsorships - NFT Technology </h3>
    <hr />

    <div style="font-family:Arial;">
        <%=Unchained.NFTBrowse.GetNFTDisplayList(this, true, false, false) %>
    </div>
</asp:Content>
