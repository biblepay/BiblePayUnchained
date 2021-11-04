using BiblePayCommon;
using System.Data;
using System.Linq;
using static BiblePayCommon.DataTableExtensions;
using static Unchained.Common;
using static BiblePayCommonNET.DataTableExtensions;

namespace Unchained
{
    public partial class NewsList : BBPPage
    {

        protected string GetNews()
        {
            // Shows the comments section for the object.  Also shows the replies to the comments.
            BBPDataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable3(IsTestNet(this), "news1");
            dt = dt.FilterBBPDataTable("URL like '%https%'");
            dt = (BBPDataTable)dt.OrderBy("time desc");
            string sHTML = "<table class='news'>";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string sTitle = dt.GetColValue(i, "Title");
                if (sTitle.Length > 1)
                {
                    string sID = dt.GetColValue(i, "id");
                    string sAdded = dt.GetColDateTime(i, "time").ToShortDateString();
                    string sAsset = "<a target='_blank' href='" + dt.GetColValue(i, "URL") + "'><h2 class='headline'>" + dt.GetColValue(i, "Title") + "</h2>"
                        + "<br>Read more<br></a>";
                    string sRow = "<tr><td>FEATURED • " + sAdded + " • " + GetObjectRating(IsTestNet(this), sID, "news1", gUser(this)) + " • "
                        + "<br>" + sAsset + "<hr></td></tr>";
                    sHTML += sRow;
                }
            }
            sHTML += "</table>";
            return sHTML;
        }
    }
}