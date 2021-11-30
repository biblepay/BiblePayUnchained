<%@ Page Title="NFT - Add" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="NFTAdd.aspx.cs" Inherits="Unchained.NFTAdd" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
  
     <asp:UpdatePanel runat="server" ID="UpdatePanel1">
     <ContentTemplate>


    <h3>BiblePay - Non Fungible Tokens</h3>&nbsp;&nbsp;  <font color="red"> <asp:Label ID="lblAction" runat="server" Text=""></asp:Label></font>

    <br />
    NFT Name:
    <br />
     <asp:TextBox ID="txtName" width="600px" runat="server"></asp:TextBox>
    <br />
    Description:
    <br />
    <asp:TextBox runat="server" width="600px" type="number" ID="txtDescription" TextMode="MultiLine" Rows="5" />
    <br />
    Buy-it-Now Amount:
    <br />
     <asp:TextBox ID="txtBuyItNowAmount" type="number" width="200px" runat="server"></asp:TextBox>
    <br />
    Reserve Amount:
    <br />
     <asp:TextBox ID="txtReserveAmount" type="number" width="200px" runat="server"></asp:TextBox>
    <br />
    
    Minimum Bid Amount:
    <br />
     <asp:TextBox ID="txtMinimumBidAmount" type="number" width="200px" runat="server"></asp:TextBox>
    <br />
    Low-Quality URL:
    <br />
     <asp:TextBox ID="txtLowQualityURL" width="600px" runat="server"></asp:TextBox>
    <br />
    
    High-Quality URL:
    <br />
     <asp:TextBox ID="txtHighQualityURL" width="600px" runat="server"></asp:TextBox>
    <br />
    Owner Address:
    <br />
     <asp:TextBox ID="txtOwnerAddress" readonly width="400px" runat="server"></asp:TextBox>
    <br />
    NFT Type:
    <br />
    <asp:dropdownlist AutoPostBack="true" runat="server" id="ddNFTType">  </asp:dropdownlist>   
    <br />
    <asp:CheckBox ID="ckMarketable" runat="server" Text="Show in Web Marketplace? (Item is for sale, or orphan is sponsorable)?" />
<br />
    <asp:CheckBox ID="ckDeleted" runat="server" Text="Delete this NFT?" />

<br />
    <asp:Button ID="btnSubmit" runat="server" Text="Submit" OnClick="btnSubmit_Click" />
    <br />

   <br />

   <asp:Label ID="lblInfo" runat="server" Text="Note:  It costs 100 BBP to create or edit an NFT."></asp:Label>

   </ContentTemplate>
</asp:UpdatePanel>
         

</asp:Content>
