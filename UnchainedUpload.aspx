<%@ Page Title="Unchained-Upload" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UnchainedUpload.aspx.cs" Inherits="Unchained.UnchainedUpload"  ValidateRequest="false" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>




<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">


    
<script>
    function getCount() {
        var upload = $find("<%=AsyncUpload1.ClientID%>");
        return upload.getUploadedFiles().length;
    }
</script>
   	<link rel="stylesheet" type="text/css" href="~/Content/telerikstyles.css" />

    <h3>Decentralized Social Media - Upload your video or file</h3>
    <br />
    <small><font color="red"></font></small>
    <fieldset>
        <legend>
           <asp:Label ID="lblPageHeading" runat="server" />
        </legend>
    
         <br />

         <asp:Label runat="server" ID="lblSubject">Title:</asp:Label>
         <br />
         <asp:TextBox ID="txtSubject" width="400px" runat="server"></asp:TextBox>
         <br />
        
         <asp:Label runat="server" ID="lblBody">Body:</asp:Label>
         <br />
         <asp:TextBox ID="txtBody" runat="server" TextMode="MultiLine"  Rows="10" style="width: 900px">        </asp:TextBox>
         <br />
         <asp:Label runat="server" ID="Label1">Category:</asp:Label>
         <br />
         <%=Unchained.UICommon.GetVideoCategories("ddCategory", "") %>
         <br />

         <!-- <asp:FileUpload ID="FileUpload1" onchange="UploadFile(this);" runat="server"  MultipleFileSelection="Automatic" for multiple files />-->
         <div class="attachment-container">
            <telerik:RadAsyncUpload RenderMode="Lightweight" runat="server" CssClass="async-attachment" ID="AsyncUpload1"
                HideFileInput="true" MultipleFileSelection="Automatic" AllowedFileExtensions=".jpeg,.jpg,.bmp,.svg,.png,.pdf,.docx,.xlsx,.mp3,.mp4" />
            <span class="allowed-attachments">Select file(s) to upload 
                <span class="allowed-attachments-list">(<%= String.Join( ",", AsyncUpload1.AllowedFileExtensions ) %>)</span>
            </span>
       </div>
     
       <br /><br />
        
       <asp:Button ID="btnUnchainedSave" runat="server" OnClick="btnUnchainedSave_Click" 
                OnClientClick="var oSubject=document.getElementById('MainContent_txtSubject'); var oBody=document.getElementById('MainContent_txtBody'); if (oSubject.value.length < 3 || oBody.value.length < 3) {alert('Sorry, the Subject or Body must be greater than 3 characters.'); return false;}  if (getCount()==0) { alert('Please choose at least one file before saving the record. If you have already chosen files, please wait until they have uploaded (the dot will become green when they are finished).');return false;} showSpinner();" Text="Save Record" />
       <br /><br />
       <asp:Label ID="lblmessage" runat="server" />
</fieldset>

     
</asp:Content>
