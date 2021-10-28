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

namespace Unchained
{
    public partial class Reports : BBPPage
    {

        protected new void Page_Load(object sender, EventArgs e)
        {

        }

        protected void HoursByDeveloper_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("ReportHoursByDeveloper");
        }


    }
}