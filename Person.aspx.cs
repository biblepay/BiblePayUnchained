using BiblePayCommonNET;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using static BiblePayCommon.Common;
using static BiblePayCommon.DataTableExtensions;
using static BiblePayCommon.Entity;
using static BiblePayCommonNET.CommonNET;
using static BiblePayCommonNET.UICommonNET;
using static Unchained.Common;
using static Unchained.UICommon;

namespace Unchained
{
    public partial class Person : BBPPage
    {
         protected User user;
        protected bool IsMe;
        protected bool IsTestNet;
        protected bool fHomogenized;
        protected DACResult IsMyFriend;
        protected User MySelf;
        protected new void Page_Load(object sender, EventArgs e)
        {
            if (Request.Path.Contains("comment"))
            {
                string data = Request.Headers["headeraction"].ToNonNullString();
                var comment = Newtonsoft.Json.JsonConvert.DeserializeObject<comment1>(data);
                var status = SaveComment(comment);
                Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(status));
                Response.End();

            }
            if (Request.Path.Contains("voting"))
            {
                string s1 = Request.Headers["headeraction"].ToNonNullString();
                string parentId = Common.GetElement(s1, "|", 1);
                string voteType = Common.GetElement(s1, "|", 0);
                var status = Voting(parentId, voteType);
                VoteSums v = GetVoteSum(IsTestNet(this), parentId);
                object o = new { status, nUpvotes = v.nUpvotes, nDownvotes = v.nDownvotes };
                Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(o));
                Response.End();
            }
            if (Request.Path.Contains("deletecomentbyid"))
            {
                string id = Request.Headers["headeraction"].ToNonNullString();
                bool fDeleted = BiblePayDLL.Sidechain.DeleteObject(Common.IsTestNet(this), "comment1",
                                          id, Common.gUser(this));
                object o = new { status = fDeleted };
                Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(o));
                Response.End();
            }
            if (Request.Path.Contains("editcomentbyid"))
            {
                object result = new { status = true };

                string data = Request.Headers["headeraction"].ToNonNullString();
                var comment = Newtonsoft.Json.JsonConvert.DeserializeObject<comment1>(data);
                comment1 comment2 = (comment1)Common.GetObject(Common.IsTestNet(this), "comment1", comment.id);
                if (comment2 == null)
                {
                    result = new { status = true };
                }
                else
                {
                    comment2.Body = HttpUtility.UrlDecode(BiblePayCommon.Encryption.Base64DecodeWithFilter(comment.Body));
                    //comment.Body = HttpUtility.UrlDecode(BiblePayCommon.Encryption.Base64DecodeWithFilter(_bbpevent.Extra.Output.ToString()));

                    DACResult r = DataOps.InsertIntoTable(this, Common.IsTestNet(this), 
                        comment2, Common.gUser(this));
                    if (!r.fError())
                    {
                        result = new { status = true, data = comment2.id };
                    }
                    else
                    {
                        result = new { status = false };
                    }
                }
                Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(result));
                Response.End();
            }
            string sID = Request.QueryString["id"].ToNonNullString();
            string sFirstName = Request.QueryString["firstname"].ToNonNullString();
            string sLastName = Request.QueryString["lastname"].ToNonNullString();
            fHomogenized = Request.QueryString["homogenized"].ToNonNullString() == "1";
            string sUserID = Request.QueryString["id"].ToNonNullString();

            MySelf = gUser(this);
            if (sUserID == "")
            {
                sUserID = MySelf.id;
            }
            this.user = gUserById(this, sUserID);

            IsMe = (this.user.id == gUser(this).id);
            IsTestNet = IsTestNet(this);

            IsMyFriend = AmIFriend(this, user.id, gUser(this).id);
        }

       
        public static DACResult AmIFriend(Page p, string sFriendUserGuid, string sMyUserGuid)
        {
            DACResult r = new DACResult();
            
            DataTable dtOriginal = BiblePayDLL.Sidechain.RetrieveDataTable3(IsTestNet(p), "FriendRequest");
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
            dtOriginal = BiblePayDLL.Sidechain.RetrieveDataTable3(IsTestNet(p), "FriendRequest");

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

            dtOriginal = BiblePayDLL.Sidechain.RetrieveDataTable3(IsTestNet(p), "Friend");
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
            dtOriginal = BiblePayDLL.Sidechain.RetrieveDataTable3(IsTestNet(p), "Friend");

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
            r.Result = "Friend Request";
            return r;
        }

        #region new function for person
        public object SaveComment(comment1 data)
        {
            if (!Common.gUser(this).LoggedIn)
            {
                return new { status = false, data = "You must login first to comment." };
            }
            object result = null;

            try
            {
                BiblePayCommon.Entity.comment1 o = new BiblePayCommon.Entity.comment1();
                o.UserID = Common.gUser(this).id;


                o.Body = HttpUtility.UrlDecode(BiblePayCommon.Encryption.Base64DecodeWithFilter(data.Body));
                o.ParentID = data.ParentID;

                BiblePayCommon.Common.DACResult r = DataOps.InsertIntoTable(this, Common.IsTestNet(this), o, Common.gUser(this));
                if (!r.fError())
                {
                    result = new { status = true, data = o.id };
                }
                else
                {
                    result = new { status = false };
                }

            }
            catch (Exception ex)
            {
                result = (new { status = false, data = ex.InnerException == null ? ex.Message : ex.InnerException.Message });
            }
            return result;
        }

        public object Voting(string parentID, string sVoteType)
        {
            if (!Common.gUser(this).LoggedIn)
            {
                return new { status = false, data = "You must login first to comment." };
            }
            object result = null;

            try
            {
                if (!gUser(this).LoggedIn)
                {
                    return new { status = false };
                }

                BiblePayCommon.Entity.vote1 o = new BiblePayCommon.Entity.vote1();
                o.UserID = gUser(this).id;
                o.ParentID = parentID;
                o.VoteType = sVoteType;
                if (sVoteType == "upvote")
                {
                    o.VoteValue = 1;
                }
                else if (sVoteType == "downvote")
                {
                    o.VoteValue = -1;
                }
                else
                {
                    return new { status = false };
                }

                DACResult r = DataOps.InsertIntoTable(this, IsTestNet(this), o, gUser(this));
                
                if (!r.fError())
                {
                    result = new { status = true, data = o.id };
                }
                else
                {
                    result = new { status = false };
                }

            }
            catch (Exception ex)
            {
                result = (new { status = false, data = ex.InnerException == null ? ex.Message : ex.InnerException.Message });
            }
            return result;
        }


        #endregion
        protected override void Event(BBPEvent e)
        {
            if (e.EventName == "AddTimeline_Click")
            {
                if (!Common.gUser(this).LoggedIn)
                {
                    UICommon.MsgBox("Error", "You Must be logged in first.", this);
                    return;
                }

                Timeline t = new Timeline();

                try
                {
                    t.Privacy = e.Extra.Privacy.ToString();
                }
                catch { }
                try
                {
                    t.URL = e.Extra.URL.ToString();
                }
                catch { }
                try
                {
                    string data = BiblePayCommon.Encryption.Base64Decode0(e.Extra.URLTitle.ToString(), true);
                    string decoded = HttpUtility.UrlDecode(data);
                    t.URLTitle = decoded;
                }
                catch { }
                try
                {
                    t.URLDescription = HttpUtility.UrlDecode(BiblePayCommon.Encryption.Base64DecodeWithFilter(e.Extra.URLDescription.ToString()));
                }
                catch { }
                try
                {
                    t.URLPreviewImage = HttpUtility.UrlDecode(e.Extra.URLPreviewImage?.ToString());
                }
                catch { }

                t.Body = HttpUtility.UrlDecode(BiblePayCommon.Encryption.Base64DecodeWithFilter(e.Extra.Body.ToString()));
                t.UserID = gUser(this).id;
                BiblePayCommon.Common.DACResult r = DataOps.InsertIntoTable(this, IsTestNet(this), t, gUser(this));
                if (r.fError())
                {
                    BiblePayCommonNET.UICommonNET.MsgModal(this, "Error", "Sorry, the timeline was not saved.", 500, 200, true);
                    return;
                }
                else
                {
                    //SendBlastOutForTimeline(this, t);
                    ToastLater(this, "Success", "Your timeline entry has been saved!");
                }

            }
            else if (e.EventName == "AddTimelineURL_Click")
            {
                if (!Common.gUser(this).LoggedIn)
                {
                    UICommon.MsgBox("Error", "You Must be logged in first.", this);
                    return;
                }

                Timeline t = new Timeline();
                string sData = BiblePayCommon.Encryption.Base64DecodeWithFilter(e.Extra.Body.ToString());

                t.Body = UICommon.MakeShareableLink(sData, "");
                t.UserID = gUser(this).id;
                if (t.Body == "")
                {
                    BiblePayCommonNET.UICommonNET.MsgModal(this, "Error", "Sorry, the URL was not shared because we could not access the document title; please be sure the target points to a valid web resource. ", 500, 200, true, true);
                    return;
                }
                BiblePayCommon.Common.DACResult r = DataOps.InsertIntoTable(this, IsTestNet(this), t, gUser(this));
                if (r.fError())
                {
                    BiblePayCommonNET.UICommonNET.MsgModal(this, "Error", "Sorry, the URL was not shared.", 500, 200, true);
                    return;
                }
                else
                {

                    //SendBlastOutForTimeline(this, t);
                    ToastLater(this, "Success", "Your URL entry has been shared!");
                }
            }

            else if (e.EventName == "EditUserProfile_Click")
            {
                Response.Redirect("Profile");
            }
            else if (e.EventName == "AddTimelineAttachment_Click")
            {
                string sURL = "UnchainedUpload?action=setattachment&type=Timeline&parentid=" + e.EventValue;
                Response.Redirect(sURL);
            }
            else if (e.EventName == "AddProfileAttachment_Click")
            {
                string sURL = "UnchainedUpload?action=setattachment&type=Profile&parentid=" + e.EventValue;
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
                              , 700, "", "", UICommon.InputType.multiline, false, e.EventValue);

            }
            else if (e.EventName == "DeleteTimeline_Click")
            {

                string sID = _bbpevent.EventValue;
                bool fDeleted = BiblePayDLL.Sidechain.DeleteObject(Common.IsTestNet(this), "Timeline",
                          sID, Common.gUser(this));
                if (fDeleted)
                {
                    BiblePayCommonNET.UICommonNET.ToastNow(this.Page, "Success!", "Your object was deleted!");
                }
                else
                {
                    UICommon.MsgBox("Error", "Sorry, the object could not be deleted. ", this);
                }

            }
            else if (e.EventName == "EditTimeline_Click")
            {
                BiblePayCommon.Entity.Timeline t = (BiblePayCommon.Entity.Timeline)Common.GetObject(Common.IsTestNet(this), "Timeline", _bbpevent.EventValue);
                if (t == null)
                {
                    UICommon.MsgBox("Error", "Sorry, cannot locate object.", this.Page);
                    return;
                }

                UICommon.MsgInput(this, "EditedTimelineBody_Click", "Edit Comment", "Edit your Timeline Post:"
                         , 700, "", "", UICommon.InputType.multiline, false, _bbpevent.EventValue, t.Body);
            }
            else if (_bbpevent.EventName == "EditedTimelineBody_Click")
            {
                if (!Common.gUser(this).LoggedIn)
                {
                    UICommon.MsgBox("Error", "You Must be logged in first.", this);
                    return;
                }

                BiblePayCommon.Entity.Timeline t = (BiblePayCommon.Entity.Timeline)Common.GetObject(Common.IsTestNet(this), "Timeline", _bbpevent.EventValue);

                if (t == null)
                {
                    UICommon.MsgBox("Error", "Sorry, cannot locate object.", this.Page);
                    return;
                }
                t.Body = BiblePayCommon.Encryption.Base64DecodeWithFilter(_bbpevent.Extra.Output.ToString());


                BiblePayCommon.Common.DACResult r = DataOps.InsertIntoTable(this, Common.IsTestNet(this), t, Common.gUser(this));
                if (!r.fError())
                {
                    BiblePayCommonNET.UICommonNET.ToastNow(this.Page, "Saved", "Your Timeline Post been Edited!");
                }
                else
                {
                    UICommon.MsgBox("Error while inserting comment", "Sorry, the Timeline Post was not edited: " + r.Error, this);
                }
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
                o.Body = BiblePayCommon.Encryption.Base64DecodeWithFilter(e.Extra.Output.ToString());
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
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable3(fTestNet, "Friend");
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

        public static void SendBlastOutForTimeline(Page p, Timeline t)
        {
            try
            {
                string sMyFriendsList = GetFriendsList(IsTestNet(p), gUser(p).id);
                if (sMyFriendsList.Length > 1)
                {
                    sMyFriendsList = sMyFriendsList.Replace("userid in", "id in");
                }
                DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable3(IsTestNet(p), "user1");
                dt = dt.FilterDataTable(sMyFriendsList);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    User u = gUserById(p, dt.Rows[i]["id"].ToString());
                    if (u.EmailAddress != null && u.EmailAddress != "" && u.EmailAddress.Length > 3)
                    {
                        EmailNarr e = GetEmailFooter(p);

                        MailMessage m = new MailMessage();
                        string sDomainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
                        string sURL = sDomainName + "/Person?homogenized=1";
                        string sURLA = "<a href='" + sURL + "'>here</a>";
                        string sNarr = "Dear " + u.FirstName + ",<br><br>One of your friends has posted this on their timeline:"
                            + ".<br><br>" + t.Body + "<br><br>To view the post, click " + sURLA + ".<br><br>Thank you.<br>The " + e.DomainName + " Team<br>";
                        m.Subject = "[Timeline Notification] A new post from " + gUser(p).FirstName;
                        m.Body = sNarr;
                        m.IsBodyHtml = true;
                        m.To.Add(new MailAddress(u.EmailAddress, u.FirstName));
                        // m.Bcc.Add(new MailAddress(gUser(p).EmailAddress, gUser(p).FirstName));

                        DACResult r = BiblePayDLL.Sidechain.SendMail(IsTestNet(p), m, e.DomainName);
                    }
                }
            }catch(Exception ex)
            {
                Log("SendBlastOutForTimeline Issue:: " + ex.Message);
            }
        }

        protected string GetUserProfileStuff(User u)
        {
            string html = "";
            bool fMe = (u.id == gUser(this).id);

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


            string sUserAvatar = u.GetAvatarImageNoDims("person");
            string sUserAnchor = UICommon.GetStandardAnchor("ancUser", "EditUserProfile", "", sUserAvatar, "Edit my User Profile Fields");
            if (!fMe)
                sUserAnchor = sUserAvatar;

            html += "<table class='saved2'><tr><td rowspan=7>" + sUserAnchor + "</td><td>Name: " + u.FullUserName() + "</td></tr>";
            html += "<tr><td>Age: " + UnixTimeStampToDisplayAge(u.BirthDate) + "</td></tr>";
            html += "<tr><td>Gender: " + u.Gender;
            html += "<tr><td>Telegram: <a href='" + u.TelegramLinkURL + "'>" + u.TelegramLinkName + "</a>";

            html += "<tr><td>" + u.TelegramLinkDescription;
            // Their video channel:
            string sVURL = "VideoList?userid=" + u.id;
            string sVideoAnchor = "<a href='" + sVURL + "'>My Video Channel</a>";
            string sModifyProfile = UICommon.GetStandardButton("btnModifyProfile", "Modify my Profile", "EditUserProfile", "Modify my Profile");
            string sAddProfileAttachment = UICommon.GetStandardButton(u.id, "Add Profile Media", "AddProfileAttachment", 
                "Add Profile Media Attachment (Picture, Video, PDF, etc..)", "", "");

            html += "<tr><td>" + sVideoAnchor;

            if (gUser(this).id == u.id)
            {
                html += "<td>" + sModifyProfile + "&nbsp;" + sAddProfileAttachment;
            }

            if (gUser(this).LoggedIn)
            {
                html += "<td>" + sAddFriendButton;
            }

            html += "</tr>";
            html += "</table>";

            html += "<br><br>";

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
            string sFilter = "and title='Profile Attachment'";
            string s1 = UICommon.GetAttachments(this, u.id, sFilter, "Profile Attachments", "");
            html += s1;
            return html;
        }

        protected string GetPerson()
        {
            BiblePayPaginator.Paginator _paginator = new BiblePayPaginator.Paginator();
            _paginator.Page = this;
            string sID = Request.QueryString["id"].ToNonNullString();
            bool fHomogenized = Request.QueryString["homogenized"].ToNonNullString() == "1";
            
            if (sID == "")
            {
                sID = gUser(this).id;
            }
            // For each timeline entry, pull in attachments (mp4, mp3, video, pdf) - Sort timeline desc:
            User u = gUserById(this, sID);
            bool fMe = (u.id == gUser(this).id);
            string html = "<div id='user" + u.id + "'>";

            if (u.id == null)
            {
                html += "Sorry, we cannot find this user.</div>";
                return html;
            }
            html += "";

            // Begin User Profile
            string sUserProfileSection = GetUserProfileStuff(u);
            if (!fHomogenized)
            {
                html += sUserProfileSection;
            }

            // End user profile
            
            // Add the "Share something with the world" (Append timeline): 
            string sAddTimelineButton = "<input class='pc90' autocomplete='off' id='timeline1'></input><button id='btntimeline1' onclick=\""
	                       + "var o=document.getElementById('timeline1');var e={};e.Event='AddTimeline_Click';e.Value='"
	                       + sID + "';e.Body=XSS(o.value);BBPPostBack2(null, e);\">Share something with the world, " 
	                       + gUser(this).FirstName + "</button> ";
	        string sAddURLButton = "<button id='btnurl1' onclick=\""
	                       + "var o=document.getElementById('timeline1');var e={};e.Event='AddTimelineURL_Click';e.Value='"
	                       + sID + "';e.Body=XSS(o.value);BBPPostBack2(null, e);\">Share a URL</button><br> ";

            if (gUser(this).LoggedIn)
            {
                html += "<br>" + sAddTimelineButton + sAddURLButton + "<hr>";
            }

            // For each timeline entry...
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable3(IsTestNet(this), "Timeline");
            if (fHomogenized)
            {
                string sMyFriendsList = GetFriendsList(IsTestNet(this), gUser(this).id);
                // Per Mike, just show system wide timeline until phase 2 - this will allow newbies to see some timeline data
                if (false)
                {
                    DataOps.FilterDataTable(ref dt, sMyFriendsList);
                    if (dt.Rows.Count == 0)
                    {
                        // Show homogenized view with everyones posts (since I have no friends yet, and no timeline posts yet):
                        dt = BiblePayDLL.Sidechain.RetrieveDataTable3(IsTestNet(this), "Timeline");
                    }
                }
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

                string sDeleteTimelineButton =  UICommon.GetStandardButton(dt.Rows[i]["id"].ToString(),
                    "<i class='fa fa-trash'></i>", "DeleteTimeline", "Delete timeline event...");

                string sEditTimelineButton = UICommon.GetStandardButton(dt.Rows[i]["id"].ToString(),
                    "<i class='fa fa-edit'></i>", "EditTimeline", "Edit timeline event...");


                // Add the User - Said - @ timestamp (head of the timeline entry):
                string sTime = dt.GetColDateTime(i, "time").ToString();
                string sEntry = "<div style='float:none;'>" + UICommon.GetUserAvatarAndName(this, dt.Rows[i]["userid"].ToString(), true) + " • " + sTime + ":" + "</div>";

                string sTimeline = "<br><div style='float:none;' id='timeline" + dt.Rows[i]["id"].ToString() + "'>" + sEntry + "<br>";

                string sBody = dt.Rows[i]["body"].ToString();
                string sValueControl = String.Empty;
                if (sBody.Contains("shareablelink"))
                {
                    // ToDo:  Make a better way to detect an html literal in the post...
                    sValueControl = "<div class='comments'>" + sBody + "</div>";
                }
                else
                {
                    string[] vRows = sBody.Split("\n");
                    int nRows = vRows.Length + 2;
                    sValueControl = "<textarea class='comments' rows='" + nRows.ToString() + "' cols='200' name='timeline_" + dt.Rows[i]["id"].ToString() + "' readonly>"
                        + sBody + "</textarea>";
                }

                string sTimelineRowButtons = "";
                if (gUser(this).Administrator==1 || gUser(this).id == dt.Rows[i]["UserID"].ToString())
		{
		                    sTimelineRowButtons += sAddTimelineAttachmentButton + "&nbsp;" + sDeleteTimelineButton + "&nbsp;" + sEditTimelineButton + "&nbsp;";
                }

                sTimelineRowButtons += sAddCommentButton;

                string sTimelineRow = "<table width=90%><tr><td width=75%>" + sValueControl + "</td><td width=16% nobreak>" + sTimelineRowButtons + "</td></tr></table>";

                sTimeline += sTimelineRow;

                // Display the attachments
                sTimeline += UICommon.GetAttachments(this, dt.Rows[i]["id"].ToString(), "", "Timeline Attachments", "style='background-color:white;padding-left:30px;'");
                sTimeline += UICommon.GetComments(IsTestNet(this), dt.Rows[i]["id"].ToString(), this, true);
                // Display the comments for the timeline entry
                html += sTimeline;
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
