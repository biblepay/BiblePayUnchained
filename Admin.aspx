<%@ Page Title="Admin" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Admin.aspx.cs" Inherits="Unchained.Admin"  ValidateRequest="false" %>


<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h3>Admin</h3>


    <fieldset>
        <legend>Add Organization:</legend>

        Organization Name: <asp:TextBox width="400px" ID="txtOrganizationName" runat="server"></asp:TextBox>
        <br />
        Domain Name: <asp:TextBox width="400px" ID="txtDomainName" runat="server"></asp:TextBox>
        <br />

        <asp:Button ID="btnSaveOrganization" runat="server" onclick="btnSaveOrganization_Click"  Text="Save Organization" />
        
    </fieldset>

    <br />
    <br />

    <fieldset>
        <legend>Add Role:</legend>
        Organization:<%=Unchained.UICommon.GetDropDownOrganization(this, "ddorganization", "") %>
        <br />
        Name: <asp:TextBox width="400px" ID="txtRoleName" runat="server"></asp:TextBox>
        <br />
        <asp:Button ID="btnSaveRole" runat="server" onclick="btnSaveRole_Click"  Text="Save Role" />
        
    </fieldset>

    <br />
    <br />

    <fieldset>
        <legend>Add User Role:</legend>
        User:<%=Unchained.UICommon.GetDropDownUser(this, "ddusers", "", "", false) %>
        <br />
        Role:<%=Unchained.UICommon.GetDropDownRole(this, "ddroles", "") %>
        <br />
        <asp:Button ID="btnSaveUserRole" runat="server" onclick="btnSaveUserRole_Click"  Text="Save User Role" />
    </fieldset>

    <br />
    <br />
    <fieldset>
        <legend>Add Role Permission:</legend>
        Organization:<%=Unchained.UICommon.GetDropDownOrganization(this, "ddroleorganization", "") %>
        <br />
        Role:<%=Unchained.UICommon.GetDropDownRole(this, "ddpermissionroles", "") %>
        <br />
        Entity Name: <asp:TextBox width="400px" ID="txtEntityName" runat="server"></asp:TextBox>
        <br />
        Permissions: 
        <br />
        <asp:CheckBox ID="chkRead" runat="server" Text="Read" checked="true" />
        <asp:CheckBox ID="chkAdd" runat="server" Text="Add" checked="true" />
        <asp:CheckBox ID="chkUpdate" runat="server" Text="Update" checked="true" />
        <asp:CheckBox ID="chkDelete" runat="server" Text="Delete" checked="true" />
        <br />
        <asp:Button ID="btnSavePerm" runat="server" onclick="btnSavePermission_Click"  Text="Save Permission" />
    </fieldset>

</asp:Content>



