using System;
using System.Data;
using System.IO;
using System.Text;
using Telerik.Web.UI;
using static BiblePayCommon.Common;
using static BiblePayCommonNET.StringExtension;
using static BiblePayCommonNET.UICommonNET;
using static Unchained.Common;

namespace Unchained
{
    public partial class UnchainedUpload : BBPPage
    {
        public string _action = "";
        protected new void Page_Load(object sender, EventArgs e)
        {
            _action = Request.QueryString["action"] ?? "";
            if (_action == "setavatar")
            {
                lblPageHeading.Text = "Upload your avatar picture here:";
                lblBody.Text = "(Optional) Avatar Text:";
                lblSubject.Text = "(Optional) Avatar Subject:";
                string[] vExt = ".jpeg,.jpg,.bmp,.png".Split(",");
                AsyncUpload1.AllowedFileExtensions = vExt;
            }
            else if (_action == "setattachment")
            {
                lblPageHeading.Text = "Upload your timeline attachment here:";
                lblBody.Text = "(Optional) Write something about your timeline post:";
                lblSubject.Text = "(Optional) Write a subject about your attachment:";
                txtBody.Text = "Timeline Attachment";
                txtSubject.Text = "Timeline Attachment";
            }
            else
            {
                lblPageHeading.Text = "Upload your contribution here:";
                lblBody.Text = "Body:";
                lblSubject.Text = "Subject:";
            }
            if (!IsPostBack)
            {
               
            }
        }

        protected string OffchainUpload(BiblePayCommon.Entity.object1 o1, int iFileNo)
        {
            string sCat = Request.Form["input_ddCategory"].ToNonNullString();
            if (iFileNo == 1)
            {
                o1.Category = sCat;
            }
            BiblePayDLL.Sidechain.UploadIntoDSQL_Background(IsTestNet(this), ref o1, gUser(this));
            return o1.URL;
        }

        protected void btnUnchainedSave_Click(object sender, EventArgs e)
        {

            if (!gUser(this).LoggedIn)
            {
                UICommon.MsgBox("Error", "Sorry you must log in first. ", this);
                return;
            }

            if (gUser(this).FullUserName() == "N/A" || gUser(this).FirstName == null || gUser(this).FirstName == "")
            {
                UICommon.MsgBox("Error", "Please set your First Name in your profile so that others can recognize you.", this);
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
                string sSubj = txtSubject.Text;
                string sBody = txtBody.Text;
                bool fAvatar = (_action == "setavatar");
                iFileNo++;

                if (!fAvatar)
                {
                    if (sSubj.Length < 3 || sBody.Length < 3)
                    {
                        UICommon.MsgBox("Error", "Sorry, the subject or body must be a little longer.", this);
                    }
                }
                string sPath = Common.GetFolderUnchained("Temp");
                string extension = Path.GetExtension(f.FileName);
                string newName = Guid.NewGuid().ToString() + extension;
                string fullpath = Path.Combine(sPath, newName);
                string sParentID = Request.QueryString["parentid"].ToNonNullString();

                if (UICommon.IsAllowableExtension(extension))
                {
                    f.SaveAs(fullpath);

                    BiblePayCommon.Entity.object1 o = new BiblePayCommon.Entity.object1();
                    o.FileName = fullpath;
                    o.ParentID = sParentID;
                    o.Title = txtSubject.Text;
                    o.Subject = txtSubject.Text;
                    o.Body = txtBody.Text;

                    o.Attachment = 0;

                    if (_action == "setavatar" || _action == "setattachment")
                    {
                        o.Attachment = 1;
                    }
                    string sURL = OffchainUpload(o, iFileNo);
                    fSuccess = true;  //mission critical; check the calling process, verify this fSuccss flag is actually valid.


                    if (_action == "setavatar")
                    {
                        User u = gUser(this);
                        u.AvatarURL = sURL;
                        bool fSaved = SaveUserRecord(IsTestNet(this), u, this);
                        if (fSaved)
                        { 
                            Session["stack"] = UICommon.Toast("Updated", "Your avatar has been updated!");
                        }
                        else
                        {
                            Session["stack"] = UICommon.Toast("Not Updated", "Your avatar failed.");
                        }
                        Response.Redirect("RegisterMe.aspx");
                    }
                    else if (_action == "setattachment")
                    {
                        Response.Redirect("Person.aspx");
                    }
                }
            }

            if (fSuccess)
            {
                string narr = "Thank you for using our Decentralized Social Media System.  <br><br>Your video will be available as soon as it is transcoded (Usually within 20 minutes). ";
                UICommon.MsgBox("Social Media Video Uploaded", narr, this);
            }
            else
            {
                UICommon.MsgBox("Upload Error", "An upload error occurred.  File Upload Count: " + AsyncUpload1.UploadedFiles.Count.ToString(), this);
            }
        }
    }
}
