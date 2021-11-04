using System;
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

        protected new void Page_Load(object sender, EventArgs e)
        {
            if (!gUser(this).LoggedIn)
            {
                UICommon.MsgBox("Security Error", "Sorry, you must be logged in.", this);
            }
        }
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

                DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable3(IsTestNet(this), sTable);
                if (sMode == "edit")
                {
                    dt = dt.FilterDataTable("id='" + sID + "'");
                    if (dt.Rows.Count < 1)
                    {
                        Session["stack"] = UICommon.Toast("Failed", "Your Edit Failed!");
                        return;
                    }
                }
                // End of Edit

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
                          if (sMode != "add")
                          {
                            object oOrigValue1 = Session["input_" + sColName];
                            if (oOrigValue1 != null)
                            {
                                BiblePayCommon.EntityCommon.SetEntityValue(o, sColName, oOrigValue1);
                            }
                           }
                        }
                }
                

                if (e.EventAction == "Delete_Click")
                {
                    BiblePayCommon.EntityCommon.SetEntityValue(o, "deleted", "1");
                }
                if (sMode == "add")
                {
                    o.id = null;   //New ID
                    o.UserID = gUser(this).id;

                }
              

                DACResult r = InsertIntoTable(this, IsTestNet(this), o, gUser(this));

                if (!r.fError())
                {
                    Session["stack"] = UICommon.Toast("Saved", "Your " + sTable + " has been " + sMode + "(ed)!");
                }
                else
                {
                    BiblePayCommonNET.UICommonNET.MsgModal(this, "Error", r.Error, 400, 200);
                }
            }
        }

        protected string GetFormView()
        {
            string sTable = Request.QueryString["table"] ?? "";
            string sID = Request.QueryString["id"] ?? "";
            string sMode = Request.QueryString["action"] ?? "";
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable3(IsTestNet(this), sTable);
            
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

            string sNarrative = "";
            if (sTable == "VideoRequest" && sMode == "add")
            {
                sNarrative = "<font color=red><ul><li>Do not submit copyrighted videos - they must be public domain (like rapture dreams etc) or they must adhere to the Creative Commons license."
                    + "<li>Do not post any illegal, questionable, or illicit material."
                    + "<li>The video URL must be the actual playable URL (copy it from the play screen) - not the URL from the search list. "
                    + "<li>For bitchute and rumble, the URL must be the Embed Iframe URL" + "</ul></font><br><br>";
                
            }
            string sDisplayableID = sID == "" ? "New" : "Object ID " + sID;
            string sDisplayableMode = sMode == "add" ? "Add" : sMode;
            string html = "<fieldset><legend>" + sTable + " - " + sDisplayableID + " - " + sDisplayableMode + ":</legend>";
            html += sNarrative;

            html += "<table width=90%>";
            
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                string sColName = dt.Columns[i].ColumnName;
                string sOrigValue = dt.GetColValue(0, dt.Columns[i].ColumnName);
                sOrigValue = BiblePayCommon.Encryption.CleanseXSS(sOrigValue);

                // object oOrigValue = dt.Rows[0][i];
                if (sMode == "add")
                {
                    sOrigValue = "";
                }
                bool fRestricted = BiblePayCommon.EntityCommon.IsRestrictedColumn(sColName);
                // if (System.Diagnostics.Debugger.IsAttached)
                bool fHidden = BiblePayCommon.EntityCommon.IsHidden(sTable, sColName);
                bool fReadonly = BiblePayCommon.EntityCommon.IsReadOnly(sTable, sColName);
                string sReadOnly = (sMode == "view" || fReadonly) ? " readonly " : "";
                if (sTable == "news1" && sColName == "URL")
                    sReadOnly = "";

                if (sTable == "VideoRequest" && sColName == "URL")
                    sReadOnly = "";

                string sValueControl = "<input name='input_" + sColName + "'" + sReadOnly + " class='pc90' value = '" + sOrigValue + "' />";

                if (sOrigValue.Length > 255 || sColName.ToLower() == "body" || sColName.ToLower() == "transcript")
                {
                    sValueControl = "<textarea rows='10' cols='80' name='input_" + sColName + "' class='pc90' " + sReadOnly + ">" + sOrigValue + "</textarea>";
                }
                // If this is a dropdown...
                if (sTable == "video1" && sColName.ToLower() == "category")
                {
                    sValueControl = UICommon.GetVideoCategories("input_category", sOrigValue);
                }
                
                string sRow = "<tr><td width=25%><span>" + sColName + ":</span><td>" + sValueControl + "</tr>\r\n";
                if (!fRestricted && !fHidden)
                {
                    html += sRow;
                }
                if (fRestricted || fHidden)
                {
                    // Store in session
                    Session["input_" + sColName] = sOrigValue;
                }
            }
            html += "</table>";
            // Submit an edit
            string sButton = UICommon.GetStandardButton(sID, "Save", "Save", "Save");
            if (false && System.Diagnostics.Debugger.IsAttached)
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