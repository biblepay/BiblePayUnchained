<%@ Page Title="Account Settings" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RegisterMe.aspx.cs" Inherits="Unchained.RegisterMe" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">


    <h3>Welcome to Your User Record - v<%=BiblePayDLL.Sidechain.GetVersion().ToString()%></h3>
    
    <br />
        

    <fieldset>
        <legend>User Record:</legend>

        <label class="offset" for="txtFirstName">Enter your First Name: </label><asp:TextBox width="400px" ID="txtFirstName" runat="server"></asp:TextBox>

        <br />
        <label class="offset" for="txtLastName">Enter your Last Name: </label><asp:TextBox width="400px" ID="txtLastName" runat="server"></asp:TextBox>

        <br />

        <label class="offset" for="txtEmailAddress">E-mail Address: </label><asp:TextBox width="400px" ID="txtEmailAddress" runat="server"></asp:TextBox> 
        
               <asp:Button ID="btnVerifyEmail" runat="server" Text="Verify E-Mail" OnClick="btnVerifyEmail_Click"/>
       
               <asp:Label ID="lblEmailVerified" runat="server" Text="NA" style="color:red;"></asp:Label>
        <br />
        <label class="offset" for="ddTheme">Choose Theme: </label><asp:dropdownlist runat="server" id="ddTheme"></asp:dropdownlist>   

        <br />
        <label class="offset">Account 2FA Enabled:</label><asp:TextBox ID="txtTwoFactorEnabled" readonly="true" runat="server"></asp:TextBox>
        <br />


        <label class="offset" for="txtPhoneNumber">Cell Phone Number:</label><asp:TextBox width="200px" ID="txtPhoneNumber" runat="server"></asp:TextBox>                        
        
                 <small><font color="red">** Optional</font></small>
        
        <!--
                 <asp:Button ID="btnSendCode" runat="server" Text="Send SMS Code" OnClick="btnSendSMSCode_Click"/>         
                 <input type='button' id="bnexplain" 
                     onclick='showModalDialog("Explain SMS Verification", "<br />Verified users receive higher voting weight.  <br>Message and Data Rates may apply. ", 500, 300);'
                        value="Explain" /> 
                 <asp:Button ID="btnVerifySMSCode" runat="server" Text="Verify Pin" OnClick="btnVerifySMSCode_Click"/>
                             &nbsp;<asp:Label ID="lblVerified" runat="server" Text="NA" style="color:red;"></asp:Label>                    

        -->

         <br />
         <label class="offset" for="txtPRIVLOGONINFORMATION">Password: </label><asp:TextBox width="400px" autocomplete="off" class="SENSITIVE" ID="txtPRIVLOGONINFORMATION" runat="server"></asp:TextBox> 
                   <i class="fa fa-eye" id="togglePassword1"></i>
        <br />

         <label class="offset" for="txtPRIVLOGONINFORMATIONConfirm">Confirm Password: </label><asp:TextBox width="400px" autocomplete="off" class="SENSITIVE" ID="txtPRIVLOGONINFORMATIONConfirm" runat="server"></asp:TextBox> 
                   <i class="fa fa-eye" id="togglePassword2"></i>
       
         <br />
      <br />
    
        <asp:Button ID="btnRegister" runat="server" Text="Save" OnClick="btnRegister_Click" />
        <asp:Button ID="btnSetAvatar" runat="server" Text="Set Avatar" OnClick="btnSetAvatar_Click" />

        <asp:Button ID="btnSetTwoFactor"  runat="server" Text="Set Up 2FA" OnClick="btnSetTwoFactor_Click" />
        <asp:Button ID="btnCheckTwoFactor" runat="server" Text="Test 2FA" OnClientClick="return true;var pin=prompt('Enter PIN >');var e={};e.Event='Validate2FA_Click';e.Value=pin;BBPPostBack2(null,e);return true;" 
            OnClick="btnValidateTwoFactor_Click" />
        <asp:Button ID="btnRemoveTwoFactor"  runat="server" Text="Remove 2FA" OnClick="btnRemoveTwoFactor_Click" />
        &nbsp;<asp:Label ID="lblStatus" runat="server" Text="" style="color:red;"></asp:Label>                    

        <br />
        <asp:Label ID="lblQR" runat="server" Text="">QR Code:<br /></asp:Label>                    
        <asp:Image ID="imgQrCode" runat="server" Width="220" Height="220" />
        
    </fieldset>

    <br />

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

    <br />


    <fieldset>
         <legend>Your Public Social Media Page: <small>&nbsp;&nbsp;<font color=red>** This section is optional</small></font></legend>

        <label class="offset" for="ddGender">My Gender: </label><asp:dropdownlist runat="server" id="ddGender"></asp:dropdownlist>   
        <br />


        <label class="offset" for="txtBirthDate">My Birth Date: </label><asp:TextBox width="200px" id="txtBirthDate" runat="server"></asp:TextBox> 

        <br />
        <label class="offsetv" for="txtPublicText">About Me (Public): </label><asp:TextBox width="700px" TextMode="MultiLine"  Rows="5" 
            ID="txtPublicText" runat="server"></asp:TextBox> 

        <br />
        <label class="offsetv" for="txtPrivateText">About Me (Private/Friends Only): </label><asp:TextBox width="700px" TextMode="MultiLine"  Rows="5" 
            ID="txtPrivateText" runat="server"></asp:TextBox> 

        <br />
        <label class="offsetv" for="txtProfessionalText">About Me (Professional): </label><asp:TextBox width="700px" TextMode="MultiLine"  Rows="5" 
            ID="txtProfessionalText" runat="server"></asp:TextBox> 

        <br />
        <label class="offsetv" for="txtReligiousText">About Me (Beliefs): </label><asp:TextBox width="700px" TextMode="MultiLine"  Rows="5" 
            ID="txtReligiousText" runat="server"></asp:TextBox> 
        <br />
        <asp:Button ID="btnUpdateSocial" runat="server" Text="Update Social Media Profile" OnClick="btnUpdateSocialMediaProfile_Click"/>         
        
    </fieldset>
