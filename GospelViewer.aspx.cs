using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using static Unchained.StringExtension;
using static Unchained.Common;

namespace Unchained
{
    public partial class GospelViewer : BBPPage
    {


        protected string GetContent()
        {
            string sSource = Request.QueryString["type"].ToNonNullString();
            string sPath = Server.MapPath("JesusChrist/" + sSource + ".htm");
            if (!System.IO.File.Exists(sPath))
            {
                MsgBox("Error", "Sorry, this content does not exist.", this);
            }

            string sData = System.IO.File.ReadAllText(sPath);
            return sData;
        }

    }
}