<%@ Page Title="Live Recorder" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="LiveRecord.aspx.cs" Inherits="Unchained.LiveRecord" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <script src="https://webrtc.github.io/adapter/adapter-latest.js"></script>
    <script src="Scripts/VideoRecord7.js" async></script>

	<div id="container">

      <h1>
            Live Video Recording <small>v1.7</small>
      </h1>

      <video id="theVideo" autoplay muted poster="/images/VideoRecordIcon.png" style="border: 1px solid rgb(15, 158, 238); height: 240px; width: 320px;"></video>
      <div>
          <button id="record" onclick='return false;' style='font-size:20px;width:200px;height:60px;' class="btn btn-info">Start Recording</button>
          <button id="play" onclick='return false;' style='font-size:20px;width:200px;height:60px;' class="btn btn-warning">Play</button>

          <br />
          <br />
          <span style='font-size:20px;'>Category:</span>
          <br />
          <%=Unchained.UICommon.GetVideoCategories("ddCategory", "") %>
          <button id="btnUpload" disabled onclick='return false;' style='font-size:20px;width:200px;height:60px;' class="btn btn-info">Upload and Post</button>
      </div>
        <select style='visibility:hidden;' id="codecPreferences" disabled></select>
      <div>
            <span id="errorMsg"></span>
      </div>
    </div>

<br />
<br />
</asp:Content>
