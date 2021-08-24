using System;
using System.Data;
using static BiblePayCommon.DataTableExtensions;
using static Unchained.Common;

namespace Unchained
{
    public partial class ListView : BBPPage
    {
        protected new void Page_Load(object sender, EventArgs e)
        {

            
        }

        
        protected string GetList()
        {
            string sTable = Request.QueryString["objecttype"] ?? "";
            string sFilterType = Request.QueryString["filterttype"] ?? "";

            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable2(IsTestNet(this), sTable);
            if (sFilterType == "mine" && sTable=="invoice1")
            {
                // Billed to Me only
                dt = dt.FilterDataTable("BillToAddress='" + gUser(this).BiblePayAddress + "'");
            }
            if (sTable == "invoice1")
            {
                // Retail Only
                dt = dt.FilterDataTable("InvoiceType in ('retail','Service_Provider')");
            }
            string sObjectName = sTable; // Todo - get the human readable name from the static class property.
            // Table header
            string html = "<table class=saved><tr class='objheader'><th class='objheader'>"
                + "<h3>" + sObjectName + "</h3><th class='objheader' colspan=19><div style='text-align:right;'>"
                + "<a onclick=\"__doPostBack('Event','AddPrayer_Click');\"><i class='fa fa-plus'></i></a></div></th></tr>";
            // Column headers
            string sRow = "<tr>";

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                sRow += "<th>" + dt.Columns[i].ColumnName + "</th>";
            }
            sRow += "</tr>";
            html += sRow;

            for (int y = 0; y < dt.Rows.Count; y++)
            {
                sRow = "<tr>";
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string sOrigValue = dt.GetColValue(y, dt.Columns[j].ColumnName);
                    string sValue = Left(sOrigValue, 256);
                    int iLen = sOrigValue.Length;
                    if (iLen > 256)
                        sValue += " ... ";
                    string sID = dt.Rows[y]["id"].ToString();

                    string sURL = "FormView?table=" + sTable + "&id=" + sID;

                    string sAnchor = "<a href='" + sURL + "'>" + sValue + "</a>";

                    sRow += "<td>" + sAnchor + "</td>";

                    
                }
                html += sRow;
            }
            html += "</table>";
            return html;
        }
    }
}