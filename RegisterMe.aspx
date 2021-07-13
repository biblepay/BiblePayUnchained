<%@ Page Title="Account Settings" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RegisterMe.aspx.cs" Inherits="Unchained.RegisterMe" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">


    
    <script>

        function AddEncryptedPassword() {
            var p1 = prompt('Enter Password (Note: Once you encrypt your wallet, it cannot be used without the password, and can ONLY be re-created if you have the seed words.  This password is NOT stored on your device, so please remember it): ', '');
            var o1 = document.getElementById('passforencryption');
            o1.value = p1;
        }

        function AddDecryptionPassword() {
            var p1 = prompt('Enter Decryption Password:', '');
            var o1 = document.getElementById('passforencryption');
            o1.value = p1;
        }

        function SetStorage() {
            localStorage.setItem('a', 'Tom');
        }

        


    </script>


    <h3>My User Record - Maintenance - v<%=BiblePayDLL.Sidechain.GetVersion().ToString()%>.<%=GetSubVersion()%></h3>
    <br />
    Welcome to your BiblePay Wallet

    <br />
    
    <br />
    
    This page allows you to manage your wallet settings.  
   
    Your wallet is stored on your machine, encrypted.
    
    <br />

    <fieldset>
          <legend>User Record:</legend>
          <label for="txtNickName">Enter your Nickname: </label>
          <asp:TextBox width="400px" ID="txtNickName" runat="server"></asp:TextBox>
        <br />
       <asp:Button ID="btnRegister" runat="server" Text="Save" OnClick="btnRegister_Click" />
    </fieldset>
    
        
    <br />

    <fieldset>
        <legend>Wallet Settings:</legend>
        <label>My BiblePay Address:</label>
           <asp:TextBox ID="txtAddress" width=405px readonly="true" runat="server"></asp:TextBox>
        <br />
        <asp:Button ID="btnGenerate" runat="server" Text="Generate New Address" OnClientClick="showSpinner();" OnClick="btnGenerateNewAddress_Click"/>
        <asp:Button ID="btnRecover" runat="server" Text="Recover Wallet from Seed Words" OnClientClick="" OnClick="btnRecoverFromSeedWords_Click"/>
        <asp:Button ID="btnEncrypt" runat="server" Text="Encrypt Wallet" OnClientClick="AddEncryptedPassword();" OnClick="btnEncryptWallet_Click"/>

        <!--         <asp:Button ID="btndestroy" runat="server" Text="Destroy Wallet" OnClientClick="" OnClick="btnDestroyWallet_Click"/> -->

         &nbsp;<asp:Label ID="lblEncrypted" runat="server" Text="NA" style="color:red;"></asp:Label>                    
         
    </fieldset>

<br />
    
  <fieldset>
        <legend>Send Money:</legend>
        <label>Send to Address:</label>

           <asp:TextBox ID="txtDestination" width=305px runat="server"></asp:TextBox>
      <br />
        <label>Amount to Send:</label>

           <asp:TextBox ID="txtSpendAmount" width=105px runat="server"></asp:TextBox>

        <br />
        <asp:Button ID="btnSendMoney" runat="server" Text="Send BBP to this Address" OnClick="btnSendMoney_Click"/>
    </fieldset>
    

    <br />
  
    <fieldset>
        <legend>Chain:</legend>
        <asp:Button ID="btnTestNet" runat="server" Text="TestNet" OnClick="btnTestNet_Click"/>
        <asp:Button ID="btnMainNet" runat="server" Text="MainNet" OnClick="btnMainNet_Click"/>
        <!--         <asp:Button ID="Button11" runat="server" Text="Test Storage" OnClientClick="SetStorage();GetStorage();" OnClick="btnTest_Click"/> -->


    </fieldset>
  

      <fieldset style="visibility:hidden">
        <legend>QR Setup:</legend>
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
  

</asp:Content>
