# TelegramApi

Based on [**webogram**](https://github.com/zhukov/webogram)

## Getting started

1) Install package via npm
```
npm install telegram-api-js
```
Or bower
```
bower install telegram-api-js
```

2) Add a ```<script>``` to your index.html
```
<html>
<head>
    <title>My amazing app</title>
</head>
<body>
    <script src="js/jquery.js"></script>
    <script src="node_modules/telegram-api-js/dist/telegramApi.js"></script>
<body>
</html>
```

3) Set your app configuration
```
/* You should register your application on https://my.telegram.org/ */
telegramApi.setConfig({
  app: {
    id: 0, /* App ID */
    hash: 'qwertyasdfghzxcvbnqwertyasd', /* App hash */
    version: '0.0.0' /* App version */
  },
  server: {
    test: [
      {
        id: 2, /* DC ID */
        host: '0.0.0.0',
        port: 443
      }
    ],
    production: [
      {
        id: 2, /* DC ID */
        host: '0.0.0.0',
        port: 123
      }
    ]
  }
});
```

4) Check your status
```
telegramApi.getUserInfo().then(function(user) {
    if (user.id) {
        // You have already signed in
    } else {
        // Log in
    }
});
```

[API documentation](./docs/API.md)
