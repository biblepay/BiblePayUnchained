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

        protected string GetLogOnLogOffButton(Page p)
        {
            string s1 = UICommon.GetStandardButton("btnLogIn", Common.gUser(p).LoggedIn ? "Log Off" : "Log In", Common.gUser(p).LoggedIn ? "LogOut" : "LogIn",
                Common.gUser(p).LoggedIn ? "Log Out of the system" : "Log into the system", "", "largebuttondark");
            return s1;
        }
        protected string GetHeaderImage(Page p)
        {
            string sPoweredBy = Common.Config("poweredby");
            string sAnchor = BiblePayCommon.Common.ExtractXML(sPoweredBy, "<a", ">");
            string sFullAnchor = "";
            if (sAnchor.Length > 1)
            {
                sFullAnchor = "<a " + sAnchor + " target='_blank'>";
            }
            string sImgPath = "Images/" + Common.Config("logo");
            string sHTMLImage = sFullAnchor + "<img style='padding:1px;height:42px;position:relative;' src='" + sImgPath + "' />";
            if (sFullAnchor.Length > 1)
            {
                sHTMLImage += "</a>";
            }

            string html = sHTMLImage + "<br /><span style='text-align:right;left:1px;top:-20px;position:relative;font-size:7px;'>" + sPoweredBy + "</span>";
            return html;
        }
        protected void btnMessageBox_Click(object sender, EventArgs e)
        {
        }
    }
}