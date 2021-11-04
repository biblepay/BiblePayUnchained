<%@ Page Title="Greeing Cards" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="GreetingCard.aspx.cs" Inherits="Unchained.GreetingCard" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <h3>BiblePay Greeting Card and Virtual Gift Card Home Delivery - v1.4</h3>

    <br />
    Recipient Name:
    <br />
    <asp:TextBox ID="txtRecipientName" runat="server" class='pc90'>        </asp:TextBox>
    <br />
    Opening Salutation:
   <br />
    <asp:TextBox ID="txtOpeningSalutation" runat="server" class='pc90'>        </asp:TextBox>
    <br />
    Paragraph 1:
    <br />
    <asp:TextBox ID="txtParagraph1" runat="server" TextMode="MultiLine"  Rows="5" class='pc90'>        </asp:TextBox>
    <br />
    Paragraph 2:
    <br />
    <asp:TextBox ID="txtParagraph2" runat="server" TextMode="MultiLine"  Rows="5" class='pc90'>        </asp:TextBox>
    <br />
    Closing Salutation:
    <br />
    <asp:TextBox ID="txtClosingSalutation" runat="server" class='pc90'>        </asp:TextBox>
   <br />
   Virtual Gift Card Amount $:
    <br />
   <asp:TextBox ID="txtAmount" type="number" runat="server" style="width: 200px">        </asp:TextBox>
   <br />
    Street Address Line 1:
    <br />
     <asp:TextBox ID="txtAddress1" runat="server" style="width: 600px">        </asp:TextBox>
  <br />
    Street Address Line 2:
    <br />
     <asp:TextBox ID="txtAddress2" runat="server" style="width: 600px">        </asp:TextBox>
  <br />
    City:
    <br />
     <asp:TextBox ID="txtCity" runat="server" style="width: 500px">        </asp:TextBox>
    <br />
  State:
    <br />
     <asp:TextBox ID="txtState" runat="server" style="width: 200px">        </asp:TextBox>
    <br />
    Zip Code:
    <br />
     <asp:TextBox ID="txtZipCode" type="number" runat="server" style="width: 200px">        </asp:TextBox>
  <br />
    Card Type:
    <br />
      <asp:dropdownlist runat="server" id="ddCardType" AutoPostBack="true" OnSelectedIndexChanged="ddCardType_SelectedIndexChanged"></asp:dropdownlist>   
    <br />
    <asp:CheckBox ID="chkDeliver" runat="server" Text="Deliver this for Real? (If unchecked we will create a proof, if checked we will charge and deliver)." checked="true" />
    <br />
      
     <asp:Button ID="btnSend" runat="server" onclick="btnSend_Click"  Text="Send" />
    <br />
    <br />

     <asp:Label ID="lblInfo" runat="server" style="color:red;" Text=""></asp:Label></td>

  <br />
   
</asp:Content>
