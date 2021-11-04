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
using BiblePayCommonNET;

namespace Unchained
{
    public partial class NFTList : BBPPage
    {
        protected string GetNFTList()
        {

            string html = "<table class=saved>";
            // Column headers
            string sRow = "<tr><th>NFT ID<th>Type<th>NFT Name<th>Description<th>Buy It Now Amount<th>Low Quality URL</tr>";
            html += sRow;
            BBPDataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable3(IsTestNet(this), "NFT");
            dt = dt.FilterBBPDataTable("isnull(fDeleted,'false')='false' and UserID='" + gUser(this).BiblePayAddress + "'");

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string a = "NFTAdd.aspx?id=" + dt.Rows[i]["id"].ToString() + "&action=edit";

                sRow = "<tr><td><a href='" + a + "'>" + dt.Rows[i]["hash"].ToString()
                     + "</a><td>" + dt.Rows[i]["Type"].ToNonNullString()
                     + "<td>" + dt.Rows[i]["Name"].ToNonNullString()
                     + "<td>" + dt.Rows[i]["Description"].ToNonNullString()
                     + "<td>" + dt.Rows[i]["BuyItNowAmount"].ToNonNullString()
                     + "<td>" + dt.Rows[i]["LowQualityURL"].ToNonNullString();
                html += sRow;
            }
            html += "</table>";
            return html;
        }

    }
}
