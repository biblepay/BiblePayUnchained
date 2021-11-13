using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using static BiblePayCommon.Common;
using static BiblePayCommon.DataTableExtensions;
using static BiblePayCommon.EntityCommon;
using static BiblePayCommonNET.CommonNET;
using static BiblePayCommonNET.StringExtension;
using static Unchained.Common;

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

        public static dynamic GetFollowingList(bool fTestNet, string sUserID)
        {
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable3(fTestNet, "follow1");
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
            // phase 2:  builder
            var builder = Builders<BiblePayCommon.Entity.video1>.Filter;
            FilterDefinition<BiblePayCommon.Entity.video1> filter = builder.Eq("id", 101);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                filter |= builder.Eq("UserID", dt.Rows[i]["FollowedId"].ToString());
            }
            // (Reserved for SQL)   return sList;
            return filter;
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
            string sAction = Request.QueryString["a"].ToNonNullString();
            string sUserID = Request.QueryString["userid"].ToNonNullString();
            string sPag = Request.QueryString["pag"].ToNonNullString();
            bool fAsc = false;
            string sSortBy = "";
            // When the request URL matches a cache... of video objects..return the cache instead...
            if (sCategory != "" && sTheirChannel != "" && sType == "")
            {
                sType = "video";
            }

            if (sType == "")
            {
                sType = "video";
            }

            // Global filter (Federated vs. Private) etc.
            var builder = Builders<BiblePayCommon.Entity.video1>.Filter;
            FilterDefinition<BiblePayCommon.Entity.video1> filter;
            filter = builder.Ne("deleted", 1);
            string sVideoFilter = Config("videofilter");

            if (sVideoFilter != "")
            {
                filter &= builder.Regex("domain", new BsonRegularExpression(sVideoFilter, "i"));
            }
            
            if (sUserID != "")
            {
                filter = builder.Eq("UserID", sUserID);
                sType = "video";
            }
            else if (sTheirChannel != "")
            {
                filter = builder.Eq("UserID", sTheirChannel);
                sType = "video";
            }
            else if (sAction == "mychannel")
            {
                filter &= builder.Eq("UserID", gUser(this).id);
                sType = "video";
            }
            else if (sAction == "myeditingroom")
            {
                // User videos that have been uploaded as 'mass' or 'batch', and have not been categorized yet
                filter &= builder.Eq("UserID", gUser(this).id) & builder.Eq("Category", "");
                sType = "video";
            }
            else if (sAction == "following")
            {
                try
                {
                    var followFilter = GetFollowingList(IsTestNet(this), gUser(this).id);
                    filter &= followFilter;
                }
                catch(Exception ex)
                {
                    filter &= builder.Eq("id", "0");
                    BiblePayCommonNET.UICommonNET.MsgModal(this, "Following Error", "Sorry, you are not following anyone yet.", 250, 200, true, false);
                }
                sType = "video";
            }
            else if (sAction == "popular" || sAction == "")
            {
                if (sType == "video")
                {
                    // highest Watched
                    sSortBy = "WatchSum";
                    fAsc = false;
                    sType = "video";
                }
            }
            else if (sAction == "favorites")
            {
                sSortBy = "VoteSum";
                fAsc = false;
                sType = "video";
            }
            else if (sAction == "webm")
            {
                filter &= builder.Regex("URL", new BsonRegularExpression(".*webm.*", "i"));
                sType = "video";
            }
            else if (sAction == "recent")
            {
                sSortBy = "time";
                fAsc = false;

                sType = "video";
            }
            else if (sAction == "hashtags")
            {
                // mission critical dt = dt.FilterDataTable(GetHashTagList(gUser(this)));
                sType = "video";
            }

            if (sSearch != "")
            {
                //string sPage = this.Request.Url.AbsoluteUri;
                sSearch = sSearch.Replace(" ", ".");
                filter &= builder.Ne("SVID", "") & builder.Ne("FID", "");
                filter &= builder.Regex("Body", new BsonRegularExpression(".*" + sSearch + ".*", "ix"))
                    | builder.Regex("Title", new BsonRegularExpression(".*" + sSearch + ".*", "ix"))
                    | builder.Regex("Transcript", new BsonRegularExpression(".*" + sSearch + ".*", "ix"));
            }
            // Filter by type starts here:
            if (sType == "video")
            {
                filter &= builder.Ne("FID", "0");
                filter &= builder.Ne("SVID", "");
                filter &= builder.Ne("SVID", BsonNull.Value);
                filter &= builder.Ne("SVID", 0);
                filter &= builder.Ne("URL2", BsonNull.Value);
                filter &= builder.Ne("SVID", DBNull.Value);
                filter &= builder.Ne("Attachment", 1);
                filter &= builder.Exists("SVID", true);
                filter &= builder.Regex("URL", new BsonRegularExpression(".*mp4.*", "i"))
                    | builder.Regex("URL", new BsonRegularExpression(".*webm.*", "i"));

            }
            else if (sType == "pdf")
            {
                filter &= builder.Regex("URL", new BsonRegularExpression(".*pdf.*", "i"));
            }
            else if (sType == "wiki")
            {
                //dt = dt.FilterDataTable("URL like '%.htm%'");
            }
            else if (sType == "image")
            {
                filter &= builder.Regex("URL", new BsonRegularExpression(".*png.*", "i"))
                  | builder.Regex("URL", new BsonRegularExpression(".*gif.*", "i"))
                  | builder.Regex("URL", new BsonRegularExpression(".*jpeg.*", "i"))
                  | builder.Regex("URL", new BsonRegularExpression(".*jpg.*", "i"))
                  | builder.Regex("URL", new BsonRegularExpression(".*bmp.*", "i"));
            }

            if (sCategory != "")
            {
                filter &= builder.Regex("Category", new BsonRegularExpression(".*" + sCategory + ".*", "i"));
                sSortBy = "time";
                fAsc = false;
            }


            string sRawURL = HttpContext.Current.Request.Url.PathAndQuery + sSearch;
            
            IList<BiblePayCommon.Entity.video1> l1 = BiblePayDLL.Sidechain.GetChainObjects<BiblePayCommon.Entity.video1>(IsTestNet(this), "video1", filter,
                SERVICE_TYPE.PUBLIC_CHAIN, sSortBy, fAsc, sRawURL);

            bool fVideoContainer = (sType == "video");

            string html = UICommon.GetGallery(this, l1, null, sType, 33, 400, 300, fVideoContainer, false, "");
            return html;
        }
    }
}

