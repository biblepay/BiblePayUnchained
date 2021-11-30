<%@ Page Title="Register New User" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RegisterNewUser.aspx.cs" Inherits="Unchained.RegisterNewUser" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

   <asp:UpdatePanel runat="server" ID="UpdatePanel7">
   <ContentTemplate>
 

    <fieldset>
        <legend>Register as New User:</legend>

        <label class="offset" for="txtFirstName">Enter your First Name: </label><asp:TextBox width="400px" ID="txtFirstName" runat="server"></asp:TextBox>

        <br />
        <label class="offset" for="txtLastName">Enter your Last Name: </label><asp:TextBox width="400px" ID="txtLastName" runat="server"></asp:TextBox>

        <br />

        <label class="offset" for="txtEmailAddress">E-mail Address: </label><asp:TextBox width="400px" ID="txtEmailAddress" runat="server"></asp:TextBox> 
        
        <br />

         <br />
         <label class="offset" for="txtPRIVLOGONINFORMATION">Password: </label><asp:TextBox width="400px" autocomplete="off" class="SENSITIVE" ID="txtPRIVLOGONINFORMATION" runat="server"></asp:TextBox> 
                   <i class="fa fa-eye" id="togglePassword1"></i>
         <br />

         <label class="offset" for="txtPRIVLOGONINFORMATIONConfirm">Confirm Password: </label><asp:TextBox width="400px" autocomplete="off" class="SENSITIVE" ID="txtPRIVLOGONINFORMATIONConfirm" runat="server"></asp:TextBox> 
                   <i class="fa fa-eye" id="togglePassword2"></i>
       
         <br />
      <br />

      <!--GDPR-->

        <asp:CheckBox ID="chkGDPRVerified" runat="server" Text="..." checked="false" />
        <br />

        <asp:Button ID="btnRegister" runat="server" Text="Save" OnClick="btnRegister_Click" />


        <br />
        
    </fieldset>

    <br />

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

    </ContentTemplate>
   </asp:UpdatePanel>

</asp:Content>
