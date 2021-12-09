
//BibleVerses

var refTagger = {
            settings: {
                bibleVersion: "ESV"
            }
};

(function (d, t) {
            var g = d.createElement(t), s = d.getElementsByTagName(t)[0];
            g.src = '//api.reftagger.com/v2/RefTagger.js';
            s.parentNode.insertBefore(g, s);
}(document, 'script'));

function getParameterByName(name, url = window.location.href) {
    name = name.replace(/[\[\]]/g, '\\$&');
    var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, ' '));
}

// Infinite scrolling paginator; start at record 29
var iGallery = 29;
function MakeMoreVisible() {
    //alert(iGallery);
    for (var i = iGallery; i < iGallery + 30; i++)
    {
        // $('#gtd' + i.toString()).css('visibility', 'visible');
        $('#gtd' + i.toString()).toggleClass('galleryinvisible');
    }
    iGallery += 30;
}

// Scroll event listener
window.addEventListener('scroll', () => {
    const {
        scrollTop,
        scrollHeight,
        clientHeight
    } = document.documentElement;

    var signalType = 0;
    if (scrollTop === 0)
    {
        signalType = 1;
    }

    if (scrollTop + clientHeight >= scrollHeight - 50) {
        signalType = 2;
    }

    if (signalType !== 0)
    {
        var pag = "0" + getParameterByName('pag');
        var nPag = parseInt(pag);
        var url = window.location.href;
        var nOffset = 0;

        if (signalType === 1) {
            nOffset = -30;
        }
        else if (signalType === 2) {
            nOffset = 30;
        }

        if (url.includes('TelegramChat') && signalType === 1) {
            nOffset = 30;
        }

        var nNew = nPag + nOffset;
        if (nNew < 1)
            nNew = 1;
        // First if this param exists; remove
        if (nPag > 0) {
            var sOld = "pag=" + (nPag).toString();
            
            var sNew = "pag=" + (nNew).toString();
            url = url.replace(sOld, sNew);
        }
        else {
            var fQ = url.includes('?');
            url += fQ ? '&pag=' : '?pag=';
            url += (nNew).toString();
        }
        // If the paginator is enabled for the page:
        if (nPag < 2 && nNew < 2) {
            // Noop
            return;
        }
        if (url.includes('TelegramChat') && signalType === 1) {
            window.location.href = url;
        }
        if (url.includes('VideoList')) {
            window.location.href = url;
        }
    }
}, {
    passive: true
});


// Global application function key handler (Reserved for future use)
$(document).ready(function () {
    var ctrlDown = false,
        ctrlKey = 17,
        cmdKey = 91,
        vKey = 86,
        cKey = 67;
        // Key Mappings: 112=f1, 123=f12, 27=esc, w=87

    $(document).keydown(function (e) {
        if (e.keyCode === ctrlKey || e.keyCode === cmdKey) ctrlDown = true;
    }).keyup(function (e) {
        if (e.keyCode === ctrlKey || e.keyCode === cmdKey) ctrlDown = false;
        // F1-F20
        if (e.keyCode === 113) {
            window.location = "VideoList";
        }
        else if (e.keyCode === 115) {
            window.location = "Status";
        }
    });

    $(".no-copy-paste").keydown(function (e) {
        // if (ctrlDown && (e.keyCode === vKey || e.keyCode === cKey)) return false;
    });
   
    // Document Ctrl + C/V 
    $(document).keydown(function (e) {

        if (ctrlDown && (e.keyCode === 68)) {
            // ctrl d example; take user to videos:
            //window.location = "VideoList";
        }

        if (ctrlDown && (e.keyCode === cKey)) {
            // console.log("Document catch Ctrl+C");
        }
        if (ctrlDown && (e.keyCode === vKey)) {
            // console.log("Document catch Ctrl+V");
        }
    });
});
   

// Continue with core JS functionality here:
function showModalDialog(title, body, width, height, fReload)
{
            $("#divdialog").dialog({
                "body": body,
                "title": title,
                "show": true,
                "width": width,
                "height": height,
                buttons: {
                    OK: function () {
                        if (fReload) {
                            document.location.reload();
                        }
                        else {
                            $(this).dialog("close");
                        }
                    }
                },
            });
            var e = document.getElementById("spandialog");
            e.innerHTML = body;
}

function showModalDialogWithRedirect(title, body, width, height, sRedirect)
{
    $("#divdialog").dialog({
        "body": body,
        "title": title,
        "show": true,
        "width": width,
        "height": height,
        buttons: {
            OK: function ()
            {
               window.location.href = sRedirect;
               $(this).dialog("close");
            
            }
        },
    });
    var e = document.getElementById("spandialog");
    e.innerHTML = body;
}

