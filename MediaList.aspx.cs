using static Unchained.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static BiblePayDLL.Shared;
using static Unchained.DataOps;
using static BiblePayCommon.Common;
using BiblePayCommon;

namespace Unchained
{
    public partial class MediaList : BBPPage
    {
        protected new void Page_Load(object sender, EventArgs e)
        {

        }


        protected string GetMediaList()
        {
            DataTable dtGroup = UICommon.GetGroup(IsTestNet(this), "video1", "url like '%mp4%'", "Category");
            string html = "<table class=saved><tr><th width=80%>Category</th></tr>";
            for (int y = 0; y < dtGroup.Rows.Count; y++)
            {
                string sAnchor = "<a href='VideoList.aspx?category=" + dtGroup.Rows[y]["Category"].ToString() 
                    + "'>" + dtGroup.Rows[y]["Category"].ToString() + "</a>";
                string div = "<tr><td>" + sAnchor + "</td></tr>";
                html += div + "\r\n";
            }
            html += "</table>";
            return html;
        }
    }
}
