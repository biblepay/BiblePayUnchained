using static Unchained.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static BiblePayDLL.Shared;

namespace Unchained
{
    public partial class VideoList : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {

        }


        private string GetInnerVideo(string sID, string sURL)
        {
            bool fPlayable = false;

            string sDims = "width='350' height='200'";
            string sAutoPlay = fPlayable ? "autostart autoplay controls playsinline" : "preload='metadata'";

            string sHTML = "<video id='vid" + sID + "' class='connect-bg' " + sDims + " " + sAutoPlay + " style='background-color:black'>";
            string sLoc = !fPlayable ? "#t=7" : "#t=7";
          
            sHTML += "<source src='" + sURL + sLoc + "' type='video/mp4'></video>";
            return sHTML;
        }
        private string CurateVideo(string sID, string sNickName, string sURL, string sAdded, string sSubject, string sNotes)
        {
            string sDiv = "<div style='height: 290px; width:360px; overflow: hidden; '><a href=Media.aspx?mediaid=" + sID + ">";
            sDiv += GetInnerVideo(sID, sURL);
            sDiv += "</a><br>" + sSubject + " • Uploaded by " + sNickName + "</div>";
            return sDiv;
        }

        public struct Paginator
        {
            public int nColsPerRow;
            public int nObjsPerPage;
            public int nPageNo;
            public int nStartRow;
            public int nEndRow;
            public int nRows;
            public int nRowsPerPage;
            public double nTotalPages;
            public int nObjCount;
        };

        private Paginator GetPaginator(Page myPage, int nObjCount, int nColsPerRow, int nRowsPerPage)
        {
            var uri = new Uri(myPage.Request.Url.AbsoluteUri);
            string sURL = uri.GetLeftPart(UriPartial.Path);
            Paginator p = new Paginator();
            p.nColsPerRow = nColsPerRow;
            p.nRowsPerPage = nRowsPerPage;
            p.nObjsPerPage = nRowsPerPage * nColsPerRow;
            // this one is 0 based
            p.nPageNo = (int)Common.GetDouble(Session["pag_" + sURL] ?? "") - 1;
            p.nPageNo = (int)Common.GetDouble(Request.Form["hpag1"] ?? "") - 0;
            p.nStartRow = p.nPageNo * p.nObjsPerPage;
            p.nEndRow = p.nStartRow + p.nObjsPerPage - 1;
            if (p.nEndRow >= nObjCount)
                p.nEndRow = nObjCount - 1;
            p.nObjCount = nObjCount;
            p.nRows = p.nObjCount / nColsPerRow;
            p.nTotalPages = (int)Math.Ceiling((double)(nObjCount / p.nObjsPerPage)) + 1;
            // this one is 1 based
            double nPage = Common.GetDouble(Request.Form["hpag1"] ?? "") + 1;
            myPage.Session["pag_" + sURL] = nPage.ToString();
            return p;
        }

        protected string GetVideoList()
        {
            string sSearch = txtSearch.Text;
            string sType = Session["key_filetype"].ToNonNullString();
            string sSearch2 = "";
            if (sType == "video")
            {
                sSearch2 = "mp4";
            }
            else if (sType == "pdf")
            {
                sSearch2 = "pdf";
            }
            else if (sType == "image")
            {
                sSearch2 = "image";
            }
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable2(IsTestNet(this), "video1", "", "", "username,time,id,body,URL,subject", sSearch, sSearch2);
            string html = "<table><tr>";
            int iCol = 0;
            Paginator p = GetPaginator(this, dt.Rows.Count, 4, 3);
            for (int y = p.nStartRow; y <= p.nEndRow; y++)
            {
                string sURL = dt.Rows[y]["URL"].ToNonNullString();
                if (sURL != "")
                {

                    if (sType == "video" && sURL.Contains(".mp4"))
                    {
                        string sVideo = CurateVideo(dt.Rows[y]["id"].ToNonNullString(), dt.Rows[y]["username"].ToNonNullString(), dt.Rows[y]["URL"].ToNonNullString(),
                             dt.Rows[y]["time"].ToNonNullString(), dt.Rows[y]["Subject"].ToNonNullString(), dt.Rows[y]["Body"].ToNonNullString());
                        string sRow = "<td width=25% style='padding-left:25px;'>" + sVideo + "</td>";
                        html += sRow;
                        iCol++;
                    }
                    else if (sType == "pdf" && sURL.Contains(".pdf"))
                    {
                        string sFullURL = "http://api.screenshotlayer.com/api/capture?access_key=" + Config("freezerkey")                             + "&url=" + HttpUtility.UrlEncode(sURL) + "&viewport=290x350&width=350&format=png";
                        string sAsset = "<a href='" + sURL + "'><img style='height:130px;width:130px;' src='https://foundation.biblepay.org/images/pdf_icon.png'></a>";
                        string sDiv = "<div style='height: 290px; width:360px; overflow: hidden; '>" + sAsset;
                        sDiv += "<br>" + dt.Rows[y]["Subject"].ToNonNullString() + " • Uploaded by " + dt.Rows[y]["username"].ToNonNullString() + "</div>";
                        string sRow = "<td width=25% style='padding-left:25px;'>" + sDiv + "</td>";
                        html += sRow;
                        iCol++;
                    }
                    else if (sType == "image")
                    {
                        if (sURL.Contains(".png") || sURL.Contains(".jpg") || sURL.Contains(".jpeg") || sURL.Contains(".bmp") || sURL.Contains(".gif"))
                        {
                            string sAsset = "<a href='" + sURL + "'><img style='height:130px;width:130px;' src='" + sURL + "'></a>";
                            string sDiv = "<div style='height: 290px; width:360px; overflow: hidden; '>" + sAsset;
                            sDiv += "<br>" + dt.Rows[y]["Subject"].ToNonNullString() + " • Uploaded by " + dt.Rows[y]["username"].ToNonNullString() + "</div>";
                            string sRow = "<td width=25% style='padding-left:25px;'>" + sDiv + "</td>";
                            html += sRow;
                            iCol++;
                        }
                    }
                    if (iCol == p.nColsPerRow)
                    {
                        html += "</tr>\r\n<tr>";
                        iCol = 0;
                    }
                }
            }
            html += "</table>";

            html += UICommon.GetPagControl(this, p.nTotalPages);

            return html;
        }
    }
}