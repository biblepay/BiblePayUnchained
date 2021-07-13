using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static Unchained.StringExtension;

namespace Unchained
{
    public partial class LandingPage : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string sAction = Request.QueryString["action"].ToNonNullString();
            string sValue = Request.QueryString["value"].ToNonNullString();
            if (sAction == "session")
            {
                string sKey = Request.QueryString["key"].ToNonNullString();
                string sNewPage = Request.QueryString["newpage"].ToNonNullString();
                Session["key_" + sKey] = sValue;
                Response.Redirect(sNewPage);

            }
        }


    }
}