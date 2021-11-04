using BiblePayCommon;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Net.Mail;
using System.Reflection;
using System.Threading.Tasks;
using static BiblePayCommon.Common;
using static BiblePayCommon.DataTableExtensions;
using static BiblePayCommon.Entity;
using static BiblePayCommon.EntityCommon;
using static BiblePayCommonNET.BiblePay;
using static BiblePayCommonNET.StringExtension;

namespace BiblePayDLL
{

    public static class Sidechain
    {
        public static DACResult BuySomething2(bool fTestNet, BiblePayCommon.Entity.invoice1 invoice, User u, string sPin)
        {
            BiblePayCommon.Common.DACResult dr2 = BiblePayTestHarness.BBPInterface.CreateFundingTransaction(fTestNet,
                BiblePayCommon.Common.GetDouble(invoice.Amount), invoice.BillFromAddress, u, sPin, "<test></test>", true);
            dr2.Invoice = invoice;
            if (dr2.fError())
            {
                // Error is handled on screen by the caller, so we dont wan't a double modal here
            }
            else
            {
                string sProdID = invoice.ProductID.ToNonNullString();
                if (sProdID == "")
                    sProdID = "N/A";
                string sAmt = invoice.Amount.ToNonNullString();
                InsertIntoDSQL(fTestNet, (IBBPObject)dr2.Invoice, u);
                dr2.URL = "You have purchased " + sProdID + " for " + sAmt
                        + " on TXID " + dr2.TXID + "!  Navigate here to see the invoice.  ";
            }
            return dr2;
        }

        public static int GetVersion()
        {
            return 1015;
        }

        public static bool Verify2FA(bool fTestNet, User u, string sPin)
        {
            bool fV = BiblePayTestHarness.BBPInterface.Verify2FA(fTestNet, u, sPin);
            return fV;
        }

        public static void SetBiblePayAddressAndSignature(bool fTestNet, string sDomainName, ref User u)
        {
            BiblePayTestHarness.BBPInterface.SetBiblePayAddressAndSignature(fTestNet, sDomainName, ref u);
        }

        public static string sRSAPublicKey = "4309FF1439AA24569FF22109AA437FF25AA12059FF11441AA6119FF1907AA485FF223AA19339FF11133AA4883FF2695AA53491FF39675AA31301FF12055AA108389FF59641AA35047FF6741AA6739FF1253AA7897FF1499AA14351FF2749AA3397FF641AA1865FF289AA";
        public static string RSAEncryptValue(string sData)
        {
            string sEnc = BiblePayTestHarness.RSA.RSAEncryptString(sRSAPublicKey, sData);
            return sEnc;
        }

        public static string EncryptWithUserKeyPair(bool fTestNet, string sData, User u)
        {
            // This RSA encryption method uses the Foundation RSA Public key and the biblepay public key.
            // To decrypt the value, you will need perfect secrecy elements (RSA Priv Key, BBP Priv Key, BBP Signature, signature fields, and the perfect secrecy requirements)
            string sEnc = BiblePayTestHarness.BBPInterface.EncryptStringWithUserKeyPair(fTestNet, sData, u);
            return sEnc;
        }

        public static string DecryptStringWithUserKeyPair(bool fTestNet, string sData, User u)
        {
            string sDec = BiblePayTestHarness.BBPInterface.DecryptStringWithUserKeyPair(fTestNet, sData, u);
            return sDec;
        }

        public static List<SimpleUTXO> GetBBPUTXOs(bool fTestNet, string sOwnerAddress, string sAddress)
        {
            // Used by Portfolio Builder.
            List<SimpleUTXO> sUTXO = new List<SimpleUTXO>();
            List<NBitcoin.Crypto.UTXO> l = NBitcoin.Crypto.BBPTransaction.GetBBPUTXOs(fTestNet, sAddress);
            for (int i = 0; i < l.Count; i++)
            {
                SimpleUTXO s = new SimpleUTXO();
                s.nAmount = l[i].Amount.Satoshi / 100000000;
                s.Address = sAddress;
                s.TXID = l[i].TXID.ToString();
                s.Ticker = "BBP";

                int nPin = (int)BiblePayCommon.Common.AddressToPin(sOwnerAddress, sAddress);
                bool fPin = BiblePayCommon.Common.CompareMask(l[i].Amount.Satoshi, nPin);
                if (fPin)
                    sUTXO.Add(s);
            }
            return sUTXO;
        }
        public static bool VerifySignature(bool fTestNet, string sBBPAddress, string sMessage, string sSignature)
        {
            bool fSigned = BiblePayTestHarness.BBPInterface.VerifySignature(fTestNet, sBBPAddress, sMessage, sSignature);
            return fSigned;
        }