function showModalDialogWithCancel(title, body, width, height) {
    $("#divdialog").dialog({
        "body": body,
        "title": title,
        "show": true,
        "width": width,
        "height": height,
        buttons: {
            CANCEL: function ()
            {
                    $(this).dialog("close");
            }
        },
    });
    var e = document.getElementById("spandialog");
    e.innerHTML = body;
}



function closeModalDialog2() {

    $('#divdialog').dialog("destroy");
    $('#divdialog').html("");
    //$(this).dialog("close");
}


function updateModalEmptyDialog(body) {
    var e = document.getElementById("spandialog");
    e.innerHTML = body;
}

$(document).click(function () {
    //alert('click');
    if (!$(event.target).closest('#divdialog').length && !$(event.target).is('#divdialog')) {
        var clickedID = event.target.id;
        if (clickedID !== "hfNewButton1")
        {
            if ($("#divdialog").is(":visible")) {
                try {
                    $('#divdialog').dialog('close');
                } catch (e) {
                    //
                }
            }

        }
    }
});

function testPerformance() {
    // Lets test BBPPostback vs standard callback
    for (var i = 0; i < 10; i++)
    {
        var E = {};
        E.actionname = 'test';
        E.action = i.toString();
        BBPPostBack2(null, E);
        console.log(i);
    }
}


function showModalEmptyDialog(sParent, title, body, width, height)
{
    var oParent = document.getElementById(sParent);
    if (!oParent) {
        alert('error 11112021');
        return;
    }
    
    $("#divdialog").dialog({
        "body":body,
        "title": title,
        "show": true,
        "width": width,
        "height": height,
        "position": {
            my: 'left+370 top+80',
            at: 'left top',

        },
        close: function (event, ui) {
            $(this).dialog('close');
        },
        open: function (event, ui) {
            //$("#spandialog").focus();
        }
    });

    var e = document.getElementById("spandialog");
    e.innerHTML = body;
    $("#spandialog").parent().parent().focus();
}

function updateNotificationCount() {
    var c = "setRemoteValue('refreshnotifications', 'id|' + 'refreshnotifications', 'gNotificationConsole');";
    setTimeout(c, 1000);
}

        function transmitVote(id, action, table, sParentType, affectedID, affectedID2)
        {
            setRemoteValue('voting', id + "|" + action + "|" + table + "|" + sParentType, affectedID, affectedID2);
            updateNotificationCount();
        }

function transmitRead(id) {
    setRemoteValue('read_notification', id + "|read_notification|Notification|", "","");
    updateNotificationCount();
}

        function transmitfollow(id, action, affectedID, affectedID2) {
            // Change class in affectedID2
            var curaction = $("#follow1" + id).html();
            if (curaction === "Unfollow") {
                $("#span1" + id).addClass("fa-heart");
                $("#span1" + id).removeClass("fa-heart-broken");
                $("#follow1" + id).html("Follow");
                setRemoteValue('voting', id + "|unfollow", '', '');
            }
            else {
                $("#span1" + id).addClass("fa-heart-broken");
                $("#span1" + id).removeClass("fa-heart");
                $("#follow1" + id).html("Unfollow");
                setRemoteValue('voting', id + "|follow", '', '');
            }
        }

        function showToast() {
            $.toast({ heading: heading, text: text1, icon: 'info', loader: true, loaderBg: '#9EC600' });
        }


        function populateWiki(url) {
            setRemoteValue('wiki', url, 'wiki', '');
        }

        function GetItem(n) {
            if (!localStorage.getItem(n))
                return '';
            return localStorage.getItem(n);
        }
        function GetStorage0(n) {
            var v = GetItem(n);
            var s = n + "<COL>" + v + "<ROW>";
            return s;
        }
        function SetItem(sKey, sValue) {
            localStorage.setItem(sKey, sValue);
            //TransmitSerializedLocalStorage();
        }

        /*
        function getRemoteValue(sessionid) {
            $.ajax({    
                type: "GET",
                url: "Legacy.ashx/sessionid=" + sessionid,
                dataType: "html",   //expect html to be returned                
                success: function (response) {
                    var s = response + "%";
                    $("#spanLoader1").html(s);
                }
            });
        }
        */


var lastupdate = 0;
function UpdateChatWindow() {
    var ts = Date.now();
    var elapsed = ts - lastupdate;
    if (elapsed > 1500) {
        setRemoteValue('voting', 'chat|', 'chatinner', '');
        lastupdate = ts;
        setTimeout("UpdateChatWindow()", 3000);
    }
}

var lastupdatepollchat = 0;

function PollChat() {
    var ts = Date.now();
    var elapsed = ts - lastupdatepollchat;
    if (elapsed > 1500) {
        var oComment = document.getElementById("txtComment");
        if (oComment.value.length === 0) {
            setRemoteValue('voting', 'pollchat|', '', '');
            setTimeout("PollChat()", 3000);
        }
    }
}

