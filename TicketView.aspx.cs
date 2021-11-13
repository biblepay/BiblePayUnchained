using System;
using System.Data;
using static BiblePayCommon.DataTableExtensions;
using static Unchained.Common;
using static BiblePayCommon.Common;
using static BiblePayCommonNET.CommonNET;
using System.Net.Mail;
using System.Web;
using static Unchained.UICommon;

namespace Unchained
{
    public partial class TicketView : BBPPage
    {

        protected new void Page_Load(object sender, EventArgs e)
        {
            this.Title = _CollectionName + " - Add";
        }

        protected override void Event(BBPEvent e)
        {
            if (e.EventAction == "SaveTicketHistory_Click")
            {

                string id = Request.QueryString["id"] ?? "";

                if (!gUser(this).LoggedIn)
                {
                    UICommon.MsgBox("Not Logged In", "Sorry, you must be logged in first.", this);
                    return;
                }
                string sBody = Request.Form["txtComment"].ToString();
                string sHours = Request.Form["txtHours"].ToNonNullString2();
                if (id == "" || id == null)
                {
                    UICommon.MsgBox("Error", "The id is invalid.", this);
                    return;
                }
                string sDisposition = Request.Form["dddispositions"] ?? "";
                string sAssignee = Request.Form["ddAssignees"] ?? "";
                if (sDisposition == "")
                {
                    BiblePayCommonNET.UICommonNET.MsgModal(this, "Error", "The ticket must have a disposition.", 400, 200, true);
                    return;
                }
                if (sAssignee == "")
                {
                    BiblePayCommonNET.UICommonNET.MsgModal(this, "Error", "The ticket must have an assignee.", 400, 200, true);
                    return;

                }

                BiblePayCommon.Entity.TicketHistory th = new BiblePayCommon.Entity.TicketHistory();
                th.ParentID = id;
                th.UserID = gUser(this).id;
                th.Body = sBody;
                th.Disposition = sDisposition;
                th.Hours = GetDouble(sHours);
                th.id = Guid.NewGuid().ToString();
                th.AssignedTo = sAssignee;
                User uAssignee = gUserById(this,sAssignee);
                BiblePayCommon.Common.DACResult rh = DataOps.InsertIntoTable(this, IsTestNet(this), th, gUser(this));
                if (rh.fError())
                    UICommon.MsgBox("Error while inserting object", "Sorry, the object was not saved: " + rh.Error, this);
                // Update the ticket with the current disposition
                BiblePayCommon.Entity.Ticket T = (BiblePayCommon.Entity.Ticket)GetObject(IsTestNet(this), "Ticket", id);
                T.Disposition = sDisposition;
                T.AssignedTo = sAssignee;
                DACResult rt = BiblePayDLL.Sidechain.InsertIntoDSQL(IsTestNet(this), T, gUser(this));
                if (rt.fError())
                    UICommon.MsgBox("Error", "Error while saving the ticket. " + rt.Error, this);

                // Notify the assignee
                if (uAssignee.FirstName.ToNonNullString2() != "")
                {
                    MailMessage m = new MailMessage();
                    string sLastNarr = gUser(this).FirstName + " said:<br>" + th.Body + "<br><br>";
                    if (th.Hours > 0)
                    {
                        sLastNarr += "<br>Hours: " + th.Hours.ToString() + "<br><br>";
                    }
                    string sDomainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
                    string sURL = sDomainName + "/TicketView?id=" + id;
                    EmailNarr e1 = GetEmailFooter(this);

                    string sNarr = "Dear " + uAssignee.FirstName + ",<br><br>Ticket number " + T.TicketNumber.ToString() 
                        + " has been assigned to you for " + T.Disposition + ".  "
                        + "<br><br>To view the ticket, click <a href='" + sURL + "'>here. </a><br><br>" + sLastNarr 
                        +" Thank you.<br>The " + e1.DomainName + " Team<br>";
                    m.Subject = "[Transactional Message] Ticket #" + T.TicketNumber + " - " + T.Title + " has been assigned to you for " + T.Disposition;
                    // CC everyone who touched this ticket also
                    BiblePayCommon.BBPDataTable th1 = BiblePayDLL.Sidechain.RetrieveDataTable3(IsTestNet(this), "TicketHistory");
                    th1 = th1.FilterBBPDataTable("parentid='" + id + "'");
                   
                    for (int i = 0; i < th1.Rows.Count; i++)
                    {
                        string sAssignedTo = th1.Rows[i]["AssignedTo"].ToNonNullString2();
                        User u1 = gUserById(this, sAssignedTo);
                        if (u1.EmailAddress.ToNonNullString2() != "")
                        {
                            MailAddress mad = new MailAddress(u1.EmailAddress, u1.FullUserName());
                            if (!m.CC.Contains(mad) && !m.To.Contains(mad) && !m.Bcc.Contains(mad))
                                 m.CC.Add(mad);
                        }
                    }
                    m.Body = sNarr;
                    m.IsBodyHtml = true;
                    m.To.Add(new MailAddress(uAssignee.EmailAddress, uAssignee.FullUserName()));
                    DACResult r = BiblePayDLL.Sidechain.SendMail(IsTestNet(this), m, e1.DomainName);
                }
                Response.Redirect("TicketList");
            }
            else if (e.EventName == "AddAttachment_Click")
            {
                string sURL = "UnchainedUpload?action=setticketattachment&parentid=" + e.EventValue;
                Response.Redirect(sURL);
            }
        }

