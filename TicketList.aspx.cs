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

        public static string GetTicketIdsFromHistory(bool fTestNet, string sUserID)
        {
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable3(fTestNet, "TicketHistory");
            dt = dt.FilterDataTable("AssignedTo='" + sUserID + "'");
            string sList = "id in (";
            if (dt.Rows.Count == 0)
            {
                sList = Mid(sList, 0, sList.Length - 1);
                sList += ")";
                return sList;
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //string sRequester = dt.Rows[i]["RequesterID"].ToString();
                //string sRequestedBy = dt.Rows[i]["UserID"].ToString();
                string sID = dt.Rows[i]["ParentID"].ToString();
                sList += "'" + sID + "',";
            }
            sList = Mid(sList, 0, sList.Length - 1);
            sList += ")";
            return sList;
        }

        protected new void Page_Load(object sender, EventArgs e)
        {
            this.Title = _CollectionName + " - List";
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            // Refine the query
        }

        protected string GetTicketList()
        {
            // Assigned to Me


            string sTicketHistoryIDs = GetTicketIdsFromHistory(IsTestNet(this), gUser(this).id);

            string s1 = GetTicketListBase("isnull(disposition,'') <> 'Closed' and (AssignedTo='" 
                + gUser(this).id + "' or UserID='" + gUser(this).id + "')",
                "Assigned To Me or Added by Me");

            string s2 = GetTicketListBase("isnull(disposition,'') <> 'Closed' and (" + sTicketHistoryIDs + ")",
                "Assigned By Me or Worked by Me");


            // string s2 = GetTicketListBase("UserID='" + gUser(this).id + "' and isnull(disposition,'') <> 'Closed'", "Added by Me");
            // All tickets in the system , order by time desc
            string s3 = "";
            if (gUser(this).Administrator == 1)
            {
                s3 = GetTicketListBase("isnull(assignedto,'')='' and isnull(disposition,'') <> 'Closed'", "Unassigned Tickets");
            }
            string s4 = s1 + s2 + s3;
            return s4;
        }

        public static string GetTd(string sID, string sValue, string sDestination, string sExtra = "")
        {
            string sAnchor = "<a href='" + sDestination + ".aspx?id=" + sID + sExtra + "'>";
            string td = "<td>" + sAnchor + sValue + "</a></td>";
            return td;
        }
        protected string GetTicketListBase(string sFilter, string sNarrative)
        {
            _EntityName = "Ticket";

            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable3(IsTestNet(this), _EntityName);
            if (txtSearch.Text != "")
            {
                if (gUser(this).Administrator == 0)
                {
                    dt = dt.FilterDataTable(sFilter);
                }
                else
                {
                    dt = dt.FilterDataTable("isnull(TicketNumber,0)='" + GetDouble(txtSearch.Text)
                        + "' or isnull(Body,'') like '%" + txtSearch.Text + "%' or isnull(Disposition,'') like '%"
                        + txtSearch.Text + "%' or isnull(Title,'') like '%" + txtSearch.Text + "%'");
                }
            }
            else
            {
                dt = dt.FilterDataTable(sFilter);
            }

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
                        + "" + GetTd(dt.Rows[y]["id"].ToString(), dt.Rows[y]["Title"].ToString(), "TicketView", "&entity=" + _EntityName) + "</tr>";
                    html += div + "\r\n";
            }
            html += "</table>";
            return html;
        }
    }
}

