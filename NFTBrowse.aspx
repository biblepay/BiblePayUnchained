<%@ Page Title="Non Fungible Tokens" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="NFTBrowse.aspx.cs" Inherits="Unchained.NFTBrowse" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h3> NFT Marketplace - Digital Goods </h3>

    <hr />

    <div>
    <table id="tbl1">
        <tr><td>
    <asp:CheckBox ID="chkDigital" runat="server" Text="Digital Goods (Audio, Video, MP3, MP4, PDF, E-Books)" AutoPostBack="true"  checked="true"   OnCheckedChanged="chkDigital_Changed" />
            </td></tr>
        <tr><td>
    <asp:CheckBox ID="chkSocial" runat="server" Text="Social Media (Tweets, Posts, URLs)"                    AutoPostBack="true" checked="true"    OnCheckedChanged="chkTweet_Changed" />
            </td></tr>
        </table>
    </div>
    <hr />


    <div>
        <%=GetNFTDisplayList(this, false, chkDigital.Checked, chkSocial.Checked) %>
    </div>

</asp:Content>
