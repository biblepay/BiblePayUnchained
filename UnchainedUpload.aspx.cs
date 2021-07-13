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
using static Unchained.Common;

namespace Unchained
{
    public partial class UnchainedUpload : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!gUser(this).LoggedIn)
            {
                MsgBox("Error", "Sorry you must set up your nickname in Account Settings first. ", this);
            }
            
            /*
            if (!IsPostBack)
            {
                FileUpload1.Attributes["onchange"] = "UploadFile(this)";
            }
            */
        }

        bool IsAllowableExtension(string path)
        {
            string ext = Path.GetExtension(path).ToLower();
            if (ext.Length < 1) return false;
            ext = ext.Substring(1, ext.Length - 1);
            string allowed = "jpg;jpeg;gif;bmp;png;pdf;csv;mp4";
            string[] vallowed = allowed.Split(";");
            for (int i = 0; i < vallowed.Length; i++)
            {
                if (vallowed[i] == ext)
                    return true;
            }
            return false;
        }
        protected void btnUnchainedSave_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
                       

            if (FileUpload1.HasFile)
            {

                string sSubj = txtSubject.Text;
                string sBody = txtBody.Text;
                if (sSubj.Length < 2 || sBody.Length < 2)
                {
                    MsgBox("Error", "Sorry, the subject or body must be a little longer.", this);
                }


                sb.AppendFormat(" Uploading file: {0}", FileUpload1.FileName);
                    string sPath = Common.GetFolderUnchained("Temp");
                    string extension = Path.GetExtension(FileUpload1.FileName);
                    string newName = Guid.NewGuid().ToString() + extension;
                    string fullpath = Path.Combine(sPath, newName);
                if (IsAllowableExtension(extension))
                {
                    FileUpload1.SaveAs(fullpath);
                    //Showing the file information
                    sb.AppendFormat("<br/> Save As: {0}", FileUpload1.PostedFile.FileName);
                    sb.AppendFormat("<br/> File type: {0}", FileUpload1.PostedFile.ContentType);
                    sb.AppendFormat("<br/> File length: {0}", FileUpload1.PostedFile.ContentLength);
                    sb.AppendFormat("<br/> File name: {0}", FileUpload1.PostedFile.FileName);
                   
                    // Make an Unchained Object
                    BiblePayDLL.SharedCommon.DACResult r = BiblePayDLL.Sidechain.UploadFileTypeVideo(IsTestNet(this), fullpath, GetFundingAddress(IsTestNet(this)), GetFundingKey(IsTestNet(this)));
                    // Store the DSQL object also

                    if (r.Error != "")
                    {
                        MsgBox("Error", "Unable to upload [06052021]=" + r.Error + ".", this);
                    }
                    string sURL = r.Result;
                    dynamic o = new System.Dynamic.ExpandoObject();
                    o.Subject = txtSubject.Text;
                    o.Body = txtBody.Text;
                    o.UserName = gUser(this).UserName.ToString();
                    o.URL = sURL;
                    string ext = Path.GetExtension(extension).ToLower();
                    ext = ext.Substring(1, ext.Length - 1);

                    ext = ext.ToLower();
                    string sClassification = "Unknown";
                    if (ext == "jpg" || ext == "png" || ext == "jpeg" || ext == "bmp" || ext == "gif")
                    {
                        sClassification = "image";
                    }
                    else if (ext == "pdf")
                    {
                        sClassification = "pdf";
                    }
                    else if (ext == "mp4")
                    {
                        sClassification = "video";
                    }
                    else if (ext == "mp3")
                    {
                        sClassification = "audio";
                    }
                    o.Classification = sClassification;

                    string sID = GetSha256Hash(sURL);
                    r = DataOps.InsertIntoTable(IsTestNet(this), o, "video1", sID);

                    if (r.Error != "")
                    {
                        MsgBox("Error", "Unable to store video - " + r.Error, this);
                    }
                    
                    string narr = "Thank you for using BiblePay Social Media.  <br>Your post should be available within 1 block on our home page. <br><a href='Default.aspx'>Please click here.<br>"
                        + "<br></a>";
                     MsgBox("Social Media Video Uploaded", narr, this);
                }
                else
                {
                    MsgBox("Error", "Sorry, this extension is not yet supported. ", this);
                }
            }
            else
            {
                lblmessage.Text = sb.ToString();
            }
        }
    }
}