        public static string SignMessage(bool fTestNet, string sPrivKey, string sMessage)
        {
            string s = BiblePayTestHarness.BBPInterface.SignMessage(fTestNet, sPrivKey, sMessage);
            return s;
        }
        public static DACResult UploadFileTypeBlob(bool fTestNet, string sFullPath, User u = new User())
        {
            DACResult r = BiblePayTestHarness.BBPInterface.UploadFileTypeBlob(fTestNet, sFullPath, u);
            return r;
        }

        public static BiblePayCommon.Entity.accountingobject GetAccountingBalance(bool fTestNet, string sBillFrom, string sBillTo)
        {
            BiblePayCommon.Entity.accountingobject o = BiblePayTestHarness.BBPInterface.GetAccountingBalance(fTestNet, sBillFrom, sBillTo);
            return o;
        }

        public static IList<T> GetChainObjects<T>(bool fTestNet, string sTable, FilterDefinition<T> myFilter, SERVICE_TYPE stType, string sSortBy = "", bool fAscending = false, string sURL = "")
        {
            string fqTable = BiblePayCommon.EntityCommon.GetFQTableName(fTestNet, sTable);
            

            IList<T> l1 = BiblePayTestHarness.BBPInterface.DSQL.GetChainObjects<T>(fTestNet, sTable, myFilter, stType, sSortBy, fAscending, sURL);
            return l1;
        }



        public static DACResult AdjustServiceAccountBalance(bool fTestNet, double nAdjAmt, string sBillFromAddress,
            string sBillToAddress, string sID, string sLineItem1, string sLineItem2, string sServiceName, bool fFlushNow, User u)
        {
            // Either the buyer or the seller must sign the accounting entry, otherwise the accounting entry is rejected.
            // If a fraudulent person bills something to you from *their* address, it succeeds, but obviously it does not need to be paid.
            // If a user tips another user, the billTo is the Tipper, and BillFrom is the Tippee and the BillTo is signed.
            BiblePayTestHarness.BBPInterface.AdjustServiceAccountBalance(fTestNet, nAdjAmt, sBillFromAddress, sBillToAddress, sID,
                sLineItem1, sLineItem2, sServiceName, fFlushNow);
            DACResult r1 = new DACResult();
            // Mission critical todo; whoever calls this function from BuyPet needs to rewrite this call...
            return r1;
        }


        public static BiblePayCommon.Common.DACResult SpeedyInsertMany(bool fTestNet, string sTable, List<dynamic> oList, SERVICE_TYPE stType, User u)
        { 
            DACResult t = BiblePayTestHarness.BBPInterface.DSQL.SpeedyInsertMany(fTestNet, sTable, oList, stType, u);
            return t;
        }


        public static string DownloadResourceAsString(string sURL)
        {
            try
            {
                string sFile = Guid.NewGuid().ToString();
                string sPath = Path.Combine(Path.GetTempPath(), sFile);
                BiblePayDLL.Sidechain.DownloadStoredObject(sURL, sPath);
                string sData = System.IO.File.ReadAllText(sPath);
                BiblePayCommon.Common.EraseTempData(sPath);
                return sData;
            }
            catch (Exception ex)
            {
                Log2("Cannot DownloadResource from " + sURL + " at file location " + ex.Message);
                return "";
            }
        }

        public static int GetVersion2()
        {
            return 1010;
        }
        public static DACResult InsertIntoDSQL(bool fTestNet, BiblePayCommon.IBBPObject o, User u = new User(), bool fBulkInsert = false)
        {
            DACResult r = new DACResult();
            fBulkInsert = false;

            try
            {
                r = BiblePayTestHarness.BBPInterface.InsertIntoDSQL2(fTestNet, o, u, fBulkInsert);
                if (r.fError())
                {
                    Log2("InsertDSQL::Error::" + r.Error);
                }
                else
                {
                    if (!fBulkInsert)
                    {
                        string sEntityName = BiblePayCommon.EntityCommon.GetEntityName(fTestNet, o);
                        UpdateDictionary(fTestNet, sEntityName, r.ExpandoObject);
                    }
                }
                return r;
            }
            catch (Exception ex)
            {
                Log2("ins into dsql::" + ex.Message);
                return r;
            }
        }

