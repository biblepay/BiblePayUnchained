<%@ Page Title="View Person" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="People.aspx.cs" Inherits="Unchained.People" %>
<%@ Register TagPrefix="BBP" Namespace="BiblePayPaginator"  Assembly="BiblePayPaginator"%>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <script>
  </script>

    
    <div style="z-index:1;top:-50px;">&nbsp;&nbsp;&nbsp;
        <small><asp:TextBox ID="txtSearch" width="500px" runat="server" ></asp:TextBox>
        <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" /></small>
    </div>
    
    
    <%=GetPeople()%>
    <BBP:Paginator id="paginator1" runat="server"></BBP:Paginator>


</asp:Content>
