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
using MongoDB.Driver;
using static BiblePayCommon.EntityCommon;
using System.Net.Sockets;

namespace Unchained.WebApis
{
    public class PersonController : ApiController
    {

        #region Functions
        
        
        
        
	
	 bool IsShadyAddress(string url)
	 {
	            try
	            {
	
	                Uri myUri = new Uri(url);
	                string host = myUri.Host;  
	                bool fContainsNumbers = host.All(char.IsDigit);
	                if (fContainsNumbers)
	                {
	                    return true;
	                }
	
	                IPHostEntry hostEntry;
	                hostEntry = Dns.GetHostEntry(host);
	                IPAddress[] ipv4Addresses = Array.FindAll(
	                        Dns.GetHostEntry(host).AddressList,
	                            a => a.AddressFamily == AddressFamily.InterNetwork);
	
	                IPAddress[] ipv4MyAddresses = Dns.GetHostAddresses(Dns.GetHostName());
	                //DNS supports more than one record
	                for (int i = 0; i < hostEntry.AddressList.Length; i++)
	                {
	                    bool fIsLoopback = IPAddress.IsLoopback(hostEntry.AddressList[i]);
	                    if (fIsLoopback)
	                    {
	                        return true;
	                    }
	
	                }
	                for (int i = 0; i < ipv4Addresses.Length; i++)
	                {
	                    bool fIsLoopback = IPAddress.IsLoopback(ipv4Addresses[i]);
	                    if (fIsLoopback)
	                    {
	                        return true;
	                    }
	                    for (int j = 0; j < ipv4MyAddresses.Length; j++)
	                    {
	                        if (ipv4Addresses[i].ToString() == ipv4MyAddresses[j].ToString())
	                            return true;
	                    }
	
	                }
	                string s99 = "";
	
	            }
	            catch (Exception ex)
	            {
	                return true;
	            }
	            return false;
	
        }
        
        
        
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
        public object GetPosts(string category, string sID, bool fHomogenized, bool me, bool IsTestNet, int pno)
        {
            // For each timeline entry...
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable3(IsTestNet, "Timeline");

            if (fHomogenized)
            {
               
            }
            else
            {
                DataOps.FilterDataTable(ref dt, "userid='" + sID + "'");
            }

            if (!category.ToLower().Equals("a"))
            {
                DataOps.FilterDataTable(ref dt, "category='" + category + "'");

            }
            int take = 5;
            int skip = pno * take;
            
            dt = dt.OrderBy("time desc");
            var data = dt.AsEnumerable().Skip(skip).Take(take);
            
