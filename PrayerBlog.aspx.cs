using System;
using System.Data;
using System.Linq;
using static Unchained.Common;
using static BiblePayCommon.DataTableExtensions;

namespace Unchained
{
    public partial class PrayerBlog : BBPPage
    {
        protected new void Page_Load(object sender, EventArgs e)
        {

        }

        
        protected override void Event(BBPEvent e)
        {
            if (e.EventAction == "AddPrayer_Click")
            {
                Response.Redirect("PrayerAdd");
            }
        }
        protected string GetPrayerBlogs()
        {
            // Harvest To Do:  Add Order by, PrayerRequest.Added desc (and inner join equiv for avatar display)
            BiblePayCommon.BBPDataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable2(IsTestNet(this), "pray1");
            dt = dt.FilterBBPDataTable("isnull(deleted,0) <> 1");

            // Order by
            dt = dt.OrderBy("time desc");
            
            string html = "<table class=saved><tr class='objheader'><th class='objheader'>"
               + "<h3>Prayer Requests</h3><th class='objheader' colspan=2><div style='text-align:right;'>"
               + "<a onclick=\"__doPostBack('Event','AddPrayer_Click');\"><i class='fa fa-plus'></i></a></div></th></tr>"
               + "<tr><th width=20%>User</th><th width=20%>Added<th width=50%>Subject</tr>";
            for (int y = 0; y < dt.Rows.Count; y++)
            {
                string sBody = dt.Rows[y]["body"].ToString();
                if (sBody != "")
                {
                    string div = "<tr>" 
                        + "<td>" + UICommon.GetUserAvatarAndName(this, dt.GetColValue(y, "UserID"))
                        + "<td>" + UnixTimeStampToDateTime(dt.GetColDouble(y, "Time"))
                        + UICommon.GetTd(dt.Rows[y], "subject", "PrayerView") + "</tr>";
                    html += div + "\r\n";
                }
            }
            html += "</table>";
            return html;
        }
    }
}