using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenHtmlToPdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI;
using static Unchained.Common;
using static BiblePayDLL.Encryption;

namespace Unchained
{
    public partial class Admin1 : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

         
        }


        protected void BatchJob1()
        {
            // rapture videos
            string sql = "Select * from Rapture where filename like '%.mp4%'";
            DataTable d11 = gData.GetDataTable(sql, false);
            
            for (int i = 0; i < d11.Rows.Count; i++)
            {
                string sURL = d11.Rows[i]["url"].ToString();
                string sFN = d11.Rows[i]["FileName"].ToString();
                string sPath = "s:\\san\\Rapture2\\" + sFN;
                bool fExists = System.IO.File.Exists(sPath);
                if (fExists)
                {
                    BiblePayDLL.SharedCommon.DACResult r = BiblePayDLL.Sidechain.UploadFileTypeVideo(false, sPath, GetFundingAddress(IsTestNet(this)), GetFundingKey(IsTestNet(this)));
                    string url2 = r.Result;

                    if (url2 != "")
                    {
                        sql = "Update Rapture set Url='" + url2 + "',san='1' where url = '" + sURL + "'";
                        gData.Exec(sql);
                        Debug.WriteLine(sql);
                    }
                }
            }
        }


        protected void BatchJob2()
        {
            // rapture videos
            string sql = "Select * from Rapture where filename like '%.mp4%'";
            DataTable d11 = gData.GetDataTable(sql, false);
            for (int i = 0; i < d11.Rows.Count; i++)
            {
                string sURL = d11.Rows[i]["url"].ToString();
                dynamic o = new System.Dynamic.ExpandoObject();
                string sFN = d11.Rows[i]["FileName"].ToString();
                o.Subject =  d11.Rows[i]["Title"].ToString();
                o.Body  = d11.Rows[i]["Notes"].ToString();
                o.UserName = gUser(this).UserName.ToString();
                o.URL = d11.Rows[i]["URL"].ToString();
                o.Body = Mid(o.Body, 0, 777);
                string sID = GetSha256Hash(o.URL);
                BiblePayDLL.SharedCommon.DACResult r = DataOps.InsertIntoTable(IsTestNet(this), o, "video1", sID);
                string t = r.TXID;
            }
        }

        public static class ExpandoObjectHelper
        {
            public static bool HasProperty(ExpandoObject obj, string propertyName)
            {
                return obj != null && ((IDictionary<String, object>)obj).ContainsKey(propertyName);
            }
        }

        private void MigrateVideos(bool fTestNet)
        {
            DataTable dt1 = BiblePayDLL.Sidechain.RetrieveDataTable2(IsTestNet(this), "object", "", "", "url", "", "");
            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                string sURL1 = dt1.Rows[i]["URL"].ToString();
            }
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable2(IsTestNet(this), "video1", "", "", "username,time,id,body,URL,subject", "", "");
            for (int y = 0; y < dt.Rows.Count; y++)
            {
                string sURL = dt.Rows[y]["URL"].ToNonNullString();
                string sID = dt.Rows[y]["id"].ToNonNullString();
                dynamic o = new System.Dynamic.ExpandoObject();
                o.Size = 1000000;
                o.FileName = "";
                o.DirectoryName = "";
                o.URL = sURL;
                BiblePayDLL.SharedCommon.DACResult r = DataOps.InsertIntoTable(IsTestNet(this), o, "object", sID);
                string t = r.TXID;
            }
        }
        private void Migrate(string sTable, bool fTestNetFromChain, bool fTestNetToChain)
        {
            Internals.BBPWebClient w = new Internals.BBPWebClient();
            string sURL = BiblePayDLL.Sidechain.GetChosenSanctuary(fTestNetFromChain) + "/rest/dsqlquery/" + sTable;
            string sData = w.DownloadString(sURL);
            dynamic oJson = JsonConvert.DeserializeObject<dynamic>(sData);
            int i = 0;
            foreach (var j in oJson)
            {
                string sKey = j.Name;
                dynamic oNewton = j.Value;
                if (j.Value != null)
                {
                    dynamic oDSQL = BiblePayDLL.Sidechain.NewtonSoftToExpando(oNewton);
                    if (oDSQL != null)
                    {
                        if (ExpandoObjectHelper.HasProperty(oDSQL, "table"))
                        {
                            if (oDSQL.table == sTable)
                            {
                                i++;
                                BiblePayDLL.Sidechain.SidechainDSQLInsert(fTestNetToChain, ref oDSQL, sTable, i.ToString(), GetFundingKey(IsTestNet(this)), GetFundingAddress(IsTestNet(this)));
                            }
                        }
                    }
                }
            }
        }


        protected void btnSave_Click(object sender, EventArgs e)
        {
            //  Migrate("pray1", true, false);
            //  Migrate("comment1", true, false);
            //  Migrate("video1", true, false);
            //  MigrateVideos(false);
            MsgBox("Success", "1", this);
            return;
        }
    }
}