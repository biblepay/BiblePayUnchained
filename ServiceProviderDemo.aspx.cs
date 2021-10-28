using System;
using System.Web.UI.WebControls;
using static BiblePayCommon.Common;
using static BiblePayCommonNET.CommonNET;
using static BiblePayCommonNET.StringExtension;
using static Unchained.Common;

namespace Unchained
{
    public partial class ServiceProviderDemo : BBPPage
    {

        protected override void Event(BBPEvent e)
        {
            if (e.EventAction == "MakeServiceProviderPayment")
            {
                string sPin = BiblePayCommon.Encryption.Base64DecodeWithFilter((e.Extra.Output ?? "").ToString());
                DACResult r30 = UICommon.BuySomething2(this, sPin);
                string sError = e.Extra.Error;
                string sSumm = sError.IsEmpty() ? "Paid" : "Error";
                string sNarr = sError.IsEmpty() ? "You have successfully paid the service provider on TXID " + e.Extra.TXID + "!" : sError;
                string sBillFromAddress = BiblePayCommon.Encryption.GetDSQLAddress(IsTestNet(this));
                string sBillToAddress = gUser(this).BiblePayAddress;
                string sID = (e.Extra.TXID ?? "").ToString();
                if (!r30.fError())
                {
                    BiblePayDLL.Sidechain.AdjustServiceAccountBalance(IsTestNet(this), -1 * 500, sBillFromAddress, sBillToAddress, sID,
                        "Service Provider Payment", "", e.Extra.TXID.ToNonNullString(), true, gUser(this));
                }
            }
        }
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
                // In this case, the user is the signer, and their BillToAddress equals their BiblePayAddress
                DACResult r1 = BiblePayDLL.Sidechain.AdjustServiceAccountBalance(IsTestNet(this), nAmt,
                    sBillFromAddress, sBillToAddress, sID, sLineItem1, "", sServiceProvider, false, gUser(this));
                string sRow = "<tr><td>" + sLineItem1 + "<td>" + nAmt.ToString() + "<td>" + r1.Amount.ToString() + "</tr>";
                html += sRow;

                if (r1.Amount > 200)
                {
                    // Credit limit exceeded;
                    html += "<tr><td>ERROR<td>CREDIT LIMIT EXCEEDED</tr>";
                    break;
                }
            }
            // Generate invoice
            DACResult r2 = BiblePayDLL.Sidechain.AdjustServiceAccountBalance(IsTestNet(this), 0,
                    sBillFromAddress, sBillToAddress, "", "", "", sServiceProvider, true, gUser(this));
            html += "</table>";
            Response.Write(html);
            Response.End();
        }

        protected void btnMakePayment_Click(object sender, EventArgs e)
        {
            // This pays the service provider, therefore you will have a credit balance (or an applied payment).
            if (gUser(this).LoggedIn == false)
            {
                UICommon.MsgBox("Error", "Sorry, you must be logged in first", this);
                return;
            }
            string sBillFromAddress = BiblePayCommon.Encryption.GetDSQLAddress(IsTestNet(this));
            string sServiceProvider = ddServiceProvider.SelectedItem.Text;

            BiblePayCommon.Entity.invoice1 i = new BiblePayCommon.Entity.invoice1();
            i.BillFromAddress = sBillFromAddress;
            i.BillToAddress = gUser(this).BiblePayAddress;
            i.Amount = 500;
            i.Data = "SERVICE PROVIDER PAYMENT " + sServiceProvider;
            i.ProductID = "SERVICE PROVIDER PAYMENT";
            i.ServiceName = sServiceProvider;
            i.InvoiceType = "PAYMENT";
            i.InvoiceDate = System.DateTime.Now.ToString();
            UICommon.BuySomething(this, i, "MakeServiceProviderPayment");
        }
    }
}