using System;
using System.Data;
using System.Linq;
using static Unchained.Common;
using static BiblePayCommon.DataTableExtensions;
using static BiblePayCommon.Common;
using System.Collections.Generic;
using static BiblePayCommon.EntityCommon;
using static BiblePayCommonNET.CommonNET;
using static BiblePayCommonNET.DataTableExtensions;


namespace Unchained
{
    public partial class TicketList : BBPPage
    {

        protected override void Event(BBPEvent e)
        {
            if (e.EventAction == "AddObject_Click")
            {
                Response.Redirect("TicketAdd?entity=" + _EntityName);
            }
        }

        protected new void Page_Load(object sender, EventArgs e)
        {
            this.Title = _CollectionName + " - List";
        }
        protected string GetTicketList()
        {
            // Assigned to Me
            string s1 = GetTicketListBase("AssignedTo='" + gUser(this).id + "' and isnull(disposition,'') <> 'Closed'", "Assigned to Me");
            // Added By Me
            string s2 = GetTicketListBase("UserID='" + gUser(this).id + "' and isnull(disposition,'') <> 'Closed'", "Added by Me");
            // All tickets in the system , order by time desc
            string s3 = "";
            if (gUser(this).Administrator == 1)
            {
                s3 = GetTicketListBase("isnull(assignedto,'')='' and isnull(disposition,'') <> 'Closed'", "Unassigned Tickets");
            }
            string s4 = s1 + s2 + s3;
            return s4;
        }
        protected string GetTicketListBase(string sFilter, string sNarrative)
        {
            _EntityName = "Ticket";

            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable2(IsTestNet(this), _EntityName);
            dt = dt.FilterDataTable(sFilter);
            dt = dt.OrderBy("time desc");

            string html = "<table class=saved><tr class='objheader'><th class='objheader' colspan=3>"
               + "<h3>" + _CollectionName + " - " + sNarrative + "</h3><th class='objheader' colspan=2><div style='text-align:right;'>";
            html += "<tr><th width=10%>Number<th width=15%>Added By</th><th width=20%>Assigned To</th><th>Added<th>Disposition<th width=50%>Subject</tr>";
            for (int y = 0; y < dt.Rows.Count; y++)
            {

                string sAssignedTo = dt.GetColValue(y, "AssignedTo");
                string sDisposition = dt.GetColValue(y, "disposition");
                if (sDisposition == "")
                    sDisposition = "N/A";

                string div = "<tr><td>" + dt.GetColValue(y, "TicketNumber").ToString()
                        + "<td>" + UICommon.GetUserAvatarAndName(this, dt.GetColValue(y, "UserID"))
                        + "<td>" + UICommon.GetUserAvatarAndName(this, sAssignedTo)
                        + "<td>" + dt.GetColDateTime(y, "Time").ToString()
                        + "<td>" + sDisposition 
                        + "" + UICommon.GetTd(dt.Rows[y], "Title", "TicketView", "&entity=" + _EntityName) + "</tr>";
                    html += div + "\r\n";
            }
            html += "</table>";
            return html;
        }
    }
}

