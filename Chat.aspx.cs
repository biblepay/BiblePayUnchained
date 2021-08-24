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
    public static class Chatter
    {
        public static List<string> lChat = new List<string>();
    }
    public partial class Chat : BBPPage
    {
        protected void btnChat_Click(object sender, EventArgs e)
        {
            string sRow = gUser(this).UserName + " [" + DateTime.Now.ToString() + "]: " + txtChat.Text;
            Chatter.lChat.Add(sRow);
            txtChat.Text = "";
            txtChat.Focus();
        }

        private int _lastdict = Chatter.lChat.Count;

        protected string GetChat()
        {
            string sScreen = "<div id='chat' style='overflow-y:scroll;border:1px solid gold;height:500px;min-height:500px;'>";
            for (int i = 0; i < Chatter.lChat.Count; i++)
            {
                sScreen += "" + Chatter.lChat[i] + "<br>";
            }
            sScreen += "</div>";

            if (Chatter.lChat.Count > 20)
            {
                 Chatter.lChat.RemoveAt(0);
            }
            return sScreen;
        }

        protected void UpdateTimer_Tick(object sender, EventArgs e)
        {
            // This event refreshes the chat contents...
        }
    }
}