        public static bool CheckForUpgrade()
        {
            try
            {
                string sDomain = "https://foundation.biblepay.org/Uploads/";
                Type t1 = Type.GetType("BiblePayTestHarness.BBPInterface, BiblePayTestHarness, Culture=neutral, PublicKeyToken=null");
                object classInstance = Activator.CreateInstance(t1);
                MethodInfo methodInfo = t1.GetMethod("GetVersion");
                int nVersion = (int)methodInfo.Invoke(classInstance, null);
                string sFullPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "bin", "BiblePayTestHarness.dll");
                BiblePayCommon.BiblePayClient w = new BiblePayCommon.BiblePayClient();
                double nNewVersion = BiblePayCommon.Common.GetDouble(w.DownloadString(sDomain + "bbpdllversion.txt"));
                if (nNewVersion > nVersion)
                {
                    string sURL = sDomain + "BiblePayTestHarness.dll";
                    w.DownloadFile(sURL, sFullPath);
                }
                classInstance = Activator.CreateInstance(t1);
                nVersion = (int)methodInfo.Invoke(classInstance, null);
                if (nNewVersion > nVersion + 9)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            } 
            catch(Exception ex)
            {
                Log2("Unable to CheckForUpgrade::" + ex.Message);
                return false;
            }
        }

        public static void RD()
        {
            BiblePayTestHarness.BBPInterface.DSQL.RestoreDatabase("c:\\dump.txt");

        }
        public static DACResult SendMail(bool fTestNet, MailMessage message, string sFromFullName)
        {
            return BiblePayTestHarness.BBPInterface.SendMail(fTestNet, message, sFromFullName);
        }
        public static void InsertIntoDSQL_Background(bool fTestNet, BiblePayCommon.IBBPObject o, User u)
        {
            // Used for Likes and Video Watch Counts (so that the UI is not disrupted)
            BiblePayTestHarness.BBPInterface.InsertIntoDSQL_Background(fTestNet, o, u);
        }
        public static double QueryAddressBalance(bool fTestNet, string sAddress)
        {
            return NBitcoin.Crypto.BBPTransaction.QueryAddressBalance(fTestNet, sAddress);
        }

        public static byte[] DownloadBytes(string sURL, string sFileName)
        {
            byte[] b = BiblePayTestHarness.BBPInterface.DownloadBytes(sURL, sFileName);
            return b;
        }

        public static string DownloadString(string sURL)
        {
            return BiblePayTestHarness.BBPInterface.DownloadString(sURL);
        }

        public static void UploadIntoDSQL_Background(bool fTestNet, ref BiblePayCommon.Entity.object1 o, User u)
        {
            BiblePayTestHarness.BBPInterface.UploadIntoDSQL_Background(fTestNet, ref o, u);
        }
        public static string GetChosenSanctuary(bool fTestNet)
        {
            return NBitcoin.Crypto.BBPTransaction.GetChosenSanctuary(fTestNet);
        }

        public static string GetGSC(bool fTestNet)
        {
            return NBitcoin.Crypto.BBPTransaction.GetGSC(fTestNet);
        }
        public static string GetPubKeyFromPrivKey(bool fTestNet, string sPrivKey)
        {
            NBitcoin.BitcoinSecret scSpendingKey = new NBitcoin.BitcoinSecret(sPrivKey, fTestNet ? NBitcoin.Network.BiblepayTest : NBitcoin.Network.BiblepayMain);
            string sPubKey = scSpendingKey.ScriptPubKey.GetDestinationAddress(fTestNet ? NBitcoin.Network.BiblepayTest : NBitcoin.Network.BiblepayMain).ToString();
            return sPubKey;
        }
        public static bool ValidateAddress(bool fTestNet, string sAddress)
        {
            byte[] byteArray = NBitcoin.DataEncoders.Encoders.Base58.DecodeData(sAddress);
            string hex = NBitcoin.DataEncoders.Encoders.Hex.EncodeData(byteArray);
           
            try
            {
                NBitcoin.BitcoinAddress addr = NBitcoin.BitcoinAddress.Create(sAddress, fTestNet ? NBitcoin.Network.BiblepayTest : NBitcoin.Network.BiblepayMain);
                string a1 = addr.ToString();
                return a1 == sAddress;
            }
            catch(Exception)
            {
                return false;
            }
        }

