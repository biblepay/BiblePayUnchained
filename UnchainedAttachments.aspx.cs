using System;
using System.Data;
using System.IO;
using System.Text;
using System.Web.UI;
using Telerik.Web.UI;
using static BiblePayCommon.Common;
using static BiblePayCommonNET.StringExtension;
using static BiblePayCommonNET.UICommonNET;
using static Unchained.Common;

namespace Unchained
{
    public partial class UnchainedAttachments : BBPPage
    {
        public string _action = "";

        protected new void Page_Load(object sender, EventArgs e)
        {
           
        }
        protected string GetAttachments()
        {
            string id = Request.QueryString["parentid"] ?? "";// "1638026921";
            string a = UICommon.GetAttachmentsWithoutHeader(this, id, "", "Ticket Attachments", "");
            return a;
        }
        protected string OffchainUpload(BiblePayCommon.Entity.object1 o1, int iFileNo)
        {
            string sCat = Request.Form["ddCategory"].ToNonNullString();
            if (iFileNo == 1 || true)
            {
                o1.Category = sCat;
            }
            BiblePayDLL.Sidechain.UploadIntoDSQL_Background(IsTestNet(this), ref o1, gUser(this));
            return o1.URL;
        }

        protected void btnUnchainedSave_Click(object sender, EventArgs e)
        {

            string sParentID = Request.QueryString["parentid"].ToNonNullString();

            if (!gUser(this).LoggedIn)
            {
                UICommon.MsgBox("Error", "Sorry you must log in first. ", this);
                return;
            }
            if (sParentID == "")
            {
                UICommon.MsgBox("Error", "Sorry, this parent is not found. ", this);
                return;
            }

            StringBuilder sb = new StringBuilder();
            bool fSuccess = false;
            if (AsyncUpload1.UploadedFiles.Count < 1)
            {
                MsgModal(this, "Upload Error", "Sorry, you have not uploaded any files yet.  You must select the file and upload it before saving the record.  (NOTE: Please ensure that the dot is green before clicking SAVE.)", 500, 200);
                return;
            }

            int iFileNo = 0;
            foreach (UploadedFile f in AsyncUpload1.UploadedFiles)
            {
                string sSubj = "attachment " + DateTime.Now.ToString();
                string sBody = "attachment " + DateTime.Now.ToString();
                iFileNo++;
                string sPath = Common.GetFolderUnchained("Temp");
                string extension = Path.GetExtension(f.FileName);
                string newName = Guid.NewGuid().ToString() + extension;
                string fullpath = Path.Combine(sPath, newName);

                if (UICommon.IsAllowableExtension(extension))
                {
                    f.SaveAs(fullpath);

                    BiblePayCommon.Entity.object1 o = new BiblePayCommon.Entity.object1();
                    o.FileName = fullpath;
                    o.ParentID = sParentID;
                    o.Title = sSubj;
                    o.Subject = sSubj;
                    o.Body = sBody;
                    o.Attachment = 1;

                    if (_action == "setavatar" || _action == "setattachment" || _action == "setticketattachment")
                    {
                        o.Attachment = 1;
                        //o.id = Guid.NewGuid().ToString();
                    }
                    string sURL = OffchainUpload(o, iFileNo);
                    fSuccess = true;  //mission critical; check the calling process, verify this fSuccss flag is actually valid.

                }
            }
            // Redirects 


            if (fSuccess)
            {
                Response.Redirect("UnchainedAttachments?parentid=" + sParentID);
                // string narr = "Thank you for using our Decentralized Social Media System.  <br><br>Your video will be available as soon as it is transcoded (Usually within 20 minutes). ";
                // UICommon.MsgBox("Social Media Video Uploaded", narr, this);
            }
            else
            {
                UICommon.MsgBox("Upload Error", "An upload error occurred.  File Upload Count: " + AsyncUpload1.UploadedFiles.Count.ToString(), this);
            }
        }
    }
}
