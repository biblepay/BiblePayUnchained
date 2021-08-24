<%@ Page Title="Bible Viewer" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="BibleViewer.aspx.cs" Inherits="Unchained.BibleViewer" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

<h2>Bible Viewer - English - KJV</h2>


    <br />

    <div id="bibleViewport" style="min-height:500px; max-height:500px;overflow-y:auto;">

        <table id="bibleviewport" style="">
            <tr><td>
                    <%=GetContent() %>
                </td></tr>
            </table>
    </div>
    <br />

    <table>
           <tr><td>        <asp:Label ID="lblBook" runat="server" Text="Book:"></asp:Label>   
               <td>        <asp:dropdownlist runat="server" AutoPostBack="true" id="ddBook"> 
                           </asp:dropdownlist>   </tr>
           <tr><td>        <asp:Label ID="lblChapter" runat="server" Text="Chapter:"></asp:Label>   
               <td>        <asp:dropdownlist AutoPostBack="true" runat="server" id="ddChapter"> 
                           </asp:dropdownlist>   </tr>
     </table>
<br />

</asp:Content>
