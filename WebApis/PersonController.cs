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

namespace Unchained.WebApis
{
    public class PersonController : ApiController
    {
        [Route("api/post/posts")]
        public DataTable GetPosts(string sID, bool fHomogenized, bool me, bool IsTestNet)
        {
            // For each timeline entry...
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable2(IsTestNet, "Timeline");

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

            // Default Sort, time desc:
            // TODO: Figure out why this line doesnt work, but the OrderBy works: dt= dt.SortBy("time desc");

            dt = dt.OrderBy("time desc");
            dt.Columns.Add("ProfilePicture");
            dt.Columns.Add("FullName");
            dt.Columns.Add("PostedOn");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var user = UICommon.GetUserRecord(IsTestNet, dt.Rows[i]["userid"].ToString());
                dt.Rows[i]["ProfilePicture"] = string.IsNullOrEmpty(user.AvatarURL) ? "" : user.AvatarURL;
                dt.Rows[i]["FullName"] = user.FullUserName();
                dt.Rows[i]["PostedOn"] = dt.GetColDateTime(i, "time");
            }
            return dt;
        }

        [Route("api/people/friends")]
        public object GetFriendsList(string sID, bool isTestNet)
        {
            string id = sID;
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable2(isTestNet, "Friend");
            dt = dt.FilterDataTable("UserID='" + sID + "' or RequesterID='" + sID + "'");
            List<object> o = new List<object>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string sRequestor = dt.Rows[i]["RequesterID"].ToString();
                string sUserID = dt.Rows[i]["UserID"].ToString();
                User user = id == sUserID ? UICommon.GetUserRecord(isTestNet, sRequestor) : UICommon.GetUserRecord(isTestNet, sUserID);
                var a = new
                {
                    ProfilePicture = string.IsNullOrEmpty(user.AvatarURL) ? "" : user.AvatarURL,
                    FullName = user.FullUserName(),
                    RequesterId = sRequestor,
                    UserID = sUserID
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

            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable2(isTestNet, "video1");

            dt = dt.FilterDataTable("userid='" + sID + "'");
            dt = dt.FilterDataTable("URL like '%.png%' or URL like '%.gif' or URL Like '%.jpeg' or URL like '%.jpg%' or URL like '%.jpeg%' or URL like '%.gif%'");

            dt = dt.OrderBy("time desc");

            var result = dt.AsEnumerable().Take(9).Select(s => new { URL = s.Field<string>("URL"), Title = s.Field<string>("Title") });

            return result;
        }

        [HttpPost]
        [Route("api/posts/create")]
        public object Post(bool IsTestNet, string userId, PostRequest body)
        {
            var user = UICommon.GetUserRecord(IsTestNet, userId);
            if (user.FirstName == "Guest")
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