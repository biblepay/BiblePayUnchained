using System;
using System.Data;
using static BiblePayCommon.DataTableExtensions;
using static Unchained.Common;
using static BiblePayCommon.Common;
using MongoDB.Driver;
using static BiblePayCommon.EntityCommon;
using System.Collections.Generic;
using System.Web.UI;

namespace Unchained
{
    public partial class ChatViaDatabase : BBPPage
    {

        protected string _roomhash = String.Empty;
        protected string _objid = String.Empty;
        protected string _roomname = String.Empty;
        protected UICommon.ChatStructure _chat = new UICommon.ChatStructure();
        public static string GetRoomHashCode(string ParticipantID1, string sParticipantID2)
        {
            List<string> l = new List<string>();
            l.Add(ParticipantID1);
            l.Add(sParticipantID2);
            l.Sort();
            string sParticipants = String.Join("", l.ToArray());
            string sHashCode = BiblePayCommon.Encryption.GetSha256HashI(sParticipants);
            return sHashCode;
        }

        public static string ParticipantNotMe(Page p, UICommon.ChatStructure c)
        {
            if (c.startedByUser != gUser(p).id)
            {
                return c.startedByUser;
            }
            return c.chattingWithUser;
        }
        public static void AddChatParticipant(Page p, ref UICommon.ChatStructure c)
        {
            if (c.participants == null)
            {
                c.participants = new List<string>();
            }
            if (c.participants.Contains(gUser(p).id))
                return;

            c.participants.Add(gUser(p).id);
            c.NeedsRefreshed = true;
            c.LastTyper = gUser(p).id;
            UICommon.dictChats2[c.chatGuid] = c;
        }
        protected new void Page_Load(object sender, EventArgs e)
        {
            string id = Request.QueryString["id"] ?? "";
            if (gUser(this).LoggedIn == false)
            {
                return;
            }
            bool fGot = UICommon.GetChatStruct(Common.gUser(this).id, out _chat);
            if (fGot)
            {
                AddChatParticipant(this, ref _chat);
                _roomhash = GetRoomHashCode(_chat.chattingWithUser, _chat.startedByUser);
            }
            else
            {
                UICommon.MsgBox("Failure", "The chat no longer exists.", this);
            }

            User u1 = gUserById(this, _chat.startedByUser);
            User u2 = gUserById(this, _chat.chattingWithUser);
            if (u1.EmailAddress == null || u2.EmailAddress == null)
            {
                UICommon.MsgBox("Failure", "The chat between these two users is missing key elements.", this);
            }

            _roomhash = GetRoomHashCode(u1.id, u2.id);
            this.Title = "Chat - with " + u1.FirstName + " and " + u2.FirstName;
            // Store the chat guid
            BiblePayCommon.Entity.Chat c1 = (BiblePayCommon.Entity.Chat)Common.GetObjectWithFilter(Common.IsTestNet(this), "Chat", "RoomHash='" + _roomhash + "'");

            if (c1.id == null)
            {
                c1 = new BiblePayCommon.Entity.Chat();
                c1.Participant1 = _chat.chattingWithUser;
                c1.Participant2 = _chat.startedByUser;
                c1.RoomHash = _roomhash;
                DACResult d0 = BiblePayDLL.Sidechain.InsertIntoDSQL(Common.IsTestNet(this), c1, gUser(this));
            }
            _objid = c1.id;
            _roomname = this.Title;
        }
        public string GetChatPanel()
        {
            if (_roomhash == "" || _objid=="")
            {
                return "Unknown chat room.";
            }

            string div = "<h4>" + _roomname + "</h4><br>";
            div += UICommon.GetChatComments(IsTestNet(this), _objid, this, _chat);
            return div;
        }
    }
}