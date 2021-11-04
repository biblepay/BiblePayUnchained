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

        private static int nVersion = 1405;
        protected void Page_Load(object sender, EventArgs e)
        {
            string sThemeName = UICommon.GetCurrentThemeName(this.Page);
            string sFullPath = "/content/sidenav_" + sThemeName + ".css";
            //<link rel="stylesheet" type="text/css" href="https://forum.biblepay.org/Themes/Offside/css/index.css?fin20" />
            string sLink = "<link rel=\"stylesheet\" type='text/css' href='" + sFullPath + "' />";
            sitecss.Text = sLink;
        }

        protected void btnMessageBox_Click(object sender, EventArgs e)
        {
        }
    }
}