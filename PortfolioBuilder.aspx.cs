using System;
using static BiblePayCommonNET.CommonNET;
using static BiblePayCommonNET.StringExtension;
using static BiblePayCommonNET.UICommonNET;
using static Unchained.BiblePayUtilities;
using static Unchained.Common;

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

                lblDWU.Text = "DWU: <font color=lime>"+Math.Round(GetDWU(IsTestNet(this))*100,2).ToString() + "% </font>";
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
                MsgModal(this, "Nothing Found", "Sorry, you must select the ticker first. ", 255, 255);
            }
            Portfolios p = new Portfolios();
            try
            {
                p =  QueryUTXOList(IsTestNet(this), gUser(this).BiblePayAddress, sTicker, txtAddress.Text, 0);
                string sHTML = "<table border='1px'><tr><TH>TXID<TH>Amount</tr>";
                for (int i = 0; i < p.lPositions.Count; i++)
                {
                    string sRow = "<tr><td>" + p.lPositions[i].TXID.ToNonNullString()
                      + "&nbsp;<td>" + p.lPositions[i].nAmount.ToString() + "</tr>";
                    sHTML += sRow;
                }
                sHTML += "</table>";
                if (p.lPositions.Count > 0)
                {
                    MsgModal(this, "Your Unspent UTXOs", sHTML, 700, 500);
                }
                else
                {
                    MsgModal(this, "Nothing Found", "Sorry, we couldn't find any unspent UTXOs for this address. ", 255, 255);
                }
            }catch(Exception ex)
            {
                //happens if you query prod from testnet, etc.
                MsgModal(this, "Wrong chain", "Sorry, you are querying the wrong chain.", 255, 255);
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

            if (sTicker.ToLower()=="bbp" && !BiblePayDLL.Sidechain.ValidateAddress(IsTestNet(this), txtAddress.Text))
            {
                sError = "Sorry, Invalid BBP address.  Wrong Chain?";
            }
            if (!BiblePayDLL.Sidechain.ValidateAddress(IsTestNet(this), gUser(this).BiblePayAddress))
            {
                sError = "Sorry, you must have a valid Receive Address.";
            }

            if (!BiblePayCommonNET.BiblePay.ValidateAddress3(sTicker, txtAddress.Text) || txtAddress.Text.Length > 128)
            {
                sError = "Invalid Address.";
            }
            if (sError != "")
            {
                MsgModal(this, "Error", sError, 400, 200);
                return;
            }
            BiblePayCommon.Entity.utxostake1 o = new BiblePayCommon.Entity.utxostake1();
            o.UserID = gUser(this).id;
            o.Address = txtAddress.Text;
            o.Ticker = sTicker;
            o.OwnerAddress = gUser(this).BiblePayAddress;

            o.Tithe = 0; // Todo - add a tithe checkbox and process the tithes...
            o.Amount = 0;
            o.id = Guid.NewGuid().ToString();

            bool fExists = UICommon.UTXORecordExists(IsTestNet(this), "utxostake1", "address='" + o.Address + "' and OwnerAddress='" + gUser(this).BiblePayAddress + "'");

            if (fExists)
            {
                UICommon.MsgBox("Duplicate Address", "Sorry, this address already exists as a portfolio builder position.", this);
            }
            BiblePayCommon.Common.DACResult r = DataOps.InsertIntoTable(this, IsTestNet(this), o, gUser(this));
            if (!r.fError())
            {
                Session["stack"] = UICommon.Toast("Saved", "Your Portfolio Builder Position has been Saved!  Thank you for using BiblePay Retirement Accounts!");
                Response.Redirect("PortfolioBuilder");
            }
            else
            {
                UICommon.MsgBox("Error while inserting Portfolio Builder Position", "Sorry, the record was not saved: " + r.Error, this);
            }
        }
    }
}