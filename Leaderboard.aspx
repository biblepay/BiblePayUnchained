<%@ Page Title="Portfolio Builder Leaderboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="Leaderboard.aspx.cs" Inherits="Unchained.Leaderboard" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Portfolio Builder Leaderboard:</h2>
     <asp:Button ID="btnSummary" runat="server" onclick="btnSummary_Click"  Text="Summary" />
     <asp:Button ID="btnDetail" runat="server" onclick="btnDetail_Click"  Text="Details" />
     <asp:Label ID="lblMode" runat="server" style="color:red;" Text=""></asp:Label></td>

  <br />

    <%=GetLeaderboard()%>




</asp:Content>
