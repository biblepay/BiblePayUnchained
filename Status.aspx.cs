using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using static Unchained.StringExtension;
using static Unchained.Common;
using BiblePayDLL;

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


                    string sRow = "<tr><td>" + s + "<td>" + sHash + "<td>" + nHeight.ToString() + "</tr>";
                    sTable += sRow;
                }
            }
            sTable += "</table>";
            return sTable;
        }

    }
}