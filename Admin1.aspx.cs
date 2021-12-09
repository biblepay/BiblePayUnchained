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
using System.Text;
using System.Reflection;

namespace Unchained
{
    public partial class Admin1 : BBPPage
    {
        protected new void Page_Load(object sender, EventArgs e)
        {
            if (gUser(this).Administrator!=1)
            {
                UICommon.MsgBox("Error", "You are not authorized", this);
            }
        }

        private static int ctr = 0;
        protected override void Event(BBPEvent e)
        {
        }
                

        protected void btnSave_Click(object sender, EventArgs e)
        {

            Unchained.Code.Retired.TestCoercedUploadFromWebhook();
            return;


            Unchained.Code.DataMigration.SchemaCreator(false);
        }
    }
}