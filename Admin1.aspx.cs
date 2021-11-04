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

namespace Unchained
{
    public partial class Admin1 : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

         
        }
        public void GetNotesHTML(string data, out string sTitle, out string sNotes)
        {
            string[] vData = data.Split("\r\n");
            if (vData.Length < 2)
            {
                sTitle = data;
                sNotes = "";
                return;
            }
            sTitle = vData[0];

            sNotes = "";
            for (int i = 1; i < vData.Length; i++)
            {
                if (vData[i] != "")
                {
                    sNotes += vData[i] + "<br>";
                }
            }
            
            return;
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

        protected void btnSave_Click(object sender, EventArgs e)
        {

            InnerJoinExample();

            return;


            List<dynamic> lv = new List<dynamic>();
            for (int i = 26; i < 35; i++)
            {
                BiblePayCommon.Entity.video1 v= new BiblePayCommon.Entity.video1();
                v.id = i.ToString();
                v.URL = "https://" + i.ToString() + ".com";
                v.time = i;
                lv.Add(v);
            }
            // mission critical - deleted flag <> 1 throughout
            var t = BiblePayDLL.Sidechain.SpeedyInsertMany(false, "customvideo1", lv, SERVICE_TYPE.PRIVATE_CHAIN, gUser(this));

  
            //IList<BiblePayCommon.Entity.video1> l1 = BiblePayDLL.Sidechain.GetChainObjects<BiblePayCommon.Entity.video1>(false, "customvideo", filter, SERVICE_TYPE.PUBLIC_CACHE);
            IList<BiblePayCommon.Entity.video1> l1 = BiblePayDLL.Sidechain.GetChainObjects<BiblePayCommon.Entity.video1>(false, 
                "customvideo1", null, SERVICE_TYPE.PRIVATE_CHAIN);
            return;
        }
    }
}