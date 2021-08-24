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
using System.IO;

namespace Unchained
{
    public partial class DownloadFile : BBPPage
    {

        protected void DownloadFile1()
        {

            string sURL = Request.QueryString["filename"] ?? "";
            string sNewFN = Guid.NewGuid().ToString() + ".mp4";
            string sPath = Path.Combine(Path.GetTempPath(), sNewFN);
            bool fSuccess = BiblePayDLL.Sidechain.DownloadStoredObject(sURL, sPath);

            using (FileStream fs = File.OpenRead(sPath))
            {
                int length = (int)fs.Length;
                byte[] buffer;

                string r = _cancelurl;

                using (BinaryReader br = new BinaryReader(fs))
                {
                    buffer = br.ReadBytes(length);
                }

                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", String.Format("attachment;filename={0}", Path.GetFileName(sPath)));
                Response.ContentType = "video/mp4";
                Response.BinaryWrite(buffer);
                Response.End();
            }

        }

        protected new void Page_Load(object sender, EventArgs e)
        {
            string sURL = Request.QueryString["filename"] ?? "";
            System.Threading.Thread t = new System.Threading.Thread(DownloadFile1);
            return;

        }
    }
}