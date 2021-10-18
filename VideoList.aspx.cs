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
        protected new void Page_Load(object sender, EventArgs e)
        {

            if (IsPostBack)
            {
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
            string sType = Request.QueryString["type"].ToNonNullString();
            string sCategory = (Request.QueryString["category"]  ?? "").ToString();
            string sTheirChannel = Request.QueryString["channelid"].ToNonNullString();
            string sLastName = Request.QueryString["LastName"].ToNonNullString();
            string sFirstName = Request.QueryString["FirstName"].ToNonNullString();
            string sAction = Request.QueryString["a"].ToNonNullString();
            string sUserID = Request.QueryString["userid"].ToNonNullString();

            if (sCategory != "" || sTheirChannel != "" || sType == "")
            {
                sType = "video";
            }
            // Global filter (Federated vs. Private) etc.

            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable2(IsTestNet(this), "video1");
            string sData = "";
            string sVideoFilter = Config("videofilter");
            dt = dt.FilterDataTable(sVideoFilter);
            dt = dt.FilterDataTable("isnull(attachment,0)=0");
            if (sType == "video")
            {
                dt = dt.FilterDataTable("fid <> '0'");
                if (sAction == "")
                {

                    dt = dt.SortDataTable("WatchSum desc");
                    dt.DefaultView.ApplyDefaultSort = true;

                    sData = "";
                    foreach (DataRowView drv in dt.DefaultView)
                    {
                        sData += drv.Row["WatchSum"].ToString() + ",";
                    }

                    string s99 = "";


                }
            }

            if (sFirstName != "")
            {
                dt = BiblePayDLL.Sidechain.RetrieveDataTable2(IsTestNet(this), "video1");
                User u1 = gUser(this, sFirstName, sLastName);
                dt = dt.FilterDataTable("userid='" + u1.id + "'");
                sType = "video";
            }
            else if (sUserID != "")
            {
                dt = dt.FilterDataTable("userid='" + sUserID + "'");
                sType = "video";
            }
            else if (sTheirChannel != "")
            {
                dt = dt.FilterDataTable("userid='" + sTheirChannel + "'");
                sType = "video";
            }
            else if (sAction == "mychannel")
            {
                dt = dt.FilterDataTable("userid='" + gUser(this).id + "'");
                sType = "video";
            }
            else if (sAction == "myeditingroom")
            {
                // User videos that have been uploaded as 'mass' or 'batch', and have not been categorized yet
                dt = BiblePayDLL.Sidechain.RetrieveDataTable2(IsTestNet(this), "video1");
                dt= dt.FilterDataTable("userid='" + gUser(this).id + "' and isnull(category,'')=''");
                sType = "video";
            }
            else if (sAction == "following")
            {
                dt = dt.FilterDataTable(GetFollowingList(IsTestNet(this), gUser(this).id));
                sType = "video";
            }
            else if (sAction == "popular")
            {
                // highest rated videos in last 90 days or something
                dt = dt.SortDataTable("WatchSum desc");
                sType = "video";
            }
            else if (sAction == "favorites")
            {
                dt = dt.SortDataTable("VoteSum desc");
                sType = "video";
            }
            else if (sAction == "webm")
            {
                dt = dt.FilterDataTable("url like '%webm%'");
                sType = "video";
            }
            else if (sAction == "recent")
            {
                dt = dt.SortDataTable("time desc");
                sType = "video";
            }
            else if (sAction == "hashtags")
            {
                dt = dt.FilterDataTable(GetHashTagList(gUser(this)));
                sType = "video";
            }

            if (sSearch != "")
            {
                //string sPage = this.Request.Url.AbsoluteUri;
                dt = dt.FilterDataTable("body like '%" + sSearch + "%' or title like '%" + sSearch + "%' or Transcript like '%" + sSearch + "%'");
            }
            // Filter by type starts here:
            if (sType == "video")
            {
                dt = dt.FilterDataTable("SVID <> ''");
            }
            else if (sType == "pdf")
            {
                dt = dt.FilterDataTable("URL like '%.pdf%'");
            }
            else if (sType == "wiki")
            {
                dt = dt.FilterDataTable("URL like '%.htm%'");
            }
            else if (sType == "image")
            {
                dt = dt.FilterDataTable("URL like '%.png%' or URL like '%.gif' or URL Like '%.jpeg' or URL like '%.jpg%' or URL like '%.jpeg%' or URL like '%.gif%'");
            }

            if (sCategory != "")
            {
                dt = dt.FilterDataTable("subject like '%" + sCategory + "' or title like '%" + sCategory + "' or category like '%" + sCategory + "%'");
            }

            string html = UICommon.GetGallery(this, dt, null, sType, 33, 400, 300, true, false, "");
            return html;
            
        }
    }
}