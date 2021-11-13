using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using static BiblePayCommon.Common;
using static BiblePayCommonNET.DataTableExtensions;
using static BiblePayCommon.DataTableExtensions;
using System.Text;

namespace Unchained
{
    public static class Report
    {
        public static string GetTableBeginning(string sTableName)
        {
            string css = "<style> html {    font-size: 1em;    color: black;    font-family: verdana }  .r1 { font-family: verdana; font-size: 10; }</style>";
            string logo = "https://www.biblepay.org/wp-content/uploads/2018/04/Biblepay70x282_96px_color_trans_bkgnd.png";
            string sLogoInsert = "<img width=300 height=100 src='" + logo + "'>";
            string HTML = "<HTML>" + css + "<BODY><div><div style='margin-left:12px'><TABLE class=r1><TR><TD width=70%>" + sLogoInsert
                + "<td width=25% align=center>" + sTableName + "</td><td width=5%>" + DateTime.Now.ToShortDateString() + "</td></tr>";
            HTML += "<TR><TD><td></tr>" + "<TR><TD><td></tr>" + "<TR><TD><td></tr>";
            HTML += "</table>";
            return HTML;
        }

        public static string GetTableHTML(string sReportName, DataTable dt, string sCols, string sTotalCol)
        {
            StringBuilder HTML = new StringBuilder();

            HTML.Append(GetTableBeginning(sReportName));

            string[] vCols = sCols.Split(new string[] { ";" }, StringSplitOptions.None);
            string sHeader = "<tr>";
            for (int i = 0; i < vCols.Length; i++)
            {
                sHeader += "<th>" + vCols[i] + "</th>";
            }
            sHeader += "</tr>";

            HTML.Append("<table width=100%>" + sHeader + "<tr><td colspan=5 width=100%><hr></tr>");

            double nTotal = 0;
            
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string sRow = "<tr>";

                for (int j = 0; j < vCols.Length; j++)
                {
                    string sValueControl = dt.Rows[i][vCols[j]].ToString();
                    if (vCols[j].ToLower()=="time")
                    {
                        sValueControl = BiblePayCommon.Common.ConvertFromUnixTimestamp((int)dt.GetColDouble(i, "time")).ToShortDateString();
                    }
                    if (sValueControl.Length > 255)
                        sValueControl = Mid(sValueControl, 0, 254);

                    sRow += "<td align=right>" + sValueControl + "</td>";
                }
                sRow += "</tr>";
                if (sTotalCol != "")
                {
                    nTotal += GetDouble(dt.Rows[i][sTotalCol]);
                }
                HTML.Append(sRow);

            }


            HTML.Append("<tr><td>&nbsp;</td></tr>");
            HTML.Append("<tr><td>TOTAL:<td><td>" + nTotal.ToString() + "</tr>");

            HTML.Append("</body></html>");
            return HTML.ToString();

        }

    }
}