<%@ Page Title="Memorize Scriptures" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="MemorizeScriptures.aspx.cs" Inherits="Unchained.MemorizeScriptures" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
  
 

     <h3>Memorize Scriptures Tool <small>v1.1</small></h3>
    

    <table width="95%">

        <tr>
            <td width="15%">        <asp:Label ID="lblBook" runat="server" Text="Book:"></asp:Label>            </td>
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

            <td>        <asp:TextBox ID="txtScripture" runat="server" TextMode="MultiLine"  Rows="10" class='pc90'>        </asp:TextBox></td>

        </tr>

        <tr>
            <td>        <asp:Label ID="lblPractice" runat="server" Text="Practice Typing Scripture Here:"></asp:Label></td>
            <td>        <asp:TextBox ID="txtPractice" runat="server" TextMode="MultiLine"  Rows="10" class='pc90'>        </asp:TextBox>                </td>

        </tr>
        </table>

    <br />

        <asp:Button ID="btnSave" runat="server" Text="Next Scripture" OnClick="btnNextScripture_Click" />
        <asp:Button ID="btnSwitchToTest" runat="server" Text="Switch to TEST Mode" OnClick="btnSwitchToTestMode_Click" />
        <asp:Button ID="btnGrade" runat="server" Text="Done" OnClick="btnGrade_Click" />
   <br />

             <asp:Label ID="lblInfo" runat="server" Text=""></asp:Label>

    <br />
    <hr />
    <ul>
        <li> Become a Bible Thumper! 
            <li> Please read the scripture that is on the screen, then type it in the input box below the scripture.  
                  This will help you memorize the scripture.  
         <li>You may also recite the scripture repeatedly. 
         <li>In Learn Mode, you should just type and recite the scriptures, and continue to click Next to learn more.  
         <li>In Test Mode, you will read the scripture and then try to Type the Book, Chapter and Verse in the text boxes.  
         <li>We will keep track of a tally to see how you did, and when you click Done, we will display the Grade!
         <li> Where do these scriptures come from?  The scripture reference is stored in the sidechain and new ones may be submitted and suggested on our forum expanding and contracting the list.  
        
    </ul>
        
</asp:Content>
