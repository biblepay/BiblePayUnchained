﻿using Google.Authenticator;
using System;
using System.Net.Mail;
using System.Web;
using static BiblePayCommon.Common;
using static BiblePayCommonNET.CommonNET;
using static BiblePayCommonNET.UICommonNET;
using static Unchained.Common;

namespace Unchained
{
    public partial class RegisterMe : BBPPage
    {

        User _user = new User();
        protected new void Page_Load(object sender, EventArgs e)
        {

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
                    this.Page.Session["stack"] = Toast("Updated", "Thank you!  Your e-mail has been verified!");
                    Response.Redirect("RegisterMe");
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
                  lblVerified.Text = _user.Verified == 1 ? "Verified" : "Not Verified";
                  txtBiblePayAddress.Text = _user.BiblePayAddress;
                  txtPublicText.Text = _user.PublicText;
                  txtPrivateText.Text = _user.PrivateText;
                  txtReligiousText.Text = _user.ReligiousText;
                  txtProfessionalText.Text = _user.ProfessionalText;
                  txtTelegramLinkName.Text = _user.TelegramLinkName;
                  txtTelegramDescription.Text = _user.TelegramLinkDescription;
                  txtTelegramURL.Text = _user.TelegramLinkURL;
                  ddGender.Items.Clear();
                  ddGender.Items.Add("Male");
                  ddGender.Items.Add("Female");
                  ddGender.SelectedValue = _user.Gender;
                  txtBirthDate.Text = BiblePayCommon.Common.ConvertFromUnixTimestamp(_user.BirthDate).ToShortDateString();

                  ddTheme.Items.Clear();
                  ddTheme.Items.Add("Black");
                  ddTheme.Items.Add("Maroon");
                  ddTheme.Items.Add("Blue");
                  ddTheme.Items.Add("Grey");
                  ddTheme.SelectedValue = _user.ThemeName;

                  imgQrCode.Visible = false;
                  lblQR.Visible = false;
                  txtTwoFactorEnabled.Text = _user.FA2Verified == 1 ? "2FA Enabled" : "Not Enabled";
                  lblEmailVerified.Text = _user.EmailVerified == 1 ? "Verified" : "Not Verified";

                 
            }




            if (_user.LoggedIn)
            {
                btnSetAvatar.Visible = true;
                btnVerifyEmail.Visible = true;
                btnSendCode.Visible = true;
                btnSetTwoFactor.Visible = true;
                btnCheckTwoFactor.Visible = true;
                btnRemoveTwoFactor.Visible = true;
            }
            else
            {
                btnSetAvatar.Visible = false;
                btnVerifyEmail.Visible = false;
                btnSendCode.Visible = false;
                btnSetTwoFactor.Visible = false;
                btnRemoveTwoFactor.Visible = false;
                btnCheckTwoFactor.Visible = false;
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




        }

        protected override void Event(BBPEvent e)
        {
             if (e.EventName == "SentMoney")
            {
                string sPin = BiblePayCommon.Encryption.Base64Decode((e.Extra.Output ?? "").ToString());
                DACResult r30 = UICommon.BuySomething2(this, sPin);
            }
            else if (e.EventName=="ValidateTwoFactor_Click")
            {
                string sPin = BiblePayCommon.Encryption.Base64Decode((e.Extra.Output ?? "").ToString());
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
                    bool fSaved = SaveUserRecord(IsTestNet(this), u, this);
                    if (fSaved)
                    {
                        Session["2FA"] = null;

                        this.Page.Session["stack"] = Toast("Saved", "Your pin has been verified!");
                        Response.Redirect("RegisterMe");
                    }
                }
                else
                {
                    MsgModal(this, sSucNar, sNarr, 450, 250, true);
                }
            }
        }
        protected void btnVerifyEmail_Click(object sender, EventArgs e)
        {
            User u = gUser(this);
            if (!u.LoggedIn)
            {
                MsgModal(this, "Error", "Sorry, you must be logged in", 400, 200, true);
                return;
            }
            string sID = (u.id ?? "").ToString();
            if (sID == "")
            {
                MsgModal(this, "Error", "Sorry, you must save your user record first before verifying your e-mail address", 400, 200, true);
                return;
            }

            MailMessage m = new MailMessage();
            string sDomainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string sURL = sDomainName + "/RegisterMe?action=verifyemail&id=" + sID;
            string sURLA = "<a href='" + sURL + "'>confirm your user record field that you requested</a>";
            string sNarr = "Dear " + u.FirstName + ",<br><br>Please " + sURLA + " to unlock your account privileges"
                +".<br><br>Thank you.<br>The Crypto Team<br>";
            m.Subject = "[Transactional Message] Please confirm the user record field that you requested.";
            m.Body = sNarr;
            m.IsBodyHtml = true;
            m.To.Add(new MailAddress(u.EmailAddress, u.FirstName));
            DACResult r = BiblePayDLL.Sidechain.SendMail(IsTestNet(this), m);
            MsgModal(this, r.OverallResult ? "Sent" : "Not Sent", r.OverallResult ? "Your notification has been sent" : "Your notification failed.", 400, 200, true);
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
            Response.Redirect("RegisterMe");
        }

