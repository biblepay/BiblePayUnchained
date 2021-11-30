<%@ Page Title="Friend Requests" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="FriendRequests.aspx.cs" Inherits="Unchained.FriendRequests" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <input type="hidden" id="hfPostback" />
    <link href="Content/pages/peopledirectory.css" rel="stylesheet" />
   <div class="row position-relative" id="people-directory">
                <div class="loader1">
                    <i class="fa fa-sun fa-spin fa-4x"></i>
                </div>
                <%=GetFriendRequests()%>
            </div>
    

</asp:Content>
