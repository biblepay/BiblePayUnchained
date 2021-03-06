<%@ Page Title="Unchained-Upload" Language="C#" MasterPageFile="~/SiteFrame.Master" AutoEventWireup="true"  CodeBehind="UnchainedUpload.aspx.cs" Inherits="Unchained.UnchainedUpload"  ValidateRequest="true" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

<link rel="stylesheet" type="text/css" href="Content/telerikstyles.css" />

<script>

    function validateForm() {
        var sPrefix = 'MainContent_';
        var oSubject = document.getElementById(sPrefix + 'txtSubject');
        var oBody = document.getElementById(sPrefix + 'txtBody');
        if (oSubject.value.length < 3 || oBody.value.length < 3) {
            alert('Sorry, the Subject or Body must be greater than 3 characters.'); return false;
        }

        if (getCount() == 0)
        {
            alert('Please choose at least one file before saving the record. If you have already chosen files, please wait until they have uploaded (the dot will become green when they are finished).');
            return false;
        }
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
    xbackground-position-x: -100px !important;
    xleft:-100px !important;
    xposition:relative !important;
    xcursor:hand !important;
}

    .ruFileProgress {
        xmax-width:200px;
    }

    .ruFileLI .ruUploading {
        position:relative !important;
        top:-40px !important;
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

.DropZone2 {
  margin-top: -100px;
  margin-left: -250px;
  width: 500px;
  height: 200px;
  border: 4px dashed #fff;
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

    <small>
    <asp:Label ID="lblHideArea" runat="server" Text="">
    <asp:Label runat="server" ID="lblSubject">Title:</asp:Label>
       <br />
    <asp:TextBox ID="txtSubject" width="200px" runat="server"></asp:TextBox>
       <br /> 
    <asp:Label runat="server" ID="lblBody">Body:</asp:Label>
       <br />
    <asp:TextBox ID="txtBody" runat="server" TextMode="MultiLine"  Rows="2" style="width: 200px">        </asp:TextBox>
       <br />
    <asp:Label runat="server" ID="Label1">Category:</asp:Label>

        <br />
    </asp:Label>
    <%=GetVideoCategories()%>
        <br />
        <br />

    &nbsp;&nbsp;<asp:Button ID="btnUnchainedSave" runat="server" OnClick="btnUnchainedSave_Click" OnClientClick="return validateForm();"                      Text="Save your File" />

    <br />
    </small>
        <div class="DropZone1" style="max-width:290px;" onclick="clickmou(0);">
    
    <div class="attachment-container" style="padding-left:1px;xposition:relative;">
                  <telerik:RadAsyncUpload RenderMode="Lightweight" runat="server" CssClass="async-attachment" enablefileinputskinning="true" Height="150" Width="250px" InputSize="4"
                style="color:gold;"    ID="AsyncUpload1" HideFileInput="true" MultipleFileSelection="Automatic" DropZones=".DropZone1" 
                    AllowedFileExtensions=".jpeg,.jpg,.bmp,.svg,.png,.pdf,.gif,.xlsx,.mp3,.webm,.mp4,.mov" />

            <div style="position:relative;left:-50px;top:-72px;font-size:20px;color:lime">
            Drop files or click here
        </div>
    </div>
    </div>
     
</asp:Content>