function PollChat2() {
    setInterval(function () {
        setRemoteValue2('voting', 'pollchat|', '', '');
    }, 1500);

}
function setRemoteValue2(actionname, data1, elementToUpdate, elementToUpdate2) {

    if (remoterequest != null) {
   if (remoterequest.readyState != 4)
        return false;
    }
    remoterequest = $.ajax({
        type: "POST",
        url: "LP.aspx/" + actionname,
        data: { mydata: data1 },
        headers: { headeraction: data1 },
        success: function (response) {
            
            var oResponse;

            try {
                oResponse = JSON.parse(response);
            }
            catch (e) {
                console.log(actionname);
                console.log(data1);
                console.log(response);
            }

            if (oResponse.Error !== null) {
                alert(oResponse.Error);
                return false;
            }

            if (oResponse.CustomResponse === "ChatResponse") {
                PollChatComplete(oResponse.CustomResponseValue);
                return true;
            }
            var e1 = document.getElementById(elementToUpdate);
            var e2 = document.getElementById(elementToUpdate2);

            if (e1) {
                e1.innerHTML = oResponse.Div1;
                console.log('updated');
            }
            if (e2) {
                e2.innerHTML = oResponse.Div2;
            }
            return true;
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log(jqXHR);
            console.log(textStatus);
            console.log(errorThrown);
            if (!notified) {
                alert('Wallet error' + textStatus);
                notified = true;
                return false;
            }
        }
    });
}


function PollChatComplete(returnValue) {

    if (returnValue === "1") {
        var e = {};
        e.EventName = "PollChatComplete";
        BBPPostBack3(null, e);

    }
    else {
        //setTimeout("PollChat()", 12000);
    }
}

var lastvalue1 = "";
var notified = false;
var remoterequest;
function setRemoteValue(actionname, data1, elementToUpdate, elementToUpdate2) {
    if (remoterequest) {
        remoterequest.abort();
        remoterequest = null;
    }
     remoterequest = $.ajax({
                type: "POST",
                url: "LP.aspx/" + actionname,
                data: { mydata: data1 },
                headers: { headeraction: data1 },
                success: function (response) {

                    var oResponse;

                    try
                    {
                        oResponse = JSON.parse(response);
                    }
                    catch (e)
                    {
                        console.log(actionname);
                        console.log(data1);
                        console.log(response);
                    }

                    if (oResponse.Error !== null) {
                        alert(oResponse.Error);
                        return false;
                    }

                    if (oResponse.CustomResponse === "ChatResponse") {
                        PollChatComplete(oResponse.CustomResponseValue);
                        return true;
                    }
                    var e1 = document.getElementById(elementToUpdate);
                    var e2 = document.getElementById(elementToUpdate2);

                    if (e1)
                    {
                        e1.innerHTML = oResponse.Div1;
                        console.log('updated');
                    }
                    if (e2)
                    {
                        e2.innerHTML = oResponse.Div2;
                    }
                    return true;
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    console.log(jqXHR);
                    console.log(textStatus);
                    console.log(errorThrown);
                    if (!notified) {
                        alert('Wallet error' + textStatus);
                        notified = true;
                        return false;
                    }
                }
            });
        }





function setRemoteValue2Retired(actionname, data1, elementToUpdate)
{
    $.ajax({
        type: "POST",
        url: "LP.aspx/" + actionname,
        data: { mydata: data1 },
        headers: { headeraction: data1 },
        success: function (response) {
        // Can we make a json response here....

         var e = document.getElementById(elementToUpdate);
         if (e) {
             e.innerHTML = response;
             console.log('updated2');
         }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log(jqXHR);
            console.log(textStatus);
            console.log(errorThrown);
            if (!notified) {
                alert('Wallet error' + textStatus);
                notified = true;
                return false;
            }
        }
    });
}

        var spinning = false;
        function showSpinner() {
            spinning = true;
            UpdateSpinner();
            $("#load1").removeClass("hidden");
        }

        function hideSpinner() {
            spinning = false;
            $("#load1").addClass("hidden");
            nSpinValue = 0;
        }

        var nSpinValue = 0;
        function UpdateSpinner() {
            if (spinning) {
                nSpinValue++;
                var s = "Working[0]...<br>ETA: N/A<br>Elapsed: " + nSpinValue + "s";
            }
        }


function afterload() {
           /*
            * The only reason I'm saving this is in case we ever need a footer... which I doubt.
            var pageheight = $(".Page").height() + 70;
            if (pageheight < $(window).height()) {
                $(".footer_wrapper").addClass("fixed");
                $(".footer_wrapper").removeClass("hidden");
            } else {
                $(".footer_wrapper").removeClass("fixed");
                $(".footer_wrapper").removeClass("hidden");
            }
            */
        }

