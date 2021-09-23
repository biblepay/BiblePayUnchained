using System;
using System.Data;
using System.Web.UI;
using static BiblePayCommon.Common;
using static BiblePayCommon.DataTableExtensions;
using static Unchained.Common;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text;
using System.IO;
using static BiblePayCommonNET.UICommonNET;
using static BiblePayCommonNET.StringExtension;
using static BiblePayCommonNET.CommonNET;

namespace Unchained
{
    public partial class Media : BBPPage
    {

        protected new void Page_Load(object sender, EventArgs e)
        {


        }

        protected override void Event(BBPEvent e)
        {

            string data1 = Request.Form.ToString();

            if (e.EventAction == "EditVideo_Click")
            {
                Response.Redirect("FormView?action=edit&id=" + e.EventValue + "&table=video1");
            }
            else if (e.EventAction == "ContextMenu_Back")
            {
                Response.Redirect("VideoList");
            }
            else if (e.EventAction == "btnTip_Click")
            {
                if (gUser(this).LoggedIn == false)
                {
                    MsgModal(this, "Error", "Sorry, You must log in first.", 400, 200);
                    return;

                }
                User uTip  = UICommon.GetUserRecord(IsTestNet(this), e.Extra.TipTo.ToString());
                string sChannelName = uTip.FullUserName();
                if (uTip.BiblePayAddress==null)
                {
                    MsgModal(this, "Error", "Sorry, this user does not have a biblepay address.", 400, 240, true);
                    return;
                }
                Session["tipto"] = uTip.id.ToString();
                Session["videoid"] = e.Extra.ContentID.ToString();

                UICommon.MsgInput(this, "Tipping_Click", "Tipping", "Enter tip amount for "
                        + sChannelName + ":", 700,
                        "", "", UICommon.InputType.number, false);
            }
            else if (e.EventName == "Tipping_Click")
            {
                string sPub = gUser(this).BiblePayAddress;
                double nAmt = BiblePayCommon.Common.GetDouble(BiblePayCommon.Encryption.Base64Decode(e.Extra.Output.ToString()));
                string sTipTo = (Session["tipto"] ?? "").ToString();
                User uTip = UICommon.GetUserRecord(IsTestNet(this), sTipTo);
                string sChannelName = uTip.FullUserName();
                if (sPub == String.Empty)
                {
                        MsgModal(this, "Error", "Sorry, you must have a wallet in order to tip.", 400, 200);
                        return;
                }
                if (sTipTo == String.Empty)
                {
                        MsgModal(this, "Error", "Sorry, the user you are trying to tip does not exist.", 400, 200);
                        return;
                }
 
                string sProductID = e.EventValue;  // Verify if this is the video ID 
                BiblePayCommon.Entity.invoice1 i = new BiblePayCommon.Entity.invoice1();
                i.BillFromAddress = uTip.BiblePayAddress;
                i.BillToAddress = gUser(this).BiblePayAddress;
                i.Amount = nAmt;
                i.Data = "Tipped " + sChannelName + " [Video] for " + nAmt.ToString() + " BBP.";
                i.ProductID = e.EventValue;
                i.ServiceName = sChannelName;
                i.InvoiceType = "Tip";
                i.InvoiceDate = System.DateTime.Now.ToString();
                UICommon.BuySomething(this, i, "Tipped_Click");
            }
            else if (e.EventName=="Tipped_Click")
            {
                string sPin = BiblePayCommon.Encryption.Base64Decode(e.Extra.Output.ToString());

                DACResult r30 = UICommon.BuySomething2(this, sPin);
                if (r30.fError())
                {
                    this.Page.Session["stack"] = Toast("Failure", "The tip failed! " + r30.Error);

                }
                else
                {
                    this.Page.Session["stack"] = Toast("Tipped", "You have tipped this channel on TXID " + r30.Result);
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {

        }

        protected void btnAdmin_Click(object sender, EventArgs e)
        {
       
        }

        protected string RenderControl2(HtmlGenericControl h)
        {
            StringBuilder generatedHtml = new StringBuilder();
            using (var htmlStringWriter = new StringWriter(generatedHtml))
            {
                using (var htmlTextWriter = new HtmlTextWriter(htmlStringWriter))
                {
                    h.RenderControl(htmlTextWriter);
                    string data = generatedHtml.ToString();
                    return data;
                }
            }
        }

        protected string GetComments()
        {
            string sID = Request.QueryString["id"].ToNonNullString();
            string html1 = UICommon.GetComments(IsTestNet(this), sID, this);
            return html1;
        }

        protected string GetVideo()
        {
            BiblePayVideo.Video video1 = new BiblePayVideo.Video(this);
            string sID = Request.QueryString["id"].ToNonNullString();
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable2(IsTestNet(this), "video1");
            dt=dt.FilterDataTable("id='" + sID + "'");
            if (dt.Rows.Count < 1)
                return String.Empty;
            string html = String.Empty;
            video1.URL = dt.GetColValue("URL");
            video1.SVID = dt.GetColValue("SVID");
            video1.Body = dt.GetColValue("Body");
            video1.Title = dt.GetColValue("Title");
            if (video1.Title == "")
            {
                video1.Title = "N/A";
            }
            video1.Width = 800;
            video1.Height = 600;
            video1.ID = "video1" + sID;

            string sFID = dt.GetColValue("FID");
            string sDownloadLink = "<a href='" + video1.URL + "'>Download</a>";
            string sTheirChannel = "VideoList?channelid=" + dt.GetColValue("userid");

            video1.Footer = "Uploaded by <a href='" + sTheirChannel + "'>"
                + UICommon.GetUserAvatarAndName(this, dt.GetColValue("userid"), true)
                + "</a> • " + GetObjectRating(IsTestNet(this), sID, "video1", gUser(this)) + " • "
                + UICommon.GetFollowControl(IsTestNet(this), dt.GetColValue("userid"), gUser(this).BiblePayAddress)
                + " • " + UICommon.GetTipControl(IsTestNet(this), dt.GetColValue("id"), dt.GetColValue("userid"))
                + "<br>" + UICommon.GetWatchSum(IsTestNet(this), sID) + " view(s) • "
                + dt.GetColDateTime(0, "time").ToString()
                + " • " + sDownloadLink + " • " + dt.GetColValue("Category");

            video1.Footer += " • " + dt.GetColValue("domain");

            video1.Playable = true;
            if (HasOwnership(IsTestNet(this), sID, "video1", gUser(this).id))
            {
                string sButton = "&nbsp;&nbsp;" + UICommon.GetStandardButton(sID, "Edit", "EditVideo", "Edit Video");
                video1.Footer += sButton;
            }
            string sTranscript = dt.GetColValue("Transcript");
            sTranscript = sTranscript.Replace("\n", "<br>");
            string sVideoControl = UICommon.RenderControl(video1);
            html = "<table><tr><td class='videosingle' >" 
                + sVideoControl + "</td><td valign='top'><div class='videosingle' id='transcript1'>" + sTranscript + "</div></td></tr></table>";
            // Increment the count
            UICommon.StoreCount(sID, this, "video");
            return html;
        }
    }
}
