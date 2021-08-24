<%@ Page Title="Prayer Blog" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="PortfolioBuilder.aspx.cs" Inherits="Unchained.PortfolioBuilder" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
  

    <h2>BiblePay Portfolio Builder</h2>



    <img src="https://foundation.biblepay.org/Images/index.png" />
    <br />

       <table>
           <tr>
               <td>DWU %:</td>
               <td>   <asp:Label ID="lblDWU" runat="server" Text="NA%"></asp:Label></td>

           </tr>
        <tr>
            <td>        <asp:Label ID="lbl1" runat="server" Text="Coin Ticker:"></asp:Label>            </td>
            <td>        <asp:dropdownlist AutoPostBack="true" runat="server" id="ddTicker"> 
                        </asp:dropdownlist>   

            </td>

        </tr>


    <tr>
        <td>        <asp:Label ID="lbl3" runat="server" Text="UTXO Receive Address:"></asp:Label></td>
        <td>        <asp:TextBox ID="txtAddress" onpaste="__doPostBack('event_pin', 'event_pin');" onchange="__doPostBack('event_pin', 'event_pin');" width="400px" runat="server"></asp:TextBox></td>
   </tr>

        <tr>
            <td>    <asp:Label ID="lbl2" runat="server" Text="Pin:"></asp:Label></td>
            <td>    <asp:TextBox ID="txtPin" readonly="true" width="100px" type="number" runat="server"></asp:TextBox></td>
        </tr>


    
        </table>


        <asp:Button ID="btnQuery" runat="server" Text="Query UTXO" OnClick="btnQueryUTXO_Click" />
        <asp:Button ID="btnSubmit" runat="server" Text="Submit" OnClick="btnSubmit_Click" />
    <br />

   <br />

         <asp:Label ID="lblInfo" runat="server" Text="Welcome to the Portfolio Builder!"></asp:Label>



</asp:Content>
