using System;
using System.Collections.Generic;
using System.Web;
using static BiblePayCommon.Common;
using static BiblePayCommon.DataTableExtensions;
using static BiblePayCommonNET.CommonNET;
using static BiblePayCommonNET.UICommonNET;
using static Unchained.Common;

namespace Unchained
{
    public partial class People : BBPPage
    {
        protected new void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void Event(BBPEvent e)
        {

            //string data1 = Request.Form.ToString();
            if (e.EventAction == "btnTip_Click")
            {
                if (gUser(this).LoggedIn == false)
                {
                    MsgModal(this, "Error", "Sorry, You must log in first.", 400, 200);
                    return;

                }
                User uTip = UICommon.GetUserRecord(IsTestNet(this), e.Extra.TipTo.ToString());
                string sChannelName = uTip.FullUserName();
                if (uTip.BiblePayAddress.ToNonNullString2() == "")
                {
                    MsgModal(this, "Error", "Sorry, this user does not have a biblepay address.", 400, 240, true);
                    return;
                }
                Session["tipto"] = uTip.id.ToString();
                Session["videoid"] = e.Extra.ContentID.ToString();

                UICommon.MsgInput(this, "Tipping_Click", "Tipping", "Enter tip amount for "
                        + sChannelName + ":", 700,
                        "", "", UICommon.InputType.number, false, "","");
            }
            else if (e.EventName == "Tipping_Click")
            {
                string sPub = gUser(this).BiblePayAddress;
		double nAmt = BiblePayCommon.Common.GetDouble(BiblePayCommon.Encryption.Base64DecodeWithFilter(e.Extra.Output.ToString()));

                string sTipTo = (Session["tipto"] ?? "").ToString();
                User uTip = UICommon.GetUserRecord(IsTestNet(this), sTipTo);
                string sChannelName = uTip.FullUserName();
                if (sPub == String.Empty)
                {
                    MsgModal(this, "Error", "Sorry, you must have a wallet in order to tip.", 400, 200);
                    return;
                }
                if (sTipTo == String.Empty)
                {
                    MsgModal(this, "Error", "Sorry, the user you are trying to tip does not exist.", 400, 200);
                    return;
                }

                string sProductID = e.EventValue;  // Verify if this is the video ID 
                BiblePayCommon.Entity.invoice1 i = new BiblePayCommon.Entity.invoice1();
                i.BillFromAddress = uTip.BiblePayAddress;
                i.BillToAddress = gUser(this).BiblePayAddress;
                i.Amount = nAmt;
                i.Data = "Tipped " + sChannelName + " [Video] for " + nAmt.ToString() + " BBP.";
                i.ProductID = e.EventValue;
                i.ServiceName = sChannelName;
                i.InvoiceType = "Tip";
                i.InvoiceDate = System.DateTime.Now.ToString();
                UICommon.BuySomething(this, i, "Tipped_Click");
            }
            else if (e.EventName == "Tipped_Click")
            {
                string sPin = BiblePayCommon.Encryption.Base64DecodeWithFilter(e.Extra.Output.ToString());

                DACResult r30 = UICommon.BuySomething2(this, sPin);
                if (r30.fError())
                {
                    this.Page.Session["stack"] = Toast("Failure", "The tip failed! " + r30.Error);

                }
                else
                {
                    this.Page.Session["stack"] = Toast("Tipped", "You have tipped this channel on TXID " + r30.Result);
                }
            }
        }

        protected string GetPeople()
        {
            BiblePayCommon.BBPDataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable3(IsTestNet(this), "user1");
            string sFilter = Config("hideusersdomain");
            if (sFilter != "")
            {
                dt = dt.FilterBBPDataTable(sFilter);
            }

            // Reserved:Filter by Domain:
            if (txtSearch.Text != "")
            {
                dt = dt.FilterBBPDataTable("FirstName like '%" + txtSearch.Text
                    + "%' or LastName like '%" + txtSearch.Text + "%' or TelegramLinkName like '%"
                    + txtSearch.Text + "' or TelegramLinkURL like '%" + txtSearch.Text + "%' or TelegramLinkDescription like '%" + txtSearch.Text + "%'");
            }
            List<BiblePayCommon.Entity.user1> l = new List<BiblePayCommon.Entity.user1>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                BiblePayCommon.Entity.user1 o = (BiblePayCommon.Entity.user1)BiblePayCommon.EntityCommon.TableRowToStronglyCastObject(dt, "user1", i);
                l.Add(o);
            }
            string html = UICommon.GetUserGallery(this, l, paginator1, 3);
            return html;
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            // Refine the query
        }

    }
}
