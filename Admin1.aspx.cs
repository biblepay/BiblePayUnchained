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


        private void MigrateVideos(bool fTestNet)
        {
            DataTable dt1 = BiblePayDLL.Sidechain.RetrieveDataTable2(IsTestNet(this), "video1");
            // clear the transcript field
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
                    v.Title = dt1.GetColValue(i, "Title");
                    v.URL = dt1.GetColValue(i, "URL");
                    v.UserID = gUser(this).id;
                    //v.Size = (int)dt1.GetColDouble(i, "Size");
                    string sOrigFilename = "s:\\san\\rapture2\\"; // + UrlToFn(v.URL);
                    if (System.IO.File.Exists(sOrigFilename))
                    {
                        DACResult r = BiblePayDLL.Sidechain.UploadFileTypeBlob(fTestNet, sOrigFilename, gUser(this));
                        v.FID = r.TXID;
                        v.URL = r.Result;
                        DataOps.InsertIntoTable(this, fTestNet, v, gUser(this));
                    }

                }
            }
        }


        private void InitializeObject(bool fTestNet)
        {
            BiblePayCommon.Entity.versememorizer1 n = new BiblePayCommon.Entity.versememorizer1();

            DataOps.InsertIntoTable(this, true, n, gUser(this));
            DataOps.InsertIntoTable(this, false, n, gUser(this));

        }



        protected void btnSave_Click(object sender, EventArgs e)
        {

            //UICommon.ReskinCSS("darkblue", "blue", "#00004B", "blue");
            //UICommon.ReskinCSS("maroon", "#c32148", "#400000", "maroon");
            //UICommon.ReskinCSS("#696969", "#808080", "#393939", "grey");
            //            UICommon.ReskinCSS("black", "#202020", "#393939", "black");
            FixVideos(false);

            //            Bibl/ePayDLL.Sidechain.FixVidCat();

            string sTest1 = "";

            return;

    
        }
    }
}