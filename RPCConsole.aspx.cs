using BiblePayCommonNET;
using System;
using System.Collections.Generic;
using System.Web.UI;
using static Unchained.Common;
using static BiblePayCommonNET.StringExtension;
using static BiblePayCommon.Common;
using static BiblePayCommonNET.CommonNET;


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

        private string GetChain(Page p)
        {
            string sChain = IsTestNet(this.Page) ? "test" : "main";
            return sChain;
        }

        public string EncodeCommand(string command)
        {
            string sRow = "";
            bool fInQuote = false;
            for (int i = 0; i < command.Length; i++)
            {
                string sChar = command.Substring(i, 1);
                if (sChar == "\"")
                {
                    fInQuote = !fInQuote;
                }
                if (sChar == " ")
                {
                    if (!fInQuote)
                        sChar = "<SPACE>";
                }
                sRow += sChar;
            }
            sRow = sRow.Replace("\"", "");
            return sRow;
        }
        public string ExecuteCommand(string command1)
        {
            string command = EncodeCommand(command1);

            string[] vCmd = command.Split("<SPACE>");
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
                else if (vCmd[0] == "addverse")
                {
                    string sFrom = vCmd[1];
                    string sTo = vCmd[2];
                    string[] vFrom = sFrom.Split("|");
                    string[] vTo = sTo.Split("|");

                    BiblePayCommon.Entity.versememorizer1 o = new BiblePayCommon.Entity.versememorizer1();
                    o.UserID = Common.gUser(this).id;
                    o.BookFrom = vFrom[0];
                    o.ChapterFrom = vFrom[1].ToInt();
                    o.VerseFrom = vFrom[2].ToInt();
                    o.BookTo = vTo[0];
                    o.ChapterTo = vTo[1].ToInt();
                    o.VerseTo = vTo[2].ToInt();

                    BiblePayCommon.Common.DACResult r = DataOps.InsertIntoTable(this, Common.IsTestNet(this), o, gUser(this));
                    string sResult = r.Result + ", " + r.Error;
                    return sResult;
                }
                else if (vCmd[0] == "setaddress")
                {
                    if (vCmd.Length > 1)
                    {
                        string sAddress = vCmd[1];
                        string[] vAddress = sAddress.Split(",");
                        if (vAddress.Length == 5)
                        {
                            BMS.StoreCookie("setaddress", sAddress);
                            return "Your delivery address has been set in your browser.  Note: When you clear your cookies you will have to set it again. ";
                        }
                        else
                        {
                            return "You must include 'name,address,city,state,zip'.";
                        }
                    }
                    else
                    {
                        return "You must include the 'name,address,city,state,zip'.";
                    }
                }
            }
            return "Command not found.";
        }

        protected void btnCommand_Click(object sender, EventArgs e)
        {
            string sResult = ExecuteCommand(txtCommand.Text);

            string sRow = "<font color=blue>" + gUser(this).FullUserName() + " [" + DateTime.Now.ToString() + "] " + txtCommand.Text + ":</font><br>";
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
