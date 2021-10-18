<%@ Page Title="Profile" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="Profile.aspx.cs" Inherits="Unchained.Profile" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    
    <fieldset>
         <legend>Your Public Social Media Page: <small>&nbsp;&nbsp;<font color=red></small></font></legend>

        <label class="offset" for="ddGender">My Gender: </label><asp:dropdownlist runat="server" id="ddGender"></asp:dropdownlist>   
        <br />


        <label class="offset" for="txtBirthDate">My Birth Date: </label><asp:TextBox width="200px" type="date" id="txtBirthDate" runat="server"></asp:TextBox> 

        <br />
        <label class="offsetv" for="txtPublicText">About Me (Public): </label><asp:TextBox width="700px" TextMode="MultiLine"  Rows="5" 
            ID="txtPublicText" runat="server"></asp:TextBox> 

        <br />
        <label class="offsetv" for="txtPrivateText">About Me (Private/Friends Only): </label><asp:TextBox width="700px" TextMode="MultiLine"  Rows="5" 
            ID="txtPrivateText" runat="server"></asp:TextBox> 

        <br />
        <label class="offsetv" for="txtProfessionalText">About Me (Professional): </label><asp:TextBox width="700px" TextMode="MultiLine"  Rows="5" 
            ID="txtProfessionalText" runat="server"></asp:TextBox> 

        <br />
        <label class="offsetv" for="txtReligiousText">About Me (Beliefs): </label><asp:TextBox width="700px" TextMode="MultiLine"  Rows="5" 
            ID="txtReligiousText" runat="server"></asp:TextBox> 
        <br />
        <asp:Button ID="btnUpdateSocial" runat="server" Text="Update Social Media Profile" OnClick="btnUpdateSocialMediaProfile_Click"/>         
        
    </fieldset>
<br />

    <fieldset>
        <legend>Your Telegram Fields:</legend>

        <label class="offset" for="txtTLN">Telegram Name:</label><asp:TextBox width="700px" id="txtTelegramLinkName" runat="server"></asp:TextBox> 
        <br />
        <label class="offset" for="txtTLU">Telegram URL:</label><asp:TextBox width="700px" id="txtTelegramURL" runat="server"></asp:TextBox> 
        <br />
        <label class="offset" for="txtTLD">Telegram Description:</label><asp:TextBox width="700px" id="txtTelegramDescription" TextMode="MultiLine" Rows="5" runat="server"></asp:TextBox> 
        <br />
        <asp:Button ID="btnUpdateTelegramFields" runat="server" Text="Update Telegram Profile" OnClick="btnUpdateTelegramProfile_Click"/>         
        
    </fieldset>
    <br />




</asp:Content>
