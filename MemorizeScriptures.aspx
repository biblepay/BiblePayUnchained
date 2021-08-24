<%@ Page Title="Memorize Scriptures" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="MemorizeScriptures.aspx.cs" Inherits="Unchained.MemorizeScriptures" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
  
 

     <h3>Memorize Scriptures Tool <small>v1.1</small></h3>
    

    <table>

        <tr>
            <td>        <asp:Label ID="lblBook" runat="server" Text="Book:"></asp:Label>            </td>
            <td>        <asp:dropdownlist runat="server" id="ddBook"> 
                        </asp:dropdownlist>   

            </td></tr>

        <tr>
            <td><asp:Label ID="lblChapter" runat="server" Text="Chapter:"></asp:Label></td>
            <td>       <asp:TextBox ID="txtChapter" width="400px" type="number" runat="server"></asp:TextBox></td>
        </tr>

    <tr>
        <td>        <asp:Label ID="lblVerse" runat="server" Text="Verse:"></asp:Label></td>
        <td>        <asp:TextBox ID="txtVerse" width="400px" type="number" runat="server"></asp:TextBox></td></tr>

    
        <tr>
            <td>        <asp:Label ID="lblScripture" runat="server" Text="Scripture:"></asp:Label></td>

            <td>        <asp:TextBox ID="txtScripture" runat="server" TextMode="MultiLine"  Rows="10" style="width: 900px">        </asp:TextBox></td>

        </tr>

        <tr>
            <td>        <asp:Label ID="lblPractice" runat="server" Text="Practice Typing Scripture Here:"></asp:Label></td>
            <td>        <asp:TextBox ID="txtPractice" runat="server" TextMode="MultiLine"  Rows="10" style="width: 900px">        </asp:TextBox>                </td>

        </tr>
        </table>


        <asp:Button ID="btnSave" runat="server" Text="Next Scripture" OnClick="btnNextScripture_Click" />
        <asp:Button ID="btnSwitchToTest" runat="server" Text="Switch to TEST Mode" OnClick="btnSwitchToTestMode_Click" />
        <asp:Button ID="btnGrade" runat="server" Text="Grade" OnClick="btnGrade_Click" />
   <br />

             <asp:Label ID="lblInfo" runat="server" Text=""></asp:Label>

    

</asp:Content>
