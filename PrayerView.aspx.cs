using System;
using System.Data;
using static BiblePayCommon.DataTableExtensions;
using static Unchained.Common;
using static BiblePayCommon.Common;

namespace Unchained
{
    public partial class PrayerView : BBPPage
    {

        protected new void Page_Load(object sender, EventArgs e)
        {
            this.Title = _CollectionName + " - Add";
        }
        public string GetPrayer()
        {
            // Displays the prayer that the user clicked on from the web list.
            string id = Request.QueryString["id"] ?? "";
            if (id == "")
                return "N/A";
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable2(IsTestNet(this), _EntityName);
            DataOps.FilterDataTable(ref dt, "id='" + id + "'");

            if (dt.Rows.Count < 1)
            {
                UICommon.MsgBox("Not Found", "We are unable to find this object.", this);
                return "";
            }

            string sBody = " <textarea class='comments' id=txtbody rows='10' cols='65' readonly>" + dt.Rows[0]["body"].ToString() + "</textarea>";

            string div = "<table class='comments'>"
                + "<tr><th class='objheader' colspan=3><h3>" + _ObjectName + " - View</h3><th class='objheader' colspan=3><div class='prayer'>"
                + UICommon.GetStandardAnchor(id, "DeleteObject", id, "<i class='fa fa-trash'></i>", _EntityName ) 
                + "</div></th></tr>"
                + "<tr><td width=10%>User:<td>" + UICommon.GetUserAvatarAndName(this, dt.GetColValue("UserID")) 
                + "<tr><td>Added:<td>" + dt.GetColDateTime(0, "time").ToString() + "</td></tr>"
                + "<tr><td>Subject:<td>" + dt.Rows[0]["subject"].ToString() + "</td></tr>"
                + "<tr><td>Body:<td colspan=2>" + sBody + "<br>" 
                + GetObjectRating(IsTestNet(this), id, _EntityName, gUser(this)) + "</td></tr></table>";
            div += UICommon.GetComments(IsTestNet(this), id, this);
            return div;
        }
    }
}