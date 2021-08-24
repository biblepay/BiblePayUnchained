using System;
using System.Data;
using static BiblePayCommon.DataTableExtensions;
using static Unchained.Common;

namespace Unchained
{
    public partial class TownHallView : BBPPage
    {
        protected new void Page_Load(object sender, EventArgs e)
        {
        }

        protected override void Event(BBPEvent e)
        {
            if (e.EventAction == "TownHallDelete_Click")
            {
                if (HasOwnership(IsTestNet(this), e.EventID, "townhall1", gUser(this).BiblePayAddress))
                {
                    // Delete the object (logically)
                    bool fDeleted = BiblePayDLL.Sidechain.DeleteObject(IsTestNet(this), "townhall1", 
                        e.EventID, GetFundingAddress(IsTestNet(this)), GetFundingKey(IsTestNet(this)));
                    if (fDeleted)
                    {
                        Response.Redirect("TownHallList.aspx");
                    }
                    else
                    {
                        MsgBox("Error", "Sorry, the object could not be deleted.", this);
                    }
                }
                else
                {
                    MsgBox("Error", "Sorry, you must have ownership of this object to delete it.", this);
                }
            }
        }
        public string GetTownHallThread()
        {
            // Displays the itemthat the user clicked on from the web list.
            string id = Request.QueryString["id"] ?? "";
            if (id == "")
                return "N/A";
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable2(IsTestNet(this), "townhall1");
            DataOps.FilterDataTable(ref dt, "id='" + id + "'");

            if (dt.Rows.Count < 1)
            {
                MsgBox("Not Found", "We are unable to find this topic.", this);
                return "";
            }

            string sUserName = NotNull(dt.Rows[0]["subject"].ToString());
            if (sUserName == "")
                sUserName = "N/A";
            string sBody = " <textarea style='width: 70%;' id=txtbody rows=10 cols=65 readonly>" + dt.Rows[0]["body"].ToString() + "</textarea>";

            string div = "<table style='padding:10px;' width=100%>"
                + "<tr><th class='objheader' colspan=2><h3>Town Hall - "+ dt.Rows[0]["subject"].ToString()  + "</h3><th class='objheader' colspan=3><div style = 'text-align:right;' > "
                + "<a onclick=\"__doPostBack('Event_" + id + "', 'PrayerDelete_Click');\"><i class='fa fa-trash'></i></a></div></th></tr>"
                + "<tr><td width=10%>User:<td>" + UICommon.GetUserAvatarAndName(this, dt.GetColValue("UserID")) 
                + "<tr><td>Added:<td>" + UnixTimeStampToDateTime(dt.GetColDouble(0, "time")).ToString() + "</td></tr>"
                + "<tr><td>Subject:<td>" + dt.Rows[0]["subject"].ToString() + "</td></tr>"
                + "<tr><td>Body:<td colspan=2>" + sBody + "<br>" + GetObjectRating(IsTestNet(this), id) + "</td></tr></table>";
            div += UICommon.GetComments(IsTestNet(this), id, this);
            return div;
        }
    }
}