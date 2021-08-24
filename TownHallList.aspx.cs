using System;
using System.Data;
using System.Linq;
using static BiblePayCommon.DataTableExtensions;
using static Unchained.Common;

namespace Unchained
{
    public partial class TownHallList : BBPPage
    {
        protected new void Page_Load(object sender, EventArgs e)
        {

            
        }

        
        protected override void Event(BBPEvent e)
        {
            if (e.EventAction == "AddTownHallTopic_Click")
            {
                Response.Redirect("TownHallAdd");
            }
        }
        protected string GetTownHallList()
        {
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable2(IsTestNet(this), "townhall1");
            
            string html = "<table class=saved><tr class='objheader'><th class='objheader'>"
               + "<h3>Town Hall - Topics</h3><th class='objheader' colspan=2><div style='text-align:right;'>"
               + "<a onclick=\"__doPostBack('Event','AddTownHallTopic_Click');\"><i class='fa fa-plus'></i></a></div></th></tr>"
               + "<tr><th width=20%>User</th><th width=20%>Added<th width=50%>Subject</tr>";
            for (int y = 0; y < dt.Rows.Count; y++)
            {
                string sBody = dt.Rows[y]["body"].ToString();
                if (sBody != "")
                {
                    string div = "<tr>" 
                        + "<td>" + UICommon.GetUserAvatarAndName(this, dt.GetColValue(y, "UserID"))
                        + "<td>" + UnixTimeStampToDateTime(dt.GetColDouble(y, "Time"))
                        + UICommon.GetTd(dt.Rows[y], "subject", "TownHallView") + "</tr>";
                    html += div + "\r\n";
                }
            }
            html += "</table>";
            return html;
        }
    }
}