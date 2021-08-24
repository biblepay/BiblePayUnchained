using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using static Unchained.StringExtension;
using static Unchained.Common;
using BiblePayDLL;
using System.Data;
using static BiblePayCommon.DataTableExtensions;

namespace Unchained
{
    public partial class NewsList : BBPPage
    {

        protected string GetNews()
        {
            // Shows the comments section for the object.  Also shows the replies to the comments.
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable2(IsTestNet(this), "news1");
            dt = dt.FilterDataTable("URL like '%https%'");
            string sHTML = "<table style='padding:70px;' width=80%>";

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string sTitle = dt.GetColValue(i, "Title");
                if (sTitle.Length > 1)
                {
                    string sID = dt.GetColValue(i, "id");
                    string sAdded = UnixTimeStampToDateTime(dt.GetColDouble(i, "time")).ToShortDateString();
                    string sAsset = "<a target='_blank' href='" + dt.GetColValue(i, "URL") + "'><h2 class='headline'>" + dt.GetColValue(i, "Title") + "</h2>"
                        + "<br>Read more<br></a>";
                    string sRow = "<tr><td>FEATURED • " + sAdded + " • " + GetObjectRating(IsTestNet(this), sID) + " • "
                        + "<br>" + sAsset + "<hr></td></tr>";

                    sHTML += sRow;
                }
            }
            sHTML += "</table>";
            return sHTML;
        }

    }
}