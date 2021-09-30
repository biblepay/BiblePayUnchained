<%@ Page Title="Ticket - Add" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TicketAdd.aspx.cs" Inherits="Unchained.TicketAdd" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h3>Add New Ticket <small>v1.1</small></h3>

        <asp:Label ID="lblSubject" runat="server" Text="Title - Type a short summary of this issue or request: " ></asp:Label>
        <br />
        <asp:TextBox ID="txtSubject" width="400px" runat="server"></asp:TextBox>
        <br />
        <br />

        <asp:Label ID="lblBody" runat="server" Text="Ticket Body:"></asp:Label>
        <br />

        <asp:TextBox ID="txtBody" runat="server" TextMode="MultiLine"  Rows="10" class='pc90'>
        </asp:TextBox>
        <br />
    <br />


    <br />
    <asp:Button ID="btnSave" runat="server" Text="Submit Ticket" OnClick="btnSave_Click" />
    

</asp:Content>
