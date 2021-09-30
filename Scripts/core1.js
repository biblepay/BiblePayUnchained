
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

        //CORE

        // Control Keys
$(document).ready(function () {
    var ctrlDown = false,
        ctrlKey = 17,
        cmdKey = 91,
        vKey = 86,
        cKey = 67;
     // 112=f1, 123=f12, 27=esc, w=87

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
            // ctrl d:
            //window.location = "VideoList";
        }

        if (ctrlDown && (e.keyCode === cKey)) {
          //  console.log("Document catch Ctrl+C");
        }
        if (ctrlDown && (e.keyCode === vKey)) {
            //console.log("Document catch Ctrl+V");
        }
    });
});
   


        function showModalDialog(title, body, width, height)
        {
            $("#divdialog").dialog({
                "body": body,
                "title": title,
                "show": true,
                "width": width,
                "height": height,
                buttons: {
                    OK: function () { $(this).dialog("close"); }
                },
            });
            var e = document.getElementById("spandialog");
            e.innerHTML = body;
        }

        function transmit(id,action,affectedID,affectedID2)
        {
            setRemoteValue('voting', id + "|" + action, affectedID, affectedID2);
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

        function getRemoteValue(sessionid) {
            $.ajax({    
                type: "GET",
                url: "Web.ashx/sessionid=" + sessionid,
                dataType: "html",   //expect html to be returned                
                success: function (response) {
                    var s = response + "%";
                    $("#spanLoader1").html(s);
                }
            });
        }

        function setRemoteValue(actionname, data1, elementToUpdate, elementToUpdate2) {
            $.ajax({
                type: "POST",
                url: "LP.aspx/" + actionname,
                data: { mydata: data1 },
                headers: { headeraction: data1 },
                success: function (response) {
                   
                    if (elementToUpdate === "wiki") {
                        loadWikiDocument(response);
                    }
                    else {
                        var parts = response.split("|");
                        if (parts.length > 1) {

                            if (parts[0] === "notloggedin") {
                                // User is not logged in
                                showModalDialog("Action failed", "Sorry, you must be logged in first.  Click Log In from the left menu. ", 475, 400);
                            }
                            else {
                                if (elementToUpdate) {
                                    var e = document.getElementById(elementToUpdate);
                                    e.innerHTML = parts[0];
                                }
                                if (elementToUpdate2) {
                                    var f = document.getElementById(elementToUpdate2);
                                    f.innerHTML = parts[1];
                                }
                            }
                        }
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    alert('Wallet error');
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

                $("#spanLoader1").html(s);

                getRemoteValue(1);
                setTimeout(UpdateSpinner, 1000);
            }
        }


        function afterload() {

           /*
        
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


function BBPPostBack2(oParent, ExtraObjectValues) {
    ExtraObjectValues.Iteration = getRandomInt(99999);
    
    if (oParent === null || oParent === undefined || oParent.id === undefined) {
        oParent = document.getElementById('divdialog');
    }

    var o = document.getElementById("hfPostback");
    if (!o) {
        var input = document.createElement("input");
        input.type = "hidden";
        input.className = "";
        input.id = "hfPostback";
        input.name = "hfPostback";
        oParent.appendChild(input);
        o = document.getElementById("hfPostback");
    }
    o.value = window.btoa(JSON.stringify(ExtraObjectValues));
    var oButton = document.getElementById("hfButton");
    if (!oButton) {
        var oButton1 = document.createElement("input");
        oButton1.type = "submit";
        oButton1.id = "hfButton";
        oButton1.name = "hfButton";
        oButton1.style.display = 'none';
        oParent.appendChild(oButton1);
        oButton = document.getElementById("hfButton");
    }
    oButton.click();
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


