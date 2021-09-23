using static Unchained.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static BiblePayDLL.Shared;
using static BiblePayCommon.DataTableExtensions;
using static BiblePayCommon.Common;
using static BiblePayCommonNET.StringExtension;
using static BiblePayCommonNET.CommonNET;
using static BiblePayCommonNET.DataTableExtensions;

namespace Unchained
{
    public partial class VideoList : BBPPage
    {
        BiblePayPaginator.Paginator _paginator = null;
        protected new void Page_Load(object sender, EventArgs e)
        {
            _paginator = (BiblePayPaginator.Paginator)this.Master.FindControl("MainContent").FindControl("paginator1");
            int nPageNo = _paginator.PageNumber;

            if (IsPostBack)
            {
                //_paginator.PageNumber = 0;
                Session["search"] = txtSearch.Text;

            }
            else
            {
                txtSearch.Text = Session["search"].ToNonNullString();




            }
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {

        }

        protected override void Event(BBPEvent e)
        {
            if (e.EventID == "Delete")
            {
                  // Delete the object (logically)
                  bool fDeleted = BiblePayDLL.Sidechain.DeleteObject(IsTestNet(this), "video1", e.EventValue, gUser(this));
                  if (fDeleted)
                  {
                            UICommon.RunScriptSM(this, UICommon.Toast("Deleted", "Your Object was Deleted!"));
                  }
                  else
                  {
                            UICommon.RunScriptSM(this, UICommon.Toast("Not Deleted", "FAILURE: The object was not deleted."));
                  }
            }
        }

        private string GetInnerVideoRetired(string sID, string sURL)
        {
            bool fPlayable = false;
            string sDims = "width='400px' height='250px'";
            string sAutoPlay = fPlayable ? "autostart autoplay controls playsinline" : "preload='metadata'";
            string sHTML = "<video id='vid" + sID + "' " + sDims + " class='connect-bg' " + " " 
                + sAutoPlay + " >";
            string sLoc = !fPlayable ? "#t=7" : "#t=7";
            sHTML += "<source src='" + sURL + sLoc + "' type='video/mp4'></video>";
            return sHTML;
        }


        public static string GetTrendingList(bool fTestNet)
        {
            try
            {
                DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable2(fTestNet, "vote1");
                dt = dt.FilterDataTable("VoteValue in(0,1,-1)");

                if (dt.Rows.Count < 1)
                    return "";

                dt = dt.AsEnumerable()
                    .GroupBy(r => new { Col1 = r["ParentID"] })
                .Select(g =>
                {
                    var row = dt.NewRow();
                    row["VoteValue"] = g.Sum(r => Convert.ToInt32(r["VoteValue"]));
                    row["ParentID"] = g.Key.Col1;
                    return row;
                }).CopyToDataTable();

                dt = dt.FilterDataTable("VoteValue > 1");

                if (dt.Rows.Count == 0)
                    return "";
                string sList = "id in (";
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sList += "'" + dt.Rows[i]["ParentID"].ToString() + "',";
                    int nCt = (int)dt.GetColDouble(i, "VoteValue");
                }
                sList = Mid(sList, 0, sList.Length - 1);
                sList += ")";
                return sList;
            }
            catch(Exception ex)
            {
                Log("Error in GetTrending List :: " + ex.Message);
                return "";
            }
        }

        public static string GetFollowingList(bool fTestNet, string sUserID)
        {
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable2(fTestNet, "follow1");
            dt = dt.FilterDataTable("deleted=0 and UserID='" + sUserID + "'");
            if (dt.Rows.Count == 0)
                return "";
            string sList = "userid in (";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                sList += "'" + dt.Rows[i]["FollowedId"].ToString() + "',";
            }
            sList = Mid(sList, 0, sList.Length - 1);
            sList += ")";
            return sList;
        }

