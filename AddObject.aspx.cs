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
using static BiblePayCommonNET.StringExtension;
using static BiblePayCommon.DataTableExtensions;
using static BiblePayCommonNET.CommonNET;
using static BiblePayCommon.EntityCommon;
using MongoDB.Driver;

namespace Unchained
{
    public partial class AddObject : BBPPage
    {

        protected override void Event(BBPEvent e)
        {
            if (e.EventAction == "Save_Click")
            {
                string sID = Request.QueryString["id"] ?? "";
                string sMode = Request.QueryString["action"] ?? "";
                string sTable = Request.QueryString["table"] ?? "";
                DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable3(IsTestNet(this), sTable);
                dt = dt.FilterDataTable("id='" + sID + "'");

                if (dt.Rows.Count < 1)
                    return;

                BiblePayCommon.IBBPObject o = (BiblePayCommon.IBBPObject)BiblePayCommon.EntityCommon.GetInstance("BiblePayCommon.Entity+" + sTable);
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    string sColName = dt.Columns[i].ColumnName;
                    string sOrigValue = dt.GetColValue(0, dt.Columns[i].ColumnName);
                    if (Request.Form["input_" + sColName] != null)
                    {
                        string sNewValue = Request.Form["input_" + sColName].ToString();
                        PropertyInfo propertyInfo = o.GetType().GetProperty(sColName);
                        if (propertyInfo != null)
                        {
                            propertyInfo.SetValue(o, Convert.ChangeType(sNewValue, propertyInfo.PropertyType), null);
                        }
                    }
                }
                // Permissions checked during Insert (User must have ownership to edit a record)
                InsertIntoTable(this, IsTestNet(this), o, gUser(this));
            }
        }

        protected string GetAddObjectForm()
        {
            return "";
        }
        protected string GetFormView()
        {
            string sTable = Request.QueryString["table"] ?? "";
            string sID = Request.QueryString["id"] ?? "";
            string sMode = Request.QueryString["action"] ?? "";
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable3(IsTestNet(this), sTable);
            dt = dt.FilterDataTable("id='" + sID + "'");
            string html = "<fieldset><legend>" + sTable + " - Object " + sID + ":</legend>";
            html += "<table width=90%>";
            string sReadOnly = sMode == "edit" ? "" : "readonly";

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                string sColName = dt.Columns[i].ColumnName;
                string sOrigValue = dt.GetColValue(0, dt.Columns[i].ColumnName);
                string sValueControl = "<input name='input_" + sColName + "'" + sReadOnly + " style= 'width:90%;' value = '" + sOrigValue + "' />";
                if (sOrigValue.Length > 255)
                {
                    sValueControl = "<textarea rows='10' cols='80'>" + sOrigValue + "</textarea>";
                }
                bool fMask = false;
                if (sColName == "objecthash")
                    fMask = true;

                string sRow = "<tr><td width=25%><span>" + sColName + ":</span><td>" + sValueControl + "</tr>\r\n";
                if (!fMask)
                   html += sRow;
            }
            html += "</table>";
            // Submit an edit
            string sButton = UICommon.GetStandardButton(sID, "Save", "Save", "Save");
          
            if (sMode == "edit")
            {
                html += sButton;
            }
            html += "</fieldset>";
            return html;
        }
    }
}