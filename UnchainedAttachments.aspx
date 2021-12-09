<%@ Page Title="Unchained Attachments" Language="C#" MasterPageFile="~/SiteFrame.Master" AutoEventWireup="true"  CodeBehind="UnchainedAttachments.aspx.cs" Inherits="Unchained.UnchainedAttachments"  ValidateRequest="true" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
<link rel="stylesheet" type="text/css" href="Content/telerikstyles.css" />

<script>

    function validateForm() {
        //var sPrefix = 'MainContent_';
        var btn = document.getElementById("MainContent_btnUnchainedSave");
        if (getCount() == 0) {
            if (btn.value == "Save File(s)") {
                alert('Please choose at least one file before saving the record. If you have already chosen files, please wait until they have uploaded (the dot will become green when they are finished).');
            }
            else {
                clickmou();
            }
            return false;
        }
        return true;
    }

    function doValidate() {
        return true;
    }

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

    function clickmou(i)
    {
        //ctl00_MainContent_AsyncUpload1
        for (var i = 0; i < 9; i++)
        {
            var sPrefix = 'ctl00_MainContent_AsyncUpload1file' + i.toString();
            var o = document.getElementById(sPrefix);
            if (o)
            {
                o.click();
            }
        }
    }


    $(document).ready(function () {
    });


    function MyFileUploaded(sender, args) {
        var contentType = args.get_fileInfo().ContentType;
        var filename = args.get_fileName();
        var currentRowElement = args.get_row();
        // Change the text 
        var btn = document.getElementById("MainContent_btnUnchainedSave");
        btn.value = "Save File(s)";

    }


</script>

   

<style type="text/css">
.RadUpload .ruBrowse
{
     display:none !important;
     width:1px !important;
     height:1px !important;
}

.ruFileInput
{
     position:absolute;
     top:1px;
     left:1px;
     width:1px !important;
     height:1px !important;
    
}

.ruButton {
    xbackground-position-x: -10px !important;
    xleft:-10px !important;
    xposition:relative !important;
    xcursor:hand !important;
}

    .ruFileProgress {
        xmax-width:200px;
    }

    .ruFileLI .ruUploading {
        position:relative !important;
        top:-20px !important;
        color:aqua !important;
        xpadding:20px;
    }
    .ruFileProgressWrap{
       max-width:250px;
    }

.RadUpload .ruStyled .ruFileInput
{
  position:relative !important;
}

.RadUpload .ruFileProgress
{
    height:7px !important;
}



.DropZone1 {
    width: 290px;
    height: 150px;
    border-color: #CCCCCC;
    text-align: center;
    font-size: 16px;
    color: white;
    position:relative;
    left:0px;
    top:10px;
    background: repeating-linear-gradient( 45deg, black, #606dbc 10px, #465298 10px, #465298 20px );
    background-color:black;
}


</style>
    <!-- attachments area -->

    <%=GetAttachments() %>

    <!--new file area -->
   
        <div class="DropZone1" style="max-width:290px;" >
            <div class="attachment-container" style="padding-left:1px;position:relative;top:-20px;max-width:290px;max-height:145px;">
                          <telerik:RadAsyncUpload RenderMode="Lightweight" runat="server" CssClass="async-attachment" enablefileinputskinning="true" Height="150" Width="250px" InputSize="4"
                   OnClientFileUploaded="MyFileUploaded"     style="color:gold;"    ID="AsyncUpload1" HideFileInput="true" MultipleFileSelection="Automatic" DropZones=".DropZone1" 
                            AllowedFileExtensions=".jpeg,.jpg,.bmp,.svg,.png,.pdf,.gif,.xlsx,.mp3,.webm,.mp4,.mov" />
                    <div style="position:relative;left:-1px;top:-41px;font-size:20px;color:lime" onclick="clickmou(0);">
                        Drop files here
                </div>
            </div>
              
    </div>
    <br />
    <div style="z-index:9999;font-size:125%;position:relative;">
    <asp:Button ID="btnUnchainedSave" runat="server" OnClick="btnUnchainedSave_Click" OnClientClick="return validateForm();"  Text="+ Add Attachment" />
        </div>
     
</asp:Content>

