<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AddNewsFeedItem.aspx.cs" Inherits="Unchained.AddNewsFeedItem" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <table>
        <tr>
            <td colspan="2">
                       <h3>Add News Feed Item</h3>
            </td>
            <td>
                       &nbsp;</td>
        </tr>
        <tr>
            <td colspan="2">
                   <asp:Label ID="lblMsg" runat="server" Font-Size="Large" ForeColor="#009900" ></asp:Label>
                   <asp:HiddenField ID="hdnID" runat="server" />
            </td>
            <td>
                       &nbsp;</td>
        </tr>
        <tr>
            <td>
                   <asp:Label ID="lblFeedName" runat="server" Text="Feed Name:" ></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtFeedName" width="400px" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:RequiredFieldValidator ID="rfvFeedName" runat="server" ControlToValidate="txtFeedName" ErrorMessage="Please add feed name" Font-Bold="True" Font-Size="Large" ForeColor="Red">*</asp:RequiredFieldValidator>
            </td>
        </tr> 
          <tr>
            <td>
                  <asp:Label ID="lblUrl" runat="server" Text="Url:" ></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtUrl" width="400px" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:RequiredFieldValidator ID="rfvUrl" runat="server" ControlToValidate="txtUrl" ErrorMessage="Please add url" Font-Bold="True" Font-Size="Large" ForeColor="Red">*</asp:RequiredFieldValidator>
            </td>
        </tr> 
            <tr>
            <td>
                  <asp:Label ID="lblWeight" runat="server" Text="Weight:" ></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtWeight" onkeypress="return isNumberKey(event)"  width="400px" runat="server"></asp:TextBox>
                 <asp:rangevalidator id="rvCompare"
                  controltovalidate="txtWeight" 
                  minimumvalue="0"
                  maximumvalue="100" 
                  type="Double" 
                  errormessage="Weight range must between 0 to 100" 
                  runat="server"/>
            </td>
            <td>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtWeight" ErrorMessage="Please add weight" Font-Bold="True" Font-Size="Large" ForeColor="Red">*</asp:RequiredFieldValidator>
            </td>
        </tr> 
        <tr>
            <td style="vertical-align:top">
                 <asp:Label ID="lblNotes" runat="server" Text="Notes:"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtNotes" runat="server" Width="400px" ></asp:TextBox>
            </td>
            <td style="vertical-align:top">
                <asp:RequiredFieldValidator ID="rfvNotes" runat="server" ControlToValidate="txtNotes" ErrorMessage="*" Font-Bold="True" Font-Size="Large" ForeColor="Red" Enabled="False"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td style="vertical-align:top; height: 18px;">
                 </td>
            <td style="height: 18px">
                </td>
            <td style="vertical-align:top; height: 18px;">
                </td>
        </tr>
        <tr>
            <td style="vertical-align:top">
                 &nbsp;</td>
            <td>
                    <asp:Button ID="btnSave" runat="server" Text="Submit Feed" OnClick="btnSave_Click" ForeColor="Black" />
                    <asp:Button ID="btnList" runat="server" Text="Feed List" OnClick="btnList_Click" ForeColor="Black" CausesValidation="False" />
                    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True" ShowSummary="False" />
            </td>
            <td style="vertical-align:top">
                &nbsp;</td>
        </tr>
        <tr>
            <td colspan="2">
                    &nbsp;</td>

            <td>
                    &nbsp;</td>

        </tr>

    </table>


     

    
    <SCRIPT language=Javascript>
      function isNumberKey(evt)
      {
         var charCode = (evt.which) ? evt.which : evt.keyCode;
          if (charCode > 31 && (charCode < 48 || charCode > 57) && charCode != 46)
            return false;    
         return true;
      }
    </SCRIPT>

</asp:Content>
