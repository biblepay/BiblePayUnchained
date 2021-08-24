using static Unchained.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static BiblePayDLL.Shared;
using static Unchained.DataOps;
using static BiblePayCommon.Common;

namespace Unchained
{
    public partial class BBPUniversity : BBPPage
    {
        protected new void Page_Load(object sender, EventArgs e)
        {
            Session["exammemory"] = null;
            
        }

    }
}