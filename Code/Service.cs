using BiblePayDLL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using static Unchained.Common;
using static BiblePayCommonNET.BiblePay;
using static BiblePayCommonNET.StringExtension;

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

        static long nLastUTXO = 0;
        public static void DailyUTXOExport(bool fTestNet)
        {
            long nElapsed = BiblePayCommon.Common.UnixTimestampUTC() - nLastUTXO;
            if (nElapsed < 60 * 60)
                return;
            nLastUTXO = BiblePayCommon.Common.UnixTimestampUTC();
            int nNextHeight = 0;
            bool fExists = BiblePayUtilities.PortfolioBuilderExportExists(fTestNet, out nNextHeight);
            if (fExists || nNextHeight == 0)
            {
                //Log("Export exists for " + fTestNet.ToString());
                return;
            }
            
            //todo check if it already exists here.
            BiblePayCommon.Entity.utxointegration2 o = new BiblePayCommon.Entity.utxointegration2();
            string sData = BiblePayUtilities.ExecutePortfolioBuilderExport(fTestNet, nNextHeight);
            o.UserID = Config("");
            o.Data = sData;
            o.DateTime = System.DateTime.UtcNow.ToString();

            BiblePayCommon.Common.DACResult r = DataOps.InsertIntoTable(null, fTestNet, o, CoerceUser(fTestNet));
            Log("Created daily export for " + fTestNet.ToString());

            if (r.fError())
            {

            }

        }

        public static void SerializeDataTable(DataTable dtSnap, string sPath)
        {
            // We don't currently need to serialize a microsoft data table to XML, but we may in the future
            try
            {
                dtSnap.WriteXml(sPath, XmlWriteMode.WriteSchema, true);
            }
            catch(Exception ex)
            {
                Log("Error while serializing datatable::" + ex.Message);
            }

        }


        // This is the background thread that executes mission critical services.
        public static void Executor()
        {
            Log("Starting Executor v1.3...");
            System.Threading.Thread.Sleep(20000);
            bool fAttached = System.Diagnostics.Debugger.IsAttached;

            while (true)
            {
                try
                {
                    if (true)
                    {
                        
                        Sidechain.TranscodeVideos(false, GetFundingAddress(false), GetFundingKey(false), CoerceUser(false));
                        Sidechain.TranscodeVideos(true, GetFundingAddress(true), GetFundingKey(true), CoerceUser(true));

                        Sidechain.TranscriptVideos(true, GetFundingAddress(true), GetFundingKey(true), CoerceUser(true));
                        Sidechain.TranscriptVideos(false, GetFundingAddress(false), GetFundingKey(false), CoerceUser(false));
                        
                        DailyUTXOExport(true);
                        DailyUTXOExport(false);

                        Sidechain.UpdateWatchCounts(true);
                        Sidechain.UpdateWatchCounts(false);
                        

                        double nPrice1 = BiblePayCommon.Common.GetDouble(BiblePayCommon.HalfordDatabase.GetKVDWX("price1"));
                        if (nPrice1 == 0)
                        {
                            BiblePayCommon.HalfordDatabase.SetKVDWX("price1", 1, 60 * 60 * 24 * 1);
                            BiblePayUtilities.GetCryptoPrices(true);
                            BiblePayUtilities.GetCryptoPrices(false);
                        }
                        if (fAttached)
                            Sidechain.GhostVideos();
                        
                    }

                    System.Threading.Thread.Sleep(fAttached ? 60000 * 1 : 60000 * 6);
                }
                catch(Exception ex)
                {
                    Log("Error in Executor::" + ex.Message);
                }
                


            }
        }
    }
}
