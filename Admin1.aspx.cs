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
            //            FixVideos(false);
          
            //            Bibl/ePayDLL.Sidechain.FixVidCat();

            string sTest1 = "";

            return;

    
        }
    }
}