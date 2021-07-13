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

namespace Unchained
{
    public partial class RegisterMe : Page
    {

        protected string GetSubVersion()
        {
            double nSubVersion = GetDouble(GetLocalStorage(this, "subversion"));
            return nSubVersion.ToString();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            string sSess = Base64Decode(Request.Form["sessionLocalStorage"].ToNonNullString());
            if (sSess != "")
            {
                Session["localStorage"] = sSess;
            }

            string sPostback = Session["POSTBACK_seedWords"].ToNonNullString();
            if (sPostback != "")
            {
                Session["POSTBACK_seedWords"] = "";
                string k1 = GenNewWallet(sPostback);

                string sNarr = "Wallet re-created.  "
                    + "Click <a href='#' onclick='" + TriggerFormSubmit(this, "RegisterMe", false) + "'>here</a> to continue.  ";
                Session["balance"] = null;

                MsgBox("Wallet Re-Created", sNarr + k1, this);

                // After updating the cookies, we need to refresh the page.
                //Response.Write("Generated");
                //TriggerFormSubmit(this, "RegisterMe", true, "");
                return;
                //Response.Redirect("RegisterMe");
            }
            // Normal flow:
            // if (!IsPostBack)
            //  {
            if (!IsPostBack)
            {
                if ((gUser(this).UserName ?? "") != "")
                {
                    txtNickName.Text = gUser(this).UserName;
                }
            }
                txtAddress.Text = GetLocalStorage(this, "bbp_address");
                string EncStatus = BiblePayDLL.Shared.GetDouble(GetLocalStorage(this, "enc")) == 1 ? "ENCRYPTED" : "NON-ENCRYPTED";
                lblEncrypted.Text = EncStatus;
                if (EncStatus == "ENCRYPTED")
                {
                    btnSendMoney.Attributes.Add("onclick", "AddDecryptionPassword();");
                }
                else
                {
                    btnSendMoney.Attributes.Add("onclick", "showSpinner();");
                }
                /*
                // If we recover both, let's repersist them so they dont lose their keys easily...
                if (txtAddress.Text.Length == 34 && sPriv.Length == 52)
                {
                    (GetBBPAddressCookieName(this), txtAddress.Text, TimeSpan.FromHours(24 * 365));
                    (GetBBPAddressPKCookieName(this), 56(sPriv), TimeSpan.FromHours(24 * 365));
                }
                */

           // }
        }


        protected void btnTest_Click(object sender, EventArgs e)
        {

        }
        protected void btnSetTwoFactor_Click(object sender, EventArgs e)
        {
            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
            string title = gUser(this).UserName.Replace(" ", "_");
            string sSecKey = txtAddress.Text;
            if (sSecKey == "")
            {
                MsgBox("Error", "Unable to create 2fa code: You must generate a wallet first.", this);
            }
            var setupInfo = tfa.GenerateSetupCode("BiblePay Unchained", title, sSecKey, false, 100);
            this.imgQrCode.ImageUrl = setupInfo.QrCodeSetupImageUrl;
            this.imgQrCode.Visible = true;
            lblQR.Visible = true;
            lblQR.Text = "Manual Entry: " + setupInfo.ManualEntryKey;
        }

        protected void btnDestroyWallet_Click(object sender, EventArgs e)
        {
            SetLocalStorage(this, "bbp_address", "");
            SetLocalStorage(this, "bbp_privkey", "");
            SetLocalStorage(this, "enc", "0");
            TriggerFormSubmit(this, "RegisterMe");
        }
        protected string GenNewWallet(string words)
        {
            words = words.Trim();
            string[] vWords = words.Split(" ");
            if (vWords.Length != 10)
            {
                MsgBox("Error", "Unable to recreate wallet from an unknown number of words.", this);
            }
            string sHash = GetSha256HashNoSpaces(words);
            BiblePayDLL.Shared.KeyType k = BiblePayDLL.Shared.DeriveNewKey(IsTestNet(this), sHash);

            string k1 =   SetLocalStorage(this, "bbp_address", k.PubKey);
            k1 +=  SetLocalStorage(this, "bbp_privkey", BiblePayDLL.Encryption.EncryptAES256(k.PrivKey, ""));
            k1 +=  SetLocalStorage(this, "enc", "0");
            double nSubVersion = GetDouble(GetLocalStorage(this, "subversion"));

            k1 += SetLocalStorage(this, "subversion", (nSubVersion+1).ToString());


            Log("Recovering from words: [" + words + "]" + k.PubKey);
            return k1;
            
        }
        protected void btnGenerateNewAddress_Click(object sender, EventArgs e)
        {
            if (txtAddress.Text != "")
            {
                MsgBox("Error", "Sorry, but you already have a wallet.  If you want to destroy your old wallet, please delete your browser cookies or submit a feature request for us to add a Destroy Wallet feature.", this);
            }
            string words = BiblePayDLL.Data.GetRandomWords();
            string sNarr = "Wallet created.  Please copy the 10 biblical words below into notepad and save these in a safe place.  Note that if you lose these you will lose access to your balance.  Additionally, no one at BIBLEPAY can help you recover your wallet if you lose these codes.  The browser will store these encrypted for a while but once you clear your cookies we will ask you to log in again.  Without these codes, you will NOT BE ABLE TO LOG IN.  Please back them up NOW before continuing! ";
            sNarr += "<br><br>SAVE THESE IN A SAFE PLACE:<br><br><font color=red><input style='width:700px;' type='text' id='w' name='w' value='" + words + "' readonly /></font><br><br>"
                + "Next, click <a href='#' onclick='" + TriggerFormSubmit(this, "RegisterMe", false) + "'>here</a> to continue.  "
                + "Now you will see that you have a biblepay address.  ";
            string k1 = GenNewWallet(words);
            
            MsgBox("Please Save These Seed Words", sNarr + k1, this);
        }

