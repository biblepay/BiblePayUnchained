<%@ Page Title="RPC Console" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="RPCConsole.aspx.cs" Inherits="Unchained.RPCConsole" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
        <script>
        function search(ele) 
        {
            if (event.key === 'Enter')
            {
                document.getElementById('<%= btnCommand.ClientID %>').click();
                ele.value = "";
            }
        }
            function fixscroll()
            {
                var history = document.getElementById("rpcconsole");
                history.scrollTop = history.scrollHeight;
            }
        </script>

    <asp:UpdatePanel runat="server" ID="UpChat">
    <ContentTemplate>
        <!--<asp:HiddenField id="theChat2" runat="server" value="" />-->
        <asp:Timer runat="server" id="UpdateTimer" interval="90000" ontick="UpdateTimer_Tick" />
        <h4>Welcome to the BiblePay RPC Console</h4>
            <%=GetRPC()%>
            <br />
            Enter command and Press [ENTER]:
            <asp:Button ID="btnCommand" runat="server" CausesValidation="false" style='visibility:hidden;' Text="Command" OnClick="btnCommand_Click" /></small>
    </ContentTemplate>
    <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnCommand" EventName="Click"></asp:AsyncPostBackTrigger>
    </Triggers>
    </asp:UpdatePanel>
    <asp:TextBox runat="server" style="width:500px;" id="txtCommand" onkeydown = "search(this);return (event.keyCode!=13);" />
</asp:Content>
