using System.Data;
using static BiblePayCommon.Common;
using static BiblePayCommon.DataTableExtensions;
using static BiblePayCommonNET.CommonNET;
using static BiblePayCommonNET.StringExtension;
using static Unchained.Common;
using static Unchained.DataOps;


namespace Unchained
{
    public partial class FormView : BBPPage
    {
        protected override void Event(BBPEvent e)
        {
            if (e.EventAction == "Save_Click" || e.EventAction == "Delete_Click")
            {
                if (!gUser(this).LoggedIn)
                {
                    UICommon.MsgBox("Error", "Sorry, you must be logged in first to save a record. ", this);
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
                    // During save (of a generic form object), first if it is not a restricted or hidden field :
                    if (Request.Form["input_" + sColName] != null)
                    {
                        string sNewValue = Request.Form["input_" + sColName].ToString();
                        BiblePayCommon.EntityCommon.SetEntityValue(o, sColName, sNewValue);
                    }
                    // otherwise, we pull value from session (since it was restricted or hidden)
                    else if (Request.Form["input_" + sColName] == null)
                    {
                        object oOrigValue = Session["input_" + sColName];
                        if (oOrigValue != null)
                        {
                            BiblePayCommon.EntityCommon.SetEntityValue(o, sColName, oOrigValue);
                        }
                    } 
                }


                if (e.EventAction == "Delete_Click")
                {
                    BiblePayCommon.EntityCommon.SetEntityValue(o, "deleted", "1");
                }
              

                DACResult r = InsertIntoTable(this, IsTestNet(this), o, gUser(this));

                if (!r.fError())
                {
                    Session["stack"] = UICommon.Toast("Saved", "Your " + sTable + " has been " + sMode + "(ed)!");
                }
            }
        }

        public string GetStandardDropDown(string sID, string sTable, string sColumn, string sSelectedValue)
        {
            DataTable dtGroup = UICommon.GetGroup(IsTestNet(this), "video1", "url like '%mp4%'", "Category");
            string sOptions = "";
            for (int y = 0; y < dtGroup.Rows.Count; y++)
            {
                bool fSelected = sSelectedValue.ToLower() == dtGroup.Rows[y]["category"].ToString().ToLower();
                string sSel = fSelected ? " SELECTED " : "";

                string sRow = "<option " + sSel + "value='" + dtGroup.Rows[y]["category"].ToString() + "'>" 
                    + dtGroup.Rows[y]["category"].ToString() + "</option>\r\n";
                sOptions += sRow;
            }
            string sDD = "<select name='input_" + sID + "' id='input_" + sID + ">";
            sDD += sOptions;
            sDD += "</select>";
            return sDD;
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
                BiblePayDLL.Sidechain.InsertIntoDSQL(IsTestNet(this), o, gUser(this));

            }
            if (sMode != "add")
            {
                // Filter on edit and view
                dt = dt.FilterDataTable("id='" + sID + "'");
            }
            string html = "<fieldset><legend>" + sTable + " - Object " + sID + " - " + sMode + ":</legend>";
            html += "<table width=90%>";
            
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                string sColName = dt.Columns[i].ColumnName;
                string sOrigValue = dt.GetColValue(0, dt.Columns[i].ColumnName);
                object oOrigValue = dt.Rows[0][i];
                if (sMode == "add")
                {
                    sOrigValue = "";
                }
                bool fRestricted = BiblePayCommon.EntityCommon.IsRestrictedColumn(sColName);
                // if (System.Diagnostics.Debugger.IsAttached)
                bool fHidden = BiblePayCommon.EntityCommon.IsHidden(sColName);
                bool fReadonly = BiblePayCommon.EntityCommon.IsReadOnly(sColName);
                string sReadOnly = (sMode == "view" || fReadonly) ? "readonly" : "";
                string sValueControl = "<input name='input_" + sColName + "'" + sReadOnly + " class='pc90' value = '" + sOrigValue + "' />";
                if (sOrigValue.Length > 255 || sColName.ToLower() == "body" || sColName.ToLower() == "transcript")
                {
                    sValueControl = "<textarea rows='10' cols='80' name='input_" + sColName + "' class='pc90' " + sReadOnly + ">" + sOrigValue + "</textarea>";
                }
                // If this is a dropdown...
                if (sTable == "video1" && sColName.ToLower() == "category")
                {
                    sValueControl = GetStandardDropDown(sColName, "video1", sColName, sOrigValue);
                }
                
                string sRow = "<tr><td width=25%><span>" + sColName + ":</span><td>" + sValueControl + "</tr>\r\n";
                if (!fRestricted && !fHidden)
                   html += sRow;
                if (fRestricted || fHidden)
                {
                    // Store in session
                    Session["input_" + sColName] = oOrigValue;
                }
            }
            html += "</table>";
            // Submit an edit
            string sButton = UICommon.GetStandardButton(sID, "Save", "Save", "Save");
            if (System.Diagnostics.Debugger.IsAttached)
            {
                string sDeleteButton = UICommon.GetStandardButton(sID, "Delete", "Delete", "Delete");
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