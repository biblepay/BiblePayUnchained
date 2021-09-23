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
using static Unchained.BiblePayUtilities;
using BiblePayCommonNET;

namespace Unchained
{
    public partial class Leaderboard : BBPPage
    {
        protected new void Page_Load(object sender, EventArgs e)
        {

        }

        protected void UpdateInfo()
        {
            string sMode = Session["leaderboardmode"].ToNonNullString() == "summary" ? "Summary" : "Details";
            lblMode.Text = sMode;

        }
        protected string GetLeaderboard()
        {

            double nSuperblockLimit = 350000;

            string html = "<table class=saved>";
            // Column headers
            string sRow = "<tr><th>UserID<th width=10%>Nick Name<th>Currency<th>Total BBP<th>Total Foreign<th>USD Value BBP<th>USD Value Foreign<th>Assessed USD<th>Coverage<th>Earnings<th>Strength</tr>";
            html += sRow;
            Dictionary<string, PortfolioParticipant> u = GenerateUTXOReport(IsTestNet(this));
            foreach (KeyValuePair<string, PortfolioParticipant> pp in u)
            {
                double nEarnings = nSuperblockLimit * pp.Value.Strength;
                if (pp.Value.Strength > 0)
                {
                    sRow = "<tr><td>" + pp.Value.UserID
                        + "<td>" + pp.Value.NickName
                        + "<td>Various"
                        + "<td>" + pp.Value.AmountBBP.ToString()
                        + "<td>" + pp.Value.AmountForeign.ToString()
                        + "<td>" + pp.Value.AmountUSDBBP.ToString()
                        + "<td>" + pp.Value.AmountUSDForeign.ToString()
                        + "<td>" + pp.Value.AmountUSD.ToString()
                        + "<td>" + Math.Round(pp.Value.Coverage * 100, 2).ToString() + "%"
                        + "<td>" + Math.Round(nEarnings, 2).ToString()
                        + "<td>" + Math.Round(pp.Value.Strength * 100, 2).ToString() + "%</tr>";
                    html += sRow;
                    if (lblMode.Text == "Details")
                    {
                        string sTD = "<td style='background-color:grey;'>";
                        for (int i = 0; i < pp.Value.lPortfolios.Count; i++)
                        {
                            if (pp.Value.lPortfolios[i].AmountBBP > 0 || pp.Value.lPortfolios[i].AmountForeign > 0)
                            {
                                sRow = "<tr>" + sTD + sTD + sTD + pp.Value.lPortfolios[i].Ticker
                                    + sTD + pp.Value.lPortfolios[i].AmountBBP.ToString()
                                    + sTD + pp.Value.lPortfolios[i].AmountForeign.ToString()
                                    + sTD + pp.Value.lPortfolios[i].AmountUSDBBP.ToString()
                                    + sTD + pp.Value.lPortfolios[i].AmountUSDForeign.ToString()
                                    + sTD + sTD + sTD + sTD;
                                html += sRow;
                            }

                        }
                    }
                }
            }
            html += "</table>";
            return html;
        }

        protected void btnSummary_Click(object sender, EventArgs e)
        {
            Session["leaderboardmode"] = "summary";
            UpdateInfo();
        }

        protected void btnDetail_Click(object sender, EventArgs e)
        {
            Session["leaderboardmode"] = "detail";
            UpdateInfo();
        }
    }
}
