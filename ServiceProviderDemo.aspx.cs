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

namespace Unchained
{
    public partial class ServiceProviderDemo : BBPPage
    {


        protected new void Page_Load(object sender, EventArgs e)
        {
            ddServiceProvider.Items.Add(new ListItem("Internet Provider", "bb1"));
            ddServiceProvider.Items.Add(new ListItem("Social Media Site", "bb2"));
            ddServiceProvider.Items.Add(new ListItem("Game Transaction", "bb3"));
        }



        protected void btnStart_Click(object sender, EventArgs e)
        {

            string html = "<html><table><tr><th>Line Item<th>Line Amount<th>Balance Due</tr>";
            string sBillFromAddress = BiblePayCommon.Encryption.GetDSQLAddress(IsTestNet(this));
            string sBillToAddress = gUser(this).BiblePayAddress;
            string sServiceProvider = ddServiceProvider.SelectedItem.Text;

            for (int i = 1; i < 100; i++)
            {
                double nAmt = .50 + (i * .01);

                string sLineItem1 = sServiceProvider + " item (" + i.ToString() + ") Amt " + nAmt.ToString() + " BBP";

                string sID = i.ToString();
                double nBalance = BiblePayDLL.Sidechain.AdjustServiceAccountBalance(IsTestNet(this), nAmt,
                    GetFundingAddress(IsTestNet(this)), GetFundingKey(IsTestNet(this)), 
                    sBillFromAddress, sBillToAddress, sID, sLineItem1, "", sServiceProvider, false);
                string sRow = "<tr><td>" + sLineItem1 + "<td>" + nAmt.ToString() + "<td>" + nBalance.ToString() + "</tr>";
                html += sRow;
            }
            // Generate invoice
            double nFinal = BiblePayDLL.Sidechain.AdjustServiceAccountBalance(IsTestNet(this), 0,
                   GetFundingAddress(IsTestNet(this)), GetFundingKey(IsTestNet(this)),
                    sBillFromAddress, sBillToAddress, "", "", "", sServiceProvider, true);

            html += "</table>";
 

            Response.Write(html);
            Response.End();

        }
    }
}