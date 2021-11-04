using BiblePayCommonNET;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using static Unchained.Common;

namespace Unchained
{
    public partial class Status : BBPPage
    {

        protected new void Page_Load(object sender, EventArgs e)
        {
            string sAction = Request.QueryString["action"].ToNonNullString();
            if (sAction == "datadump")
            {
                DumpDatabases();

            }

        }


        protected string GetStatus()
        {
            // For each chain
            List<string> lEntities = BiblePayCommon.EntityCommon.GetBiblePayEntities();
            string sTable = "Status v1.0.12<br><table class='saved'><tr><th>Object<th>Hash<th>Height</tr>";
            string sChainPrefix = IsTestNet(this) ? "testnet_" : "";
            for (int i = 0; i < lEntities.Count; i++)
            {
                if (BiblePayDLL.Sidechain.dictTables.ContainsKey(sChainPrefix + lEntities[i]))
                {
                    string s = sChainPrefix + lEntities[i];
                    string sHash = BiblePayDLL.Sidechain.dictTables[s].BestBlockHash;
                    int nHeight = BiblePayDLL.Sidechain.dictTables[s].Rows.Count;
                    string sAnchor = gUser(this).Administrator == 1 ? "<a href=ListView?includedeleted=0&objecttype=" + s + ">" + s + "</a>" : s;
                    string sRow = "<tr><td class='saved'>" + sAnchor + "<td>" + sHash + "<td>" + nHeight.ToString() + "</tr>";
                    sTable += sRow;
                }
            }
            sTable += "</table>";
            string sBurnAddress = BiblePayCommon.Encryption.GetBurnAddress(IsTestNet(this));
            string sPubKey = IsTestNet(this) ? BiblePayCommon.Encryption._keyset.userTestNet.BiblePayAddress : BiblePayCommon.Encryption._keyset.userProd.BiblePayAddress;
            BiblePayCommon.Entity.accountingobject oBalance = BiblePayDLL.Sidechain.GetAccountingBalance(IsTestNet(this), sBurnAddress, sPubKey);
            string sBalance = "<br>Server Balance: " + oBalance.Balance.ToString();
            sBalance += "<br>Current Invoice: <pre>" + oBalance.Data + "</pre>";
            sTable += sBalance;
            return sTable;
        }


        
        protected void DumpDatabases()
        {
            return;
            List<string> lEntities = BiblePayCommon.EntityCommon.GetBiblePayEntities();
            StringBuilder sb = new StringBuilder();
            // Provide server key to access the "emailaddress, passwordhash, and any organization fields for the client"
            for (int z = 0; z <= 1; z++)
            {
                string sChainPrefix = z==0 ? "testnet_" : "";
                string sChain = z == 0 ? "TESTNET" : "MAINNET";

                for (int i = 0; i < lEntities.Count; i++)
                {
                    DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable3(z==0, lEntities[i]);

                    if (dt.Rows.Count > 0)
                    {
                        for (int j = 0; j < dt.Rows.Count; j++)
                        {
                            BiblePayCommon.IBBPObject o = BiblePayCommon.EntityCommon.TableRowToStronglyCastObject(dt, lEntities[i], j);
                            string json = Newtonsoft.Json.JsonConvert.SerializeObject(o);
                            string data = "<ENTITYROWSPLITTER/><CHAIN>" + sChain + "</CHAIN><ENTITYNAME>" + lEntities[i] + "</ENTITYNAME><ERN>" + j.ToString()
                                + "</ERN><ENTITYROW>" + json + "</ENTITYROW>\r\n";
                            sb.Append(data);
                        }
                    }
                }
            }

            System.IO.File.WriteAllText("c:\\dump.txt", sb.ToString());
        }


    }
}