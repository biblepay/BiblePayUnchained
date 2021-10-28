using BiblePayCommon;
using System.Data;
using System.Linq;
using static BiblePayCommon.DataTableExtensions;
using static Unchained.Common;
using static BiblePayCommonNET.DataTableExtensions;

namespace Unchained
{
    public partial class Meeting : BBPPage
    {

        protected string GetMeeting()
        {
            string sMeetingName = "Public";

            string sData = "<iframe allow=\"camera; microphone; fullscreen; display-capture; autoplay\" "
                + "src=\"https://meet.biblepay.org/" + sMeetingName + "\" style='height: 100%; width: 100%; min-height:500px; border: 0px;'></iframe>";
            return sData;
        }
    }
}