<%@ Page Title="Decentralized Videos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="VideoList.aspx.cs" Inherits="Unchained.VideoList" %>

 
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

       <asp:UpdatePanel runat="server" ID="UpdatePanel4">
       <ContentTemplate>

   
        <script>
              function search(ele) {
                  if (event.key === 'Enter') {
                      document.getElementById('<%= btnSearch.ClientID %>').click();
                      ele.value = "";
                  }
              }
        </script>
        

     <div style="top:-20px;">
        <small>
        <asp:TextBox ID="txtSearch" width="300px" runat="server" onkeydown = "search(this);return (event.keyCode!=13);"></asp:TextBox>
        <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" /></small>
    </div>
    
    
     <%=GetVideoList() %>
     </ContentTemplate>
     </asp:UpdatePanel>

    
         
</asp:Content>
    