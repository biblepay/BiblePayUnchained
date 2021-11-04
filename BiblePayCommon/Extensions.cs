using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BiblePayCommon;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using static BiblePayCommon.Common;


namespace BiblePayCommon
{
        public static class DataTableExtensions
        {

        public static BBPDataTable OrderBy0(this BBPDataTable table, string sOrderBy)
        {
            try
            {
                DataRow[] dr1 = table.Select("", sOrderBy);
                BBPDataTable dtNew = new BBPDataTable();
                if (dr1.Length > 0)
                {
                    dtNew.GetData = table.GetData.Clone();
                    foreach (DataRow temp in dr1)
                    {
                        dtNew.ImportRow(temp);
                    }
                }

                return dtNew;
            }
            catch (Exception)
            {
                return table;
            }
        }

        public static BBPDataTable FilterBBPDataTable(this BBPDataTable table, string sql, bool fReturnEmpty = false)
        {
            try
            {
                DataRow[] dr1 = table.Select(sql);
                BBPDataTable dtNew = new BBPDataTable();
                if (dr1.Length > 0)
                {
                    dtNew.GetData = table.GetData.Clone();
                    foreach (DataRow temp in dr1)
                    {
                        dtNew.ImportRow(temp);
                    }
                }
                return dtNew;
            }
            catch (Exception)
            {
                BBPDataTable dt1 = new BBPDataTable();
                return dt1;
            }
        }

        public static DataTable FilterDataTable(this DataTable table, string sql)
        {
            try
            {
                DataRow[] dr1 = table.Select(sql);
                DataTable dtNew = new DataTable();
                if (dr1.Length > 0)
                {
                    dtNew = table.Clone();

                    foreach (DataRow temp in dr1)
                    {
                        dtNew.ImportRow(temp);
                    }
                }
                return dtNew;
            }
            catch (Exception ex)
            {
                DataTable dt1 = new DataTable();
                return dt1;
            }
        }

        public static DataTable SortDataTable(this DataTable table, string sql)
        {
            try
            {
                table.DefaultView.Sort = sql;
                table.DefaultView.ApplyDefaultSort = true;
                return table;
            }
            catch(Exception ex)
            {
                return table;
            }

        }

        public static DataTable FilterAndSort(this DataTable table, string sFilter, string sSort)
        {
            try
            {
                DataRow[] dr1 = table.Select(sFilter, sSort);
                DataTable dtNew = new DataTable();
                if (dr1.Length > 0)
                {
                    dtNew = table.Clone();

                    foreach (DataRow temp in dr1)
                    {
                        dtNew.ImportRow(temp);
                    }
                }
                return dtNew;
            }
            catch (Exception)
            {
                DataTable dt1 = new DataTable();
                return dt1;
            }
        }


        public static string GetColValue(this DataTable table, string colName)
        {
            if (table.Rows.Count < 1)
                return String.Empty;
            if (!table.Columns.Contains(colName))
                return String.Empty;
            return table.Rows[0][colName].ToString();
        }

        public static double GetColDouble(this DataTable table, string colName)
        {
            return BiblePayCommon.Common.GetDouble(table.Rows[0][colName].ToString());
        }

        public static int GetColInt(this DataTable table, string colName)
        {
            return (int)BiblePayCommon.Common.GetDouble(table.Rows[0][colName].ToString());
        }

        public static string GetColValue(this DataTable table, int iRow, string colName)
        {
            if (!table.Columns.Contains(colName))
            {
                return "";
            }
            return table.Rows[iRow][colName].ToString();
        }

        public static double GetColDouble(this DataRowView dr, string colName)
        {
            if (dr == null)
                return 0;


            double nOut = BiblePayCommon.Common.GetDouble(dr[colName].ToString());
            return nOut;
        }


        public static double GetColDouble(this DataTable table, int iRow, string colName)
        {
            if (table.Rows.Count == 0)
                return 0;


            double nOut = BiblePayCommon.Common.GetDouble(table.Rows[iRow][colName].ToString());
            return nOut;
        }
        public static DateTime GetColDateTime(this DataTable table, int iRow, string sColName)
        {
            DateTime dt = new DateTime();

            if (table.Rows.Count == 0)
                return dt;

            double nOut = BiblePayCommon.Common.GetDouble(table.Rows[iRow][sColName].ToString());

            dt = BiblePayCommon.Encryption.UnixTimeStampToDateTime(nOut);
            return dt;
        }

    }


    public class BBPDataTable : DataTable
    {
        public string BestBlockHash;
        public int Height;
        public string SnapshotHash;
    
        public DataTable GetData
        {
            get
            {
                return this;
            }
            set
            {
                this.Clear();
                this.Merge(value);
            }
        }
        public BBPDataTable()
        {

        }
    }
}
