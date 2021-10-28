

var mAccessHash = "0";
var mChatID = 0;
var mTitle = "";
var mPhoneNumber = "";

function send_out(data)
{

	alert('sending ' + data + ' ' + mAccessHash + ' ' + mPhoneNumber);
	if (mAccessHash != "0" && data != "")
	{
        telegramApi.sendMessage(mChatID, data, mAccessHash).then(function(updates)
        {
	    // Confirm Sent
	    alert('Message Sent');
    	});
	}
}


	telegramApi.setConfig({
	  app: {
	    id: 7348399, /* App ID */
	    hash: '3d9d71b361c3ddb24c7611d8a440f551',/* App hash */
	    version: '0.15.1' /* App version */
	  },
	  server: {
	    test: [
	        { id: 1, host: '149.154.175.10', port: 80 },
	        { id: 2, host: '149.154.167.40', port: 443 },
	        { id: 3, host: '149.154.175.117', port: 80 },
	    ],
	    production: [
			{ id: 1, host: '149.154.167.50', port: 443 },
	        { id: 2, host: '149.154.167.50', port: 443 },
	        { id: 3, host: '149.154.175.100', port: 80 },
	        { id: 4, host: '149.154.167.50', port: 443 },
	        { id: 5, host: '149.154.171.5', port: 80 },
	    ],
	    https: [
	        { id: 1, host: 'pluto.web.telegram.org', port: 80 },
	        { id: 2, host: 'venus.web.telegram.org', port: 80 },
	        { id: 3, host: 'aurora.web.telegram.org', port: 80 },
	        { id: 4, host: 'vesta.web.telegram.org', port: 80 },
	        { id: 5, host: 'flora.web.telegram.org', port: 80 },
	    ]
	  }
	});


	telegramApi.getUserInfo().then(function(user)
	{
	    if (user.id)
	    {
		    // You have already signed in
		    // var chatid= 777000; //telegram, works
		    // var chatid=-529483766; //tokemom (works)
		    // var chatid=-1001508518578;//truthbook
		    var chatid=1551158058; //BCN
            chatid = 1508518578; // Remove the minus 100 for some reason, for Truthbook
		    chatid=1551158058; //BCN

			alert(user.id);


			telegramApi.getDialogs(0, 99).then(function (data)
		    {
			   data.result.chats.forEach(function(chat)
			   {
  		          if (chat._==="channel" && chatid==chat.id)
			      {
			    	 console.log(chat.id);
			    	 console.log(chat.access_hash);
		    	  	 console.log(chat.title);
	    		  	 mTitle = chat.title;
    			  	 mAccessHash = chat.access_hash;
     			  	 mChatID = chat.id;
			      }
	       })
	});
    }
    else
    {
	        // Log in to telegram
	        if (mPhoneNumber == "")
	        {
	         	return;
	        }
	        console.log('loggingin');
	        telegramApi.sendCode(mPhoneNumber).then(function(sent_code)
			{

		    	if (!sent_code.phone_registered) {
		    	    alert('Sorry, you are not a registered telegram user.');
		    	    return;
			    }

			    // phone_code_hash will need to sign in or sign up
			    window.phone_code_hash = sent_code.phone_code_hash;
			    var code = prompt('Enter the telegram code >', '0');

			    telegramApi.signIn(mPhoneNumber, window.phone_code_hash, code).then(function()
			    {
				    // Sign in complete
					alert('signed in');

				    delete window.phone_code_hash;
				}, function(err) {
				    switch (err.type) {
				        case 'PHONE_CODE_INVALID':
				           alert('invalid telegram 5 digit code');
				           break;
				        case 'PHONE_NUMBER_UNOCCUPIED':
				            alert('user not registered');
				            break;
				    }
				});
		});
	    }
	});



