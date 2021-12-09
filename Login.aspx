<%@ Page Title="Login" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="Login.aspx.cs" Inherits="Unchained.Login" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
   <style>
       #MainContent_lblEye{
           margin-left: 0;
position: absolute;
top: 33px;
right: 5px;
       }
             @media screen and (max-width: 480px) 
       {
  .ui-dialog{
      
    width: 94% !important;
    top: 170.008px !important;
    left: 3% !important;
  }
}
   </style>
<br />
<br />
<br />
<br />
<br />
<br />
<br />

     <fieldset style="width:100%; max-width:450px; margin:auto; float:none;">

         <asp:Panel ID="p" runat="server" DefaultButton="btnLogin" CssClass="p-3">

        <legend><asp:Label ID="lblFieldset" runat="server">Log In:</asp:Label><small><asp:Label ID="lblInfo" runat="server" Text=""></asp:Label></small></legend>
           
        <asp:Label ID="lblEmailAddress" Width="110px" runat="server" CssClass="mb-1" Text="">E-Mail Address:</asp:Label><asp:TextBox ID="txtEmailAddress" style="max-width:400px;width:100%;margin-left:5px;" runat="server"></asp:TextBox>
        <br />
             <div class="position-relative">
         <asp:Label ID="lblPassword" Width="110px" runat="server" CssClass="mb-1" Text="">Password:</asp:Label><asp:TextBox ID="txtPRIVLOGONINFORMATION" autocomplete="off" class="SENSITIVE" style="max-width:400px;width:100%;margin-left:5px;" runat="server"></asp:TextBox>
         <asp:Label ID="lblEye" runat="server"><i class="fa fa-eye" id="togglePassword"></i></asp:Label>
                 </div>
       <br />

       <asp:Label ID="lbl2FA" Width="110px" runat="server" CssClass="mb-1" Text="">2FA Code:</asp:Label><asp:TextBox ID="txt2FACode" autocomplete="off" style="max-width:400px;width:100%;margin-left:5px;" runat="server"></asp:TextBox>
     <br /> <asp:Label ID="lblOptional" runat="server"><font color="red"><small style="font-size:12px;">** Optional (enter if you enabled 2FA)</small></font></asp:Label>

        <br />
        <br />
        <asp:Button ID="btnLogin" runat="server" Text="Log In" OnClick="btnLogin_Click"/>    
        <asp:Button ID="btnLogout" runat="server" Text="Log Out" OnClick="btnLogout_Click"/>    
        <asp:Button ID="btnRegister" runat="server" Text="Register" OnClick="btnRegister_Click"/>    
        <asp:Button ID="btnLockedOut" runat="server" Text="I'm Locked Out" OnClick="btnLockedOut_Click"/>    

        </asp:Panel>
     </fieldset>
     <script>
         const togglePassword = document.getElementById('togglePassword');
         const password = document.querySelector("#MainContent_txtPRIVLOGONINFORMATION");
         togglePassword.addEventListener("click", function () {
             // toggle the icon
             password.classList.toggle("SENSITIVE");
             this.classList.toggle("fa-eye-slash");
             this.classList.toggle("fa-eye");
         });
    </script>
</asp:Content>
