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

        public static string GetTd(dynamic dr, string sValue, string sDestination, string sExtra = "")
        {
            string sAnchor = "<a href='" + sDestination + ".aspx?id=" + dr._id + sExtra + "'>";
            string td = "<td>" + sAnchor + sValue + "</a></td>";
            return td;
        }
        protected string GetPrayerBlogs()
        {
            //DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable2Retired(IsTestNet(this), _EntityName);
            //dt = dt.OrderBy("time desc");

            //var builder = Builders<dynamic>.Filter;
            
            IList<dynamic> dt = BiblePayDLL.Sidechain.GetChainObjects<dynamic>(IsTestNet(this), _EntityName, null,
                SERVICE_TYPE.PUBLIC_CHAIN, "time", false, "");

            string html = "<table class=saved><tr class='objheader'><th class='objheader' colspan=3>"
               + "<h3>" + _CollectionName + "</h3><th class='objheader' colspan=2><div style='text-align:right;'>";
            string sAdd = UICommon.GetStandardAnchor("aAddObj","AddObject", _EntityName, "<i class='fa fa-plus'></i>", "Add Object", "pray1");
            html += sAdd + "<tr><th width=20%>User</th><th width=20%>Added<th width=50%>Subject</tr>";
            for (int y = 0; y < dt.Count; y++)
            {
                string sBody = dt[y].Body.ToString();
                if (sBody != "")
                {
                    string div = "<tr>" 
                        + "<td>" + UICommon.GetUserAvatarAndName(this, dt[y].UserID)
                        + "<td>" + BiblePayCommon.Common.UnixTimeStampToDateControl(dt[y].time)
                        + GetTd(dt[y], dt[y].Subject, "PrayerView", "&entity=" + _EntityName) + "</tr>";
                    html += div + "\r\n";
                }
            }
            html += "</table>";
            return html;
        }
    }
}