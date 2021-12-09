using Google.Authenticator;
using System;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using static BiblePayCommon.Common;
using static BiblePayCommonNET.CommonNET;
using static BiblePayCommonNET.UICommonNET;
using static Unchained.Common;
using static Unchained.UICommon;

namespace Unchained
{
    public partial class RegisterNewUser : BBPPage
    {

        public static string GetGDPRText()
        {
            string sOverriddenTOS = Config("TermsOfServiceURL", "Content/TermsOfService.htm");
            string sOverriddenPrivacyPolicy = Config("PrivacyPolicyURL", "Content/PrivacyPolicy.htm");

            string sTOS = "<a href='" + sOverriddenTOS + "' target='_blank'>Terms of Use</a>";
            string sPP = "<a href='" + sOverriddenPrivacyPolicy + "' target='_blank'>Privacy Policy</a>";
            string text = "&nbsp;I agree to this sites " + sTOS + " and " + sPP;
            return text;
        }
        protected new void Page_Load(object sender, EventArgs e)
        {
            chkGDPRVerified.Text = GetGDPRText();
            string sAction = Request.QueryString["action"] ?? "";
        }

        protected override void Event(BBPEvent e)
        {

        }


        protected User PrepareUserRecord()
        {
            User u = gUser(this);
            u.FirstName = txtFirstName.Text;
            u.LastName = txtLastName.Text;
            u.EmailAddress = txtEmailAddress.Text;
            u.GDPRVerified = chkGDPRVerified.Checked ? 1 : 0;
            bool fPWMatches = txtPRIVLOGONINFORMATIONConfirm.Text == txtPRIVLOGONINFORMATION.Text;
            if (fPWMatches && txtPRIVLOGONINFORMATION.Text.Length > 0)
            { 
                if (IsPasswordStrong(txtPRIVLOGONINFORMATION.Text) && fPWMatches)
                {
                    u.PasswordHash = BiblePayCommon.Encryption.GetSha256HashI(txtPRIVLOGONINFORMATION.Text);
                    this.Session["pwhash"] = BiblePayCommon.Encryption.EncryptAES256(txtPRIVLOGONINFORMATION.Text, "");

                }
                else
                {
                    u.PasswordHash = null; 
                }
            }
            return u;
        }


        protected void btnRegister_Click(object sender, EventArgs e)
        {
            User u = PrepareUserRecord();
            bool fPWMatches = txtPRIVLOGONINFORMATIONConfirm.Text == txtPRIVLOGONINFORMATION.Text;
            if (!fPWMatches)
            {
                MsgModal(this, "Password Error", "Sorry, the passwords do not match.", 350, 200, true);
                return;
            }
            if (gUser(this).LoggedIn == false && chkGDPRVerified.Checked == false)
            {
                MsgModal(this, "GDPR Error", "Sorry, you must first review the Privacy Policy and Terms of Service and agree to the terms before creating a new account.  ", 500, 200);
                return;
            }

            if (!IsEmailValid(txtEmailAddress.Text))
            {
                MsgModal(this, "Error", "Sorry, the e-mail address is invalid. ", 500, 200);
                return;
            }

            if (gUser(this).LoggedIn == true)
            {
                MsgModal(this, "Error", "Sorry, you are already logged in.", 500, 200);
                return;
            }

            if (UserExists(this, txtEmailAddress.Text))
            {
                MsgModal(this, "Error", "User already exists.  To edit your account record simply Log In then click on My Account. ", 500, 200);
                return;
            }

            this.Session["pwhash"] = BiblePayCommon.Encryption.EncryptAES256(txtPRIVLOGONINFORMATIONConfirm.Text, "");

            DACResult r0 = SaveUserRecord(IsTestNet(this), u, this);
            if (!r0.fError())
            {
                MsgModalWithRedirectToNewPage(this, "Welcome Aboard", "Welcome aboard!  It is wonderful to have you here.  You are now logged in. ", 400, 200, "VideoList");
            }
            else
            {
                MsgModal(this, "Error", "We were unable to save your record: " + r0.Error, 400, 300);

            }
        }

      }
    }
