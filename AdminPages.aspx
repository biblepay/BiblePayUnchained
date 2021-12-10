<%@ Page Title="Admin Pages" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AdminPages.aspx.cs" Inherits="Unchained.AdminPages"  ValidateRequest="false" %>


<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h3>Admin Pages</h3>
    <br />

    <a href="RoleAdd">Add Role</a>
    <br />

    <a href="ListView.aspx?objecttype=Organization">Organization Actions</a>
    <br />

    <a href="OrganizationAdd">Add Organization</a>
    <br />

    <a href="OrganizationView">View Organization</a>
    <br />
    
    <a href="BanManagement">Ban Management</a>
    <br />

    <a href="NewsFeedItemList">News Feed Management</a>
    <br />


    <br />
    <br />
    <br />
    <br />
    <br />

    <a href="FreedomViewer?pdfsource=<%=BiblePayCommon.Encryption.Base64Encode("https://unchained.biblepay.org/Content/UnchainedRoleAndPermissionSystem.pdf") %>">Help Guide</a>
    <br />


</asp:Content>



