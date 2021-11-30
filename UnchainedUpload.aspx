<%@ Page Title="Unchained-Upload" Language="C#"  AutoEventWireup="true"  CodeBehind="UnchainedUpload.aspx.cs" Inherits="Unchained.UnchainedUpload"  ValidateRequest="true" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<head runat="server">
    <asp:PlaceHolder runat="server">
       <%: Scripts.Render("/Scripts/jquery-3.4.1.js") %>
    </asp:PlaceHolder>
     <link rel="stylesheet" type="text/css" href="Content/telerikstyles.css" />
    
    <link rel="stylesheet" type="text/css" href="Content/Site1.css" />
   

</head>


<form runat="server">
  <asp:ScriptManager runat="server">
  </asp:ScriptManager>


<script>

    function validateForm() {
        var oSubject = document.getElementById('txtSubject');
        var oBody = document.getElementById('txtBody');
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

   

<style type="text/css">
.RadUpload .ruBrowse
{
     display:none !important;
     height: 64px !important;
     width: 250px !important;
     background-position-y: -25px !important;
     background-position-x: -55px !important;
     cursor:pointer !important;
}

.largeBrowseButton .ruBrowse
{
    background-position-y: -45px !important;
    height: 24px !important;
    width: 115px !important;
}

.RadUpload .ruStyled .ruFileInput
{
  position:relative !important;
}

.RadUpload .ruFileProgress
{
        height:2px !important;
}

.DropZone1 {
    width: 320px;
    height: 154px;
    border-color: #CCCCCC;
    float: left;
    text-align: center;
    font-size: 16px;
    color: white;
    position:relative;
    left:-20px;
    background: repeating-linear-gradient( 45deg, black, #606dbc 10px, #465298 10px, #465298 20px );
    background-color:black;
}


.DropZone2 {
  margin-top: -100px;
  margin-left: -250px;
  width: 500px;
  height: 200px;
  border: 4px dashed #fff;
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
    &nbsp;&nbsp;<asp:Button ID="btnUnchainedSave" runat="server" OnClick="btnUnchainedSave_Click" OnClientClick="return validateForm();"                      Text="Save your File" />

    <br />
    </small>
    
    <div class="attachment-container" style="xfont-size:10px;">
       <div class="DropZone1">
       <telerik:RadAsyncUpload RenderMode="Lightweight" runat="server" CssClass="async-attachment largeBrowseButton" width="250px" Height="190px"
                style="left:-5px;color:gold;"    ID="AsyncUpload1" HideFileInput="true" MultipleFileSelection="Automatic" DropZones=".DropZone1" 
                    AllowedFileExtensions=".jpeg,.jpg,.bmp,.svg,.png,.pdf,.gif,.xlsx,.mp3,.webm,.mp4,.mov" />
        
            <div style="position:relative;top:-60px;">
            Drop files or click here
                </div>
            <br />

        </div>
    </div>
    
     
</form>
