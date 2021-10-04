<%@ Page Title="View Person" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="People.aspx.cs" Inherits="Unchained.People" %>
<%@ Register TagPrefix="BBP" Namespace="BiblePayPaginator"  Assembly="BiblePayPaginator"%>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
          <script>
              function search(ele) {
                  if (event.key === 'Enter') {
                      document.getElementById('<%= btnSearch.ClientID %>').click();
                      ele.value = "";
                  }
              }
        </script>

    
    <div style="z-index:1;top:-50px;">
        <small><asp:TextBox ID="txtSearch" width="500px" runat="server" onkeydown = "search(this);return (event.keyCode!=13);"></asp:TextBox>
        <asp:Button ID="btnSearch" runat="server" Text="Search" UseSubmitBehavior="false" OnClick="btnSearch_Click" /></small>
    </div>
    
    
    <%=GetPeople()%>

    <BBP:Paginator id="paginator1" runat="server"></BBP:Paginator>


</asp:Content>
