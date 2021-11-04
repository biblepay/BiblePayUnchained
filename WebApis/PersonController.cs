using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using BiblePayCommon;
using static BiblePayCommon.Entity;
using BiblePayCommonNET;
using static BiblePayCommon.Common;
using System.Web;
using ScrapySharp.Network;
using System.Threading.Tasks;
using static Unchained.Common;

namespace Unchained.WebApis
{
    public class PersonController : ApiController
    {

        #region Functions
        public static VoteSums GetVoteSum(bool fTestNet, string sParentID)
        {
            VoteSums v = new VoteSums();

            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable3(fTestNet, "vote1");
            dt = dt.FilterDataTable("parentid='" + sParentID + "'");
            if (dt.Rows.Count == 0)
                return v;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                double nVoteValue = GetDouble(dt.Rows[i]["VoteValue"]);
                if (nVoteValue == 1)
                    v.nUpvotes++;
                if (nVoteValue == -1)
                    v.nDownvotes++;
            }
            return v;
        }
        #endregion
        [Route("api/post/posts")]
        public object GetPosts(string sID, bool fHomogenized, bool me, bool IsTestNet, int pno)
        {
            // For each timeline entry...
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable3(IsTestNet, "Timeline");

            if (fHomogenized)
            {
                //string sMyFriendsList = GetFriendsList(IsTestNet, sID);
                //// Per Mike, just show system wide timeline until phase 2 - this will allow newbies to see some timeline data
                //if (false)
                //{
                //    DataOps.FilterDataTable(ref dt, sMyFriendsList);
                //    if (dt.Rows.Count == 0)
                //    {
                //        // Show homogenized view with everyones posts (since I have no friends yet, and no timeline posts yet):
                //        dt = BiblePayDLL.Sidechain.RetrieveDataTable2(IsTestNet, "Timeline");
                //    }
                //}
            }
            else
            {
                DataOps.FilterDataTable(ref dt, "userid='" + sID + "'");
            }
            int take = 5;
            int skip = pno * take;
            // Default Sort, time desc:
            // TODO: Figure out why this line doesnt work, but the OrderBy works: dt= dt.SortBy("time desc");

            dt = dt.OrderBy("time desc");
            var data = dt.AsEnumerable().Skip(skip).Take(take);
            //dt.Columns.Add("ProfilePicture");
            //dt.Columns.Add("FullName");
            //dt.Columns.Add("PostedOn");

            List<object> posts = new List<object>();
            for (int i = 0; i < data.Count(); i++)
            {
                var fields = data.ElementAt(i);
                string userId = data.ElementAt(i).Field<string>("userid");
                var user = UICommon.GetUserRecord(IsTestNet, userId);
                var p = new
                {
                    id = fields.Field<string>("id"),// dt.Rows[i]["id"],
                    ProfilePicture = string.IsNullOrEmpty(user.AvatarURL) ? "" : user.AvatarURL,
                    FullName = user.FullUserName(),
                    PostedOn = Encryption.UnixTimeStampToDateTime(BiblePayCommon.Common.GetDouble(fields.Field<object>("time"))),
                    Body = fields.Field<string>("Body"),// dt.Rows[i]["Body"],
                    URL = fields.Field<string>("URL"),//dt.Rows[i]["URL"],
                    URLPreviewImage = fields.Field<string>("URLPreviewImage"),//dt.Rows[i]["URLPreviewImage"],
                    URLTitle = fields.Field<string>("URLTitle"),//dt.Rows[i]["URLTitle"],
                    UserId = user.id,
                    URLDescription = fields.Field<string>("URLDescription"),//dt.Rows[i]["URLDescription"],
                    Likes = GetVoteSum(IsTestNet, fields.Field<string>("id")),
                    Comments = new List<object>()
                };
                 
                //sTimeline += UICommon.GetAttachments(this, dt.Rows[i]["id"].ToString(), "", "Timeline Attachments", "style='background-color:white;padding-left:30px;'");
                DataTable dt2 = BiblePayDLL.Sidechain.RetrieveDataTable3(IsTestNet, "comment1");

                dt2 = dt2.FilterDataTable("parentid='" + dt.Rows[i]["id"].ToString() + "'");
                dt2 = dt2.OrderBy("time asc");

                for (int c = 0; c < dt2.Rows.Count; c++)
                {
                    var userc = UICommon.GetUserRecord(IsTestNet, dt2.Rows[c]["userid"].ToString());
                    p.Comments.Add(new
                    {
                        Body = dt2.Rows[c]["Body"],
                        id = dt2.Rows[c]["Id"],
                        ProfilePicture = string.IsNullOrEmpty(userc.AvatarURL) ? "" : userc.AvatarURL,
                        FullName = userc.FullUserName(),
                        UserId = userc.id,
                        PostedOn = dt2.GetColDateTime(c, "time"),
                        Count = GetVoteSum(IsTestNet, dt2.Rows[c]["Id"].ToString())
                    });
                }
                posts.Add(p);
            }
            return posts;
        }

