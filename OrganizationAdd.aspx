<%@ Page Title="Add Organization" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="OrganizationAdd.aspx.cs" Inherits="Unchained.OrganizationAdd"   %>


<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    

    <fieldset>
        <legend>Add Organization:</legend>

        Organization Name: <asp:TextBox width="400px" ID="txtOrganizationName" runat="server"></asp:TextBox>
        <br />
        Domain Name: <asp:TextBox width="400px" ID="txtDomainName" runat="server"></asp:TextBox>
        <br />
        BiblePay Public Key: <asp:TextBox width="400px" ID="txtBiblePayAddress" runat="server"></asp:TextBox>
        
        <br />

        <asp:Button ID="btnSaveOrganization" runat="server" onclick="btnSaveOrganization_Click"  Text="Save Organization" />
        
    </fieldset>

    
</asp:Content>



