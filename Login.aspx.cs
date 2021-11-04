using Google.Authenticator;
using System;
using static BiblePayCommon.Common;
using static BiblePayCommonNET.UICommonNET;
using static Unchained.Common;
using static BiblePayCommonNET.StringExtension;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using static Unchained.UICommon;

namespace Unchained
{
    public partial class Login : BBPPage
    {
        protected new void Page_Load(object sender, EventArgs e)
        {
             if (!IsPostBack)
            {
                txtEmailAddress.Text = gUser(this).EmailAddress.ToNonNullString();

                if (gUser(this).LoggedIn)
                {
                    btnRegister.Visible = false;
                    btnLogin.Visible = false;
                    btnLogout.Visible = true;
                    btnLockedOut.Visible = false;
                    txtPRIVLOGONINFORMATION.Visible = false;
                    txtEmailAddress.Visible = false;
                    lblEmailAddress.Visible = false;
                    lblPassword.Visible = false;
                    lbl2FA.Visible = false;
                    lblEye.Visible = false;
                    lblOptional.Visible = false;
                    lblFieldset.Text = "Log Out";
                    txt2FACode.Visible = false;
                    
                }
                else
                {
                    btnRegister.Visible = true;
                    btnLogin.Visible = true;
                    btnLogout.Visible = false;
                    btnLockedOut.Visible = true;
                    txtPRIVLOGONINFORMATION.Visible = true;
                    txtEmailAddress.Visible = true;
                    lblEmailAddress.Visible = true;
                    lblPassword.Visible = true;
                    lbl2FA.Visible = true;
                    lblEye.Visible = true;
                    lblOptional.Visible = true;
                    lblFieldset.Text = "Log In";
                    txt2FACode.Visible = true;
                }
            }
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            Response.Redirect("RegisterMe.aspx");
        }


        protected void btnLockedOut_Click(object sender, EventArgs e)
        {
            if (gUser(this).LoggedIn)
            {
                MsgModal(this, "Error", "You are already logged in, this option is only available for locked out individuals. ", 350, 150);
                return;
            }
            User g = gUser(this, txtEmailAddress.Text);
            if (g.EmailAddress == null)
            {
                MsgModal(this, "Error", "Sorry, I can't find your user record. ", 350, 150);
                return;
            }
            if (g.EmailVerified != 1)
            {
                MsgModal(this, "Error", "Sorry, your e-mail address is not verified.  I can't reset your password.", 350, 150);
                return;
            }

            double nReplay = BiblePayCommon.Common.GetDouble(BiblePayCommon.HalfordDatabase.GetKVDWX("LockedOut" + g.id));
            if (nReplay == 1)
            {
                MsgModal(this, "Error", "Sorry, you must wait at least 30 minutes before sending a new locked out e-mail.  Please check your junk folder. ", 400, 200, true);
                return;
            }
            BiblePayCommon.HalfordDatabase.SetKVDWX("LockedOut" + g.id, 1, 60 * 30);

            MailMessage m = new MailMessage();
            string sDomainName = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string sURL = sDomainName + "/RegisterMe?action=resetpassword&id=" + g.id.ToString() + "&RSAKey=" + g.RSAKey.ToString();
            string sNarr = "Dear " + g.FirstName + ", <br>To reset your password, please click <a href='" + sURL + "'>here.<br><br>Thank you.<br>";
            m.Subject = "Reset Password Request";
            m.Body = sNarr;
            m.IsBodyHtml = true;
            m.To.Add(new MailAddress(g.EmailAddress, g.FirstName));
            EmailNarr e1 = GetEmailFooter(this);

            DACResult r = BiblePayDLL.Sidechain.SendMail(IsTestNet(this), m, e1.DomainName);
            MsgModal(this, r.OverallResult ? "Sent" : "Not Sent", 
                r.OverallResult ? "Your password reset request has been e-mailed to you!" : "Your notification failed.", 400, 200, true);
        }


        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (txtEmailAddress.Text=="")
            {
                MsgModal(this, "Error", "Please enter your e-mail address to login. ", 350, 150);
                return;
            }
            if (gUser(this).LoggedIn)
            {
                MsgModal(this, "Already Logged In", "You are already logged in.", 450, 400);
            }
            else
            {
                User u = gUser(this, txtEmailAddress.Text);
                bool fPasswordPassed = (BiblePayCommon.Encryption.GetSha256HashI(txtPRIVLOGONINFORMATION.Text) == u.PasswordHash);
                if (txtPRIVLOGONINFORMATION.Text == u.PasswordHash && u.PasswordHash.Length == 4)
                {
                    fPasswordPassed = true;
                }
                bool f2FAPassed = true;
                if (u.FA2Verified == 1)
                {
                    f2FAPassed = BiblePayDLL.Sidechain.Verify2FA(IsTestNet(this), u, txt2FACode.Text);
                }
                if (!fPasswordPassed)
                {
                    MsgModal(this, "Authentication Error", "Sorry, that didn't work.", 450, 200, true);
                }
                else if (!f2FAPassed)
                {
                    MsgModal(this, "Authentication Error", "Sorry, that didn't work. [Your 2FA pin is invalid].", 450, 200, true);
                }
                else if (f2FAPassed && fPasswordPassed)
                {
                    UICommon.LogIn(this, u);
                }
                else
                {
                    MsgModal(this, "Authentication Error", "Sorry, an unknown error occurred.", 450, 200, true);
                }
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session[GetChain0(IsTestNet(this)) + "user"] = null;
            this.Page.Session["stack"] = UICommon.Toast("Logging Off", "You are now logged off.");
            // Cookie
            BMS.StoreCookie("pwhash", "");
            Response.Redirect("Login");
        }
    }
}