<%@ Page Title="Unchained-Upload" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UnchainedUpload.aspx.cs" Inherits="Unchained.UnchainedUpload"  ValidateRequest="false" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        function UploadFile(fileUpload)
        {
            if (fileUpload.value != '')
            {
                //document.getElementById("<%=btnUnchainedSave.ClientID %>").click();
            }
        }

        

</script>

    <h3>BiblePay Social Media - Upload your video or file</h3>

    <fieldset>
        <legend>Add Media Contribution:</legend>
    
         <br />

         <label>Subject:</label>
         <br />
         <asp:TextBox ID="txtSubject" width="400px" runat="server"></asp:TextBox>
         <br />
        
         <label>Body:</label>
         <br />
         <asp:TextBox ID="txtBody" runat="server" TextMode="MultiLine"  Rows="10" style="width: 900px">        </asp:TextBox>
         <br />
         

         <asp:FileUpload ID="FileUpload1" onchange="UploadFile(this);" runat="server" />
         <br /><br />
        <!--         <div id="invisible" style="xvisibility:hidden"> </div>-->
        

            <asp:Button ID="btnUnchainedSave" runat="server" OnClick="btnUnchainedSave_Click" OnClientClick="showSpinner();" Text="Save Record" style="xwidth:85px" />
         <br /><br />
         <asp:Label ID="lblmessage" runat="server" />
</fieldset>

     
</asp:Content>