function closeNav() {
            $("#mySidenav").css("transform", "translate(-240px,0)");
            $('#rightmenubutton').removeClass('hidden-lg');
            $('#rightmenubutton').removeClass('hidden-sm');
        }

        function openNav() {
            $("#mySidenav").css("transform", "translate(0px,0)");
        }

        function getRandomInt(max) {
            return Math.floor(Math.random() * max);
        }


var oLastHF = null;

function BBPPostBack2(oParent, ExtraObjectValues) {
    encodeComment();

    ExtraObjectValues.Iteration = getRandomInt(99999);
    var o = document.getElementById("hfPostback");
    o.type = "hidden";
    o.name = "hfPostback";
    if (!o) {
        alert('error');
        return;
    }
    var myData = JSON.stringify(ExtraObjectValues);
    myData.replace('<', '[lessthan]');
    myData.replace('>', '[greaterthan]');
    myData.replace('&', '[ampersand]');
    o.value = window.btoa(myData);
    var oButton = document.getElementById("hfNewButton1");
    if (oButton) {
        oButton.type = "submit";
        oButton.click();
    }
    // Mission Critical : Set the hfPostback back to null, or many strange things will happen in the UI...
}

function BBPPostBack3(oParent, ExtraObjectValues) {
    encodeComment();
    ExtraObjectValues.Iteration = getRandomInt(99999);
    var o = document.getElementById("hfPostback");
    o.type = "hidden";
    o.name = "hfPostback";
    if (!o) {
        alert('error');
        return;
    }
    var myData = JSON.stringify(ExtraObjectValues);
    //myData.replace('<', '[lessthan]');
    //myData.replace('>', '[greaterthan]');
    //myData.replace('&', '[ampersand]');
    o.value = window.btoa(myData);
    var oButton = document.getElementById("hfNewButton1");
    if (oButton) {
        oButton.type = "submit";
        oButton.click();
    }
    // Mission Critical : Set the hfPostback back to null, or many strange things will happen in the UI...
}


function ModPow(baseNum, exponent, modulus)
{
    var B = 0;
    var D = 0;
    B = baseNum;
    B %= modulus;
    D = 1;
    if ((exponent & 1) === 1) {
        D = B;
    }
    while (exponent > 1) {
        exponent >>= 1;
        B = (B * B) % modulus;
        if ((exponent & 1) === 1) {
            D = (D * B) % modulus;
        }
    }
    return D;
}

var sFoundationPublicKey = "4309FF1439AA24569FF22109AA437FF25AA12059FF11441AA6119FF1907AA485FF223AA19339FF11133AA4883FF2695AA53491FF39675AA31301FF12055AA108389FF59641AA35047FF6741AA6739FF1253AA7897FF1499AA14351FF2749AA3397FF641AA1865FF289AA";
function RSAEncrypt(oData1)
{
    console.log(oData1);
    var oData = window.btoa(oData1);
    var vPub = sFoundationPublicKey.split("AA");
    var encData = '';
    for (var i = 0; i < oData.length; i++)
    {
        var iChar = oData.charCodeAt(i);
        var nLoc = i % (vPub.length - 1);
        var vLoc = vPub[nLoc].split("FF");
        var n = parseInt(vLoc[0]);
        var e = parseInt(vLoc[1]);
        var nEnc = ModPow(iChar, e, n);
        encData += nEnc.toString() + "FF";
    }
    return encData;
}


// BiblePay's alternative to window.btoa 
// This prevents XSS attacks
function XSS(oData) {
    if (oData === null)
        return "";
    oData = oData.replace('<script>', '[script]');
    oData = oData.replace('</script>', '[endscript]');
    oData = oData.replace('<', '[lessthan]');
    oData = oData.replace('>', '[greaterthan]');
    oData = oData.replace('&', '[ampersand]');
    //oData = escape(oData);
    console.log(oData);
    var oData2 = window.btoa(oData);
    return oData2;
}

function XSS2(oData) {
    if (oData === null)
        return "";
    oData = oData.replace('<script>', '[script]');
    oData = oData.replace('</script>', '[endscript]');
    //oData = oData.replace('<', '[lessthan]');
    //oData = oData.replace('>', '[greaterthan]');
    //oData = oData.replace('&', '[ampersand]');
    //oData = escape(oData);
    console.log(oData);
    var oData2 = window.btoa(escape(oData));
    return oData2;
}


function encodeComment() {
    var oComment = document.getElementById('txtComment');
    if (!oComment)
        return;
    oComment.value = encodeHTMLTags(oComment.value);
}

function encodeHTMLTags(oData)
{
    oData = oData.replace(/</g, '&lt;').replace(/>/g, '&gt;');
    return oData;
}
