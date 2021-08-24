<%@ Page Title="Unchained-Upload" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UnchainedUpload.aspx.cs" Inherits="Unchained.UnchainedUpload"  ValidateRequest="false" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
   	<link rel="stylesheet" type="text/css" href="~/Content/telerikstyles.css" />

    <h3>BiblePay Social Media - Upload your video or file</h3>

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
         
        
        <!-- <asp:FileUpload ID="FileUpload1" onchange="UploadFile(this);" runat="server" />-->
        <div class="attachment-container">
            <telerik:RadAsyncUpload RenderMode="Lightweight" runat="server" CssClass="async-attachment" ID="AsyncUpload1"
                HideFileInput="true"  AllowedFileExtensions=".jpeg,.jpg,.bmp,.svg,.png,.pdf,.docx,.xlsx,.mp3,.mp4" />
            <span class="allowed-attachments">Select file(s) to upload 
                <span class="allowed-attachments-list">(<%= String.Join( ",", AsyncUpload1.AllowedFileExtensions ) %>)</span>
            </span>
        </div>
     

         <br /><br />
        
            <asp:Button ID="btnUnchainedSave" runat="server" OnClick="btnUnchainedSave_Click" OnClientClick="showSpinner();" Text="Save Record" />
         <br /><br />
         <asp:Label ID="lblmessage" runat="server" />
</fieldset>

     
</asp:Content>