        protected void btnSendBBP_Click(object sender, EventArgs e)
        {
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
            u.ProfessionalText = txtProfessionalText.Text;
            u.PublicText = txtPublicText.Text;
            u.ReligiousText = txtReligiousText.Text;
            u.PrivateText = txtPrivateText.Text;

            u.LastName = txtLastName.Text;
            u.BiblePayAddress = txtBiblePayAddress.Text;
            u.EmailAddress = txtEmailAddress.Text;
            bool fPWMatches = txtPRIVLOGONINFORMATIONConfirm.Text == txtPRIVLOGONINFORMATION.Text;
            if (fPWMatches && txtPRIVLOGONINFORMATION.Text.Length > 0)
            { 
                if (IsPasswordStrong(txtPRIVLOGONINFORMATION.Text) && fPWMatches)
                {
                    u.PasswordHash = BiblePayCommon.Encryption.GetSha256HashI(txtPRIVLOGONINFORMATION.Text);
                }
                else
                {
                    u.PasswordHash = null; 
                }
            }
            
            u.ThemeName = ddTheme.SelectedValue.ToString();
            u.Gender = ddGender.SelectedValue.ToString();
            u.TelegramLinkName = txtTelegramLinkName.Text;
            u.TelegramLinkURL = txtTelegramURL.Text;
            u.TelegramLinkDescription = txtTelegramDescription.Text;

            try
            {
                u.BirthDate = (int)DateTimeToUnixTimestamp(Convert.ToDateTime(txtBirthDate.Text));
            }
            catch(Exception)
            {

            }

            return u;
        }

        protected void btnUpdateSocialMediaProfile_Click(object sender, EventArgs e)
        {
            BiblePayDLL.Sidechain.dictTables.Remove("video1");
            btnRegister_Click(sender, e);
        }

        protected void btnUpdateTelegramProfile_Click(object sender, EventArgs e)
        {
            btnRegister_Click(sender, e);
        }


        protected void btnRegister_Click(object sender, EventArgs e)
        {
            string sOldTheme = gUser(this).ThemeName;
            User u = PrepareUserRecord();
            bool fPWMatches = txtPRIVLOGONINFORMATIONConfirm.Text == txtPRIVLOGONINFORMATION.Text;
            if (!fPWMatches)
            {
                MsgModal(this, "Password Error", "Sorry, the passwords do not match.", 350, 200, true);
                return;
            }
            bool fSaved = SaveUserRecord(IsTestNet(this), u, this);
            if (fSaved)
            {
                lblStatus.Text = "Updated.";
                if (sOldTheme != u.ThemeName && sOldTheme != null)
                {
                    UICommon.MsgBox("Record Saved", "Your account record has been saved and your theme has been updated.", this);
                }
                else
                {
                    this.Page.Session["stack"] = Toast("Saved", "Your user record has been Updated!");
                    Response.Redirect("RegisterMe");
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
            if (gUser(this).BiblePayAddress == null || gUser(this).BiblePayAddress == "")
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

