<%@ Page Title="Organization - View" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="OrganizationView.aspx.cs" Inherits="Unchained.OrganizationView"   %>


<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    

    
       <asp:UpdatePanel runat="server" ID="UpdatePanel7">
   <ContentTemplate>

    
    <fieldset>
        <legend>View Organization:</legend>
        Organization:<%=Unchained.UICommon.GetDropDownOrganization(this, "ddorganization", this.Request.QueryString["id"] ?? "") %>
            <asp:Button ID="Button1" runat="server" onclick="btnFindOrganization_Click"  Text="Find" />
            <a href="OrganizationAdd.aspx">Add New Organization</a>
        <br />
        Organization Name: <asp:TextBox width="400px" readonly ID="txtName" runat="server"></asp:TextBox>
        <br />
        Public Key: <asp:TextBox width="400px" readonly ID="txtPublicKey" runat="server"></asp:TextBox>
        <br />
        <br />
    
        <asp:Button ID="btnEditOrg" runat="server" onclick="btnEditOrganization_Click"  Text="Edit" />
        
    </fieldset>

    <br />
    <br />
          </ContentTemplate>
           </asp:UpdatePanel>

</asp:Content>



