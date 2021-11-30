using BiblePayCommon;
using System;
using System.Data;
using static BiblePayCommon.DataTableExtensions;
using static Unchained.Common;
using static BiblePayCommonNET.DataTableExtensions;
using static BiblePayCommonNET.CommonNET;
using System.Text;
using System.Collections.Generic;

namespace Unchained
{
    public partial class ListView : BBPPage
    {

        protected override void Event(BBPEvent e)
        {
            if (e.EventAction == "EditRoles_Click")
            {
                Response.Redirect("Admin?Organization=" + _bbpevent.EventValue);
            }
        }


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
            List<string> lCols = new List<string>();

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                bool fRestricted = BiblePayCommon.EntityCommon.IsRestrictedColumn(dt.Columns[i].ColumnName);
                bool fHidden = BiblePayCommon.EntityCommon.IsHidden(sTable, dt.Columns[i].ColumnName);
                bool fReadonly = BiblePayCommon.EntityCommon.IsReadOnly(sTable, dt.Columns[i].ColumnName);

                if (!fRestricted || fIncDeleted)
                {
                    if (!fHidden)
                    {
                        lCols.Add(dt.Columns[i].ColumnName);

                        sRow += "<th>" + dt.Columns[i].ColumnName + "</th>";
                        iColCt++;
                    }
                }
            }

            StringBuilder html = new StringBuilder();


            string h1 = "<table class=saved><tr class='objheader'><th class='objheader' colspan='" + iColCt.ToString() + "'>"
                + "<h3>" + sObjectName + "</h3>";
            html.Append(h1);

            //            html += "<span style='float:right;'>" + UICommon.GetStandardAnchor("ancPrayer1", "AddPrayer", "", "<i class='fa fa-plus'></i>", "Add Prayer", "pray1") + "</span>";

            sRow += "</tr>";
            html.Append(sRow);

            
            for (int y = 0; y < dt.Rows.Count; y++)
            {
                sRow = "<tr>";
                string sID = dt.Rows[y]["id"].ToString();

                for (int j = 0; j < lCols.Count; j++)
                {
                    string sActiveCol = lCols[j];
                    string sOrigValue = dt.GetColValue(y, sActiveCol);
                    string sValue = BiblePayCommon.Common.Left(sOrigValue, 256);
                    if (sActiveCol.ToLower() == "data")
                    {
                                string sNewValue = sValue.Replace("<col>", "[col]");
                                sNewValue = sNewValue.Replace("<row>", "[row]");
                                sNewValue = sNewValue.Replace("\r\n", "<br>");
                                sValue = BiblePayCommon.Common.Mid(sNewValue, 0, 256);
                    }
                    int iLen = sOrigValue.Length;
                    if (iLen > 256)
                                sValue += " ... ";
                    string sURL = "FormView?table=" + sTable + "&id=" + sID;
                    string sAnchor = "<a href='" + sURL + "'>" + sValue + "</a>";
                    sRow += "<td class='saved'>" + sAnchor + "</td>";
                    
                }
                if (gUser(this).Administrator == 1 && sTable.ToLower()=="organization")
                {
                    string sRoles = "<td>" + UICommon.GetStandardAnchor("ancRoles", "EditRoles", sID, "<i class='fa fa-wrench'></i>", "Edit Org Roles", "Organization") + "</td>";
                    sRow += sRoles;
                }
                if (BiblePayCommon.Common.GetDouble(dt.Rows[y]["Amount"]) == 99.51)
                {
                    string sTest = "";

                }
                html.Append(sRow + "</tr>");

            }
            html.Append("</table>");

            return html.ToString();

        }
    }
}