        public static string GetHashTagList(User g)
        {
            string[] vTags = g.HashTags.Split(" ");
            if (g.HashTags == "")
                return "";
            string sList = "hashtags like ";
            for (int i = 0; i < vTags.Length; i++)
            {
                string sTag = vTags[i];
                if (sTag.Length > 0)
                {
                    sList += "'%" + sTag + "%' or hashtags like ";
                }
            }
            sList = Mid(sList, 0, sList.Length - 18);
            sList += "";

            return sList;
        }

        protected string GetVideoList()
        {
            string sSearch = Session["Search"].ToNonNullString();


            string sType = Session["key_filetype"].ToNonNullString();
            string sCategory = (Request.QueryString["category"]  ?? "").ToString();
            string sTheirChannel = Request.QueryString["channelid"].ToNonNullString();

            if (sCategory != "" || sTheirChannel != "")
            {
                sType = "video";
            }

            if (sType == "")
            {
                sType = "video";
            }
            // Global filter (Federated vs. Private) etc.

            BiblePayCommon.BBPDataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable2(IsTestNet(this), "video1");
            
            string sVideoFilter = Config("videofilter");
            dt = dt.FilterBBPDataTable(sVideoFilter);
            dt = dt.FilterBBPDataTable("isnull(attachment,0)=0");
            dt = dt.FilterBBPDataTable("category not in (null,'')");

            if (sTheirChannel != "")
            {
                dt = dt.FilterBBPDataTable("userid='" + sTheirChannel + "'");
                sType = "video";
            }
            else if (sType == "mychannel")
            {
                dt = dt.FilterBBPDataTable("userid='" + gUser(this).id + "'");
                sType = "video";
            }
            else if (sType == "myvideoeditingroom")
            {
                // User videos that have been uploaded as 'mass' or 'batch', and have not been categorized yet
                dt = BiblePayDLL.Sidechain.RetrieveDataTable2(IsTestNet(this), "video1");
                dt=                dt.FilterBBPDataTable("userid='" + gUser(this).id + "' and isnull(category,'')=''");
                sType = "video";
            }
            else if (sType == "following")
            {
                dt = dt.FilterBBPDataTable(GetFollowingList(IsTestNet(this), gUser(this).BiblePayAddress));
                sType = "video";
            }
            else if (sType == "trending")
            {
                // highest rated videos in last 90 days or something
                dt = dt.FilterBBPDataTable(GetTrendingList(IsTestNet(this)));
                sType = "video";
            }
            else if (sType == "recentlyuploaded")
            {
                dt.DefaultView.Sort = "time desc";
                sType = "video";
            }
            else if (sType == "hashtags")
            {
                dt = dt.FilterBBPDataTable(GetHashTagList(gUser(this)));
                sType = "video";
            }
            if (sSearch != "")
            {
                //string sPage = this.Request.Url.AbsoluteUri;
                dt = dt.FilterBBPDataTable("body like '%" + sSearch + "%' or title like '%" + sSearch + "%' or Transcript like '%" + sSearch + "%'", true);
              
            }
            // Filter by type starts here:
            if (sType == "video")
            {
                dt = dt.FilterBBPDataTable("SVID <> ''");
            }
            else if (sType == "pdf")
            {
                dt=dt.FilterBBPDataTable("URL like '%.pdf%'");
            }
            else if (sType == "wiki")
            {
                dt=dt.FilterBBPDataTable("URL like '%.htm%'");
            }
            else if (sType == "image")
            {
                dt=dt.FilterBBPDataTable("URL like '%.png%' or URL like '%.jpg%' or URL like '%.jpeg%' or URL like '%.gif%'");
            }

            if (sCategory != "")
            {
                dt = dt.FilterBBPDataTable("subject like '%" + sCategory + "' or title like '%" + sCategory + "' or category like '%" + sCategory + "%'");
            }

            string html = UICommon.GetGallery(this, dt, _paginator, sType, 33, 400, 300);
            return html;
            
        }
    }
}