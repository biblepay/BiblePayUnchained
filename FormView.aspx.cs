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
using static BiblePayCommon.DataTableExtensions;

namespace Unchained
{
    public partial class FormView : BBPPage
    {
        protected new void Page_Load(object sender, EventArgs e)
        {
            

        }

        protected override void Event(BBPEvent e)
        {
            if (e.EventAction == "Save_Click" || e.EventAction == "Delete_Click")
            {
                if (!gUser(this).LoggedIn)
                {
                    MsgBox("Error", "Sorry, you must be logged in with a biblepay address and nickname to save a record. ", this);
                }
                string sID = Request.QueryString["id"] ?? "";
                string sMode = Request.QueryString["action"] ?? "";
                string sTable = Request.QueryString["table"] ?? "";
                // Mission critical, check has ownership here:

                DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable2(IsTestNet(this), sTable);
                if (sMode == "edit")
                {
                    dt = dt.FilterDataTable("id='" + sID + "'");
                    if (dt.Rows.Count < 1)
                    {
                        Session["stack"] = UICommon.Toast("Failed", "Your Edit Failed!");
                        return;
                    }
                }

                BiblePayCommon.IBBPObject o = (BiblePayCommon.IBBPObject)BiblePayCommon.EntityCommon.GetInstance("BiblePayCommon.Entity+" + sTable);
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                        string sColName = dt.Columns[i].ColumnName;
                        string sOrigValue = dt.GetColValue(0, dt.Columns[i].ColumnName);
                        if (Request.Form["input_" + sColName] != null)
                        {
                            string sNewValue = Request.Form["input_" + sColName].ToString();
                            BiblePayCommon.EntityCommon.SetEntityValue(o, sColName, sNewValue);
                        }
                }


                if (e.EventAction == "Delete_Click")
                {
                    BiblePayCommon.EntityCommon.SetEntityValue(o, "deleted", "1");

                }
                else
                {
                    //SetEntityValue(o, "UserID", gUser(this).BiblePayAddress);
                    //SetEntityValue(o, "deleted", "0");
                }

                InsertIntoTable(IsTestNet(this), o);

                Session["stack"] = UICommon.Toast("Saved", "Your " + sTable + " has been " + sMode + "(ed)!");

            }
        }

        bool IsRestricted(string sWord)
        {
            string sRestricted = "UserID,deleted,table,id,time,chain,lastblockhash,objecthash";
            string[] vRestricted = sRestricted.Split(",");
            for (int i = 0; i < vRestricted.Length; i++)
            {
                if (vRestricted[i].ToLower() == sWord.ToLower())
                    return true;
            }
            return false;
        }

        protected string GetFormView()
        {
            string sTable = Request.QueryString["table"] ?? "";
            string sID = Request.QueryString["id"] ?? "";
            string sMode = Request.QueryString["action"] ?? "";
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable2(IsTestNet(this), sTable);
            
            if (sTable == "news1" && dt.Rows.Count == 0)
            {
                BiblePayCommon.Entity.news1 o = new BiblePayCommon.Entity.news1();
                o.Title = "";
                o.URL = "";
                BiblePayDLL.Sidechain.InsertIntoDSQL(IsTestNet(this), o, "news1", GetFundingAddress(IsTestNet(this)), GetFundingKey(IsTestNet(this)));

            }
            if (sMode != "add")
            {
                // Filter on edit and view
                dt = dt.FilterDataTable("id='" + sID + "'");
            }
            string html = "<fieldset><legend>" + sTable + " - Object " + sID + " - " + sMode + ":</legend>";
            html += "<table width=90%>";
            string sReadOnly = sMode == "view" ? "readonly" : "";

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                string sColName = dt.Columns[i].ColumnName;
                string sOrigValue = dt.GetColValue(0, dt.Columns[i].ColumnName);
                if (sMode == "add")
                {
                    sOrigValue = "";
                }
                string sValueControl = "<input name='input_" + sColName + "'" + sReadOnly + " style= 'width:90%;' value = '" + sOrigValue + "' />";
                if (sOrigValue.Length > 255)
                {
                    sValueControl = "<textarea rows='10' cols='80'>" + sOrigValue + "</textarea>";
                }
                
                bool fRestricted = IsRestricted(sColName);
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    fRestricted = false;
                }

                    string sRow = "<tr><td width=25%><span>" + sColName + ":</span><td>" + sValueControl + "</tr>\r\n";
                if (!fRestricted)
                   html += sRow;
            }
            html += "</table>";
            // Submit an edit
            string sButton = "<button id='btnSave' onclick=\""
             + "__doPostBack('Event_Save_" + "_" + sID + "_', 'Save_Click');\">Save</button> ";
            if (System.Diagnostics.Debugger.IsAttached)
            {
                string sDeleteButton = "<button id='btnDelete' onclick=\""
            + "__doPostBack('Event_Delete_" + "_" + sID + "_', 'Delete_Click');\">Delete</button> ";
                html += sDeleteButton;
            }
            if (sMode == "edit" || sMode == "add")
            {
                html += sButton;
            }
            html += "</fieldset>";
            return html;
        }
    }
}