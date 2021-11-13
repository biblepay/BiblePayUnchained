<%@ Page Title="Admin" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Admin1.aspx.cs" Inherits="Unchained.Admin1"  ValidateRequest="false" %>


<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <!--     <script type="text/javascript" src="/scripts/indexvideo.js"></script> -->

    
    <h3>Admin 1</h3>
    <div>


    </div>
  <div>
         <br />

         <asp:Button ID="btnSave" runat="server" onclick="btnSave_Click"  Text="Save" />
         <br /><br />
        
      <input type="button" onclick="var e = RSAEncrypt('this is a test.');showModalDialog('This is my title', e, true);" value="showmodal"/>
      <input type="button" onclick="<%=Unchained.UICommon.Toast("title", "thetoast")%>" value="The Toast" />
      <input type="button" onclick="testPerformance();" value="Performance Test" />

      <%=GetPerfSection() %>

  </div>


     <div>
         <!--
         <h3>Edge.Network Demo - This file is pulled with range requests from multiple distributed sources (10 meg file, split over 10 sources)</h3>
         <video id="bbpdecvid1" class="" preload="auto" controls="true" with="400" height="300">
             <source src="api/MediaType/Video" type="video/mp4" />
         </video>

         <h3>Javascript - IndexedDB - Video hosted inside javascript and pulled as blob from IndexedDB (javascript::QueryAnalyzer, javascript::table)</h3>
         <video id="video2" class="" preload="auto" controls="true" with="400" height="300">
             <source src="api/MediaType/Video" type="video/mp4" />
         </video>
         -->


     </div>
</asp:Content>
