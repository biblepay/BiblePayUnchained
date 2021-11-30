<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SignalClient.aspx.cs" Inherits="Unchained.SignalClient" %>

<!DOCTYPE html>
<html>
<head>
    <title>Client Form Receiving Notifications</title>
    <script src="/Scripts/jquery-3.4.1.js" ></script>
    <script src="/Scripts/jquery.signalR-2.4.2.js"></script>
    <script src="/signalr/hubs"></script>
    <script type="text/javascript">
        $(function () {
            var proxy = $.connection.notificationHub;

            proxy.client.receiveNotification = function (message) {
                $("#container").html(message);
                $("#container").slideDown(2000);
                setTimeout('$("#container").slideUp(2000);', 5000);
            };

            $.connection.hub.start();
        });
    </script>
</head>
<body>
    <div class="notificationBalloon" id="container">
    </div>
</body>
</html>
