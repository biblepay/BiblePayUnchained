<%@ Page Title="View Media" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="Media.aspx.cs" Inherits="Unchained.Media" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
  
      <%=GetVideo() %>

    
  <asp:UpdatePanel runat="server" ID="Up3">
    <ContentTemplate>
      <%=GetComments() %>
    </ContentTemplate>
  </asp:UpdatePanel>
  
   <script>
       var original_transcript = "";
       var fIgnore = false;

       function unignore() {
           fIgnore = false;
       }

       function updateText(sNewText) {
           var o = document.getElementById('transcript1');
           var new_transcript = original_transcript.replace(sNewText, "<mark><span class='highlight2'>" + sNewText + "</span></mark>");
           o.innerHTML = new_transcript;
           var $container = $("#transcript1");
           var $scrollTo = $('.highlight2');
           $container.animate({ scrollTop: $scrollTo.offset().top - $container.offset().top + $container.scrollTop(), scrollLeft: 0 }, 300);
           setTimeout(unignore, 2000);
           fIgnore = true;
       }

       player.on('timeupdate', tdata => {
           if (fIgnore)
               return;
           var curpos = parseFloat(tdata);
           var o = document.getElementById('transcript1');
           if (original_transcript == "")
               original_transcript = o.innerHTML;
           var vrows = original_transcript.split("<br>");
           var ntime = 0;
           var nfuturetime = 0;
           for (var i = 0; i < vrows.length - 1; i++) {
               var sdata = vrows[i + 1];
               var vdata = sdata.split(" ");
               if (vdata.length > 1) 
                       nfuturetime = parseFloat(vdata[0]);
               sdata = vrows[i];
               if (curpos < nfuturetime) {
                       updateText(sdata);
                       break;
                   }
           }
           console.log(tdata + ", " + nfuturetime);
       });
    </script>

</asp:Content>
