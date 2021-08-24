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

namespace Unchained
{
    public partial class VideoList : BBPPage
    {
        BiblePayPaginator.Paginator _paginator = null;
        protected new void Page_Load(object sender, EventArgs e)
        {
            _paginator = (BiblePayPaginator.Paginator)this.Master.FindControl("MainContent").FindControl("paginator1");
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {

        }


        protected override void Event(BBPEvent e)
        {
            if (e.EventID == "Delete")
            {
                    if (HasOwnership(IsTestNet(this), e.EventID, "video1", gUser(this).BiblePayAddress))
                    {
                        // Delete the object (logically)
                         bool fDeleted = BiblePayDLL.Sidechain.DeleteObject(IsTestNet(this), "video1", e.EventValue, GetFundingAddress(IsTestNet(this)), GetFundingKey(IsTestNet(this)));
                        if (fDeleted)
                        {
                            UICommon.RunScriptSM(this, UICommon.Toast("Deleted", "Your Object was Deleted!"));
                        }
                        else
                        {
                            UICommon.RunScriptSM(this, UICommon.Toast("Not Deleted", "FAILURE: The object was not deleted."));
                        }
                    }
                    else
                    {
                        MsgBox("Error", "Sorry, you must have ownership of this object to delete it.", this);
                    }

            }
        }

        private string GetInnerVideo(string sID, string sURL)
        {
            bool fPlayable = false;
            string sDims = "width='400px' height='250px'";
            string sAutoPlay = fPlayable ? "autostart autoplay controls playsinline" : "preload='metadata'";
            string sHTML = "<video id='vid" + sID + "' " + sDims + " class='connect-bg' " + " " 
                + sAutoPlay + " style='background-color:black'>";
            string sLoc = !fPlayable ? "#t=7" : "#t=7";
            sHTML += "<source src='" + sURL + sLoc + "' type='video/mp4'></video>";
            return sHTML;
        }

        private string GetInnerPoster(string sFID, string sURL2)
        {
            if (sFID.Length < 10)
                return "";

            string sURL1 = (sURL2 == null || sURL2 == "") ? "/images/jc2.png" :  sURL2.Replace("/data", "/thumbnails/video.jpg");
            
            string HTML = "<img src='" + sURL1 + "' width=400px height=200px />";
            return HTML;
        }
        private string CurateVideo(string sID, string sNickName, string sURL2, string SVID, string FID, string sAdded, string sSubject, string sTitle)
        {
            string sDiv = "<div style='min-height:200px;height:200px;max-height:200px;overflow:hidden'><a href=Media.aspx?id=" + sID + ">";
            /*
               BiblePayVideo.Video video1 = new BiblePayVideo.Video();
               video1.SVID = SVID;
               video1.Title = sSubject + " • " + sTitle;
               video1.Width = 300;
               video1.Height = 200;
               video1.Playable = true;
               //video1.Footer = video1.Title + " • Uploaded by " + sUserName;
               string sVideo1 = UICommon.RenderControl(video1);

               */
            string sVideo1 = GetInnerPoster(FID, sURL2);

            sDiv += sVideo1;

            if (sNickName == "")
                sNickName = "N/A";

            if (sSubject.Length > 255)
            {
                sSubject = sSubject.Substring(0, 255) + "...";
            }
            if (sNickName == null)
                sNickName = "N/A";

            sDiv += "</a></div><div style='height:80px;min-height:80px;width:300px;max-width:300px;overflow:hidden;'><small>" + sSubject + " • " + sTitle + "<br>Uploaded by " + sNickName + "</small></span></div>";
            if (System.Diagnostics.Debugger.IsAttached)
            {
                string sButton = "<button id='btnDelete' onclick=\""
                 + "__doPostBack('Event_Delete_" + "_" + sID + "_', 'Delete_Click');return false;\">Delete</button> ";
                sDiv += sButton;
            }
            return sDiv;
        }

        public static string GetTrendingList(bool fTestNet)
        {
            try
            {
                DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable2(fTestNet, "vote1");
                dt = dt.FilterDataTable("VoteValue=1 or votevalue=-1 or votevalue=0");

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
            }catch(Exception ex)
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
            string sSearch = txtSearch.Text;
            string sType = Session["key_filetype"].ToNonNullString();
            string sFilterByUser = String.Empty;
            if (sType == "")
            {
                sType = "video";
            }
            if (sType == "mychannel")
            {
                sFilterByUser = gUser(this).BiblePayAddress;
                sType = "video";
            }
            BiblePayCommon.BBPDataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable2(IsTestNet(this), "video1");
            dt = dt.FilterBBPDataTable("isnull(deleted,0)=0");
            if (sType == "following")
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
                dt = dt.FilterBBPDataTable("time > " + 
                    (BiblePayCommon.Common.UnixTimestampUTC() - (11*86400)).ToString());
                sType = "video";
            }
            else if (sType == "hashtags")
            {
                dt = dt.FilterBBPDataTable(GetHashTagList(gUser(this)));
                sType = "video";
            }
            if (sSearch != "")
            {
                string sPage = this.Request.Url.AbsoluteUri;
                _paginator.PageNumber = 0;
                dt = dt.FilterBBPDataTable("body like '%" + sSearch + "%' or subject like '%" + sSearch + "%'");
            }
            if (sFilterByUser != "")
            {
                dt = dt.FilterBBPDataTable("isnull(userid,'')='" + sFilterByUser + "'");
            }
            // Filter by type.... to keep the paginator intact
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

            string html = "<table width=90%><tr>";
            int iCol = 0;
            if (dt.Rows.Count < 1)
                return html;

            _paginator.Rows = dt.Rows.Count;
            _paginator.ColumnsPerRow = 3;
            _paginator.RowsPerPage = 3;
            for (int y = _paginator.StartRow; y <= _paginator.EndRow; y++)
            {
                string sURL = dt.Rows[y]["URL"].ToNonNullString();
                if (sURL != "")
                {
                    string sUserName = UICommon.GetUserRecord(IsTestNet(this), dt.GetColValue(y, "UserID")).UserName;

                    if (sType == "video" && sURL.Contains(".mp4"))
                    {
                        string sVideo = CurateVideo(dt.Rows[y]["id"].ToNonNullString(), sUserName, dt.Rows[y]["URL2"].ToNonNullString(),
                            dt.GetColValue(y, "SVID"), dt.GetColValue(y, "FID"), dt.Rows[y]["time"].ToNonNullString(),
                            dt.Rows[y]["Subject"].ToNonNullString(), dt.Rows[y]["Title"].ToNonNullString());

                        string sRow = "<td width='33%' style='padding-left:15px;'>" + sVideo + "</td>";
            
                        html += sRow;
                        iCol++;
                    }
                    else if (sType == "pdf" && sURL.Contains(".pdf"))
                    {
                        string sAsset = "<a href='" + sURL + "'><img style='height:130px;width:130px;' src='https://foundation.biblepay.org/images/pdf_icon.png'></a>";
                        string sDiv = "<div style='height: 290px; width:360px; overflow: hidden; '>" + sAsset;
                        sDiv += "<br>" + dt.Rows[y]["Subject"].ToNonNullString() + " • Uploaded by " + sUserName + "</div>";
                        string sRow = "<td width=25% style='padding-left:25px;'>" + sDiv + "</td>";
                        html += sRow;
                        iCol++;
                    }
                    else if (sType == "wiki" && sURL.Contains(".htm"))
                    {
                        string sAsset = "<a href='" + sURL + "'><iframe style='height:400px;width:400px;' src='" + sURL + "'></iframe></a>";
                        string sDiv = "<div style='height: 400px; width:400px; overflow: hidden; '>" + sAsset;
                        sDiv += "<br>" + dt.Rows[y]["Subject"].ToNonNullString() + " • Uploaded by " + sUserName + "</div>";
                        
                        string sEdit = "<input type='button' onclick=\"location.href='CreateNewDocument?file=" + sURL + "';\" id='w" + y.ToString() 
                            + "' value='Edit' />";

                        string sRow = "<td width=25% style='padding-left:25px;'>" + sDiv + "<br>" + sEdit + "</td>";
                        html += sRow;
                        iCol++;
                    }
                    else if (sType == "image")
                    {
                        if (sURL.Contains(".png") || sURL.Contains(".jpg") || sURL.Contains(".jpeg") || sURL.Contains(".bmp") || sURL.Contains(".gif"))
                        {
                            string sAsset = "<a href='" + sURL + "'><img style='height:130px;width:130px;' src='" + sURL + "'></a>";
                            string sDiv = "<div style='height: 290px; width:360px; overflow: hidden; '>" + sAsset;
                            sDiv += "<br>" + dt.Rows[y]["Subject"].ToNonNullString() + " • Uploaded by " + sUserName + "</div>";
                            string sRow = "<td width=25% style='padding-left:25px;'>" + sDiv + "</td>";
                            html += sRow;
                            iCol++;
                        }
                    }
                    if (iCol == _paginator.ColumnsPerRow)
                    {
                        html += "</tr>\r\n<tr>";
                        iCol = 0;
                    }
                }
            }
            html += "</table>";
            return html;
        }
    }
}