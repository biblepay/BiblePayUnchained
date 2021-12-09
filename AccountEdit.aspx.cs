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
    public partial class AccountEdit : BBPPage
    {

        public static void SendEmailVerificationRequest(Page p)
        {
            User u = gUser(p);
            if (!u.LoggedIn)
            {
                MsgModal(p, "Error", "Sorry, you must be logged in", 400, 200, true);
                return;
            }
            string sID = (u.id ?? "").ToString();
            if (sID == "")
            {
                MsgModal(p, "Error", "Sorry, you must save your user record first before verifying your e-mail address", 400, 200, true);
                return;
            }

            // (Replay attack prevention)
            double nReplay = BiblePayCommon.Common.GetDouble(BiblePayCommon.HalfordDatabase.GetKVDWX("VerifyEmail" + u.id));
            if (nReplay == 1)
            {
                MsgModal(p, "Error", "Sorry, you must wait at least 7 minutes before sending a new verification e-mail.  Please check your junk folder. ", 400, 200, true);
                return;
            }
            BiblePayCommon.HalfordDatabase.SetKVDWX("VerifyEmail" + u.id, 1, 60 * 7);
            EmailNarr e1 = GetEmailFooter(p);

            string sDomainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string sURL = sDomainName + "/AccountEdit?action=verifyemail&id=" + sID;
            string sURLA = "<a href='" + sURL + "'>confirm your e-mail address</a>";
            string sNarr = "Dear " + u.FirstName + ",<br><br>Please " + sURLA + " to update as VERIFIED"
                + ".<br><br>Thank you.<br>The " + e1.DomainName + " Team<br>";
            BiblePayCommon.Common.BiblePayMailMessage m = new BiblePayMailMessage();

            m.PrimaryMailMessage.Subject = "[Transactional Message] Please confirm your E-mail Address";
            m.PrimaryMailMessage.Body = sNarr;
            m.PrimaryMailMessage.IsBodyHtml = true;
            m.UserToAddress.Add(u);

            DACResult r = BiblePayDLL.Sidechain.SendMail(IsTestNet(p), m, e1.DomainName, true);
            MsgModal(p, r.OverallResult ? "Sent" : "Not Sent", r.OverallResult ? "Your notification has been sent" : "Your notification failed.", 400, 200, true);
        }

        public static string GetGDPRText()
        {
            string sOverriddenTOS = Config("TermsOfServiceURL", "Content/TermsOfService.htm");
            string sOverriddenPrivacyPolicy = Config("PrivacyPolicyURL", "Content/PrivacyPolicy.htm");

            string sTOS = "<a href='" + sOverriddenTOS + "' target='_blank'>Terms of Use</a>";
            string sPP = "<a href='" + sOverriddenPrivacyPolicy + "' target='_blank'>Privacy Policy</a>";
            string text = "&nbsp;I agree to this sites " + sTOS + " and " + sPP;
            return text;
        }

        User _user = new User();
        protected new void Page_Load(object sender, EventArgs e)
        {
            //chkGDPRVerified.Text = GetGDPRText();

             _user = gUser(this);
            string sAction = Request.QueryString["action"] ?? "";
            if (sAction == "verifyemail")
            {
                string sID = Request.QueryString["id"] ?? "";
                User u = gUserById(this, sID);
                if (u.EmailAddress != null)
                {
                    u.EmailVerified = 1;
                    SaveUserRecord(IsTestNet(this), u, this);
                    MsgModalWithRedirectToNewPage(this, "Verified", "Thank you!  Your E-mail has been verified!", 400, 200, "AccountEdit");
                    return;
                }
                else
                {
                    MsgModal(this, "Error", "Unable to find user.", 300, 150, true);
                    return;
                }
            }
            else if (sAction == "resetpassword")
            {
                if (gUser(this).LoggedIn)
                {
                    MsgModal(this, "Error", "This feature is only for locked out users", 300, 150, true);
                    return;
                }
                string sID = Request.QueryString["id"] ?? "";
                string sPass = Request.QueryString["RSAKey"] ?? "";
                User u = gUserById(this, sID);
                if (u.EmailAddress != null && sPass == u.RSAKey)
                { 
                    u.PasswordHash = BiblePayCommon.Encryption.GetSha256HashI(u.id);
                    this.Session["pwhash"] = BiblePayCommon.Encryption.EncryptAES256(u.id.ToString(), "");

                    SaveUserRecord(IsTestNet(this), u, this);
                    string sNarr = "Your password has been temporarily reset to: <br>" + u.id.ToString() + "<br>Please log in and change it.";
                    UICommon.MsgBox("Password Reset", sNarr, this);
                }
                else
                {
                    MsgModal(this, "Error", "Unable to find user.", 300, 150, true);
                    return;
                }

            }
            if (!IsPostBack)
            {
                  txtFirstName.Text = _user.FirstName;
                  txtLastName.Text = _user.LastName;
                  txtEmailAddress.Text = _user.EmailAddress;
                  chkDoNotSpam.Checked = _user.EmailDoNotAdvertise == 1;
                  
                  ddTheme.Items.Clear();
                  ddTheme.Items.Add("Black");
                  ddTheme.Items.Add("Maroon");
                  ddTheme.Items.Add("Blue");
                  ddTheme.Items.Add("Grey");
                  ddTheme.Items.Add("WhiteBlue");
                  ddTheme.SelectedValue = _user.ThemeName;

                  imgQrCode.Visible = false;
                  lblQR.Visible = false;
                  txtTwoFactorEnabled.Text = _user.FA2Verified == 1 ? "2FA Enabled" : "Not Enabled";
                  lblEmailVerified.Text = _user.EmailVerified == 1 ? "Verified" : "Not Verified";
                  txtDomain.Text = _user.domain.ToNonNullString2();

            }

            txtPhoneNumber.Visible = false;
            lblPhoneNumber.Visible = false;

            if (_user.LoggedIn)
            {
                btnVerifyEmail.Visible = true;
               // btnSendCode.Visible = true;
                btnSetTwoFactor.Visible = true;
                btnCheckTwoFactor.Visible = true;
                btnRemoveTwoFactor.Visible = true;
                btnModifyProfile.Visible = true;

            }
            else
            {
                btnVerifyEmail.Visible = false;
                //btnSendCode.Visible = false;
                btnSetTwoFactor.Visible = false;
                btnRemoveTwoFactor.Visible = false;
                btnCheckTwoFactor.Visible = false;
                btnModifyProfile.Visible = false;
            }

            if (_user.LoggedIn)
            {
                if (_user.FA2Verified == 1)
                {
                    btnRemoveTwoFactor.Visible = true;
                    btnSetTwoFactor.Visible = false;
                    btnCheckTwoFactor.Visible = false;
                    
                }
                else
                {
                    btnRemoveTwoFactor.Visible = false;
                    btnSetTwoFactor.Visible = true;
                    btnCheckTwoFactor.Visible = true;
                }

                if (_user.EmailVerified==1)
                {
                    btnVerifyEmail.Visible = false;
                }
                else
                {
                    btnVerifyEmail.Visible = true;
                }
            }

            // If Domain has a custom theme
            string sDomainDefault = Config("defaulttheme");
            if (sDomainDefault != "")
            {
                ddTheme.Visible = false;
                lblTheme.Visible = false;
            }

        }

        protected override void Event(BBPEvent e)
        {
            if (e.EventName=="ValidateTwoFactor_Click")
            {
                string sPin = BiblePayCommon.Encryption.Base64DecodeWithFilter((e.Extra.Output ?? "").ToString());
                if (sPin == "")
                {
                    MsgModal(this, "Pin Empty", "Sorry, the pin is empty.  Unable to test the code.  Please click back and try again. ", 450, 200);
                    return;
                }

                string sSecKey = (Session["2FA"] ?? "").ToString();
                if (sSecKey == "")
                {
                    MsgModal(this, "Empty", "Sorry, you did not Set Up the 2FA code yet - please click Set Up 2FA first. ", 450, 200);
                    return;
                }
                TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
                bool  fPassed = tfa.ValidateTwoFactorPIN(sSecKey, sPin);
                string sNarr = fPassed ? "Success.  <br>Thank you.  " : "Failure!  The 2FA code does not work.  You can try again by clicking Set Up 2FA. ";
                string sSucNar = fPassed ? "Success" : "Fail";
                if (fPassed)
                {
                    User u = gUser(this);
                    u.FA2Verified = 1;
                    u.Shared2FA = BiblePayDLL.Sidechain.RSAEncryptValue(Session["2FA"].ToString());
                    DACResult r0 = SaveUserRecord(IsTestNet(this), u, this);
                    if (!r0.fError())
                    {
                        Session["2FA"] = null;
                        this.Page.Session["stack"] = BiblePayCommonNET.UICommonNET.Toast("Saved", "Your pin has been verified!");
                        Response.Redirect("AccountEdit");
                    }
                }
                else
                {
                    MsgModal(this, sSucNar, sNarr, 450, 250, true);
                }
            }
            else if (_bbpevent.EventName == "DeleteConfirmed")
            {
                if (gUser(this).LoggedIn == false)
                {
                    return;
                }
                string sConfirm = BiblePayCommon.Encryption.Base64DecodeWithFilter((e.Extra.Output ?? "").ToString());
                if (sConfirm == "CONFIRM")
                {
                    User u = gUser(this);
                    BiblePayCommon.Entity.user1 o = ConvertUserToUserEntity(u);
                    o.deleted = 1;
                    DACResult r = DataOps.InsertIntoTable(this, IsTestNet(this), o, u);
                    if (r.fError())
                    {
                        MsgModal(this, "Failed", "Something went wrong while deleting your account.  Please email rob@biblepay.org with your username: " + r.Error, 400, 250, true);
                    }
                    else
                    {
                        Session[GetChain0(IsTestNet(this)) + "user"] = null;
                        BMS.StoreCookie("pwhash", "");
                        Session["pwhash"] = null;

                        MsgModal(this, "DELETED", "Your account has been deleted. ", 300, 200, true, true);

                    }
                }
                else
                {
                    MsgModal(this, "Failed", "Phrase incorrect.  Your account was not deleted", 300, 200);

                }
                string sData = "";
            }

        }

        protected void btnModifyProfile_Click(object sender, EventArgs e)
        {
            Response.Redirect("Profile");
        }

        protected void btnVerifyEmail_Click(object sender, EventArgs e)
        {
            SendEmailVerificationRequest(this);
        }

        protected void btnRemoveTwoFactor_Click(object sender, EventArgs e)
        {
            User u = gUser(this);
            if (!u.LoggedIn)
            {
                MsgModal(this, "Error", "Sorry, you must be logged in", 400, 200, true);
                return;
            }
            if (u.FA2Verified != 1)
            {
                MsgModal(this, "Error", "Sorry, you do not have two factor enabled.", 400, 200, true);
                return;
            }
            PrepareUserRecord();
            u.FA2Verified = 0;
            SaveUserRecord(IsTestNet(this), u, this);
            lblStatus.Text = "Removed.";
            Response.Redirect("AccountEdit");
        }

        

        protected string UserRoles()
        {
            if (_user.id == null)
                return "";
            string sUR = Admin.RunUserRolesReport(IsTestNet(this), _user.id, false, _user.id);
            return sUR;
        }
        protected void btnSetTwoFactor_Click(object sender, EventArgs e)
        {
            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
            if (!IsEmailValid(txtEmailAddress.Text))
            {
                MsgModal(this, "Error", "Your e-mail address must be valid.", 400, 200, true);
                return;
            }
            if (txtFirstName.Text.Length < 3 || txtLastName.Text.Length < 3)
            {
                MsgModal(this, "Error", "Your full name must be populated first.", 400, 200, true);
                return;
            }
            if (gUser(this).LoggedIn == false)
            {
                MsgModal(this, "Error", "You must be logged in first.", 400, 200, true);
                return;
            }

            string title = txtEmailAddress.Text.Replace(" ", "_");
            if (Session["2FA"]==null)
            {
                Session["2FA"] = Guid.NewGuid().ToString();
            }
            string sSecKey = Session["2FA"].ToString();
            var setupInfo = tfa.GenerateSetupCode(DOMAIN_NAME + "_" + GetChain0(IsTestNet(this)), title, sSecKey, false, 100);
            this.imgQrCode.ImageUrl = setupInfo.QrCodeSetupImageUrl;
            this.imgQrCode.Visible = true;
            lblQR.Visible = true;
            MsgModal(this, "Set Up 2FA", "To set up 2FA, scan the QR code using your 2FA app, by opening it and click Add New Site.  Then capture the QR code image, and ensure it is added.  Then click the Test 2FA button on this page to verify it works.", 400, 300, true);
        }
       
        protected void btnValidateTwoFactor_Click(object sender, EventArgs e)
        {
            UICommon.MsgInput(this, "ValidateTwoFactor_Click", "Validate Two Factor Code", "Please verify your two factor authentication code by entering the pin >",
                500, "", "", UICommon.InputType.number, false);
        }

        protected User PrepareUserRecord()
        {
            User u = gUser(this);
            u.FirstName = txtFirstName.Text;
            u.LastName = txtLastName.Text;
            u.EmailAddress = txtEmailAddress.Text;
            
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
            
            u.ThemeName = ddTheme.SelectedValue.ToString();
            return u;
        }


        protected void btnRegister_Click(object sender, EventArgs e)
        {
            if (gUser(this).LoggedIn == false)
            {
                MsgModal(this, "Error", "You must be logged in first. ", 500, 200);
                return;
            }
            string sOldTheme = gUser(this).ThemeName;
            User u = PrepareUserRecord();
            bool fPWMatches = txtPRIVLOGONINFORMATIONConfirm.Text == txtPRIVLOGONINFORMATION.Text;
            if (!fPWMatches)
            {
                MsgModal(this, "Password Error", "Sorry, the passwords do not match.", 350, 200, true);
                return;
            }
           
            DACResult r0 = SaveUserRecord(IsTestNet(this), u, this);
            if (!r0.fError())
            {
                lblStatus.Text = "Updated.";
                if (sOldTheme != u.ThemeName && sOldTheme != null)
                {
                    UICommon.MsgBox("Record Saved", "Your account record has been saved and your theme has been updated.", this);
                }
                else
                {
                        this.Page.Session["stack"] = BiblePayCommonNET.UICommonNET.Toast("Saved", "Your user record has been Updated!");
                        Response.Redirect("AccountEdit");
                }
            }
        }

        protected void btnSetAvatar_Click(object sender, EventArgs e)
        {
            Response.Redirect("UnchainedUpload?action=setavatar&parentid=" + gUser(this).id);
        }
        

        protected void btnSendSMSCode_Click(object sender, EventArgs e)

        {
            if (gUser(this).Verified == 1)
            {
                UICommon.MsgBox("Error", "Sorry, you are already verified.", this);
            }
            if (!gUser(this).LoggedIn)
            {
                MsgModal(this, "Error", "Sorry, you must save your user record first.", 450, 200);
            }
            Random r = new Random();
            int nRandom = r.Next(10000, 99999);
            bool fSent = BMS.SendSMSCode(txtPhoneNumber.Text, nRandom);
            Session["expecting"] = nRandom;
            if (!fSent)
            {
                UICommon.MsgBox("Error", "We were unable to send your SMS.  Please try including a +1 or a + plus the Country Code first before the phone number and try again.  "
                    + "If the problem persists, please open a ticket in github with the example phone number or send an e-mail to contact@biblepay.org. ", this);
            }
            else
            {
                UICommon.MsgBox("Sent", "We sent you a pin.  Please enter it on your account record, and then you will be verified!", this);
            }
        }
        protected void btnDeleteAccount_Click(object sender, EventArgs e)
        {
            string sNarr = "Per GDPR guidelines, we give you the ability to delete your account, your associated settings, your posts, your timeline and any associated data.  Your account will be deleted, and your corresponding data will be marked as For Deletion.  Our back end batch job will delete any personal information associated with your account and your uploads will be taken over by our Guest account.  To confirm this, please type CONFIRM into the box below";
            UICommon.MsgInput(this, "DeleteConfirmed", "Confirm Deletion", sNarr, 500, "", "", InputType.text, false, "DeleteConfirmed","");
        }

        protected void btnVerifySMSCode_Click(object sender, EventArgs e)
        {
            double nExpecting = GetDouble(Session["expecting"]);
            if (nExpecting == 0)
            {
                UICommon.MsgBox("Error", "Sorry, we are not expecting a pin from you.", this);
            }
            string pin = "";
            if (GetDouble(pin) == GetDouble(Session["expecting"]))
            {
                User u = gUser(this);
                u.Verified = 1;
                SaveUserRecord(IsTestNet(this), u, this);
                UICommon.MsgBox("Congratulations", "Your account has been upgraded to Verified", this);
            }
            else
            {
                UICommon.MsgBox("Error", "Sorry, this pin you entered is invalid.  ", this);
            }
        }
      }
    }
