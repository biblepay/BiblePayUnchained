using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Web.UI;
using static BiblePayCommon.Common;
using static Unchained.Common;
using RestSharp;
using System.Net.Http;
using static BiblePayCommon.DataTableExtensions;
using System.Net.Mail;
using static BiblePayCommonNET.StringExtension;

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


        private void FixVideos(bool fTestNet)
        {
            DataTable dt5 = BiblePayDLL.Sidechain.RetrieveDataTable2(IsTestNet(this), "video1");
            Data d = new Data(Data.SecurityType.REQ_SA);
            for (int i = 0; i < dt5.Rows.Count; i++)
            {
                BiblePayCommon.Entity.video1 v = (BiblePayCommon.Entity.video1)BiblePayCommon.EntityCommon.TableRowToStronglyCastObject(dt5, "video1", i);
                string sql = "Select Category from Rapture where title like '%" + Mid(v.Title, 0, 20) + "%'";
                string sCat = d.GetScalarString(sql, "category");
                if (v.Category == "" && sCat != "")
                {
                    v.Category = sCat;
                    BiblePayDLL.Sidechain.InsertIntoDSQL(fTestNet, v, gUser(this));
                }
                else if (v.Category == "" && !v.Title.ToLower().Contains("mass") && !v.Title.ToLower().Contains("batch"))
                {
                    v.Category = "Uncategorized";
                    BiblePayDLL.Sidechain.InsertIntoDSQL(fTestNet, v, gUser(this));
                }
            }
            string sTest = "";
            return;
        }


        private void PerformanceTest(bool fTestNet)
        {

            
            double nStartTime = UnixTimestampUTC();
            // 67 secs for writing 256 records...
            // 1 second to read 1174 records
            BiblePayCommon.Entity.performance1 p = new BiblePayCommon.Entity.performance1();
            int i = 7;
            p.field1 = i.ToString();
            p.field2 = i.ToString();
            p.field3 = i.ToString();
            p.id = Guid.NewGuid().ToString();

            //BiblePayDLL.Sidechain.SetSelector(1, true, "performance1", p);

            //1.5 secs read; 7 secs to write
            //2 secs to read (9000 docs); on sanc-rep-set; 2 secs to write


            DataTable dt5 = BiblePayDLL.Sidechain.RetrieveDataTable2(fTestNet, "performance1", true);
            Data d = new Data(Data.SecurityType.REQ_SA);
            for (int z = 0; z < dt5.Rows.Count;z++)
            {
                //BiblePayCommon.Entity.performance1 q = (BiblePayCommon.Entity.performance1)BiblePayCommon.EntityCommon.TableRowToStronglyCastObject(dt5, "performance1", z);

                
            }
            double nReadTime = UnixTimestampUTC();
            double nElapsedReadTime = nReadTime - nStartTime;

            string pause1 = "";

            double nEndTime = UnixTimestampUTC();
            double nElapsed = nEndTime - nStartTime;
            Console.WriteLine(nElapsed.ToString());
            
            string sTest = "";
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

        protected void btnSave_Click(object sender, EventArgs e)
        {
            //PerformanceTest(true);

            //string s1 = BiblePayDLL.Sidechain.EncryptWithUserKeyPair(true, "mydata1 1008 irwin st aliquippa", gUser(this));
            //string s2 = BiblePayDLL.Sidechain.DecryptStringWithUserKeyPair(true, s1, gUser(this));

            BiblePayDLL.Sidechain.RD();

            return;
        }
    }
}