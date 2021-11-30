<%@ Page Title="Roles - Add" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RoleAdd.aspx.cs" Inherits="Unchained.RoleAdd"   %>


<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h3>Admin</h3>



    <fieldset>
        <legend>Add Role:</legend>
        Organization:<%=Unchained.UICommon.GetDropDownOrganization(this, "ddorganization", this.Request.QueryString["Organization"] ?? "") %>
        <br />
        Name: <asp:TextBox width="400px" ID="txtRoleName" runat="server"></asp:TextBox>
        <br />
        <asp:Button ID="btnSaveRole" runat="server" onclick="btnSaveRole_Click"  Text="Save Role" />
        
    </fieldset>

    <br />
    <br />


</asp:Content>



