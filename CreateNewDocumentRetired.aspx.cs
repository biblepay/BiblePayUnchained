using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using static Unchained.Common;
using System.IO;

namespace Unchained
{
    public partial class CreateNewDocument : BBPPage
    {
        protected new void Page_Load(object sender, EventArgs e)
        {
            string sLoadFrom = Request.QueryString["file"] ?? "";
            /*
            if (sLoadFrom != "")
            {
                   Response.Write("<body onload=\"populateWiki('" + sLoadFrom + "');\"");
            }
            */

        }

        protected string SaveDoc(string sPath, string sTitle)
        {
            BiblePayCommon.Common.DACResult r = BiblePayDLL.Sidechain.UploadFileTypeBlob(IsTestNet(this), sPath, gUser(this));
            if (r.fError())
            {
                UICommon.MsgBox("Error", "Unable to upload [06052021]=" + r.Error + ".", this);
            }
            string sURL = r.Result;

            BiblePayCommon.Entity.video1 o = new BiblePayCommon.Entity.video1();
            o.Title = sTitle;
            o.Body = "wikipedia-article";
            o.UserID = gUser(this).id;
            o.URL = sURL;
            o.Classification = "wiki";
            r = DataOps.InsertIntoTable(this, IsTestNet(this), o, gUser(this));
            return sURL;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string sTest2 = Request.Unvalidated.Form["htmlcode"];
            string sSource = txtSaveAs.Text + ".htm";
            string sNewFN = Path.Combine(Path.GetTempPath(), sSource);
            BiblePayCommon.Common.WriteToFile(sNewFN, sTest2);
            string URL = SaveDoc(sNewFN, txtTitle.Text);
        }

    }
}
