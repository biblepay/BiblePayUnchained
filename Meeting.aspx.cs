using BiblePayCommon;
using System.Data;
using System.Linq;
using static BiblePayCommon.DataTableExtensions;
using static Unchained.Common;
using static BiblePayCommonNET.DataTableExtensions;

namespace Unchained
{
    public partial class Meeting : BBPPage
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
            bool fGot = UICommon.dictChats.TryGetValue(gUser(this).id, out myChat);

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
                    UICommon.dictChats[gUser(this).id] = myChat;
                }
            }
            
            string sData = "<h3>" + sNarrative + "</h3><br><br><iframe allow=\"camera; microphone; fullscreen; display-capture; autoplay\" "
                + "src=\"https://meet.biblepay.org/" + sMeetingName + "\" style='height: 100%; width: 100%; min-height:500px; border: 0px;'></iframe>";

            return sData;
        }
    }
}