        public static DACResult CreateFundingTransaction(bool fTestNet, double nAmount, string sSpendToAddress, string sSpendPrivKey, string sPayload, bool fSendForReal)
        {
            DACResult r = new DACResult();
            r.Result = String.Empty;
            bool fResult = NBitcoin.Crypto.BBPTransaction.CreateFundingTransaction(fTestNet, nAmount, sSpendToAddress, sSpendPrivKey,
                sPayload, fSendForReal, out r.Error, out r.TXID);
            return r;
        }

        public static void GhostVideos()
        {
            DataTable dtProd = RetrieveDataTable3(false, "video1");
            DataTable dtTest = RetrieveDataTable3(true, "video1");
            BiblePayTestHarness.BBPInterface.BBPMuse.GhostVideos(dtProd, dtTest);
        }
        public static void TranscodeVideos(bool fTestNet, string sPubKey, string sPrivKey, User u)
        {
            DataTable dt = RetrieveDataTable3(fTestNet, "video1", false,true);
            BiblePayTestHarness.BBPInterface.BBPMuse.Transcode(fTestNet, dt, sPubKey, sPrivKey, u);
        }

        public static void TranscriptVideos(bool fTestNet, string sPubKey, string sPrivKey, User u)
        {
            DataTable dt = RetrieveDataTable3(fTestNet, "video1", false,true);
            BiblePayTestHarness.BBPInterface.BBPMuse.Transcript(fTestNet, dt, sPubKey, sPrivKey, u);
        }

        public static bool DownloadStoredObject(string sURL, string sNewPath)
        {
            bool fResult = BiblePayTestHarness.BBPInterface.DownloadStoredObject(sURL, sNewPath);
            return fResult;
        }

        public static bool DeleteObject(bool fTestNet, string sTable, string sObjectID, User u = new User())
        {
            // Permissions: The user must have ownership rights (see server signing plus user signing)
            var builder = Builders<dynamic>.Filter;
            var filter = builder.Eq("_id", sObjectID);
            IList<dynamic> dt = BiblePayDLL.Sidechain.GetChainObjects<dynamic>(false, sTable, filter, SERVICE_TYPE.PUBLIC_CHAIN);
            if (dt.Count != 1)
                return false;
            dynamic v = BiblePayCommon.EntityCommon.ExpandoToStronglyCastObject(dt[0], sTable);
            DACResult r = BiblePayTestHarness.BBPInterface.DeleteObject(fTestNet, v, u);
            if (r.fError())
            {
                return false;
            }
            //UpdateDictionary(fTestNet, sTable, r.ExpandoObject);
            EvictAllCachedTables("video1");

            return true;
        }


        public static void UpdateWatchCounts(bool fTestNet)
        {
            BiblePayTestHarness.BBPInterface.DSQL.UpdateVideoCounts(fTestNet);
        }

        public static Dictionary<string, BBPDataTable> dictTables = new Dictionary<string, BBPDataTable>();

        public static void UpdateDictionary(bool fTestNet, string sTable, dynamic o)
        {

            EvictAllCachedTables(sTable);

            
            if (!dictTables.ContainsKey(BiblePayCommon.EntityCommon.GetFQTableName(fTestNet, sTable)))
            {
                return;
            }
            BBPDataTable dt = dictTables[BiblePayCommon.EntityCommon.GetFQTableName(fTestNet, sTable)];
            // First we need to find if the ID exists...
            string sID = o.id;
            BBPDataTable dt2 = dt.FilterBBPDataTable("id = '" + sID + "'");
            if (dt2.Rows.Count > 0)
            {
                // Delete the old record first... 
                dt = dt.FilterBBPDataTable("id <> '" + sID + "'");
            }
            DataRow _newRow = BBPEntityToDataRow(dt, o);
            double nDeleted = 0;
            if (_newRow.Table.Columns.Contains("deleted"))
            {
                nDeleted = GetDouble(_newRow["deleted"]);
            }
            if (nDeleted != 1)
            {
                dt.Rows.Add(_newRow);
            }
            dictTables[BiblePayCommon.EntityCommon.GetFQTableName(fTestNet, sTable)] = dt;
            // Update the best block hash here
            int nHeight = 0;
            string hash = BiblePayTestHarness.BBPInterface.DSQL.GetBestBlockHash(fTestNet, sTable, out nHeight);
            dt.BestBlockHash = hash;
            dt.Height = nHeight;
        }
        private static bool AllColsAreIncluded(List<string> lRequiredCols, dynamic oDSQLObject)
        {
            if (lRequiredCols.Count == 0)
                return true;

            bool fInclude = true;
            for (int j = 0; j < lRequiredCols.Count; j++)
            {
                bool fFoundCol = false;
                foreach (var attribute in oDSQLObject)
                {
                    string sColName1 = attribute.Name.ToLower();
                    if (lRequiredCols[j].ToLower() == sColName1)
                    {
                        fFoundCol = true;
                        break;
                    }
                }

                if (!fFoundCol)
                    fInclude = false;
            }
            return fInclude;
        }