            List<object> posts = new List<object>();
            for (int i = 0; i < data.Count(); i++)
            {
                var fields = data.ElementAt(i);
                string userId = data.ElementAt(i).Field<string>("userid");
                var user = UICommon.GetUserRecord(IsTestNet, userId);
                var builder = Builders<BiblePayCommon.Entity.video1>.Filter;
                //var filter = builder.Eq("Attachment", 1) & builder.Eq("ParentId", fields.Field<string>("id"));
                ////mission critical check the Optional Filter (sFilter) and tack that on...
                //IList<BiblePayCommon.Entity.video1> dtAttachments = 
                //    BiblePayDLL.Sidechain.GetChainObjects<BiblePayCommon.Entity.video1>(IsTestNet, "video1",
                //    filter, SERVICE_TYPE.PUBLIC_CHAIN);


                DataTable dtAttachments = BiblePayDLL.Sidechain.RetrieveDataTable3(IsTestNet, "video1");

                dtAttachments = dtAttachments.FilterDataTable("parentid='" + fields.Field<string>("id") + "'");
                dtAttachments = dtAttachments.FilterDataTable("Attachment=1");

                var p = new
                {
                    id = fields.Field<string>("id"),
                    ProfilePicture = string.IsNullOrEmpty(user.AvatarURL) ? "" : user.AvatarURL,
                    FullName = user.FullUserName(),
                    PostedOn = Encryption.UnixTimeStampToDateTime(BiblePayCommon.Common.GetDouble(fields.Field<object>("time"))),
                    Body = fields.Field<string>("Body"),
                    URL = fields.Field<string>("URL"),
                    URLPreviewImage = fields.Field<string>("URLPreviewImage"),
                    URLTitle = fields.Field<string>("URLTitle"),
                    UserId = user.id,
                    URLDescription = fields.Field<string>("URLDescription"),
                    Likes = GetVoteSum(IsTestNet, fields.Field<string>("id")),
                    Comments = new List<object>(),
                    Attachments = dtAttachments //.Select(s => new { s.id, s.ParentID, s.FileName })
                };

                
                //var attch = UICommon.GetAttachments(null, p.id, "", "", "");
                 DataTable dt2 = BiblePayDLL.Sidechain.RetrieveDataTable3(IsTestNet, "comment1");

                dt2 = dt2.FilterDataTable("parentid='" + fields.Field<string>("id") + "'");
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

        [Route("api/media")]
        public object GetImages(string sID, bool isTestNet, string type = "images", int count = 9, string id = "")
        {

            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable3(isTestNet, "video1");

            dt = dt.FilterDataTable("userid='" + sID + "'");
            if (type == "images")
                dt = dt.FilterDataTable("URL like '%.png%' or URL like '%.gif' or URL Like '%.jpeg' or URL like '%.jpg%' or URL like '%.jpeg%' or URL like '%.gif%'");
            else if (type == "")
                dt = dt.FilterDataTable("URL like '%.png%' or URL like '%.gif' or URL Like '%.jpeg' or URL like '%.jpg%' or URL like '%.jpeg%' or URL like '%.gif%'");

            dt = dt.OrderBy("time desc");

            var result = dt.AsEnumerable().Take(count).Select(s => new { id = s.Field<string>("id"), URL = s.Field<string>("URL"), Title = s.Field<string>("Title"), ParentId = s.Field<string>("ParentID") });

            return result;
        }

        [Route("api/media/get-by-id")]
        public object GetMediaById(string parentid, bool isTestNet,string id="")
        {
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable3(isTestNet, "video1");

            dt = dt.FilterDataTable("parentid='" + parentid + "'");

            List<object> result = new List<object>();
            foreach(DataRow s in dt.Rows)
            {
                string userId = s.Field<string>("UserId");
                var userc = UICommon.GetUserRecord(isTestNet, userId);
                var o = new
                {
                    id = s.Field<string>("id"),
                    URL = s.Field<string>("URL"),
                    Title = s.Field<string>("Title"),
                    UserId = userId,
                    Likes = GetVoteSum(isTestNet, s.Field<string>("id")),
                    Views = s.Field<int>("WatchSum"),
                    PostedOn = Encryption.UnixTimeStampToDateTime(BiblePayCommon.Common.GetDouble(s.Field<object>("time"))),
                    ProfilePicture = string.IsNullOrEmpty(userc.AvatarURL) ? "" : userc.AvatarURL,
                    FullName = userc.FullUserName(),
                    Comments=new List<object>()
                };

                DataTable dt2 = BiblePayDLL.Sidechain.RetrieveDataTable3(isTestNet, "comment1");

                dt2 = dt2.FilterDataTable("parentid='" + s.Field<string>("id") + "'");
                dt2 = dt2.OrderBy("time asc");

                for (int c = 0; c < dt2.Rows.Count; c++)
                {
                    var userd = UICommon.GetUserRecord(isTestNet, dt2.Rows[c]["userid"].ToString());
                    o.Comments.Add(new
                    {
                        Body = dt2.Rows[c]["Body"],
                        id = dt2.Rows[c]["Id"],
                        ProfilePicture = string.IsNullOrEmpty(userc.AvatarURL) ? "" : userc.AvatarURL,
                        FullName = userc.FullUserName(),
                        UserId = userc.id,
                        PostedOn = dt2.GetColDateTime(c, "time"),
                        Count = GetVoteSum(isTestNet, dt2.Rows[c]["Id"].ToString())
                    });
                }

                result.Add(o);
            }

            return result;
        }


        [HttpPost]
        [Route("api/web/scrap")]
        public async Task<object> Scrpper(string url)
        {
            url = HttpUtility.UrlDecode(url);
            ScrapingBrowser browser = new ScrapingBrowser();
            WebPage page;
            bool fShady = IsShadyAddress(url);
            
            
	    bool fHTTProtocols = false;
	    if (url.Contains("https://") || url.Contains("http://"))
	    {
	                    fHTTProtocols = true;
	    }
	    if (url.Contains("127.0.0.1") || url.Contains("localhost") || url.Contains("//127") || url.Contains("local"))
	    {
	                    url = "";
	    }
	    if (fShady || !fHTTProtocols)
	    {
	                    url = "";
	    }
            string webUrl = url;
            page = await browser.NavigateToPageAsync(new Uri(webUrl));

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