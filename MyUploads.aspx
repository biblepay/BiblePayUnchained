<%@ Page Title="My Uploads" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MyUploads.aspx.cs" Inherits="Unchained.MyUploads" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <h5>My Uploads (Within the last 7 days):</h5>
    <small>
        This page is provided for diagnostic purposes.  The video upload goes through phases:  SUBMITTED, RECEIVED, TRANSCODING, FINISHED or raises an error code.
        <br />
        If you have any problems with a specific video, such as failing to transcode or appear in our system, please copy the video ID to your clipboard and open a support ticket.
        <br />
        Note:  It usually takes between 1-20 minutes for a video to finish and appear in your channel.

    </small>

     <asp:UpdatePanel runat="server" ID="UpdatePanel1">
     <ContentTemplate>
        <%=GetMyUploads()%>
     </ContentTemplate>
     </asp:UpdatePanel>

</asp:Content>
