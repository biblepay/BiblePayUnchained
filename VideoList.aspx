<%@ Page Title="Decentralized Videos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="VideoList.aspx.cs" Inherits="Unchained.VideoList" %>
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

 <asp:UpdatePanel runat="server" ID="Up1">
    <ContentTemplate>
            <div style="z-index:1;top:-50px;">&nbsp;&nbsp;&nbsp;
        <small>
        <asp:TextBox ID="txtSearch" width="500px" runat="server" onkeydown = "search(this);return (event.keyCode!=13);"></asp:TextBox>
        <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" /></small>
    </div>

         <%=GetVideoList() %>
         <BBP:Paginator id="paginator2" runat="server"></BBP:Paginator>
    </ContentTemplate>
  </asp:UpdatePanel>
    
         
</asp:Content>
    