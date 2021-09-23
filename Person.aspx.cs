using BiblePayCommonNET;
using System;
using System.Data;
using static BiblePayCommon.Common;
using static BiblePayCommon.Entity;
using static BiblePayCommonNET.CommonNET;
using static BiblePayCommonNET.UICommonNET;
using static Unchained.Common;
using static BiblePayCommonNET.DataTableExtensions;
using static BiblePayCommon.DataTableExtensions;

namespace Unchained
{
    public partial class Person : BBPPage
    {
        protected new void Page_Load(object sender, EventArgs e)
        {

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
            else if (e.EventName == "AddTimelineAttachment_Click")
            {
                string sURL = "UnchainedUpload?action=setattachment&parentid=" + e.EventValue;
                Response.Redirect(sURL);
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
                bool fExists = DataExists(IsTestNet(this), "FriendRequest", "userid='" + f.UserID + "' and requesterid='" + f.RequesterID + "'");
                if (fExists)
                {
                    UICommon.MsgBox("Error", "Sorry, you already have a friends request in for this person. ", this);
                    return;
                }

                DACResult r = DataOps.InsertIntoTable(this, IsTestNet(this), f, gUser(this));
                if (r.fError())
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
                              , 700,  "", "", UICommon.InputType.text, false, e.EventValue);

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
                o.Body = BiblePayCommon.Encryption.Base64Decode(e.Extra.Output.ToString());
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


        protected string GetPerson()
        {

            BiblePayPaginator.Paginator _paginator = new BiblePayPaginator.Paginator();

            _paginator.Page = this;

            string sID = Request.QueryString["id"].ToNonNullString();
            string sFirstName = Request.QueryString["firstname"].ToNonNullString();
            string sLastName = Request.QueryString["lastname"].ToNonNullString();
            string sWho = Request.QueryString["who"].ToNonNullString();
            if (sWho == "me" || sFirstName == "")
            {
                sFirstName = gUser(this).FirstName;
                sLastName = gUser(this).LastName;
            }
            // Then Timeline descending.
            // For each timeline entry, pull in attachments (mp4, mp3, video, pdf)
            User u = gUser(this, sFirstName, sLastName);
            string html = "<div id='user" + u.id + "'>";

            if (u.id==null)
            {
                html += "Sorry, we cannot find this user.</div>";
                return html;
            }
            html += "";

            DateTime dtBirthday = BiblePayCommon.Common.ConvertFromUnixTimestamp(u.BirthDate);
            TimeSpan t = DateTime.Now.Subtract(dtBirthday);
            // Add friend request button

            string sAddFriendButton = UICommon.GetStandardButton(u.id, "<i class='fa fa-heart'></i>", "AddFriendRequest", "Send a Friend Request");
           
            html += "<table class='saved2'><tr><td rowspan=7><img src='" + u.AvatarURL 
                + "' class='person' /></td><td>Name: " + u.FullUserName() + "</td></tr>";
            html += "<tr><td>Age: " + (t.Days / 365).ToString() + "</td></tr>";
            html += "<tr><td>Gender: " + u.Gender;

            html += "<tr><td>Telegram: <a href='" + u.TelegramLinkURL + "'>" + u.TelegramLinkName + "</a>";
            html += "<tr><td>" + u.TelegramLinkDescription;

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
                 + " <button class='tablinks' onclick=\"openProfile(event, 'Religious');return false;\">Beliefs</button></div>";

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
            DataOps.FilterDataTable(ref dt, "userid='" + u.id + "'");

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                // Add Attachment button
                string sAddTimelineAttachmentButton = UICommon.GetStandardButton(dt.Rows[i]["id"].ToString(),
                    "<i class='fa fa-file'></i>", "AddTimelineAttachment", "Add media to this post, such as a URL, a video, an mp3, a pdf, an image...etc");

                string sAddCommentButton = UICommon.GetStandardButton(dt.Rows[i]["id"].ToString(),
                    "<i class='fa fa-chat'></i>", "AddTimelineComment", "Add a Comment to this timeline event...");
                
                // Add timestamp here
                string sTime = dt.GetColDateTime(i, "time").ToString();
                string sTimeline = "<div><label>" + sTime + ":</label><br>"
                    + "<textarea rows='2' cols='200' name='timeline_" + dt.Rows[i]["id"].ToString() + "' readonly>"
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

            string sAddTimelineButton = "<input class='pc90' autocomplete='false' id='timeline1'></input><button id='btntimeline1' onclick=\""
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
