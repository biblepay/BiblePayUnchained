<%@ Page Title="Portfolio Builder" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="PortfolioBuilder.aspx.cs" Inherits="Unchained.PortfolioBuilder" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
  

     <asp:UpdatePanel runat="server" ID="UpdatePanel1">
     <ContentTemplate>

    <script>
        function pb() {
            var e = {}; e.Event = 'event_pin';BBPPostBack2(this, e);
        }
    </script>
    <h2>BiblePay Portfolio Builder</h2>



    <img src="https://foundation.biblepay.org/Images/index.png" />
    <br />

    <asp:Label ID="lblDWU" runat="server" Text="NA%" ToolTip="(Dynamic-Whale-Unit): The estimated annual Return on your staked BiblePay."></asp:Label>
    <br />

    <br />
    <asp:Label ID="lbl1" runat="server" Text="Coin Ticker:"></asp:Label>            
    <br />
    <asp:dropdownlist AutoPostBack="true" runat="server" id="ddTicker"> </asp:dropdownlist>   
    <br />
    <asp:Label ID="lbl3" runat="server" Text="UTXO Receive Address:"></asp:Label>
    <br />
    <asp:TextBox ID="txtAddress" onpaste="setTimeout(pb,1000);" onchange="var e={};e.Event='event_pin';BBPPostBack2(this, e);" width="400px" runat="server"></asp:TextBox>
    <br />
    <asp:Label ID="lbl2" runat="server" Text="Pin:"></asp:Label>
    <br />
    <asp:TextBox ID="txtPin" readonly="true" width="100px" type="number" runat="server"></asp:TextBox>
    <br />
    <br />

        <asp:Button ID="btnQuery" runat="server" Text="Query UTXO" OnClick="btnQueryUTXO_Click" />
        <asp:Button ID="btnSubmit" runat="server" Text="Submit" OnClick="btnSubmit_Click" />
    <br />

    <br />
    
         <asp:Label ID="lblInfo" runat="server"  Text="Welcome to the Portfolio Builder!"></asp:Label>

         </ContentTemplate>

</asp:UpdatePanel>
         

</asp:Content>
