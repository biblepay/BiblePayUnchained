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
    public partial class PrayerBlog : BBPPage
    {

        protected override void Event(BBPEvent e)
        {
            if (e.EventAction == "AddObject_Click")
            {
                Response.Redirect("PrayerAdd?entity=" + _EntityName);
            }
        }

        protected new void Page_Load(object sender, EventArgs e)
        {
            this.Title = _CollectionName + " - List";
        }

        protected string GetPrayerBlogs()
        {
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable2(IsTestNet(this), _EntityName);
            dt = dt.OrderBy("time desc");

            string html = "<table class=saved><tr class='objheader'><th class='objheader' colspan=3>"
               + "<h3>" + _CollectionName + "</h3><th class='objheader' colspan=2><div style='text-align:right;'>";
            string sAdd = UICommon.GetStandardAnchor("aAddObj", _EntityName, "", "<i class='fa fa-plus'></i>", "");
            html += sAdd + "<tr><th width=20%>User</th><th width=20%>Added<th width=50%>Subject</tr>";
            for (int y = 0; y < dt.Rows.Count; y++)
            {
                string sBody = dt.Rows[y]["body"].ToString();
                if (sBody != "")
                {
                    string div = "<tr>" 
                        + "<td>" + UICommon.GetUserAvatarAndName(this, dt.GetColValue(y, "UserID"))
                        + "<td>" + dt.GetColDateTime(y, "Time").ToString()
                        + UICommon.GetTd(dt.Rows[y], "subject", "PrayerView", "&entity=" + _EntityName) + "</tr>";
                    html += div + "\r\n";
                }
            }
            html += "</table>";
            return html;
        }
    }
}