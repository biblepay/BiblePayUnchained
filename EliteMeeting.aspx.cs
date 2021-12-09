using BiblePayCommon;
using System.Data;
using System.Linq;
using static BiblePayCommon.DataTableExtensions;
using static Unchained.Common;
using static BiblePayCommonNET.DataTableExtensions;

namespace Unchained
{
    public partial class EliteMeeting : BBPPage
    {

        protected string GetMeeting()
        {

            // There are three types:  PUBLIC, PRIVATE, PRIVATE+FRIENDS
            // Public means anyone who comes into the room may join
            // Private is a 1:1 chat between two logged on users
            // Private+Friends starts as two, but more friends entered (friends of either party).
            string sType = Request.QueryString["type"] ?? "";
            string id = Request.QueryString["id"] ?? "";
            UICommon.ChatStructure myChat;

            if (gUser(this).LoggedIn == false)
            {
                UICommon.MsgBox("Unauthorized", "Sorry, you must be logged in to use this feature.", this);
            }
            bool fGot = UICommon.GetChatStruct(gUser(this).id, out myChat);

            string sNarrative = "";
            string sMeetingName = "";
            if (sType == "public")
            {
                sNarrative = "Public Chat";
                sMeetingName = "public";
            }
            else if (sType == "private")
            {
                if (!fGot)
                {
                    UICommon.MsgBox("Error", "Sorry, this chat can not be found.", this);
                }
                BiblePayCommon.Common.User gInvited = Common.gUserById(this,myChat.chattingWithUser);
                BiblePayCommon.Common.User gMe = Common.gUserById(this,myChat.startedByUser);
                sNarrative = "Private Chat with " + gInvited.FullUserName().ToString() + " and " + gMe.FirstName.ToString();
                sMeetingName = "private_" + myChat.chatGuid.ToString();
            }
            else if (sType == "privateplusfriends")
            {
                if (!fGot)
                {
                    UICommon.MsgBox("Error", "Sorry, this chat can not be found.", this);
                }

                sNarrative = "Private Chat Plus Friends";
                sMeetingName = "private_" + myChat.chatGuid.ToString();
                if (!myChat.NotifiedOfEntry)
                {
                    myChat.NotifiedOfEntry = true;
                    BiblePayCommonNET.UICommonNET.MsgModal(this, "Paged", "We have paged your friend.  You may move this tab to the side and continue working, and if your friend accepts the chat, they will enter the room. ", 400, 300, true);
                    UICommon.dictChats2[myChat.chatGuid] = myChat;
                }
            }
            string sRoom = "conference1";
            // APPid is the vpaas magic cookie without the suffix
            string sAppID = "vpaas-magic-cookie-84a3b874fc5348aca7f739aa03d24859";
            string sAPIKey = "vpaas-magic-cookie-84a3b874fc5348aca7f739aa03d24859/a94001";
            // Testing token
            // Jitsi Chat Only feature :  initIframeAPI();api.executeCommand('overwriteConfig',    {                toolbarButtons: ['chat']    }); 
            if (gUser(this).Administrator != 1 || !gUser(this).LoggedIn)
            {
                UICommon.MsgBox("Error", "Unauthorized.", this);
                return string.Empty;
            }
            string sJWT = BiblePayCommonNET.JAASProgram.GenerateJaasToken(gUser(this).FullUserName(), gUser(this).id, "", gUser(this).AvatarURL, sAppID, sAPIKey);
            string sHTML = "<div id='meet' /><script src='https://8x8.vc/external_api.js'></script>"
                + "<script type='text/javascript'>  let api;"
                + " const initIframeAPI=()=> {"
                + " const domain = '8x8.vc';"
                + "    const options = {"
                + "      roomName: '" + sAPIKey + "',"
                + "      jwt: '" + sJWT + "',"
                + "      width: 750,"
                + "      height: 600,"
                + "      parentNode: document.querySelector('#meet')"
                + "    };"
                + "    api = new JitsiMeetExternalAPI(domain, options);"
                + "  };\r\n"
                + "  window.onload=()=> {"
                + "  initIframeAPI();api.executeCommand('overwriteConfig',    {                dropbox: false    }); "
                + " };</script>"
                + "";

            string sData = "<h3>" + sNarrative + "</h3><br><br>" + sHTML;
           
            return sData;
        }
    }
}