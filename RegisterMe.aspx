<%@ Page Title="Account Settings" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RegisterMe.aspx.cs" Inherits="Unchained.RegisterMe" %>

<%@ Register TagPrefix="BBP" Namespace="BiblePayWallet"  Assembly="BiblePayWallet"%>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">


    <h3>Welcome to Your User Record - v<%=BiblePayDLL.Sidechain.GetVersion().ToString()%></h3>
    
    <br />
    
    <asp:Button ID="btnExplain" runat="server" Text="Explain this page / How to Get Started" OnClick="btnExplain_Click"/>
    <br />
        

    <fieldset>
        <legend>User Record:</legend>
        <label for="txtNickName">Enter your Nickname: </label>          <asp:TextBox width="400px" ID="txtNickName" runat="server"></asp:TextBox>
            <br />
        <label for="txtEmailAddress">E-mail Address: </label>     <asp:TextBox width="400px" ID="txtEmailAddress" runat="server"></asp:TextBox> <font color=red>** Optional</font>
            <br />

        <label for="txtHashTags">Hash Tags to follow:</label>    
        <asp:TextBox width="600px" ID="txtHashTags" runat="server"></asp:TextBox> 
        <br />
        <asp:Button ID="btnRegister" runat="server" Text="Save" OnClick="btnRegister_Click" />
        <asp:Button ID="btnSetAvatar" runat="server" Text="Set Avatar" OnClick="btnSetAvatar_Click" />
           &nbsp;<asp:Label ID="lblStatus" runat="server" Text="" style="color:red;"></asp:Label>                    
    </fieldset>
        
    <br />

        

<!-- new wallet -->
<!--

<BBP:WalletControl id="wallet1" runat="server"></BBP:WalletControl>
-->



<br />
     <fieldset>
        <legend>Verify Account for Full Voting Weight</legend>
         <label for="txtPhoneNumber">Enter Cell Phone Number for SMS:</label>
         <asp:TextBox width="200px" ID="txtPhoneNumber" runat="server"></asp:TextBox> 
         <asp:Button ID="btnSendCode" runat="server" Text="Send SMS Code" OnClick="btnSendSMSCode_Click"/>         
         <input type='button' id="bnexplain" onclick='showModalDialog("Explain SMS Verification","<br />We are trying to create a better version of wikipedia with integrity. Our vision is to allow users to upvote and downvote sections of articles, and call out misinformation for what it is.       <br /><br>             If users can create unlimited accounts, they can control the Rating for each section.               Conversely, if we have verified accounts, our voting will be extremely accurate!             <br /> <br>            Therefore, we aim to use the verified account flag to give verified users full voting weight. ", 575, 500);' value="Explain Reasons for SMS Verification" />
         <br />
    
         <font color=red>
             <ul><li>We will not save your phone number</li>
                 <li>If you click 'Send SMS Code', you will receive a one-time verification code to your phone number by SMS. Message and data rates may apply</li>
         </ul>
         </font>
        

         <br />
         <label for="txtPin">Enter SMS Pin to Upgrade to Verified Account:</label>
         <asp:TextBox width="200px" ID="txtPin" runat="server"></asp:TextBox> 
         <font color=red></font>
          <asp:Button ID="btnVerifySMSCode" runat="server" Text="Upgrade My Account" OnClick="btnVerifySMSCode_Click"/>
         <br />
         Profile Verified by SMS:  &nbsp;<asp:Label ID="lblVerified" runat="server" Text="NA" style="color:red;"></asp:Label>                    

    </fieldset>

<br />
    <br />
    
  
    <fieldset>
        <legend>Chain:</legend>
        <asp:Button ID="btnTestNet" runat="server" Text="TestNet" OnClick="btnTestNet_Click"/>
        <asp:Button ID="btnMainNet" runat="server" Text="MainNet" OnClick="btnMainNet_Click"/>
    </fieldset>


<!--
      <fieldset style="visibility:hidden">
        <legend>2FA Setup:</legend>
           <asp:Label ID="lblQR" runat="server" Text=""></asp:Label>                    
           <asp:Image ID="imgQrCode" runat="server" Width="220" Height="220" />
           <label>Account 2FA Enabled:</label>
           <asp:TextBox ID="txtTwoFactorEnabled" readonly="true" runat="server"></asp:TextBox>
           <label>2FA Code:</label>
           <asp:TextBox ID="txttwofactorcode" runat="server"></asp:TextBox>
           <asp:Button ID="btnSetTwoFactor"  onclientclick="alert('Once you see the QR code, click Add New Site in your authenticator app.  You may scan the code or add it manually.   ');" 
                       runat="server" Text="Set Up 2FA" OnClick="btnSetTwoFactor_Click" />
           <asp:Button ID="btnCheckTwoFactor" runat="server" Text="Test 2FA" OnClick="btnValidateTwoFactor_Click" />
    </fieldset>
-->
</asp:Content>
