<%@ Page Title="Message Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ServiceProviderDemo.aspx.cs" Inherits="Unchained.ServiceProviderDemo" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <h3>Service Provider Demo    </h3>

    Choose Your Service Provider:
    <asp:dropdownlist runat="server" id="ddServiceProvider" style="font-family:OCR A;">     </asp:dropdownlist>
    <br />

    Start Simulation: (Warning, this simulation will cost about 100 BBP)
    <asp:Button ID="btnStart" runat="server" Text="Start Simulation" OnClick="btnStart_Click" />
      
    <ul>
        <li>This simulation shows that BiblePay Unchained can handle over 50 TPS per customer.</li>
        <li>This demonstration focuses on Microtransactions, transactions less than 100 BBP each.</li>
        <li>Our microtransactions are debited or credited to the recipients account on our sidechain.</li>
        <li>Once every so often (either 100 transactions OR when they are flushed), the transactions queued are compiled into an invoice.</li>
        <li>During the time of the invoice, the transaction is settled, meaning wallet funds transfer takes place.</li>
        <li>Note that both the customer and the service providers account balance is accurate in real time and can be queried constantly during all accounting entries.</li>
        <li>You have control over how much credit you want to give the recipient; for example:  In scenario A, a user with a 500 BBP credit limit may spend up to -499 bbp before the account is denied a new charge.  In Scenario B, assuming a 0 credit limit, a user may transact a starting credit with the service provider and use it until it reaches zero before being denied a new charge. </li>

    </ul>

 </asp:Content>
