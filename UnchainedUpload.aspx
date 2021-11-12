<%@ Page Title="Unchained-Upload" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="UnchainedUpload.aspx.cs" Inherits="Unchained.UnchainedUpload"  ValidateRequest="true" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>




<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">


    
<script>
    function getCount() {
        var upload = $find("<%=AsyncUpload1.ClientID%>");
        return upload.getUploadedFiles().length;
    }

    (function () {
        var $;
        var dnd = window.dnd = window.dnd || {};

        dnd.initialize = function () {
            $ = $telerik.$;

            if (!Telerik.Web.UI.RadAsyncUpload.Modules.FileApi.isAvailable()) {
                $(".qsf-demo-canvas").html("<strong>Your browser does not support Drag and Drop. Please take a look at the info box for additional information.</strong>");
            }
            else {
                $(document).bind({ "drop": function (e) { e.preventDefault(); } });

                var dropZone1 = $(document).find(".DropZone1");
                dropZone1.bind({ "dragenter": function (e) { dragEnterHandler(e, dropZone1); } })
                    .bind({ "dragleave": function (e) { dragLeaveHandler(e, dropZone1); } })
                    .bind({ "drop": function (e) { dropHandler(e, dropZone1); } });

                var dropZone2 = $(document).find("#DropZone2");
                dropZone2.bind({ "dragenter": function (e) { dragEnterHandler(e, dropZone2); } })
                    .bind({ "dragleave": function (e) { dragLeaveHandler(e, dropZone2); } })
                    .bind({ "drop": function (e) { dropHandler(e, dropZone2); } });
            }
        };

        function dropHandler(e, dropZone) {
            dropZone[0].style.backgroundColor = "#357A2B";
        }

        function dragEnterHandler(e, dropZone) {
            var dt = e.originalEvent.dataTransfer;
            var isFile = (dt.types != null && (dt.types.indexOf ? dt.types.indexOf('Files') != -1 : dt.types.contains('application/x-moz-file')));
            if (isFile || $telerik.isSafari5 || $telerik.isIE10Mode || $telerik.isOpera)
                dropZone[0].style.backgroundColor = "#000000";
        }

        function dragLeaveHandler(e, dropZone) {
            if (!$telerik.isMouseOverElement(dropZone[0], e.originalEvent))
                dropZone[0].style.backgroundColor = "#357A2B";
        }


    })();





</script>
   	<link rel="stylesheet" type="text/css" href="Content/telerikstyles.css" />

    <h3>Decentralized Social Media - Upload your video or file</h3>
    <br />
    <small><font color="red"></font></small>
    <fieldset>
        <legend>
           <asp:Label ID="lblPageHeading" runat="server" />
        </legend>
    
         <br />

        <asp:Label ID="lblHideArea" runat="server" Text="">
        
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
        </asp:Label>

        <%=GetVideoCategories()%>
        <br />
    

         <!-- <asp:FileUpload ID="FileUpload1" onchange="UploadFile(this);" runat="server"  MultipleFileSelection="Automatic" for multiple files /> -->
        <table width="90%">

            <tr><td width="90%">
             <div class="attachment-container">
                <h4>Select the file(s) you want to upload here</h4>
         
                <span class="allowed-attachments">
                    <span class="allowed-attachments-list">(<%= String.Join( ",", AsyncUpload1.AllowedFileExtensions ) %>)</span>
                </span>
                <telerik:RadAsyncUpload RenderMode="Lightweight" runat="server" CssClass="async-attachment" 
                    ID="AsyncUpload1" HideFileInput="true" MultipleFileSelection="Automatic" DropZones=".DropZone1" 
                    AllowedFileExtensions=".jpeg,.jpg,.bmp,.svg,.png,.pdf,.gif,.docx,.xlsx,.mp3,.webm,.mp4" />
             </div>
       </td></tr>

            <tr><td>
        
                <div class="DropZone1">
                 <h3>Or, Drop Files Here</h3>
                 <p><i class="fa fa-camera"></i></p>
                </div>
         </td></tr>
            

            <tr><td>
                
                <h4>Then, Save your File:</h4>
        
                       <asp:Button ID="btnUnchainedSave" runat="server" OnClick="btnUnchainedSave_Click" 
                                OnClientClick="var oSubject=document.getElementById('MainContent_txtSubject'); var oBody=document.getElementById('MainContent_txtBody'); if (oSubject.value.length < 3 || oBody.value.length < 3) {alert('Sorry, the Subject or Body must be greater than 3 characters.'); return false;}  if (getCount()==0) { alert('Please choose at least one file before saving the record. If you have already chosen files, please wait until they have uploaded (the dot will become green when they are finished).');return false;} showSpinner();" Text="Save Record" />
                       <br /><br />
                       <asp:Label ID="lblmessage" runat="server" />
                                </td></tr>

           </table>

                   
</fieldset>

     
</asp:Content>
