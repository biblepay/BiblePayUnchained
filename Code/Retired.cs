using BiblePayCommonNET;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using static BiblePayCommon.EntityCommon;

namespace Unchained.Code
{
    public static class Retired
    {
        private static void Reskin()
        {
            UICommon.ReskinCSS("darkblue", "blue", "#00004B", "blue", "white", "white", "darkblue", "antiquewhite");
            UICommon.ReskinCSS("black", "#202020", "#393939", "black", "white", "#141414", "black", "antiquewhite");
            UICommon.ReskinCSS("maroon", "#c32148", "#400000", "maroon", "white", "#850000", "maroon", "antiquewhite");
            UICommon.ReskinCSS("#696969", "#808080", "#393939", "grey", "silver", "silver", "#808080", "silver");
            UICommon.ReskinCSS("#ffffff", "darkblue", "#ececec", "whiteblue", "#0096FF", "white", "#F0FFFF", "darkblue");  //Azure is the header color
        }

        public static void TestCoercedUploadFromWebhook()
        {
            string sUserID = "e3";
            BiblePayCommon.Common.User userProd = Common.CoerceUser(false, Global.PROD_GUID);
            string sFile = "1.pdf";
            string sPath = "c:\\1.pdf";
            BiblePayCommon.Entity.object1 o1 = new BiblePayCommon.Entity.object1();
            o1.FileName = sPath;
            o1.Title = DateTime.Now.ToString() + " Live Elite";
            o1.Subject = o1.Title;
            o1.Body = "Live Conference";
            o1.Attachment = 1;
            o1.Category = "Activism";
            BiblePayDLL.Sidechain.UploadIntoDSQL_Background(false, ref o1, userProd);

        }


        public static void InnerJoinExample()
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
                    }
                    catch (Exception ex99)
                    {
                        string s1044 = "";
                    }
                }
            }
            catch (Exception ex)
            {
                string s1099 = "";
            }
        }
        private static void InitializeObject(bool fTestNet)
        {
            BiblePayCommon.Entity.versememorizer1 n = new BiblePayCommon.Entity.versememorizer1();
            //DataOps.InsertIntoTable(this, true, n, gUser(this));
            //DataOps.InsertIntoTable(this, false, n, gUser(this));
        }


        private static void TestSideChainInsert()
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
                    v.Expiration = (int) (BiblePayCommon.Common.UnixTimestampUTC() + (60 * 60 * 24 * 30));
                    lv.Add(v);
                }
            // Insert the list
            //var t = BiblePayDLL.Sidechain.SpeedyInsertMany(false, "newsfeeditems", lv, SERVICE_TYPE.PRIVATE_CACHE, gUser(this));
            // Retrieve the list
            IList<BiblePayCommon.Entity.NewsFeedItem> l2 = BiblePayDLL.Sidechain.GetChainObjects<BiblePayCommon.Entity.NewsFeedItem>(false,
                    "newsfeeditems", null, SERVICE_TYPE.PRIVATE_CACHE);
        }

        public async static void TestAILanguageTopicSummaryModule()
        {
            string sData = "";
            sData = System.IO.File.ReadAllText("c:\\cnn.txt");
            sData = BiblePayCommon.Encryption.PunctuateTranscript(sData);
            string sSummary = await BiblePayDLL.Sidechain.AISummary(sData, 6);
            List<string> lTopics = await BiblePayDLL.Sidechain.AILanguageTopics(sData, 7);
            string sData1 = "";
            for (int i = 0; i < lTopics.Count; i++)
            {
                sData1 += lTopics[i] + ",";
            }
            string sOut = sSummary + "\r\n\r\n" + sData1;
        }

        public static void MySqlTestPerformanceTest()
        {
            // insert thousands of rows and time this.
            string sql = "";
            MySQLData m = new MySQLData();
            StringBuilder b = new StringBuilder();
            b.Append("Delete from video1 where id='1';\nSTART TRANSACTION;\n");
            string sNotes = "A";
            for (int j = 0; j < 256; j++)
            {
                sNotes += "A";
            }
            for (int i = 181000; i < 182000; i++)
            {
                string sRow = "Insert into video1 (id,url,notes,parentid) values ('" + i.ToString() + "','data " + i.ToString() + "','" + sNotes + "','bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb');\n";
                b.Append(sRow);
            }
            b.Append("COMMIT\n");

            MySQLData.ExecuteNonQuery(b.ToString());
            // pull the data as a 
            string sql1 = "Select * from video1 order by url desc";
            List<BiblePayCommon.Entity.video1> v1 = MySQLData.RunSelect<BiblePayCommon.Entity.video1>(sql1);
        }


    }
}