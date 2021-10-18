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
using static Unchained.BiblePayUtilities;
using BiblePayCommonNET;

namespace Unchained
{
    public partial class Profile : BBPPage
    {
        protected new void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                User _user = gUser(this);
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
                txtBirthDate.Text = UnixTimeStampToDateControl(_user.BirthDate);
            }

        }

        protected User PrepareUserRecord()
        {
            User u = gUser(this);
            u.ProfessionalText = txtProfessionalText.Text;
            u.PublicText = txtPublicText.Text;
            u.ReligiousText = txtReligiousText.Text;
            u.PrivateText = txtPrivateText.Text;
            u.Gender = ddGender.SelectedValue.ToString();
            u.TelegramLinkName = txtTelegramLinkName.Text;
            u.TelegramLinkURL = txtTelegramURL.Text;
            u.TelegramLinkDescription = txtTelegramDescription.Text;
            u.BirthDate = DateToUnixTimeStamp(txtBirthDate.Text);

            return u;
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            string sOldTheme = gUser(this).ThemeName;
            User u = PrepareUserRecord();
            DACResult r = SaveUserRecord(IsTestNet(this), u, this);
            if (!r.fError())
            {
                 this.Page.Session["stack"] = BiblePayCommonNET.UICommonNET.Toast("Saved", "Your user record has been Updated!");
                 Response.Redirect("Profile");
            }
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

    }
}
