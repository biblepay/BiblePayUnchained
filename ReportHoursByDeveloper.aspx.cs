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
    public partial class ReportHoursByDeveloper : BBPPage
    {

        protected new void Page_Load(object sender, EventArgs e)
        {

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
        protected void Run_Click(object sender, System.EventArgs e)
        {
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable3(IsTestNet(this), "TicketHistory");
            string sDevID = Request.Form["ddDevelopers"].ToNonNullString();
            if (sDevID == "")
            {
                UICommon.MsgBox("Missing", "Developer must be populated", this);
            }

            int nStart = DatePickerToUnixTimestamp(txtStartDate.Text);
            int nEnd = DatePickerToUnixTimestamp(txtEndDate.Text);

            dt = dt.FilterDataTable("UserID='" + sDevID + "' and Hours > 0");
            if (nStart  > 0 && nEnd > 0)
            {
                nEnd += 86400; // To midnight of the end date.
                dt = dt.FilterDataTable("time > '" + nStart.ToString() + "' and time < '" + nEnd.ToString() + "'");
            }
            string html = Report.GetTableHTML("Hours By Developer", dt, "UserID;Time;Body;Hours", "Hours");
            var result = Pdf.From(html).Portrait().Content();
            Response.Clear();
            Response.ContentType = "application/pdf";
            string accName = "Hours By Developer.pdf";
            Response.AddHeader("Content-Disposition", "attachment;filename=" + accName);
            Response.BinaryWrite(result);
            Response.Flush();
            Response.End();
        }


    }
}