<br />

    <fieldset>
        <legend>Your Telegram Fields:</legend>

        <label class="offset" for="txtTLN">Telegram Name:</label><asp:TextBox width="700px" id="txtTelegramLinkName" runat="server"></asp:TextBox> 
        <br />
        <label class="offset" for="txtTLU">Telegram URL:</label><asp:TextBox width="700px" id="txtTelegramURL" runat="server"></asp:TextBox> 
        <br />
        <label class="offset" for="txtTLD">Telegram Description:</label><asp:TextBox width="700px" id="txtTelegramDescription" TextMode="MultiLine" Rows="5" runat="server"></asp:TextBox> 
        <br />
        <asp:Button ID="btnUpdateTelegramFields" runat="server" Text="Update Telegram Profile" OnClick="btnUpdateTelegramProfile_Click"/>         
        
    </fieldset>
    <br />


    <script>
        const togglePassword1 = document.getElementById('togglePassword1');
        const togglePassword2 = document.getElementById('togglePassword2');
        const password1 = document.querySelector("#MainContent_txtPRIVLOGONINFORMATION");
        const password2 = document.querySelector("#MainContent_txtPRIVLOGONINFORMATIONConfirm");
        togglePassword1.addEventListener("click", function () {
                 password1.classList.toggle("SENSITIVE");
                 this.classList.toggle("fa-eye-slash");
                 this.classList.toggle("fa-eye");
        });
        togglePassword2.addEventListener("click", function () {
            password2.classList.toggle("SENSITIVE");
            this.classList.toggle("fa-eye-slash");
            this.classList.toggle("fa-eye");
        });

    </script>


</asp:Content>
