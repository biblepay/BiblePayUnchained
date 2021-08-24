using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net.Mail;
using static Unchained.Common;
using System.Data.SqlClient;
using System.Data;
using Google.Authenticator;
using static BiblePayCommon.Common;

namespace Unchained
{
    public partial class RegisterMe : BBPPage
    {

        User _user = new User();
        protected new void Page_Load(object sender, EventArgs e)
        {

            if (_user.LoggedIn == false)
                _user = gUser(this);

            // Normal flow:
            if (_user.LoggedIn)
            {
                if (!IsPostBack)
                {
                    txtNickName.Text = _user.UserName;
                    txtEmailAddress.Text = _user.EmailAddress;
                    lblVerified.Text = _user.Verified == 1 ? "Verified" : "Not Verified";
                    txtHashTags.Text = _user.HashTags;
                }
            }
        }

        protected override void Event(BBPEvent e)
        {
        }
        protected void btnExplain_Click(object sender, EventArgs e)
        {
            string sNarr = "In order to get started, everyone needs a biblepay unchained keypair.  This is completely free!  The keypair allows you to receive BIBLEPAY and send BIBLEPAY to one another, "
                +"allows you to tip others or content, "
                + " allows you to pay for advanced features such as programmatic uploads etc.   For the most part our social media system is free, and paid for by your generous tithes (tipping videos, donations etc). ";
            sNarr += "<br><br>Before you can save any information, you need a wallet first.  To create one, click the Wallet icon in the top left corner, and this will display your biblepay wallet.  "
                +" From there, click Generate Address.  "
                + "This action will create your keypair (you will see the address populate).  Once you do this, then you can save your Nick Name, or edit your user record.";
            sNarr += "<br><br>After you have an address, come back here (My User Maintenance) and type in a NickName, and click Save.  Optionally, you can set your avatar here also. ";
            sNarr += "<br><br> Once these things are done, then you can comment on prayers, add prayers, add videos, and comment on Videos!  You can also upvote and downvote content. ";
            sNarr += "<br><br> Thank you for using BiblePay!";

            UICommon.MsgModal(this, "Explanation for User Maintenance and Unchained Wallet", sNarr, 500, 600);

        }

        protected void btnSetTwoFactor_Click(object sender, EventArgs e)
        {
            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
            string title = gUser(this).UserName.Replace(" ", "_");
            string sSecKey = gUser(this).BiblePayAddress;
            if (sSecKey == String.Empty)
            {
                MsgBox("Error", "Unable to create 2fa code: You must generate a wallet first.", this);
            }
            var setupInfo = tfa.GenerateSetupCode("BiblePay Unchained", title, sSecKey, false, 100);
            this.imgQrCode.ImageUrl = setupInfo.QrCodeSetupImageUrl;
            this.imgQrCode.Visible = true;
            lblQR.Visible = true;
            lblQR.Text = "Manual Entry: " + setupInfo.ManualEntryKey;
        }
        
       
        protected void btnValidateTwoFactor_Click(object sender, EventArgs e)
        {
            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
            string pin = txttwofactorcode.Text;
            if (pin == "")
            {
                MsgBox("Pin Empty", "Sorry, the pin is empty.  Unable to test the code.  Please click back and try again. ", this);
            }
            bool fPassed = tfa.ValidateTwoFactorPIN(gUser(this).BiblePayAddress, pin);
            string sNarr = fPassed ? "Success.  <br>Thank you.  " : "Failure!  The 2FA code does not work.  Please click back and generate a new code and try again.  ";
            string sSucNar = fPassed ? "Success" : "Fail";
            MsgBox(sSucNar, sNarr, this);
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            if (txtNickName.Text.Length < 3)
            {
                MsgBox("Error", "Nick Name must be longer than 3 chars.", this);
            }

            User u = gUser(this);
            u.UserName = txtNickName.Text;
            u.BiblePayAddress = UICommon.GetBBPAddress(this);
            u.EmailAddress = txtEmailAddress.Text;
            u.HashTags = txtHashTags.Text;
            bool fSaved = SaveUserRecord(IsTestNet(this), u, this);

            if (fSaved)
                lblStatus.Text = "Updated.";
        }

        protected void btnSetAvatar_Click(object sender, EventArgs e)
        {
            Response.Redirect("UnchainedUpload?action=setavatar");
        }
        protected void btnMainNet_Click(object sender, EventArgs e)
        {
            Session["mainnet_mode"] = 1;
            Session["balance"] = null;
            Response.Redirect("RegisterMe");
        }

        protected void btnTestNet_Click(object sender, EventArgs e)
        {
            Session["mainnet_mode"] = 2;
            Session["balance"] = null;
            Response.Redirect("RegisterMe");
        }

        protected void btnSendSMSCode_Click(object sender, EventArgs e)
        {
            if (gUser(this).Verified == 1)
            {
                MsgBox("Error", "Sorry, you are already verified.", this);
            }
            if (gUser(this).BiblePayAddress == null || gUser(this).BiblePayAddress == "")
            {
                MsgBox("Error", "Sorry, you must create a wallet first.  Please go back to Account, then click Generate New Wallet.  Then populate your nickname, and click Save. ", this);
            }
            Random r = new Random();
            int nRandom = r.Next(10000, 99999);
            bool fSent = BMS.SendSMSCode(txtPhoneNumber.Text, nRandom);
            Session["expecting"] = nRandom;
            if (!fSent)
            {
                MsgBox("Error", "We were unable to send your SMS.  Please try including a +1 or a + plus the Country Code first before the phone number and try again.  "
                    + "If the problem persists, please open a ticket in github with the example phone number or send an e-mail to contact@biblepay.org. ", this);
            }
            else
            {
                MsgBox("Sent", "We sent you a pin.  Please enter it on your account record, and then you will be verified!", this);
            }
        }

        protected void btnVerifySMSCode_Click(object sender, EventArgs e)
        {
            double nExpecting = GetDouble(Session["expecting"]);
            if (nExpecting == 0)
            {
                MsgBox("Error", "Sorry, we are not expecting a pin from you.", this);
            }
            if (GetDouble(txtPin.Text) == GetDouble(Session["expecting"]))
            {
                User u = gUser(this);
                u.Verified = 1;
                SaveUserRecord(IsTestNet(this), u, this);
                MsgBox("Congratulations", "Your account has been upgraded to Verified", this);
            }
            else
            {
                MsgBox("Error", "Sorry, this pin you entered is invalid.  ", this);
            }
        }
      }
    }

