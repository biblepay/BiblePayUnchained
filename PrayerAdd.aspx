<%@ Page Title="Prayer Add" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PrayerAdd.aspx.cs" Inherits="Unchained.PrayerAdd" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h3><%=_ObjectName%> Request - Add New <%=_ObjectName%></h3>


        <asp:Label ID="lblSubject" runat="server" Text="Title - Type a short description of this item: " ></asp:Label>
        <br />
        <asp:TextBox ID="txtSubject" width="400px" runat="server"></asp:TextBox>
    <br />
    <br />

        <asp:Label ID="lblPrayerBody" runat="server" Text="Prayer Body"></asp:Label>
        <br />

       <asp:TextBox ID="txtBody" runat="server" TextMode="MultiLine"  Rows="10" class='pc90'>
        </asp:TextBox>
        <br />

        <asp:Button ID="btnSave" runat="server" Text="Save Item" OnClick="btnSave_Click" />
    

</asp:Content>
