<%@ Page Title="Decentralized Videos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="VideoList.aspx.cs" Inherits="Unchained.VideoList" %>
<%@ Register TagPrefix="BBP" Namespace="BiblePayPaginator"  Assembly="BiblePayPaginator"%>

 
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    

 <asp:UpdatePanel runat="server" ID="Up1">
    <ContentTemplate>
            <div style="z-index:1;top:-50px;">&nbsp;&nbsp;&nbsp;
        <small>
        <asp:TextBox ID="txtSearch" width="500px" runat="server" ></asp:TextBox>
        <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" /></small>
    </div>

         <%=GetVideoList() %>
         <BBP:Paginator id="paginator1" runat="server"></BBP:Paginator>
    </ContentTemplate>
  </asp:UpdatePanel>
    
         
</asp:Content>
    