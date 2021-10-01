using System;
using System.Data;
using static BiblePayCommon.DataTableExtensions;
using static Unchained.Common;
using static BiblePayCommon.Common;
using static BiblePayCommonNET.CommonNET;

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

                if (sBody.Length < 1)
                {
                    BiblePayCommonNET.UICommonNET.MsgModal(this.Page, "Error", "Sorry, the content of the Body or the Title must be longer.", 400, 200, true);
                    return;
                }
                if (id == "" || id == null)
                {
                    UICommon.MsgBox("Error", "The id is invalid.", this);
                    return;

                }
                string sDisposition = Request.Form["input_dddispositions"] ?? "";
                string sAssignee = Request.Form["input_ddasignees"] ?? "";
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
                th.id = Guid.NewGuid().ToString();
                th.AssignedTo = sAssignee;
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

                Response.Redirect("TicketList");
            }

    }


    public string GetTicket()
        {
            // Displays the prayer that the user clicked on from the web list.
            string id = Request.QueryString["id"] ?? "";
            string _EntityName = "Ticket";

            if (id == "")
                return "N/A";
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable2(IsTestNet(this), _EntityName);
            DataOps.FilterDataTable(ref dt, "id='" + id + "'");

            if (dt.Rows.Count < 1)
            {
                UICommon.MsgBox("Not Found", "We are unable to find this object.", this);
                return "";
            }
            string sAssignedTo = dt.Rows[0]["AssignedTo"].ToString(); // UICommon.GetTopOneByTime(this, "TicketHistory", "parentid='" + id + "'", "AssignedTo");
            string sDisposition = dt.Rows[0]["Disposition"].ToString(); // UICommon.GetTopOneByTime(this, "TicketHistory", "parentid='" + id + "'", "Disposition");


            string div = "<table class='comments'>"
                + "<tr><th class='objheader' colspan=3><h3>" + _ObjectName + " - View</h3><th class='objheader' colspan=3><div class='prayer'>"
                + UICommon.GetStandardAnchor(id, "DeleteObject", id, "<i class='fa fa-trash'></i>","Delete this " + _EntityName, _EntityName ) 
                + "</div></th></tr>"
                + "<tr><td width=10%>Added By:<td>" + UICommon.GetUserAvatarAndName(this, dt.GetColValue("UserID"))
                
                + "<tr><td>Added:<td>" + dt.GetColDateTime(0, "time").ToString() + "</td></tr>"
                + "<tr><td>Title:<td>" + dt.Rows[0]["title"].ToString() + "</td></tr>"
                + "<tr><td>Disposition:<td>" + sDisposition + "</td></tr>"
                + "<tr><td>Assigned To:<td>" + UICommon.GetUserAvatarAndName(this, sAssignedTo) + "</td></tr>"
                + "<tr><td colspan=6><textarea class='pc90 comments' rows=10 columns=10 readonly=true>" + dt.GetColValue("Body") + "</textarea></td></tr>"
                +"</table>";

            BiblePayCommon.BBPDataTable th = BiblePayDLL.Sidechain.RetrieveDataTable2(IsTestNet(this), "TicketHistory");
            th = th.FilterBBPDataTable("parentid='" + id + "'");
            th = th.OrderBy0("time desc");

            string sHistory = "<hr><table class='comments'>";

            for (int i = 0; i < th.Rows.Count; i++)
            {
                string sAssignedFromControl = "<td><td>";
                if (i < th.Rows.Count-1)
                {
                    string sAssignedFrom = th.GetColValue(i + 1, "AssignedTo");
                    sAssignedFromControl = "<td width=10%>Worked By:</td><td>" + UICommon.GetUserAvatarAndName(this, sAssignedFrom) + "</td>";
                }
                else
                {
                    sAssignedFromControl = "<td width=10%>Initiated By:</td><td>" + UICommon.GetUserAvatarAndName(this, dt.GetColValue("UserID")) + "</td>";
                }
                string sRow = "<tr><td colspan=10><hr></tr><tr>" + sAssignedFromControl;
                string sBody = th.Rows[i]["Body"].ToString();

                sRow += "<td>" + th.Rows[i]["Disposition"].ToString() + "</td>";
                sRow += "<td>" + th.GetColDateTime(i, "time").ToString() 
                    + "<tr><td colspan=6><textarea class='pc90 comments' rows=10 columns=10 readonly=true>" + sBody + "</textarea></td></tr>";
                sHistory += sRow;
            }
            
            // Ticket History


            // If the ticket is assigned to me, or if Im an admin: show the reply module
            bool fPerms = (sAssignedTo == gUser(this).id || gUser(this).Administrator == 1);

            if (fPerms)
            {
                string sReplyModule = "<tr><td width=10%>Your Comments:</td><td width=90% colspan=7><textarea id='txtComment' class='pc90 comments' name='txtComment' rows=10 cols=10></textarea><br><br></td></tr>";
                sReplyModule += "<tr><td>Assign To:<td>" + UICommon.GetAssignees(this, "ddasignees", sAssignedTo) + "</td></tr>";
                sReplyModule += "<tr><td>Disposition:<td>" + UICommon.GetDispositions("disp1", sDisposition) + "</td></tr>";
                sReplyModule += "<tr><td>" + BiblePayCommonNET.UICommonNET.GetButtonTypeSubmit("btnSaveTicket", "SaveTicketHistory_Click", "Save Ticket Comments") + "</td></tr>";

                sHistory += sReplyModule;
            }
            sHistory += "</table>";
            div += sHistory;

            return div;
        }
    }
}