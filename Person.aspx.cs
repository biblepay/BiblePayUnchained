using BiblePayCommonNET;
using System;
using System.Data;
using System.Web;
using System.Web.UI;
using static BiblePayCommon.Common;
using static BiblePayCommon.DataTableExtensions;
using static BiblePayCommon.Entity;
using static BiblePayCommonNET.CommonNET;
using static BiblePayCommonNET.UICommonNET;
using static Unchained.Common;

namespace Unchained
{
    public partial class Person : BBPPage
    {
        protected new void Page_Load(object sender, EventArgs e)
        {

        }


        public static DACResult AmIFriend(Page p, string sFriendUserGuid, string sMyUserGuid)
        {
            DACResult r = new DACResult();
            
            DataTable dtOriginal = BiblePayDLL.Sidechain.RetrieveDataTable2(IsTestNet(p), "FriendRequest");
            string sSnippet1 = "userid='" + sFriendUserGuid + "' and requesterid='" + sMyUserGuid + "'";
            DataTable dt1 = dtOriginal.FilterDataTable(sSnippet1);
            if (sMyUserGuid == sFriendUserGuid)
            {
                r.Result = "Me";
                r.Alt = "Your profile";
                r.Event = "";
                r.TXID = "SELF";
                r.Error = "Sorry, you cannot become a friend with yourself.";
                return r;
            }
            if (dt1.Rows.Count > 0)
            {
                r.Result = "Waiting for their Acceptance";
                r.Alt = "Waiting for them to accept your request.";
                r.Event = "";
                r.TXID = "FRIEND_REQUEST_SENT";
                r.Error = "Sorry, you already have a friend request in to this person.";
                return r;
            }
            string sSnippet2 = "requesterid='" + sFriendUserGuid + "' and userid='" + sMyUserGuid + "'";
            dtOriginal = BiblePayDLL.Sidechain.RetrieveDataTable2(IsTestNet(p), "FriendRequest");

            dt1 = dtOriginal.FilterDataTable(sSnippet2);
            if (dt1.Rows.Count > 0)
            {
                r.Result = "Accept friend Request";
                r.Alt = "Accept this person as a friend.";
                r.Event = "AcceptFriendRequest";
                r.TXID = "WAITING_FOR_MY_ACCEPTANCE";
                r.Error = "Sorry, this person already has a friend request in to you.";
                return r;
            }

            dtOriginal = BiblePayDLL.Sidechain.RetrieveDataTable2(IsTestNet(p), "Friend");
            dt1 = dtOriginal.FilterDataTable(sSnippet1);
            if (dt1.Rows.Count > 0)
            {
                r.Result = "Friends";
                r.Alt = "You are friends with this person. ";
                r.Event = "";
                r.TXID = "FRIENDS";
                r.Error = "Sorry, this person is already friends with you.";
                return r;
            }
            dtOriginal = BiblePayDLL.Sidechain.RetrieveDataTable2(IsTestNet(p), "Friend");

            dt1 = dtOriginal.FilterDataTable(sSnippet2);
            if (dt1.Rows.Count > 0)
            {
                r.Result = "Friends";
                r.TXID = "FRIENDS";
                r.Error = "Sorry, you are already friends with this person.";
                r.Event = "";
                r.TXID = "FRIENDS";
                r.Alt = "You are friends with this person. ";
                return r;
            }
            // By default, DACResult returns true if there is no error:
            r.TXID = "";
            r.Event = "AddFriendRequest";
            r.Result = "Add Friend";
            return r;
        }
        protected override void Event(BBPEvent e)
        {
            if (e.EventName == "AddTimeline_Click")
            {
                Timeline t = new Timeline();
                t.Body = e.Extra.Body;
                t.UserID = gUser(this).id;
                BiblePayCommon.Common.DACResult r = DataOps.InsertIntoTable(this, IsTestNet(this), t, gUser(this));
                if (r.fError())
                {
                    BiblePayCommonNET.UICommonNET.MsgModal(this, "Error", "Sorry, the timeline was not saved.", 500, 200, true);
                    return;
                }
                else
                {
                    ToastLater(this, "Success", "Your timeline entry has been saved!");
                }

            }
            else if (e.EventName == "EditUserProfile_Click")
            {
                Response.Redirect("Profile");
            }
            else if (e.EventName == "AddTimelineAttachment_Click")
            {
                string sURL = "UnchainedUpload?action=setattachment&parentid=" + e.EventValue;
                Response.Redirect(sURL);
            }
            else if (e.EventName == "AcceptFriendRequest_Click")
            {
                BiblePayCommon.Entity.FriendRequest f = new FriendRequest();
                f.RequesterID = gUser(this).id;
                f.UserID = e.EventValue;
                if (e.EventValue == "" || f.UserID == f.RequesterID)
                {
                    UICommon.MsgBox("Error", "Sorry, you cannot be friends with yourself. ", this);
                    return;
                }
                DACResult r = AmIFriend(this.Page, f.UserID, f.RequesterID);
                if (r.fError())
                {
                    UICommon.MsgBox("Error", r.Error, this);
                    return;
                }
                f.deleted = 1;
                DACResult r1 = DataOps.InsertIntoTable(this, IsTestNet(this), f, gUser(this));
                BiblePayCommon.Entity.Friend f1 = new Friend();
                f1.RequesterID = f.RequesterID;
                f1.UserID = f.UserID;
                r1 = DataOps.InsertIntoTable(this, IsTestNet(this), f1, gUser(this));

                if (r1.fError())
                {
                    BiblePayCommonNET.UICommonNET.MsgModal(this, "Error", "Sorry, the friend could not be added.", 500, 200, true);
                    return;
                }
                else
                {
                    ToastLater(this, "Success", "You are now friends!");
                }

            }
            else if (e.EventName == "AddFriendRequest_Click")
            {
                BiblePayCommon.Entity.FriendRequest f = new FriendRequest();
                f.RequesterID = gUser(this).id;
                f.UserID = e.EventValue;
                if (e.EventValue == "" ||  f.UserID == f.RequesterID)
                {
                    UICommon.MsgBox("Error", "Sorry, you cannot be friends with yourself. ", this);
                    return;
                }
                DACResult r = AmIFriend(this.Page, f.UserID, f.RequesterID);
                if (r.fError())
                { 
                    UICommon.MsgBox("Error", r.Error, this);
                    return;
                }
                DACResult r1 = DataOps.InsertIntoTable(this, IsTestNet(this), f, gUser(this));
                if (r1.fError())
                {
                    BiblePayCommonNET.UICommonNET.MsgModal(this, "Error", "Sorry, the friend request was not saved.", 500, 200, true);
                    return;
                }
                else
                {
                    ToastLater(this, "Success", "Your Friends Request has been sent!");
                }

            }
            else if (e.EventName == "AddTimelineComment_Click")
            {
                UICommon.MsgInput(this, "AddedTimelineComment_Click", "Add a timeline comment", "Type your comment:"
                              , 700,  "", "", UICommon.InputType.multiline, false, e.EventValue);

            }
            else if (e.EventName == "AddedTimelineComment_Click")
            {
                if (!Common.gUser(this).LoggedIn)
                {
                    UICommon.MsgBox("Error", "You Must be logged in first.", this);
                    return;
                }

                if (e.EventValue == "")
                {
                    UICommon.MsgBox("Error", "Sorry, cannot locate object.", this.Page);
                    return;
                }

                BiblePayCommon.Entity.comment1 o = new BiblePayCommon.Entity.comment1();
                o.UserID = Common.gUser(this).id;
                o.Body = HttpUtility.UrlDecode(BiblePayCommon.Encryption.Base64Decode(e.Extra.Output.ToString()));

                o.ParentID = e.EventValue;

                BiblePayCommon.Common.DACResult r = DataOps.InsertIntoTable(this, Common.IsTestNet(this), o, Common.gUser(this));
                if (!r.fError())
                {
                    BiblePayCommonNET.UICommonNET.ToastNow(this.Page, "Saved", "Your comment has been Saved!");
                }
                else
                {
                    UICommon.MsgBox("Error while inserting comment", "Sorry, the comment was not saved: " + r.Error, this);
                }
            }
        }

