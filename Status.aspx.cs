using System.Collections.Generic;
using static Unchained.Common;

namespace Unchained
{
    public partial class Status : BBPPage
    {

        protected string GetStatus()
        {

            // For each chain
            List<string> lEntities = BiblePayCommon.EntityCommon.GetBiblePayEntities();
            string sTable = "<table class='saved'><tr><th>Object<th>Hash<th>Height</tr>";
            string sChainPrefix = IsTestNet(this) ? "testnet_" : "";
            for (int i = 0; i < lEntities.Count; i++)
            {
                if (BiblePayDLL.Sidechain.dictTables.ContainsKey(sChainPrefix + lEntities[i]))
                {

                    string s = sChainPrefix + lEntities[i];
            
                    string sHash = BiblePayDLL.Sidechain.dictTables[s].BestBlockHash;
                    int nHeight = BiblePayDLL.Sidechain.dictTables[s].Rows.Count;

                    string sAnchor = gUser(this).Administrator == 1 ? "<a href=ListView?includedeleted=0&objecttype=" + s + ">" + s + "</a>" : s;
                    string sRow = "<tr><td>" + sAnchor + "<td>" + sHash + "<td>" + nHeight.ToString() + "</tr>";
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

    }
}