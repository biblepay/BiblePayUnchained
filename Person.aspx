<%@ Page Title="View Person" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="Person.aspx.cs" Inherits="Unchained.Person" %>
<%@ Register TagPrefix="BBP" Namespace="BiblePayPaginator"  Assembly="BiblePayPaginator"%>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <script>
    function openProfile(evt, Name) {
          // Declare all variables
          var i, tabcontent, tablinks;

          // Get all elements with class="tabcontent" and hide them
          tabcontent = document.getElementsByClassName("tabcontent");
          for (i = 0; i < tabcontent.length; i++) {
            tabcontent[i].style.display = "none";
          }

          // Get all elements with class="tablinks" and remove the class "active"
          tablinks = document.getElementsByClassName("tablinks");
          for (i = 0; i < tablinks.length; i++) {
            tablinks[i].className = tablinks[i].className.replace(" active", "");
          }

          // Show the current tab, and add an "active" class to the button that opened the tab
          var o = document.getElementById(Name);
          if (o) {
            o.style.display = "block";
          }
          if (evt.currentTarget) {
            evt.currentTarget.className += " active";
          }
        }
        
  </script>
  <br />
     <asp:UpdatePanel runat="server" ID="Up1">
      <ContentTemplate>
         <%=GetPerson()%>
      </ContentTemplate>
     </asp:UpdatePanel>
</asp:Content>
