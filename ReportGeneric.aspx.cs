using BiblePayCommonNET;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using static Unchained.Common;
using static BiblePayCommon.Common;
using static BiblePayCommonNET.DataTableExtensions;
using static BiblePayCommon.DataTableExtensions;
using OpenHtmlToPdf;

namespace Unchained
{
    public partial class ReportGeneric : BBPPage
    {

        protected new void Page_Load(object sender, EventArgs e)
        {

            string sTest = "";
            if (IsPostBack)
            {

            }
        }

        int DatePickerToUnixTimestamp(string sDatePickerValue)
        {
            if (sDatePickerValue == "")
                return 0;
            try
            {
                DateTime dt1 = DateTime.Parse(sDatePickerValue);
                int nStart = (int)DateTimeToUnixTimestamp(dt1);
                return nStart;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public static string _report = "";
        protected string GetReport()
        {
            return _report;
        }

        protected void Run_Click(object sender, System.EventArgs e)
        {
            string sTable = Request.Form["ddTables"].ToNonNullString();
            Session["last_ddTable"] = sTable;

            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable3(IsTestNet(this), sTable);

            if (sTable == "")
            {
                UICommon.MsgBox("Missing", "Table must be populated", this);
            }

            int nStart = DatePickerToUnixTimestamp(txtStartDate.Text);
            int nEnd = DatePickerToUnixTimestamp(txtEndDate.Text);

            if (nStart  > 0 && nEnd > 0)
            {
                nEnd += 86400; // To midnight of the end date.
                dt = dt.FilterDataTable("time > '" + nStart.ToString() + "' and time < '" + nEnd.ToString() + "'");
            }
            
            if (txtWhere.Text != "")
            {
                dt = dt.FilterDataTable(txtWhere.Text);
            }
            if (txtSelect.Text == "*")
            {
                // pull in all columns here
                string sCols = "";
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    sCols += dt.Columns[i].ColumnName + ";";
                }
                sCols = Mid(sCols, 0, sCols.Length - 1);
                txtSelect.Text = sCols;
            }
            txtSelect.Text = txtSelect.Text.Replace(",", ";");
            dt = dt.OrderBy(txtOrderBy.Text);

            _report = Report.GetTableHTML("Generic", dt, txtSelect.Text, "");
            
        }


    }
}