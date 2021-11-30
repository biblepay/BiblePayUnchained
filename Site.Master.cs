using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Unchained
{
    public partial class SiteMaster : MasterPage
    {

        private static int nVersion = 1407;
        protected void Page_Load(object sender, EventArgs e)
        {
            string sThemeName = UICommon.GetCurrentThemeName(this.Page);
            string sFullPath = "/content/sidenav_" + sThemeName + ".css";
            string sLink = "<link rel=\"stylesheet\" type='text/css' href='" + sFullPath + "' />";
            sitecss.Text = sLink;
            string sAppleTouchIcon = Unchained.Common.Config("appletouchicon", "/Images/unchained_apple_touch.png");
            string sFavIcon = Unchained.Common.Config("favicon", "/Images/unchained_favicon.ico");

            AppleTouchIcon.Href = sAppleTouchIcon;
            FavIcon.Href = sFavIcon;
        }

        protected void btnMessageBox_Click(object sender, EventArgs e)
        {
        }
    }
}