        protected string ToHTML(string sData)
        {
            sData = sData.Replace("\r\n", "<br>");
            return sData;
        }

        public static string GetFriendsList(bool fTestNet, string sUserID)
        {
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable2(fTestNet, "Friend");
            dt = dt.FilterDataTable("RequesterID='" + sUserID + "' or UserID='" + sUserID + "'");
            string sList = "userid in ('" + sUserID + "',";

            if (dt.Rows.Count == 0)
            {
                sList = Mid(sList, 0, sList.Length - 1);
                sList += ")";
                return sList;
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string sRequester = dt.Rows[i]["RequesterID"].ToString();
                string sRequestedBy= dt.Rows[i]["UserID"].ToString();
                if (sRequestedBy != sUserID)
                    sList += "'" + sRequestedBy + "',";
                else
                    if (sRequester != sUserID)
                    sList += "'" + sRequester + "',";
            }
            sList = Mid(sList, 0, sList.Length - 1);
            sList += ")";
            return sList;
        }


        protected string GetPerson()
        {
            BiblePayPaginator.Paginator _paginator = new BiblePayPaginator.Paginator();
            _paginator.Page = this;
            string sID = Request.QueryString["id"].ToNonNullString();
            string sFirstName = Request.QueryString["firstname"].ToNonNullString();
            string sLastName = Request.QueryString["lastname"].ToNonNullString();
            bool fHomogenized = Request.QueryString["homogenized"].ToNonNullString() == "1";

            if (sFirstName == "")
            {
                sFirstName = gUser(this).FirstName;
                sLastName = gUser(this).LastName;
            }
            // For each timeline entry, pull in attachments (mp4, mp3, video, pdf) - Sort timeline desc:
            User u = gUser(this, sFirstName, sLastName);
            bool fMe = (u.id == gUser(this).id);
            string html = "<div id='user" + u.id + "'>";

            if (u.id==null)
            {
                html += "Sorry, we cannot find this user.</div>";
                return html;
            }
            html += "";

            DateTime dtBirthday = BiblePayCommon.Common.ConvertFromUnixTimestamp(u.BirthDate);
            TimeSpan t = DateTime.Now.Subtract(dtBirthday);
            // Add friend request button (Unless they are already friends); This can say FRIEND

            DACResult r = AmIFriend(this, u.id, gUser(this).id);
            string sAddFriendButton = "";
            if (r.Result == "Me")
            {
                // No Friend Request button for SELF
            }
            else
            {
                sAddFriendButton = UICommon.GetStandardButton(u.id, r.Result, r.Event, r.Alt);
            }

            string sUserAvatar = "<img src='" + u.AvatarURL + "' class='person' />";
            string sUserAnchor = UICommon.GetStandardAnchor("ancUser", "EditUserProfile", "", sUserAvatar, "Edit your User Profile Fields");
            if (!fMe)
                sUserAnchor = sUserAvatar;

            html += "<table class='saved2'><tr><td rowspan=7>" + sUserAnchor + "</td><td>Name: " + u.FullUserName() + "</td></tr>";
            html += "<tr><td>Age: " + (t.Days / 365).ToString() + "</td></tr>";
            html += "<tr><td>Gender: " + u.Gender;
            html += "<tr><td>Telegram: <a href='" + u.TelegramLinkURL + "'>" + u.TelegramLinkName + "</a>";
            
            html += "<tr><td>" + u.TelegramLinkDescription;
            // Their video channel:
            string sVideoAnchor = "<a href=VideoList?lastname=" + u.LastName + "&firstname=" + u.FirstName + ">My Video Channel</a>";

            html += "<tr><td>" + sVideoAnchor;

            if (gUser(this).LoggedIn)
            {
                html += "<td>" + sAddFriendButton;
            }
                
            html += "</tr>";
            html += "</table>";
            html +=  "<br><br>";
           
            html += "<div class='person' ><div class='tab'><button class='tablinks' onclick=\"openProfile(event, 'Public');return false;\">Public</button>"
                 + "<button class='tablinks' onclick=\"openProfile(event, 'Friends');return false;\">Friends Only</button>"
                 + "<button class='tablinks' onclick=\"openProfile(event, 'Professional');return false;\">Professional</button>"
                 + "<button class='tablinks' onclick=\"openProfile(event, 'Religious');return false;\">Beliefs</button></div>";

            if (u.PrivateText.ToNonNullString().Length > 1)
            {
                html += "<div id='Friends' class='tabcontent'>" + ToHTML(u.PrivateText) + "</div>";
            }

            if (u.ProfessionalText.ToNonNullString().Length > 1)
            {
                html += "<div id='Professional' class='tabcontent'>" + ToHTML(u.ProfessionalText) + "</div>";
            }
            if (u.PublicText.ToNonNullString().Length > 1)
            {
                html += "<div id='Public' class='tabcontent'>" + ToHTML(u.PublicText) + "</div>";
            }
            if (u.ReligiousText.ToNonNullString().Length > 1)
            {
                html += "<div id='Religious' class='tabcontent'>" + ToHTML(u.ReligiousText) + "</div>";
            }
            html += "</div><br><br><script>openProfile(this, 'Public');</script>";

            // For each timeline entry...
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable2(IsTestNet(this), "Timeline");
            if (fHomogenized)
            {
                string sMyFriendsList = GetFriendsList(IsTestNet(this), gUser(this).id);
                DataOps.FilterDataTable(ref dt, sMyFriendsList);
            }
            else
            {
                DataOps.FilterDataTable(ref dt, "userid='" + u.id + "'");
            }

            // Default Sort, time desc:
            // TODO: Figure out why this line doesnt work, but the OrderBy works: dt= dt.SortBy("time desc");

            dt = dt.OrderBy("time desc");

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                // Add Attachment button
                string sAddTimelineAttachmentButton = UICommon.GetStandardButton(dt.Rows[i]["id"].ToString(),
                    "<i class='fa fa-file'></i>", "AddTimelineAttachment", "Add media to this post, such as a URL, a video, an mp3, a pdf, an image...etc");

                string sAddCommentButton = UICommon.GetStandardButton(dt.Rows[i]["id"].ToString(),
                    "<i class='fa fa-chat'></i>", "AddTimelineComment", "Add a Comment to this timeline event...");
                
                // Add the User - Said - @ timestamp (head of the timeline entry):
                string sTime = dt.GetColDateTime(i, "time").ToString();
                string sEntry = UICommon.GetUserAvatarAndName(this, dt.Rows[i]["userid"].ToString(), true) + " • " + sTime + ":";

                string sTimeline = "<div>" + sEntry + "<br>"
                    + "<textarea class='comments' rows='2' cols='200' name='timeline_" + dt.Rows[i]["id"].ToString() + "' readonly>"
                    + dt.Rows[i]["body"].ToString() + "</textarea>&nbsp;";

                if (gUser(this).id == u.id)
                {
                    sTimeline += sAddTimelineAttachmentButton + "&nbsp;";
                }

                sTimeline += sAddCommentButton + "<br></div>";

                // Display the attachments
                DataTable dtAttachments = BiblePayDLL.Sidechain.RetrieveDataTable2(IsTestNet(this), "video1");
                DataOps.FilterDataTable(ref dtAttachments, "attachment=1 and parentid='" + dt.Rows[i]["id"].ToString() +"'");
                string sGallery = "<div class='person'>"
                    + UICommon.GetGallery(this, dtAttachments, _paginator, "any", 25, 250, 250) + "</div>";

                sTimeline += sGallery + "<br>";
                sTimeline += UICommon.GetComments(IsTestNet(this), dt.Rows[i]["id"].ToString(), this, true);
                // Display the comments for the timeline entry
                html += sTimeline;
            }
            // Add a new Timeline 

            string sAddTimelineButton = "<input class='pc90' autocomplete='off' id='timeline1'></input><button id='btntimeline1' onclick=\""
                   + "var o=document.getElementById('timeline1');var e={};e.Event='AddTimeline_Click';e.Value='" 
                   + sID + "';e.Body=o.value;BBPPostBack2(null, e);\">Say Something</button> ";

            if (gUser(this).id == u.id)
            {
                html += "<br>" + sAddTimelineButton;
            }
            html += "</div>";
            return html;
        }

        protected void btnSummary_Click(object sender, EventArgs e)
        {
        }

        protected void btnDetail_Click(object sender, EventArgs e)
        {
        }
    }
}
