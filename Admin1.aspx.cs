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

        protected void BatchJob1(bool fTestNet)
        {
            // rapture videos
            string sql = "Select * from Rapture where filename like '%.mp4%'";
            DataTable d11 = gData.GetDataTable(sql, false);
            
            for (int i = 0; i < d11.Rows.Count; i++)
            {
                        string sURL = d11.Rows[i]["url"].ToString();
                        string sFN = d11.Rows[i]["FileName"].ToString();
                        string sPath = "s:\\san\\Rapture2\\" + sFN;
                        BiblePayCommon.Entity.video1 v = new BiblePayCommon.Entity.video1();
                        v.Classification = "video";
                        v.deleted = 0;
                        string sTitle = "";
                        string notes = "";
                        GetNotesHTML(d11.GetColValue(i, "Notes"), out sTitle, out notes);
                        v.Title = sTitle;
                        v.Body = notes;
                        v.Subject = d11.GetColValue(i, "Category");
                        v.UserID = gUser(this).BiblePayAddress;
                        v.time = UnixTimestampUTC();
                        v.URL = d11.GetColValue(i, "URL");

                        DACResult r3 = BiblePayDLL.Sidechain.InsertIntoDSQL(fTestNet, v, "video1", GetFundingAddress(fTestNet), GetFundingKey(fTestNet));


            }
        }



        private void MigrateVideos(bool fTestNet)
        {
            DataTable dt1 = BiblePayDLL.Sidechain.RetrieveDataTable2(IsTestNet(this), "video1");

            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                // if the UserID is blank.
                string FID = dt1.GetColValue(i, "FID");
                if (FID == "")
                {
                    
                    BiblePayCommon.Entity.video1 v = new BiblePayCommon.Entity.video1();
                    v.Body = dt1.GetColValue(i, "Body");
                    v.Classification = dt1.GetColValue(i, "Classification");
                    v.deleted = 0;
                    v.ParentID = dt1.GetColValue(i, "ParentID");
                    v.Subject = dt1.GetColValue(i, "Subject");
                    v.URL = dt1.GetColValue(i, "URL");
                    v.UserID = gUser(this).BiblePayAddress;
                    //v.Size = (int)dt1.GetColDouble(i, "Size");
                    string sOrigFilename = "s:\\san\\rapture2\\"; // + UrlToFn(v.URL);
                    if (System.IO.File.Exists(sOrigFilename))
                    {
                        DACResult r = BiblePayDLL.Sidechain.UploadFileTypeBlob(fTestNet, sOrigFilename, 
                            GetFundingAddress(fTestNet), GetFundingKey(fTestNet));
                        v.FID = r.TXID;
                        v.URL = r.Result;
                        DataOps.InsertIntoTable(fTestNet, v);
                    }

                }
            }
        }


        private void InitializeObject(bool fTestNet)
        {
            BiblePayCommon.Entity.versememorizer1 n = new BiblePayCommon.Entity.versememorizer1();

            DataOps.InsertIntoTable(true, n);
            DataOps.InsertIntoTable(false, n);

        }



        protected void btnSave_Click(object sender, EventArgs e)
        {

            BatchJob1(false);

            // Mission Critical Todo:  Genesis block for VerseMemorizer

            return;

    
        }
    }
}