        public static DataTable StringToDataTable(string sData)
        {
            string[] vdata = sData.Split(";");
            BBPDataTable dt = new BBPDataTable { TableName = "table1" };
            dt.Columns.Add("column1");
            dt.Columns.Add("id");

            for (int i = 0; i < vdata.Length; i++)
            {
                string sEle = vdata[i];
                DataRow _newrow = dt.NewRow();
                _newrow["column1"] = sEle;
                _newrow["id"] = sEle;
                dt.Rows.Add(_newrow);
            }
            return dt;
        }
        public static DataRow ExpandoToDataRow(DataTable dt, System.Dynamic.ExpandoObject oDSQL)
        {
            DataRow _newrow = dt.NewRow();
            try
            {
                foreach (var attribute in oDSQL)
                {
                    string sColName1 = attribute.Key;
                    dynamic oValue = attribute.Value;
                    if (!dt.Columns.Contains(sColName1))
                    {
                        // ToDo: Find out why this routine is not picking up the native int32 datatype?
                        if (sColName1 == "time" || sColName1 == "updated" || sColName1=="Hours" || sColName1=="ProcessTime" || sColName1 == "TicketNumber" || sColName1 == "VoteSum" || sColName1 == "WatchSum")
                        {
                            dt.Columns.Add(sColName1, Type.GetType("System.Int32"));
                        }
                        else if (sColName1 == "Order")
                        {
                            dt.Columns.Add(sColName1, Type.GetType("System.Double"));
                        }
                        else
                        {
                            dt.Columns.Add(sColName1);

                        }
                    }
                    _newrow[sColName1] = oValue;
                    if (sColName1 == "_id")
                    {
                        if (!dt.Columns.Contains("id"))
                        {
                            dt.Columns.Add("id");
                        }
                        _newrow["id"] = oValue;
                    }
                }
                return _newrow;
            }
            catch (Exception)
            {
                return _newrow;
            }
        }

