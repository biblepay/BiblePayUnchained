using BiblePayCommon;
using System;
using System.Data;
using static BiblePayCommon.DataTableExtensions;
using static Unchained.Common;
using static BiblePayCommonNET.DataTableExtensions;

namespace Unchained
{
    public partial class ListView : BBPPage
    {

        protected string GetList()
        {
            string sTable = Request.QueryString["objecttype"] ?? "";
            string sFilterType = Request.QueryString["filtertype"] ?? "";
            bool fIncDeleted = (Request.QueryString["includedeleted"] ?? "") == "1";

            BBPDataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable3(IsTestNet(this), sTable, fIncDeleted);

            if (sFilterType == "mine" && sTable=="invoice1")
            {
                // Billed to Me only
                dt = dt.FilterBBPDataTable("BillToAddress='" + gUser(this).BiblePayAddress + "' and amount is not null");
                dt = (BBPDataTable)dt.GetData.OrderBy("time desc");
            }
            else if (sFilterType == "myreceivables")
            {
                // Sent to me only
                dt = dt.FilterBBPDataTable("BillFromAddress='" + gUser(this).BiblePayAddress + "' and amount is not null and BillToAddress <> '" + gUser(this).BiblePayAddress + "'");
                dt = (BBPDataTable)dt.OrderBy("time desc");
            }
            else if (sFilterType == "analyzer")
            {
                // no filter
            }
            if (sTable == "invoice1")
            {
                // Retail Only
            }
            string sObjectName = sTable; // Todo - get the human readable name from the classes static property.
                                         // Table header
            string sRow = "<tr>";
            int iColCt = 0;
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                bool fRestricted = BiblePayCommon.EntityCommon.IsRestrictedColumn(dt.Columns[i].ColumnName);
                if (!fRestricted || fIncDeleted)
                {
                    sRow += "<th>" + dt.Columns[i].ColumnName + "</th>";
                    iColCt++;
                }
            }

            string html = "<table class=saved><tr class='objheader'><th class='objheader' colspan='" + iColCt.ToString() + "'>"
                + "<h3>" + sObjectName + "</h3>";
            html += "<span style='float:right;'>" + UICommon.GetStandardAnchor("ancPrayer1", "AddPrayer", "", "<i class='fa fa-plus'></i>", "Add Prayer", "pray1") + "</span>";

            sRow += "</tr>";
            html += sRow;

            for (int y = 0; y < dt.Rows.Count; y++)
            {
                sRow = "<tr>";
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    bool fRestricted = BiblePayCommon.EntityCommon.IsRestrictedColumn(dt.Columns[j].ColumnName);
                    if (!fRestricted || fIncDeleted)
                    {
                        string sOrigValue = dt.GetColValue(y, dt.Columns[j].ColumnName);
                        string sValue = BiblePayCommon.Common.Left(sOrigValue, 256);
                        if (dt.Columns[j].ColumnName.ToLower()=="data")
                        {
                            string sNewValue = sValue.Replace("<col>", "[col]");
                            sNewValue = sNewValue.Replace("<row>", "[row]");
                            sNewValue = sNewValue.Replace("\r\n", "<br>");
                            sValue = BiblePayCommon.Common.Mid(sNewValue, 0, 256);
                        }
                        int iLen = sOrigValue.Length;
                        if (iLen > 256)
                            sValue += " ... ";
                        string sID = dt.Rows[y]["id"].ToString();
                        string sURL = "FormView?table=" + sTable + "&id=" + sID;
                        string sAnchor = "<a href='" + sURL + "'>" + sValue + "</a>";
                        sRow += "<td class='saved'>" + sAnchor + "</td>";
                    }
                    
                }
                html += sRow;
            }
            html += "</table>";
            return html;
        }
    }
}