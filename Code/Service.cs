using BiblePayDLL;
using System;
using System.Collections.Generic;
using System.Data;
using static Unchained.Common;

namespace Unchained
{

    public static class Service
    {

        private static string UrlToFn(string URL)
        {
            string[] vURL = URL.Split("/");
            string FN = vURL[vURL.Length - 1];
            return FN;
        }

        public static string ExecutePortfolioBuilderExport(bool fTestNet)
        {
            Dictionary<string, PortfolioParticipant> u = GenerateUTXOReport(fTestNet);
            string sSummary = "<data>";

            foreach (KeyValuePair<string, PortfolioParticipant> pp in u)
            {
                string sSummaryRow = "<row>"
                    + pp.Value.UserID
                    + "<col>" + pp.Value.NickName
                    + "<col>"
                    + "<col>" + pp.Value.AmountBBP.ToString()
                    + "<col>" + pp.Value.AmountForeign.ToString()
                    + "<col>" + pp.Value.AmountUSDBBP.ToString()
                    + "<col>" + pp.Value.AmountUSDForeign.ToString()
                    + "<col>" + pp.Value.AmountUSD.ToString()
                    + "<col>" + Math.Round(pp.Value.Coverage, 7).ToString()
                    + "<col>" + Math.Round(pp.Value.Strength, 7).ToString() + "\r\n";
                sSummary += sSummaryRow;
            }
            sSummary += "</data>";
            string sHash = "<hash>" + GetSha256HashI(sSummary) + "</hash>";
            sSummary += sHash;
            sSummary += "\r\n<EOF>\r\n";
            return sSummary;
        }

        public static bool PortfolioBuilderExportExists(bool fTestNet)
        {
            try
            {
                DateTime dt1 = System.DateTime.UtcNow;
                string sChain = fTestNet ? "test" : "main";
                string sIntegrationObjectName = "utxointegration_" + sChain + "_" + dt1.ToString("MM_dd_yy") + ".dat";
                string sURL = "https://bbpdb.s3.filebase.com/" + sIntegrationObjectName;
                string sData = BiblePayDLL.Sidechain.DownloadResourceAsString(sURL);
                string sHash = Unchained.Common.ExtractXML(sData, "<hash>", "</hash>");
                return sHash.Length == 64 ? true : false;
            }
            catch (Exception ex)
            {
                Log("ExportExists" + ex.Message);
                return false;
            }

        }

        public static void DailyUTXOExport(bool fTestNet)
        {
            bool fExists = PortfolioBuilderExportExists(fTestNet);
            if (fExists)
            {
                //Log("Export exists for " + fTestNet.ToString());
                return;
            }
            //todo check if it already exists here.
            BiblePayCommon.Entity.utxointegration2 o = new BiblePayCommon.Entity.utxointegration2();
            string sData = ExecutePortfolioBuilderExport(fTestNet);
            o.UserID = Config("");
            o.Data = sData;
            o.DateTime = System.DateTime.UtcNow.ToString();

            BiblePayCommon.Common.DACResult r = DataOps.InsertIntoTable(fTestNet, o);
            Log("Created daily export for " + fTestNet.ToString());

            if (r.Error == "")
            {

            }

        }

        public static void SerializeDataTable(DataTable dtSnap, string sPath)
        {
            //StreamWriter sw = new StreamWriter(sPath);
            //Write a line of text
            try
            {
                dtSnap.WriteXml(sPath, XmlWriteMode.WriteSchema, true);
            }catch(Exception ex)
            {
                Log("Error while serializing datatable::" + ex.Message);
            }

        }


        // This is the background thread that executes mission critical services.
        // Service 1 is the Portfolio Builder Export
        public static void Executor()
        {
            Log("Starting Executor v1.2...");
            System.Threading.Thread.Sleep(60000);

            while (true)
            {
                try
                {
                    if (true)
                    {
                        Sidechain.TranscodeVideos(GetFundingAddress(false), GetFundingKey(false));

                        DailyUTXOExport(true);
                        DailyUTXOExport(false);
                    }
                }
                catch(Exception ex)
                {
                    Log("Error in Executor::" + ex.Message);
                }
                System.Threading.Thread.Sleep(60000 * 1);
            }
        }
    }
}
