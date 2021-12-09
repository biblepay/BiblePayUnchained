<%@ Page Title="Account Settings" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AccountEdit.aspx.cs" Inherits="Unchained.AccountEdit" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

   <asp:UpdatePanel runat="server" ID="UpdatePanel7">
   <ContentTemplate>


    <fieldset>
        <legend>My User Record:</legend>

        <label class="offset" for="txtFirstName">Enter your First Name: </label><asp:TextBox width="400px" ID="txtFirstName" runat="server"></asp:TextBox>

        <br />
        <label class="offset" for="txtLastName">Enter your Last Name: </label><asp:TextBox width="400px" ID="txtLastName" runat="server"></asp:TextBox>

        <br />

        <label class="offset" for="txtEmailAddress">E-mail Address: </label><asp:TextBox width="400px" ID="txtEmailAddress" runat="server"></asp:TextBox> 
        
        <asp:Button ID="btnVerifyEmail" runat="server" Text="Verify E-Mail" OnClick="btnVerifyEmail_Click"/>
       
        <asp:Label ID="lblEmailVerified" runat="server" Text="NA" style="color:red;"></asp:Label>
        <br />
        <asp:Label ID="lblTheme" runat="server" Text="">Choose Theme:<br /></asp:Label> <asp:dropdownlist runat="server" id="ddTheme"></asp:dropdownlist>   

        <asp:Label ID="lblPhoneNumber" runat="server" Text="Phone Number ** Optional" ></asp:Label><asp:TextBox width="200px" ID="txtPhoneNumber" runat="server"></asp:TextBox>                        
        
        

         <br />
         <label class="offset" for="txtPRIVLOGONINFORMATION">Password: </label><asp:TextBox width="400px" autocomplete="off" class="SENSITIVE" ID="txtPRIVLOGONINFORMATION" runat="server"></asp:TextBox> 
                   <i class="fa fa-eye" id="togglePassword1"></i>
         <br />

         <label class="offset" for="txtPRIVLOGONINFORMATIONConfirm">Confirm Password: </label><asp:TextBox width="400px" autocomplete="off" class="SENSITIVE" ID="txtPRIVLOGONINFORMATIONConfirm" runat="server"></asp:TextBox> 
                   <i class="fa fa-eye" id="togglePassword2"></i>
       
         <br />
      <br />

      <!--GDPR-->
        <!--        <asp:CheckBox ID="chkDoNotSpam" runat="server" Text="Do Not Send me Advertising E-Mails" checked="false" />-->

        <br />

    
        <asp:Button ID="btnRegister" runat="server" Text="Save" OnClick="btnRegister_Click" />

        <asp:Button ID="btnModifyProfile"  runat="server" Text="Modify my Social Media Profile" OnClick="btnModifyProfile_Click" />
        

        &nbsp;<asp:Label ID="lblStatus" runat="server" Text="" style="color:red;"></asp:Label>                    


        <asp:Button ID="btnDeleteAccount" runat="server" Text="Delete my Entire Account" OnClick="btnDeleteAccount_Click" />


        <br />
        <asp:Label ID="lblQR" runat="server" Text="">QR Code:<br /></asp:Label>                    
        <asp:Image ID="imgQrCode" runat="server" Width="220" Height="220" />
        
    </fieldset>

    <br />
<fieldset>
    <legend>Change Profile Picture:</legend>

            <%=Unchained.UICommon.GetProfileLink(this) %>
            <br />
</fieldset>

<br />

        <fieldset>
        <legend>Advanced Options for Power Users (Optional):</legend>

        <br />
        <label class="offset">Account 2FA Enabled:</label>
        <asp:TextBox ID="txtTwoFactorEnabled" readonly="true" runat="server"></asp:TextBox>
                         <small><font color="red">** Two-Factor authentication provides more security by requiring a password + two-factor pin.  </font></small>
        <br />
        <label class="offset">Domain:</label> <asp:TextBox ID="txtDomain" readonly="true" runat="server"></asp:TextBox>
        <br />
        <br />
        <label class="offset">User Roles:</label><%=UserRoles() %>
            <br />
            <br />

        <asp:Button ID="btnSetTwoFactor"  runat="server" Text="Set Up 2FA" OnClick="btnSetTwoFactor_Click" />
        <asp:Button ID="btnCheckTwoFactor" runat="server" Text="Test 2FA" OnClientClick="return true;var pin=prompt('Enter PIN >');var e={};e.Event='Validate2FA_Click';e.Value=pin;BBPPostBack2(null,e);return true;" 
            OnClick="btnValidateTwoFactor_Click" />
        <asp:Button ID="btnRemoveTwoFactor"  runat="server" Text="Remove 2FA" OnClick="btnRemoveTwoFactor_Click" />


        </fieldset>

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

            </ContentTemplate>
          </asp:UpdatePanel>

</asp:Content>