        [Route("api/people/friends")]
        public object GetFriendsList(string sID, bool isTestNet)
        {
            string id = sID;
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable3(isTestNet, "Friend");
            dt = dt.FilterDataTable("UserID='" + sID + "' or RequesterID='" + sID + "'");
            List<object> o = new List<object>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string sRequestor = dt.Rows[i]["RequesterID"].ToString();
                string sUserID = dt.Rows[i]["UserID"].ToString();
                string idn = id == sUserID ? sRequestor : sUserID;
                User user = UICommon.GetUserRecord(isTestNet, idn);
                var a = new
                {
                    ProfilePicture = string.IsNullOrEmpty(user.AvatarURL) ? "" : user.AvatarURL,
                    FullName = user.FullUserName(),
                    Id = idn,
                };
                o.Add(a);
            }

            return new { total=dt.Rows.Count, result=o.Take(10).ToList() };
        }

        public static string Mid(string data, int nStart, int nLength)
        {
            // Ported from VB6, except this version is 0 based (NOT 1 BASED)
            if (nStart > data.Length)
            {
                return "";
            }

            int nNewLength = nLength;
            int nEndPos = nLength + nStart;
            if (nEndPos > data.Length)
            {
                nNewLength = data.Length - nStart;
            }
            if (nNewLength < 1)
                return "";

            string sOut = data.Substring(nStart, nNewLength);
            if (sOut.Length > nLength)
            {
                sOut = sOut.Substring(0, nLength);
            }
            return sOut;
        }

        [Route("api/media/images")]
        public object GetImages(string sID, bool isTestNet)
        {

            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable3(isTestNet, "video1");

            dt = dt.FilterDataTable("userid='" + sID + "'");
            dt = dt.FilterDataTable("URL like '%.png%' or URL like '%.gif' or URL Like '%.jpeg' or URL like '%.jpg%' or URL like '%.jpeg%' or URL like '%.gif%'");

            dt = dt.OrderBy("time desc");

            var result = dt.AsEnumerable().Take(9).Select(s => new { URL = s.Field<string>("URL"), Title = s.Field<string>("Title") });

            return result;
        }

        [HttpPost]
        [Route("api/web/scrap")]
        public async Task<object> Scrpper(string url)
        {
            url = HttpUtility.UrlDecode(url);
            ScrapingBrowser browser = new ScrapingBrowser();
            WebPage page;
            string webUrl = url;
            page =await browser.NavigateToPageAsync(new Uri(webUrl));

            var title = page.Html.SelectSingleNode("//meta[@property='og:title']")?.GetAttributeValue("content", string.Empty);
            if (string.IsNullOrEmpty(title))
                title = page.Html.SelectSingleNode("//title")?.InnerText;

            var description = page.Html.SelectSingleNode("//meta[@property='og:description']")?.GetAttributeValue("content", string.Empty);
            if (string.IsNullOrEmpty(description))
                description = page.Html.SelectSingleNode("//meta[@name='description']")?.GetAttributeValue("content", string.Empty);

             var image = page.Html.SelectSingleNode("//meta[@property='og:image']")?.GetAttributeValue("content", string.Empty);
            if (string.IsNullOrEmpty(image))
                image = page.Html.SelectNodes("//img").FirstOrDefault().GetAttributeValue("src", string.Empty);

            return new { title, description, image };
            //return null;
        }

        [HttpPost]
        [Route("api/posts/create")]
        public object Post(bool IsTestNet, string userId, PostRequest body)
        {
            var user = UICommon.GetUserRecord(IsTestNet, userId);
            if (!user.LoggedIn)
            {
                return new { success = false, result = "You must log in to make a post." };
            }
            string decode = HttpUtility.UrlDecode(body.Content);
            string base64 = BiblePayCommon.Encryption.Base64DecodeWithFilter(body.Content);
            Timeline t = new Timeline();
            t.Body = BiblePayCommon.Encryption.Base64DecodeWithFilter(body.Content);

            t.UserID = user.id;
            BiblePayCommon.Common.DACResult r = DataOps.InsertIntoTable(IsTestNet, t, user);
            if (r.fError())
            {
                return new { success = false, result = "Sorry, the timeline was not saved." };
            }
            else
            {
                return new { success = true };
                //SendBlastOutForTimeline(this, t);
                //ToastLater(this, "Success", "Your timeline entry has been saved!");
            }
        }
    }

    public class PostRequest
    {
        public string Id { get; set; }
        public string Visibility { get; set; }
        public string Content { get; set; }
    }
}