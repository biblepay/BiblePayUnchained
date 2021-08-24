<%@ Page Title="Town Hall - Add Topic" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TownHallAdd.aspx.cs" Inherits="Unchained.TownHallAdd" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h3>Town Hall - Add new Topic</h3>
    

        <asp:Label ID="Label3" runat="server" Text="Topic Description"></asp:Label>
        <br />

       <asp:TextBox ID="txtBody" runat="server" TextMode="MultiLine"  Rows="10" style="width: 900px">
        </asp:TextBox>
        <br />


        <asp:Label ID="Label1" runat="server" Text="Topic Name/Subject - (The name of the discussion room you are creating): " ></asp:Label>
    <br />
        <asp:TextBox ID="txtSubject" width="400px" runat="server"></asp:TextBox>
        <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" />
    

</asp:Content>
