using BiblePayCommonNET;
using System;
using System.Data;
using System.Linq;
using static BiblePayCommon.Common;
using static BiblePayCommon.DataTableExtensions;
using static Unchained.Common;

namespace Unchained
{
    public partial class MyUploads : BBPPage
    {
        protected new void Page_Load(object sender, EventArgs e)
        {

        }

        protected string GetMyUploads()
        {
            string html = "<table class=saved>";
            string sRow = "<tr><th>ID<th>Added<th>Title<th>Status<th>URL</tr>";
            html += sRow;
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable3(IsTestNet(this), "video1",true,true);
            // They belong to you and have not been successfully added to a link yet ... At the end, use the final URL.
            long nEarlyTime = UnixTimestampUTC() - (60 * 60 * 24 * 7);
            dt = dt.FilterDataTable("userID='" + gUser(this).id + "' and (URL like '%mp4%' or URL like '%webm%' or URL like '%mov%') and time > 1637438708 and time > " + nEarlyTime.ToString());
            dt = dt.OrderBy("time desc");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string sURL = dt.GetColValue(i, "FinalURL");
                string sLink = sURL == "" ? "" : "<a href='" + sURL+ "'>Video</a>";
                sRow = "<tr><td>" + dt.Rows[i]["id"].ToString() + "</td><td>" + GetPrettyDate(dt.GetColDouble(i, "time"))
                        + "<td>" + dt.GetColValue(i, "Title")
                        + "<td>" + dt.GetColValue(i, "Status")
                        + "</td><td>" + sLink + "</tr>";
                html += sRow;
            }
            html += "</table>";
            return html;
        }

    }
}