        public static DataRow BBPEntityToDataRow(DataTable dt, IBBPObject o)
        {
            DataRow _newrow = dt.NewRow();
            try
            {
                Type myType = o.GetType();
                IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());

                foreach (PropertyInfo prop in props)
                {
                    object propValue = prop.GetValue(o, null);
                    string sColName1 = prop.Name;

                    if (!dt.Columns.Contains(sColName1))
                    {
                        // ToDo: Find out why this routine is not picking up the native int32 datatype?
                        if (sColName1 == "time" || sColName1 == "updated" || sColName1 == "TicketNumber")
                        {
                            dt.Columns.Add(sColName1, Type.GetType("System.Int32"));
                        }
                        else if (sColName1 == "Order")
                        {
                            dt.Columns.Add(sColName1, Type.GetType("System.Double"));
                        }
                        else
                        {
                            dt.Columns.Add(sColName1);
                        }
                    }

                    _newrow[sColName1] = propValue;
                }
                return _newrow;
            }
            catch (Exception ex)
            {
                string test1 = ex.Message;
                return _newrow;
            }
        }
        public static DataRow JsonObjToDataRow(DataTable dt, dynamic oDSQL)
        {
            DataRow _newrow = dt.NewRow();
            try
            {
                foreach (var attribute in oDSQL)
                {
                    string sColName1 = attribute.Name;
                    dynamic oValue = attribute.Value;
                    if (!dt.Columns.Contains(sColName1))
                    {
                        dt.Columns.Add(sColName1);
                    }
                    _newrow[sColName1] = oValue;
                }
                return _newrow;
            }
            catch (Exception ex)
            {
                string test1 = ex.Message;
                return _newrow;
            }
        }

        public struct bgObj
        {
            public string TableName;
            public string MyLastBlockHash;
            public bool TestNet;
        }

        public static void CheckForNewBlocks(object o)
        {
            bgObj b = (bgObj)o;
            int nHeight = 0;
            string hash = BiblePayTestHarness.BBPInterface.DSQL.GetBestBlockHash(b.TestNet, b.TableName, out nHeight);
            if (hash != b.MyLastBlockHash)
            { 
                string sFQName = BiblePayCommon.EntityCommon.GetFQTableName(b.TestNet, b.TableName);
                BiblePayCommon.EntityCommon.dictTableDirty[sFQName] = true;
                EvictAllCachedTables(b.TableName);
            }
        }

        public static void CheckForNewBlocks(bool fTestNet, string sTableName, string sMyLastBlockHash)
        {
            // This runs once per Web Site action per table, but only if 30 seconds have passed since the last action, and once per distinct object 
            // This lets the federated server know if the chain has changed and therefore it inducts the new block into memory, resulting in a dictionary change of the table
            string fCM = fTestNet ? "TESTNET" : "MAINNET";
            double nLastCheck = GetDouble(BiblePayCommon.HalfordMemoryCache.Read("lbc_" + fCM + sTableName));
            if (nLastCheck == 1)
                return;
            BiblePayCommon.HalfordMemoryCache.Write("lbc_" + fCM + sTableName, "1", 30);
            System.Threading.Thread t = new System.Threading.Thread(CheckForNewBlocks);
            bgObj b = new bgObj();
            b.TableName = sTableName;
            b.TestNet = fTestNet;
            b.MyLastBlockHash = sMyLastBlockHash;
            t.Start(b);
        }

     
        public static BBPDataTable RetrieveDataTable3(bool fTestNet, string sTable, bool fIncludeDeleted = false, bool fOverrideCache = false)
        {
            string fqTable = BiblePayCommon.EntityCommon.GetFQTableName(fTestNet, sTable);
            if (dictTables.ContainsKey(fqTable) && !fIncludeDeleted && !fOverrideCache)
            {
                if (BiblePayCommon.EntityCommon.dictTableDirty[fqTable] == false && dictTables[fqTable].Rows.Count > 0)
                {
                    CheckForNewBlocks(fTestNet, sTable, dictTables[fqTable].BestBlockHash);
                    return dictTables[fqTable];
                }
            }
            BBPDataTable dt = new BBPDataTable{ TableName = sTable };

            IO.Log("Retrieving DataTable " + sTable, true);

            int nRow = 0;
            List<string> lReqColumns = EntityCommon.GetAssemblyNameContainingType("BiblePayCommon.Entity", sTable);
            lReqColumns.Clear(); // For now, lets leave this empty
            try
            {
                int iCounter = 0;
                object o = BiblePayCommon.EntityCommon.GetInstance("BiblePayCommon.Entity+" + sTable);
                List<object> oList = BiblePayTestHarness.BBPInterface.DSQL.GetListOfObjects(fTestNet, sTable);
                object oSample = BiblePayCommon.EntityCommon.GetInstance("BiblePayCommon.Entity+" + sTable);
                // mission critical todo: add Bool for Hidden columns to BBPDataTable

                for (int iRow = 0; iRow < oList.Count; iRow++)
                {
                    ExpandoObject oDSQLObject = (ExpandoObject)oList[iRow];
                    DataRow _newrow = ExpandoToDataRow(dt, oDSQLObject);
                    double nDeleted = 0;
                    if (dt.Columns.Contains("deleted"))
                    {
                        nDeleted = GetDouble(_newrow["deleted"]);
                    }
                    if (nDeleted != 1 || fIncludeDeleted)
                    {
                        dt.Rows.Add(_newrow);
                        nRow++;
                        iCounter++;
                    }
                }
                dt.BestBlockHash = BiblePayTestHarness.BBPInterface.DSQL.GetBestBlockHash(fTestNet, sTable, out dt.Height);
                if (!fIncludeDeleted)
                {
                    dictTables[fqTable] = dt;
                    BiblePayCommon.EntityCommon.dictTableDirty[fqTable] = false;
                }
                return dt;
            }
            catch (Exception ex)
            {
                IO.Log("unable to retrieve data in getdatatable::" + ex.Message);
                return dt;
            }
        }
    }
}

