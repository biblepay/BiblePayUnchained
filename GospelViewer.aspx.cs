using static Unchained.Common;
using static BiblePayCommonNET.StringExtension;
using System.Net;
using System;

namespace Unchained
{
    public partial class GospelViewer : BBPPage
    {
        protected new void Page_Load(object sender, EventArgs e)
        {
            string sPDFSource = BiblePayCommon.Encryption.Base64Decode(Request.QueryString["pdfsource"].ToNonNullString());
            if (sPDFSource != "")
            {
                BiblePayCommon.BiblePayClient b = new BiblePayCommon.BiblePayClient();
                byte[] b1 = BiblePayDLL.Sidechain.DownloadBytes(sPDFSource, "pdf.pdf");
                if (b1 != null)
                {
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-length", b1.Length.ToString());
                    Response.BinaryWrite(b1);
                    Response.Flush();
                    Response.End();
                }
            }
        }
        protected string GetContent()
        {
            string sSource = Request.QueryString["type"].ToNonNullString();
            string sPath = Server.MapPath("JesusChrist/" + sSource + ".htm");
            if (!System.IO.File.Exists(sPath))
            {
                UICommon.MsgBox("Error", "Sorry, this content does not exist.", this);
            }

            string sData = System.IO.File.ReadAllText(sPath);
            return sData;
        }
    }
}