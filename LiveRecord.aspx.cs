using Google.Authenticator;
using System;
using static BiblePayCommon.Common;
using static BiblePayCommonNET.UICommonNET;
using static Unchained.Common;
using static BiblePayCommonNET.StringExtension;
using System.Net.Mail;
using System.Web;

namespace Unchained
{
    public partial class LiveRecord : BBPPage
    {
        protected new void Page_Load(object sender, EventArgs e)
        {
            if (!gUser(this).LoggedIn)
            {
                Response.Redirect("VideoList");

            }
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {

        }

    }
}