<%@ Page Title="Decentralized Videos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="VideoList.aspx.cs" Inherits="Unchained.VideoList" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    
    &nbsp; &nbsp;&nbsp; 

    <asp:Label>Search:</asp:Label>        
     <asp:TextBox ID="txtSearch" width="500px" runat="server" ></asp:TextBox>
    <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" />
    <p>



    <%=GetVideoList() %>


</asp:Content>
