using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using static BiblePayCommon.Common;
using static Unchained.Common;
using static BiblePayCommonNET.StringExtension;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Linq;
using static BiblePayCommon.EntityCommon;
using static BiblePayCommonNET.CommonNET;

namespace Unchained
{
    public partial class webhook : BBPPage
    {
        protected new void Page_Load(object sender, EventArgs e)
        {
            string headers = Request.Headers["Jesus"].ToNonNullString();
            string[] keys = Request.Form.AllKeys;
            for (int i = 0; i < keys.Length; i++)
            {
                string data = keys[i] + ": " + Request.Form[keys[i]] + "<br>";
                Log("webhook " + data);
            }
            Log("webhook headers " + headers);

            Response.Write("OK");
            Response.End();

        }

    }
}