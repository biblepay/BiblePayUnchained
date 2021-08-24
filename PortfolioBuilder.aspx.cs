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
using System.Reflection;
using static Unchained.Utils;

namespace Unchained
{
    public partial class PortfolioBuilder : BBPPage
    {
        protected new void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                ddTicker.Items.Clear();
                ddTicker.Items.Add("");
                ddTicker.Items.Add("BBP (BiblePay)");
                ddTicker.Items.Add("BCH (Bitcoin-Cash)");
                ddTicker.Items.Add("BTC (Bitcoin)");
                ddTicker.Items.Add("DASH (Dashpay)");
                ddTicker.Items.Add("DOGE (Dogecoin)");
                ddTicker.Items.Add("ETH (Ethereum)");
                ddTicker.Items.Add("LTC (Litecoin)");
                ddTicker.Items.Add("XLM (Stellar Lumens)");
                ddTicker.Items.Add("XRP (Ripple)");
                ddTicker.Items.Add("ZEC (Z-Cash)");
            }

        }

        protected override void Event(BBPEvent e)
        {
            if (e.EventAction == "event_pin")
            {
                txtPin.Text = BiblePayCommon.Common.AddressToPin(gUser(this).BiblePayAddress, txtAddress.Text).ToString();
            }
        }

        protected string GetTicker()
        {
            string[] vTicker = ddTicker.SelectedValue.Split(" ");
            if (vTicker.Length > 0)
            {
                return vTicker[0].ToUpper();
            }
            return "";
        }
        protected void btnQueryUTXO_Click(object sender, EventArgs e)
        {
            string sTicker = GetTicker();
            if (sTicker == "")
            {
                UICommon.MsgModal(this, "Nothing Found", "Sorry, you must select the ticker first. ", 255, 255);
            }
            Portfolios p = QueryUTXOList(IsTestNet(this), sTicker, txtAddress.Text, 0);
            string sHTML = "<table border='1px' style=''><tr><TH>TXID<TH>Amount</tr>";
            for (int i = 0; i < p.lPositions.Count; i++)
            {
                  string sRow = "<tr><td>" + p.lPositions[i].TXID.ToNonNullString() 
                    + "&nbsp;<td>" + p.lPositions[i].nAmount.ToString() + "</tr>";
                  sHTML += sRow;
            }
            sHTML += "</table>";
            if (p.lPositions.Count > 0)
            {
                UICommon.MsgModal(this, "Your Unspent UTXOs", sHTML, 700, 500);
            }
            else
            {
                UICommon.MsgModal(this, "Nothing Found", "Sorry, we couldn't find any unspent UTXOs for this address. ", 255, 255);
            }
       
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {

            string sError = "";
            string sTicker = GetTicker();

            if (sTicker  == "")
                sError = "Ticker must be selected.";

            if (txtAddress.Text == "")
                sError = "Address must be chosen.";

            if (!Utils.ValidateAddress3(sTicker, txtAddress.Text) || txtAddress.Text.Length > 128)
            {
                sError = "Invalid Address.";
            }
            if (sError != "")
            {
                UICommon.MsgModal(this, "Error", sError, 400, 200);
                return;
            }
            BiblePayCommon.Entity.utxostake1 o = new BiblePayCommon.Entity.utxostake1();
            o.UserID = gUser(this).BiblePayAddress;
            o.Address = txtAddress.Text;
            o.Ticker = sTicker;
            o.Tithe = 0; // Todo - add a tithe checkbox and process the tithes...
            o.Amount = 0;

            bool fExists = UICommon.RecordExists(IsTestNet(this), "utxostake1", "address='" + o.Address + "'");
            if (fExists)
            {
                MsgBox("Duplicate Address", "Sorry, this address already exists as a portfolio builder position.", this);
            }
            BiblePayCommon.Common.DACResult r = DataOps.InsertIntoTable(IsTestNet(this), o);
            if (r.Error == "")
            {
                Session["stack"] = UICommon.Toast("Saved", "Your Portfolio Builder Position has been Saved!  Thank you for using BiblePay Retirement Accounts!");
                Response.Redirect("PortfolioBuilder");
            }
            else
            {
                MsgBox("Error while inserting Portfolio Builder Position", "Sorry, the record was not saved: " + r.Error, this);
            }
        }
    }
}