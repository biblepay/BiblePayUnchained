using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using static BiblePayCommon.Common;
using static Unchained.Common;
using static BiblePayCommonNET.StringExtension;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Linq;
using static BiblePayCommon.EntityCommon;
using static BiblePayCommonNET.CommonNET;

namespace Unchained
{
    public partial class Admin1 : BBPPage
    {
        protected new void Page_Load(object sender, EventArgs e)
        {

         
        }

        private static int ctr = 0;
        protected override void Event(BBPEvent e)
        {
            if (e.EventName == "PerfCounter_Click")
            {
                ctr++;

            }

        }


        
        private void InitializeObject(bool fTestNet)
        {
            BiblePayCommon.Entity.versememorizer1 n = new BiblePayCommon.Entity.versememorizer1();
            DataOps.InsertIntoTable(this, true, n, gUser(this));
            DataOps.InsertIntoTable(this, false, n, gUser(this));
        }
        private void Reskin()
        {
            UICommon.ReskinCSS("darkblue", "blue", "#00004B", "blue", "white", "white", "darkblue", "antiquewhite");
            UICommon.ReskinCSS("black", "#202020", "#393939", "black", "white", "#141414", "black", "antiquewhite");
            UICommon.ReskinCSS("maroon", "#c32148", "#400000", "maroon", "white", "#850000", "maroon", "antiquewhite");
            UICommon.ReskinCSS("#696969", "#808080", "#393939", "grey", "silver", "silver", "#808080", "silver");
            UICommon.ReskinCSS("#ffffff", "darkblue", "#ececec", "whiteblue", "#0096FF", "white", "#F0FFFF", "darkblue");  //Azure is the header color
        }

        public void InnerJoinExample()
        {
            string sOut = "";

            try
            {
                IList<BiblePayCommon.Entity.user1> lUsers = BiblePayDLL.Sidechain.GetChainObjects<BiblePayCommon.Entity.user1>(false, "user1", null,
                    SERVICE_TYPE.PUBLIC_CHAIN);
                IList<BiblePayCommon.Entity.video1> lVideos = BiblePayDLL.Sidechain.GetChainObjects<BiblePayCommon.Entity.video1>(false, "video1",
                    null, SERVICE_TYPE.PUBLIC_CHAIN);

                var query1 = 
                    from v in lVideos
                    join u in lUsers
                    on new { userid = v.UserID }
                       equals new { userid = u.id }
                    into grpJoin
                    from u in grpJoin.DefaultIfEmpty()
                    where v != null && u != null
                    select new
                         { UserName1 = u.FirstName, UserID = u.id, VideoURL = v.URL, VideoTitle = v.Title };

                //var list = new List<object?>();
                //IEnumerable<MyObject> notNull = list.NotNull();
                //var l = query1.Where(x => x != null).ToList();
                var l = query1.Where(x => x != null).ToList();
                foreach (var v in query1)
                {
                    try
                    {
                        if (v.VideoTitle.ToNonNullString() != "")
                        {
                            try
                            {
                                string data = v.UserID + "," + v.VideoURL;
                                sOut += data;
                            }
                            catch (Exception ex9)
                            {
                                string s1004 = "";
                            }

                        }
                    }catch(Exception ex99)
                    {
                        string s1044 = "";
                    }
                }
                string sFin = "";

            }
            catch(Exception ex)
            {
                string s1099 = "";

            }

        }


        protected string GetPerfSection()
        {
            string sLbl = "<span>" + ctr.ToString() + "</span>";
            string sBt = UICommon.GetStandardButton("1",
                    "<i class='fa fa-file'></i>", "PerfCounter", "Perf Counter");


            string sAnchor = "<a id='2' onclick=\"var e={};e.Event='PerfCounter_Click';e.Value='2';e.Table='"
                + "3';BBPPostBack2(null, e);BBPPostBack2(null, e);BBPPostBack2(null, e);BBPPostBack2(null, e);" + "\" title='myclick1'>myclick1</a>";

            return sLbl + "<br>" + sBt + "<br>" + sAnchor;


        }

        protected void btnSave_Click(object sender, EventArgs e)
        {

            List<dynamic> lv = new List<dynamic>();
            for (int i = 1000; i < 1256; i++)
            {
                BiblePayCommon.Entity.NewsFeedItem v = new BiblePayCommon.Entity.NewsFeedItem();
                v.URL = "https://" + i.ToString() + ".com";
                v.NewsFeedSourceID = "CNN";
                v.Title = v.URL;
                string sKey = BiblePayCommon.Encryption.GetSha256HashI(v.NewsFeedSourceID + v.URL);
                v.id = sKey;
                v.time = i;
                v.Expiration = (int)(UnixTimestampUTC() + (60 * 60 * 24 * 30));
                lv.Add(v);
            }
            // Insert the list
            var t = BiblePayDLL.Sidechain.SpeedyInsertMany(false, "newsfeeditems", lv, SERVICE_TYPE.PRIVATE_CACHE, gUser(this));
            // Retrieve the list
            IList<BiblePayCommon.Entity.NewsFeedItem> l1 = BiblePayDLL.Sidechain.GetChainObjects<BiblePayCommon.Entity.NewsFeedItem>(false, 
                "newsfeeditems", null, SERVICE_TYPE.PRIVATE_CACHE);
            return;
        }
    }
}