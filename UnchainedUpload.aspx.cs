using OpenHtmlToPdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
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
                lblBody.Text = "(Optional) Write something about yourself IE your Testimony:";
                lblSubject.Text = "(Optional) Write your Slogan here:";
                string[] vExt = ".jpeg,.jpg,.bmp,.png".Split(",");
                AsyncUpload1.AllowedFileExtensions = vExt;
                if (!IsPostBack)
                {

                    txtBody.Text = gUser(this).Testimony;
                    txtSubject.Text = gUser(this).Slogan;
                }
            }
            else
            {
                lblPageHeading.Text = "Upload your contribution here:";
                lblBody.Text = "Body:";
                lblSubject.Text = "Subject:";
            }
        }

      
        protected void btnUnchainedSave_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            bool fSuccess = false;
            //if (FileUpload1.HasFile)
            foreach (UploadedFile f in AsyncUpload1.UploadedFiles)
            {
                string sSubj = txtSubject.Text;
                string sBody = txtBody.Text;
                bool fAvatar = (_action == "setavatar");

                if (!fAvatar)
                {
                    if (sSubj.Length < 2 || sBody.Length < 2)
                    {
                        MsgBox("Error", "Sorry, the subject or body must be a little longer.", this);
                    }
                }


                if (!gUser(this).LoggedIn || gUser(this).UserName == "")
                {
                    MsgBox("Error", "Sorry you must set up your nickname in Account Settings first. ", this);
                }

                sb.AppendFormat(" Uploading file: {0}", f.FileName);
                string sPath = Common.GetFolderUnchained("Temp");
                //string extension = Path.GetExtension(FileUpload1.FileName);
                string extension = Path.GetExtension(f.FileName);

                string newName = Guid.NewGuid().ToString() + extension;
                string fullpath = Path.Combine(sPath, newName);
                if (UICommon.IsAllowableExtension(extension))
                {
                    //FileUpload1.SaveAs(fullpath);
                    f.SaveAs(fullpath);
                    //Showing the file information
                    sb.AppendFormat("<br/> Save As: {0}", f.FileName);
                    sb.AppendFormat("<br/> File type: {0}", f.ContentType);
                    sb.AppendFormat("<br/> File length: {0}", f.ContentLength);
                    sb.AppendFormat("<br/> File name: {0}", f.FileName);
                   
                    // Make an Unchained Object
                    BiblePayCommon.Common.DACResult r = BiblePayDLL.Sidechain.UploadFileTypeBlob(IsTestNet(this), fullpath, 
                        GetFundingAddress(IsTestNet(this)), GetFundingKey(IsTestNet(this)));
                    // Store the DSQL object also

                    if (r.Error != "" && r.Error != null)
                    {
                        MsgBox("Error", "Unable to upload [06052021]=" + r.Error + ".", this);
                    }
                    string ext = Path.GetExtension(extension).ToLower();
                    ext = ext.Substring(1, ext.Length - 1);
                    ext = ext.ToLower();
                    string sURL = r.Result;


                    BiblePayCommon.Entity.video1 o = new BiblePayCommon.Entity.video1();
                    o.Subject = txtSubject.Text;
                    o.Title = txtSubject.Text;
                    o.Body = txtBody.Text;
                    o.UserID = gUser(this).BiblePayAddress.ToString();
                    o.URL = sURL;
                    o.deleted = 0;
                    
                    o.Classification = UICommon.ExtensionToClassification(ext);
                    r = DataOps.InsertIntoTable(IsTestNet(this), o);

                    if (r.Error != "")
                    {
                        MsgBox("Error", "Unable to store video - " + r.Error, this);
                    }
                    if (r.Result.ToString() != "")
                    {

                        if (_action == "setavatar")
                        {
                            User u = gUser(this);
                            u.AvatarURL = o.URL;
                            if (txtBody.Text != "")
                                u.Testimony = txtBody.Text;
                            if (txtSubject.Text != "")
                                u.Slogan = txtSubject.Text;

                            bool fSaved = SaveUserRecord(IsTestNet(this), u, this);
                            if (fSaved)
                            {
                                Session["stack"] = UICommon.Toast("Updated", "Your avatar has been updated!");
                            }
                            else
                            {
                                Session["stack"] = UICommon.Toast("Not Updated", "Your avatar failed.");
                            }
                        }


                        fSuccess = true;
                        break;
                    }
                    else
                    {
                        string test99 = "";

                    }

                }
                else
                {
                    MsgBox("Error", "Sorry, this extension is not yet supported. ", this);
                }
            }
           
            if (fSuccess)
            {
                if (_action == "setavatar")
                {
                    Response.Redirect("RegisterMe.aspx");
                }
                string narr = "Thank you for using BiblePay Social Media.  <br>Your video will be available as soon as it is transcoded (Usually within 20 minutes). ";
                MsgBox("Social Media Video Uploaded", narr, this);
            }
            else
            {
                MsgBox("Error", "An error occurred.  ", this);

            }

        }
    }
}
