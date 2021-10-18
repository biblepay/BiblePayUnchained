using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblePayCommonNET
{
    public static class DataTableExtensions
    {

        public static DataTable OrderBy(this DataTable table, string sOrderBy)
        {
            try
            {
                DataRow[] dr1 = table.Select("", sOrderBy);
                DataTable dtNew = new DataTable();
                if (dr1.Length > 0)
                {
                    dtNew = table.Clone();
                    foreach (DataRow temp in dr1)
                    {
                        dtNew.ImportRow(temp);
                    }
                }
                dtNew.AcceptChanges();

                return dtNew;
            }
            catch (Exception)
            {
                return table;
            }
        }

    }
    public static class StringExtension
    {
        public static DateTime FromUnixToDateTime(this long n)
        {
            DateTime dt = BiblePayCommon.Common.ConvertFromUnixTimestamp(n);
            return dt;
        }
        public static bool IsNullOrWhitespace(this string s)
        {
            return String.IsNullOrWhiteSpace(s);
        }

        public static bool IsEmpty(this string s)
        {
            if (s == null) return true;
            if (s == "") return true;
            if (s.Trim() == "") return true;
            return false;
        }

        public static int ToInt(this string s)
        {
            double n = BiblePayCommon.Common.GetDouble(s);
            return (int)n;
        }


        public static double ToDouble(this string s)
        {
            double n = BiblePayCommon.Common.GetDouble(s);
            return n;
        }

        public static List<string> SplitCsv(this string csvList, bool nullOrWhitespaceInputReturnsNull = false)
        {
            if (string.IsNullOrWhiteSpace(csvList))
                return nullOrWhitespaceInputReturnsNull ? null : new List<string>();

            return csvList
                .TrimEnd(',')
                .Split(',')
                .AsEnumerable<string>()
                .Select(s => s.Trim())
                .ToList();
        }
        public static string Left(this string o, int oLength)
        {
            if (o.Length < oLength)
            {
                return o;
            }
            return o.Substring(0, oLength);
        }
        public static bool IsNullOrEmpty(this string str)
        {
            if (str == null || str == String.Empty)
                return true;
            return false;
        }

        public static string TrimAndReduce(this string str)
        {
            return str.Trim();
        }

        public static string ToNonNullString(this object o)
        {
            if (o == null)
                return String.Empty;
            return o.ToString();
        }

        public static string[] Split(this string str, string sDelimiter)
        {
            string[] vSplitData = str.Split(new string[] { sDelimiter }, StringSplitOptions.None);
            return vSplitData;
        }

        public static double ToDouble(this object o)
        {
            try
            {
                if (o == null)
                    return 0;
                if (o.ToString() == string.Empty)
                    return 0;
                double d = Convert.ToDouble(o.ToString());
                return d;
            }
            catch (Exception)
            {
                return 0;
            }
        }

    }

}
