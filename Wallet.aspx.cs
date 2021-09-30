using System;
using static BiblePayCommon.Common;
using static BiblePayCommonNET.CommonNET;
using static BiblePayCommonNET.UICommonNET;
using static Unchained.Common;

namespace Unchained
{
    public partial class Wallet : BBPPage
    {
        protected new void Page_Load(object sender, EventArgs e)
        {
            txtBiblePayAddress.Text = gUser(this).BiblePayAddress;

        }

        protected override void Event(BBPEvent e)
        {
            if (e.EventName == "SentMoney")
            {
                string sPin = BiblePayCommon.Encryption.Base64Decode((e.Extra.Output ?? "").ToString());
                DACResult r30 = UICommon.BuySomething2(this, sPin);
            }
        }

        protected void btnSendBBP_Click(object sender, EventArgs e)
        {

            if (txtBiblePayAddress.Text=="")
            {
                MsgModal(this, "Error", "Sorry, you must log in first.", 400, 200, true);
                return;
            }

            BiblePayCommon.Entity.invoice1 invoice = new BiblePayCommon.Entity.invoice1();
            invoice.Amount = GetDouble(txtAmount.Text.ToString());
            invoice.BillToAddress = txtBiblePayAddress.Text;
            invoice.BillFromAddress = txtRecipientAddress.Text;
            invoice.ProductID = "User->User";
            if (!BiblePayDLL.Sidechain.ValidateAddress(IsTestNet(this), invoice.BillFromAddress))
            {
                MsgModal(this, "Error", "Sorry, invalid destination address", 400, 200, true);
                return;
            }
            UICommon.BuySomething(this, invoice, "SentMoney");
        }
    }
}
