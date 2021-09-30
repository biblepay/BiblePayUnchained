<%@ Page Title="Wallet" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="Wallet.aspx.cs" Inherits="Unchained.Wallet" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

   <fieldset>
        <legend>Wallet:</legend>

        <label class="offset" for="txtBiblePayAddress">BiblePay Address: </label>     <asp:TextBox width="400px" readonly="true" ID="txtBiblePayAddress" runat="server"></asp:TextBox> 
        <br />

        <label class="offset" for="txtSentToAddress">Send BBP To: </label>     <asp:TextBox width="400px" ID="txtRecipientAddress" runat="server"></asp:TextBox> 
        <br />

        <label class="offset" for="txtAmount">Amount: </label>     <asp:TextBox width="250px" type="number" ID="txtAmount" runat="server"></asp:TextBox> 

        <br />
        <asp:Button ID="btnSend"  runat="server" Text="Send BBP"   OnClick="btnSendBBP_Click" />
    </fieldset>



</asp:Content>