        public string GetTicket()
        {
            // Displays the prayer that the user clicked on from the web list.
            string id = Request.QueryString["id"] ?? "";
            string _EntityName = "Ticket";

            if (id == "")
                return "N/A";
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable3(IsTestNet(this), _EntityName);
            DataOps.FilterDataTable(ref dt, "id='" + id + "'");

            if (dt.Rows.Count < 1)
            {
                UICommon.MsgBox("Not Found", "We are unable to find this object.", this);
                return "";
            }
            string sAssignedTo = dt.Rows[0]["AssignedTo"].ToString();
            string sDisposition = dt.Rows[0]["Disposition"].ToString();
            BiblePayCommon.BBPDataTable th = BiblePayDLL.Sidechain.RetrieveDataTable3(IsTestNet(this), "TicketHistory");
            th = th.FilterBBPDataTable("parentid='" + id + "'");
            th = th.OrderBy0("time desc");

            string sLastInteraction = "";
            if (th.Rows.Count > 0)
            {

                sLastInteraction = th.Rows[0]["Body"].ToString();
            }
            else
            {
                sLastInteraction = dt.Rows[0]["Body"].ToString();
            }

            string div = "<table class='comments'>"
                + "<tr><th class='objheader' colspan=3><h3>" + _ObjectName + " #" + dt.GetColValue(0,"TicketNumber") 
                + " - View</h3><th class='objheader' colspan=3><div class='prayer'>"
                + UICommon.GetStandardAnchor(id, "DeleteObject", id, "<i class='fa fa-trash'></i>","Delete this " + _EntityName, _EntityName ) 
                + "</div></th></tr>"
                + "<tr><td width=10%>Added By:<td>" + UICommon.GetUserAvatarAndName(this, dt.GetColValue("UserID"))
                + "<tr><td>Added:<td>" + dt.GetColDateTime(0, "time").ToString() + "</td></tr>"
                + "<tr><td>Title:<td>" + dt.Rows[0]["title"].ToString() + "</td></tr>"
                + "<tr><td>Disposition:<td>" + sDisposition + "</td></tr>"
                + "<tr><td>Assigned To:<td>" + UICommon.GetUserAvatarAndName(this, sAssignedTo) + "</td></tr>"
                + "<tr><td>Last Interaction:<td colspan=6><textarea class='pc90 comments' rows=10 columns=10 readonly=true>" + sLastInteraction + "</textarea></td></tr>"
                + "</table>";

            // Ticket History
            string sHistory = "<hr><table class='comments'>";

            // If the ticket is assigned to me, or if Im an admin: show the reply module
            bool fPerms = (sAssignedTo == gUser(this).id || gUser(this).Administrator == 1);
            if (fPerms)
            {
                string sReplyModule = "<form id='myform10'><tr><td width=10%>Your Comments:</td><td width=90% colspan=7>"
                    + "<textarea id='txtComment' class='pc90 comments' name='txtComment' rows=10 cols=10></textarea><br><br></td></tr>";
                sReplyModule += "<tr><td>Assign To:<td>" 
                    + UICommon.GetDropDownUser(this, "ddAssignees", sAssignedTo, dt.GetColValue("UserID"), true) + "</td></tr>";
                sReplyModule += "<tr><td>Disposition:<td>" + UICommon.GetDispositions("ddDispositions", sDisposition) + "</td></tr>";
                sReplyModule += "<tr><td>Hours:<td><input name='txtHours' id='txtHours'></input></td></tr>";

                string js  = "var o1=document.getElementById(\"ddDispositions\");o1.style.visibility=\"hidden\";";
                string js2 = "var o2=document.getElementById(\"ddAssignees\");o2.style.visibility=\"hidden\";";

                sReplyModule += "<tr><td>" + BiblePayCommonNET.UICommonNET.GetButtonTypeSubmit("btnSaveTicket",
                    "SaveTicketHistory_Click", "Save Ticket Comments", js + js2, "") + "</td></tr></form>";
                sHistory += sReplyModule;
            }

            sHistory += "<tr><td colspan=10><h4><hr>Ticket History</td></tr>";
            // End of Reply Module
            BiblePayPaginator.Paginator _paginator = new BiblePayPaginator.Paginator();
            _paginator.Page = this;

            for (int i = 0; i < th.Rows.Count; i++)
            {
                string sAssignedFromControl = "<td><td>";
                if (i < th.Rows.Count-1)
                {
                    string sAssignedFrom = th.GetColValue(i + 1, "AssignedTo");
                    sAssignedFromControl = "<td width=10%>Worked By:</td><td>" + UICommon.GetUserAvatarAndName(this, sAssignedFrom) + "</td>";
                }
                string sRow = "<tr><td colspan=10><hr></tr><tr>" + sAssignedFromControl;
                string sBody = th.Rows[i]["Body"].ToString();
                // Add Attachment button
                string sAddTimelineAttachmentButton = "";
                if (th.GetColValue(i, "UserID") == gUser(this).id)
                {
                    sAddTimelineAttachmentButton = UICommon.GetStandardButton(th.Rows[i]["id"].ToString(),
                        "<i class='fa fa-file'></i>", "AddAttachment", "Add media to this post, such as a URL, a video, an mp3, a pdf, an image...etc");
                }

                sRow += "<td>" + th.Rows[i]["Disposition"].ToString() + "</td>";
                if (sBody.Length > 0)
                {
                    sRow += "<td>" + th.GetColDateTime(i, "time").ToString()
                        + "<td>Hours: " + th.GetColDouble(i,"Hours").ToString() + "</td></tr>"
                        + "<tr><td colspan=6><textarea class='pc90 comments' rows=10 columns=10 readonly=true>" + sBody + "</textarea>"
                        + sAddTimelineAttachmentButton + "</td></tr>";
                }

                sHistory += sRow;

                // Display the attachments
                //string sAttachments = "<tr><td colspan=6><table width=85%><tr><td>" + UICommon.GetAttachments(this, th.Rows[i]["id"].ToString(), "", "Ticket Attachments", "") + "</td></tr></table></td></tr>";
                string sAttachments = "<tr><td colspan=6><table width=85%><tr><td>" + UICommon.GetLightboxGallery(this,
                    th.Rows[i]["id"].ToString(), "", "Ticket Attachments", "") + "</td></tr></table></td></tr>";

                sHistory += sAttachments;
            }

            // Ticket History

            // Original Post

            string sBody1 = dt.GetColValue(0, "Body").ToString();
            string sNewRow = "<td width=10%>Initiated By:</td><td>" + UICommon.GetUserAvatarAndName(this, dt.GetColValue("UserID")) + "</td>";
            sNewRow += "<tr><td colspan=6><textarea class='pc90 comments' rows=10 columns=10 readonly=true>" + sBody1 + "</textarea>"
                + "</td></tr>";
            sHistory += sNewRow;
            // End of OP Post

            sHistory += "</table>";
            div += sHistory;

            return div;
        }
    }
}