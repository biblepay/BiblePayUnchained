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
using System.Reflection;

namespace Unchained
{
    public partial class ProposalAdd : BBPPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ddCharity.Items.Clear();
            ddCharity.Items.Add("Charity");
            ddCharity.Items.Add("PR");
            ddCharity.Items.Add("P2P");
            ddCharity.Items.Add("IT");
            ddCharity.Items.Add("XSPORK");
        }

        protected void btnSubmitProposal_Click(object sender, EventArgs e)
        {
            // Submit


        }
    }
}