<%@ Page Title="Chat Room" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="Chat.aspx.cs" Inherits="Unchained.Chat" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
        <script>
        function search(ele) 
        {
            if (event.key === 'Enter')
            {
                document.getElementById('<%= btnChat.ClientID %>').click();
                ele.value = "";
            }
        }
        </script>

    <asp:UpdatePanel runat="server" ID="UpChat">
    <ContentTemplate>
        <asp:Timer runat="server" id="UpdateTimer" interval="2000" ontick="UpdateTimer_Tick" />
        <h4>Welcome to the BiblePay Chat Room</h4>
            <%=GetChat()%>
            <br />
            Say Something and Press [ENTER]:
            <asp:Button ID="btnChat" runat="server" CausesValidation="false" style='visibility:hidden;' Text="Say Something" OnClick="btnChat_Click" /></small>
    </ContentTemplate>
    <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnChat" EventName="Click"></asp:AsyncPostBackTrigger>
    </Triggers>
    </asp:UpdatePanel>
    <asp:TextBox runat="server" style="width:500px;" id="txtChat" onkeydown = "search(this);return (event.keyCode!=13);" />
</asp:Content>