        protected void btnRecoverFromSeedWords_Click(object sender, EventArgs e)
        {
            string sPub = GetLocalStorage(this, "bbp_address");
            double nBal = 0;
            if (sPub != "")
            {
                nBal = BiblePayDLL.Data.QueryAddressBalance(IsTestNet(this), sPub);
            }

            string sWarning = nBal > 0 ? "WARNING: You have a " + nBal.ToString() + " wallet balance!  If you regenerate your wallet from seed words, you will no longer have access to your balance!  We recommend that you send this BBP out to a new wallet first!  Use extreme caution! <br><br><br>" : "Warning:  Importing a new seed word will destroy your old wallet.  You appear to have a zero balance.  <br><br>";


            string sNarr = sWarning + "<br><br>Let's regenerate your wallet from the seed words.  Please paste the 10 biblical words in the textbox below:<br> ";
            sNarr += "<br>Enter Seed Words:  <font color=red><input style='width:900px;' type='text' id='seedWords' name='seedWords' value='' /></font><br>"
                + "<br><button id='Next' type='submit'>Create a Brand New Wallet</button> ";

            Session["MSGBOX_POSTMONITOR"] = "seedWords";
            Session["MSGBOX_POSTBACK"] = "RegisterMe.aspx";
            MsgBox("Recover Wallet from Seed Words", sNarr, this);
        }
        protected void btnEncryptWallet_Click(object sender, EventArgs e)
        {
            string sPass1 = Request.Form["passforencryption"].ToNonNullString();
            string sPub = GetLocalStorage(this, "bbp_address");
            if (sPub == "")
            {
                MsgBox("Error", "You must have a wallet generated first.", this);
            }

            string sPriv = BiblePayDLL.Encryption.DecryptAES256(GetLocalStorage(this, "bbp_privkey"), "");
            if (sPriv == "" || sPriv.Length < 24)
            {
                MsgBox("Error", "Your wallet must be valid and unencrypted in order to encrypt it.", this);
            }

            string sNewPriv = BiblePayDLL.Encryption.EncryptAES256(sPriv, sPass1);
            string k1 = SetLocalStorage(this, "bbp_privkey", sNewPriv);
            k1 += SetLocalStorage(this, "enc", "1");
            string sNarr = "Click <a href='#' onclick='" + TriggerFormSubmit(this, "RegisterMe", false) + "'>here</a> to continue.  ";

            MsgBox("Encrypted", "Your wallet has been encrypted." + sNarr + k1, this);
        }
        protected void btnSendMoney_Click(object sender, EventArgs e)
        {
            string pass1 = Request.Form["passforencryption"].ToNonNullString();

            string sPriv = BiblePayDLL.Encryption.DecryptAES256(GetLocalStorage(this, "bbp_privkey"), pass1);
            string sPub = GetLocalStorage(this, "bbp_address");
            if (sPub == "")
            {
                MsgBox("Error", "You must have a wallet created first.", this);
            }

            if (sPriv.Length < 24)
            {
                MsgBox("Error", "Your wallet is corrupted.  Please restore it with Restore from Seed Phrases.  Or enter your decryption password first. ", this);
            }

            BiblePayDLL.SharedCommon.DACResult dr1 = BiblePayDLL.Data.SpendMoney(IsTestNet(this), this, BiblePayDLL.Shared.GetDouble(txtSpendAmount.Text), txtDestination.Text, sPriv, "<test>spend 06052021</test>");
            if (dr1.Error == "")
            {
                MsgBox("Success", "Sent: TXID=" + dr1.TXID, this);
            }
            else
            {
                MsgBox("Failure", dr1.Error, this);
            }

        }
        protected void btnValidateTwoFactor_Click(object sender, EventArgs e)
        {
            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
            string pin = txttwofactorcode.Text;
            if (pin == "")
            {
                MsgBox("Pin Empty", "Sorry, the pin is empty.  Unable to test the code.  Please click back and try again. ", this);

            }
            bool fPassed = tfa.ValidateTwoFactorPIN(txtAddress.Text, pin);
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
            string k1 = SetLocalStorage(this, "NickName", txtNickName.Text);
            string sNarr = "Click <a href='#' onclick='" + TriggerFormSubmit(this, "RegisterMe", false) + "'>here</a> to continue.  ";
            MsgBox("NickName Updated", sNarr + k1, this);
        }

        protected void btnMainNet_Click(object sender, EventArgs e)
        {
            Session["mainnet_mode"] = 1;
            Session["balance"] = null;

            Response.Redirect("RegisterMe");
        }

        protected void btnTestNet_Click(object sender, EventArgs e)
        {
            Session["mainnet_mode"] = 0;
            Session["balance"] = null;
            Response.Redirect("RegisterMe");
        }

    }
}