using System;
using static BiblePayCommon.Common;
using static BiblePayCommonNET.CommonNET;
using static BiblePayCommonNET.UICommonNET;
using static Unchained.Common;

namespace Unchained
{

    public partial class NFTOrphans : BBPPage
    {
        protected override void Event(BBPEvent e)
        {

            if (e.EventAction == "btnBuyNFT_Click" && e.EventValue.Length > 10)
            {

            }
        }

        protected new void Page_Load(object sender, EventArgs e)
        {
        }
    }
}