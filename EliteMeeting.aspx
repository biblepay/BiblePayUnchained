<%@ Page Title="Elite Meeting Room" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EliteMeeting.aspx.cs" Inherits="Unchained.EliteMeeting" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <small><font color=red>** To record a live video that will publish, click Start Recording and choose Cloud Recording.  When finished click Stop Recording.  Then look for your Video in My Uploads. **</font></small>
    <%=GetMeeting() %>


 </asp:Content>
