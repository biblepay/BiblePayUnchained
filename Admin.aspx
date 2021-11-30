<%@ Page Title="Admin" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Admin.aspx.cs" Inherits="Unchained.Admin"   %>


<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">


      <asp:UpdatePanel runat="server" ID="UpdatePanel61">
        <ContentTemplate>


    <h3>Organization: <asp:Label ID="lblOrganizationName" runat="server" style="" Text="???"></asp:Label></h3>


    

    <br />
    <br />

    <fieldset>
        <legend>List User Roles:</legend>
        User: <%=Unchained.UICommon.GetDropDownUser(this, "ddusers_list",Request.Form["ddusers_list"] ?? "", "", false) %>
        <br />

        <hr />

            <div>
                 <asp:Label ID="lblRoles" runat="server" style="" Text=""></asp:Label>
            </div>



        <asp:Button ID="btnRetrieveRoles" runat="server" onclick="btnListUserRoles_Click"  Text="List User Roles" />
    </fieldset>

            <br />
            <br />


    <fieldset>
        <legend>Add User Role:</legend>
        User: <%=Unchained.UICommon.GetDropDownUser(this, "ddusers", "", "", false) %>
        <br />
        Role: <%=Unchained.UICommon.GetDropDownRole(this, "ddroles", "") %>
        <br />
        <asp:Button ID="btnSaveUserRole" runat="server" onclick="btnSaveUserRole_Click"  Text="Save User Role" />
    </fieldset>

    <br />
    <br />
    <fieldset>
        <legend>Add Role Permission:</legend>
        Organization: <%=Unchained.UICommon.GetDropDownOrganization(this, "ddroleorganization", "") %>
        <br />
        Role: <%=Unchained.UICommon.GetDropDownRole(this, "ddpermissionroles", "") %>
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
            </ContentTemplate>
          </asp:UpdatePanel>
</asp:Content>



