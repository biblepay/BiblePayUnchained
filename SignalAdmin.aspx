<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SignalAdmin.aspx.cs" Inherits="Unchained.SignalAdmin" %>

<!DOCTYPE html>
<html>
<head>
    <title>Admin Form Sending Notifications</title>

    <script src="/Scripts/jquery-3.4.1.js" ></script>
    <script src="/Scripts/jquery.signalR-2.4.2.js"></script>
    <script src="/signalr/hubs"></script>

    <script type="text/javascript">
        $(function () {
            var proxy = $.connection.notificationHub;

            $("#button1").click(function () {
                proxy.server.sendNotifications($("#text1").val());
            });

            $.connection.hub.start();
        });
    </script>

</head>
<body>
    <input id="text1" type="text" />
    <input id="button1" type="button" value="Send" />
</body>
</html>