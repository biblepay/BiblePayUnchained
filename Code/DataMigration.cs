using BiblePayCommonNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using static BiblePayCommon.Common;
using static BiblePayCommon.EntityCommon;

namespace Unchained.Code
{
    public static class DataMigration
    {

        public static string SchemaCreator(bool fTestNet)
        {
            try
            {
                List<string> lEntities = BiblePayCommon.EntityCommon.GetBiblePayEntities();
                for (int i = 0; i < lEntities.Count; i++)
                {
                    BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
                    BiblePayCommon.IBBPObject o = (BiblePayCommon.IBBPObject)BiblePayCommon.EntityCommon.GetInstance("BiblePayCommon.Entity+" + lEntities[i]);

                    PropertyInfo[] properties = o.GetType().GetProperties(flags);
                    string sCommand = "CREATE TABLE " + lEntities[i] + " (";
                    foreach (PropertyInfo property in properties)
                    {
                        string sType = "";
                        if (property.PropertyType.Name == "String")
                        {
                            int iLen = 256;
                            if (property.Name.ToLower() == "data" || property.Name.ToLower() == "transcript")
                            {
                                sType = "MEDIUMTEXT";
                            }
                            else if (property.Name.ToLower() == "body" || property.Name.ToLower() == "telegramlinkdescription" || property.Name.ToLower() == "publictext"
                                || property.Name.ToLower() == "notes" || property.Name.ToLower() == "religioustext")
                            {
                                iLen = 16383;
                                sType = "TEXT";

                            }
                            else if (property.Name.ToLower() == "url")
                            {
                                iLen = 512;
                                sType = "varchar(" + iLen.ToString() + ")";

                            }
                            else if (property.Name.ToLower() == "urldescription")
                            {
                                iLen = 2000;
                                sType = "varchar(2000)";
                            }
                            else
                            {
                                sType = "varchar(" + iLen.ToString() + ")";
                            }

                        }
                        else if (property.PropertyType.Name == "Int32")
                        {
                            sType = "INT";
                        }
                        else if (property.PropertyType.Name == "Int64")
                        {
                            sType = "BIGINT";
                        }
                        else if (property.PropertyType.Name == "Double")
                        {
                            sType = "DOUBLE";
                        }
                        else if (property.PropertyType.Name == "DateTime")
                        {
                            sType = "DATETIME";
                        }
                        else if (property.PropertyType.Name == "GUID")
                        {
                            sType = "char(36)";
                        }
                        else if (property.PropertyType.Name == "Boolean")
                        {
                            sType = "tinyint";
                        }
                        else
                        {
                            string sUnknown = "";
                        }
                        if (property.Name.ToLower() == "id")
                        {
                            sType += " primary key";
                        }
                        string sTheFieldName = property.Name;
                        if (sTheFieldName.ToLower() == "read")
                        {
                            sTheFieldName = "read1";
                        }

                        string sCol = sTheFieldName + " " + sType + ",";
                        sCommand += sCol;
                        //AddProperty(ECO, property.Name, property.GetValue(obj));
                    }
                    sCommand = BiblePayCommon.Common.Mid(sCommand, 0, sCommand.Length - 1);
                    sCommand += ");";
                    // Create the insert statements
                    IList<dynamic> list1 = BiblePayDLL.Sidechain.GetChainObjects<dynamic>(fTestNet, lEntities[i], null, SERVICE_TYPE.PUBLIC_CHAIN);
                    string sFields = "";
                    foreach (PropertyInfo property in properties)
                    {
                        string sTheFieldName = property.Name;
                        if (sTheFieldName.ToLower() == "read")
                        {
                            sTheFieldName = "read1";
                        }
                        sFields += sTheFieldName + ",";
                    }
                    sFields = BiblePayCommon.Common.Mid(sFields, 0, sFields.Length - 1);
                    StringBuilder sbInserts = new StringBuilder();
                    sbInserts.Append("START TRANSACTION;\n");


                    for (int j = 0; j < list1.Count; j++)
                    {
                        string s = "Insert into " + lEntities[i] + " (" + sFields + ") values (";

                        string sValues = "";
                        BiblePayCommon.IBBPObject o1 = ExpandoToStronglyCastObject(list1[j], lEntities[i]);

                        try
                        {
                            string sID = "";

                            foreach (PropertyInfo property in properties)
                            {

                                object value = BiblePayCommon.EntityCommon.GetEntityValue(o1, property.Name);
                                if (property.Name.ToLower() == "marketable" || property.Name.ToLower() == "fdeleted" || property.Name.ToLower() == "found")
                                {
                                    string sFullValue = value.ToString() == "true" ? "1" : "0";
                                    sValues += sFullValue + ",";
                                }
                                else if (value == null)
                                {
                                    sValues += "null,";
                                }
                                else
                                {
                                    string sValue = value.ToString();
                                    sValue = sValue.Replace("'", "''");
                                    if (sValue.Length > 16384 && property.Name.ToLower() == "transcript")
                                    {
                                        sValue = Mid(sValue, 0, 16355) + "...";

                                    }
                                    string sFullValue = "'" + sValue + "'";
                                    sValues += sFullValue + ",";
                                }

                                if (property.Name.ToLower() == "id")
                                    sID = value.ToNonNullString();

                            }
                            sValues = Mid(sValues, 0, sValues.Length - 1);
                            s += sValues + ");\n";
                            if (sID != "")
                            {
                                sbInserts.Append(s);
                            }
                        }
                        catch (Exception ex1)
                        {
                            string s99 = ex1.Message;
                        }
                    }

                    sbInserts.Append("COMMIT;\n");

                    // Create the schema
                    string sCommandDrop = "DROP TABLE " + lEntities[i] + ";\n";
                    MySQLData.ExecuteNonQuery(sCommandDrop.ToString());
                    MySQLData.ExecuteNonQuery(sCommand.ToString());

                    if (list1.Count > 0)
                    {
                        // Make a database for TestNet, and one for MainNet
                        throw new Exception("Email Address must be decrypted");
                        string sPath = "c:\\migration_" + lEntities[i] + ".txt";
                        System.IO.File.WriteAllText(sPath, sbInserts.ToString());
                        // Migrate the Data
                        MySQLData.ExecuteNonQuery(sbInserts.ToString());
                    }

                    string sTest1 = "";

                }
            }
            catch (Exception ex)
            {
                string mytester = "";
            }
            return "";

        }

    }
}