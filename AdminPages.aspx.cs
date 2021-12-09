using System;
using System.Data;
using System.Web.UI;
using static BiblePayCommon.Common;
using static BiblePayCommon.DataTableExtensions;
using static BiblePayCommonNET.StringExtension;
using static Unchained.Common;

namespace Unchained
{
    public partial class AdminPages : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            bool fPlays = BiblePayDLL.Sidechain.UserPlaysRole(IsTestNet(this), "Superuser", gUser(this).id, "");
            if (!fPlays)
            {
                UICommon.MsgBox("Error", "You are not authorized", this);
            }
        }

    }
}