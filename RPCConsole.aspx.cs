using System;
using System.Collections.Generic;
using System.Web.UI;
using static Unchained.Common;

namespace Unchained
{
    public partial class RPCConsole : BBPPage
    {
        private List<string> lHistory = new List<string>();

        protected new void Page_Load(object sender, EventArgs e)
        {
            if (Session["lhistory"] != null)
            {
                lHistory = (List<string>)Session["lhistory"];
            }
        }

        public string ExecuteCommand(string command)
        {
            string[] vCmd = command.Split(" ");
            if (vCmd.Length > 0)
            {
                if (vCmd[0] == "gethash")
                {
                    if (vCmd.Length > 1)
                    {
                        string sResult = GetSha256HashI(vCmd[1]);
                        return sResult;
                    }
                }
                else if (vCmd[0] == "next")
                {
                    return "NA";
                }
            }
            return "Command not found.";
        }

        protected void btnCommand_Click(object sender, EventArgs e)
        {
            string sResult = ExecuteCommand(txtCommand.Text);

            string sRow = "<font color=blue>" + gUser(this).UserName + " [" + DateTime.Now.ToString() + "] " + txtCommand.Text + ":</font><br>";
            sRow += sResult;

            lHistory.Add(sRow);
            
            Session["lhistory"] = lHistory;
            
            txtCommand.Text = "";
            txtCommand.Focus();
        }

        protected string GetRPC()
        {
            string sScreen = "<div id='rpcconsole' style='font-family:Courier;overflow-y:scroll;border:1px solid gold;height:500px;min-height:500px;'>";
            for (int i = 0; i < lHistory.Count; i++)
            {
                sScreen += "" + lHistory[i] + "<br>";
            }
            sScreen += "</div>";

            if (lHistory.Count > 20)
            {
                 lHistory.RemoveAt(0);
                 Session["lhistory"] = lHistory;

            }
            ScriptManager.RegisterStartupScript(this, this.GetType(), "scroll1", "fixscroll();", true);

            return sScreen;

        }

        protected void UpdateTimer_Tick(object sender, EventArgs e)
        {
            // This event refreshes the chat contents...
        }
    }
}
