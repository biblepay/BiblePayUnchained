using System;
using System.Data;
using System.Data.SqlClient;
using static BiblePayDLL.Shared;
using static Unchained.Common;
using static BiblePayCommon.Common;
using System.Web.UI;
using BiblePayDLL;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using Dapper;

namespace Unchained
{
    public static class DataOps
    {
        public static void FilterDataTable(ref DataTable dt, string sSQL)
        {
            try
            {
                dt = dt.Select(sSQL).CopyToDataTable();
            }
            catch (Exception)
            {
                DataTable dt1 = new DataTable();
                dt = dt1;
            }
        }

        public static BiblePayCommon.Common.DACResult SpendMoney(bool fTestnet, Page p, double nAmount, string sSpendAddress, string sSpendPrivKey, string sXML)
        {
            p.Session["balance"] = null;
            BiblePayCommon.Common.DACResult r = BiblePayDLL.Sidechain.CreateFundingTransaction(fTestnet, nAmount, sSpendAddress, sSpendPrivKey, sXML, true);
            return r;
        }
       
        public static DACResult InsertIntoTable(Page p, bool fTestNet, BiblePayCommon.IBBPObject o, User u=new User())
        {
            // string sEntityName = BiblePayCommon.EntityCommon.GetEntityName(fTestNet, o);
            DACResult r = BiblePayDLL.Sidechain.InsertIntoDSQL(fTestNet, o, u);
            return r;
        }



        public static void InsertIntoTable_Background(bool fTestNet, BiblePayCommon.IBBPObject o, User u)
        {
            BiblePayDLL.Sidechain.InsertIntoDSQL_Background(fTestNet, o, u);
            
        }

        public static double GetTotalFrom(string userid, string table)
        {
            string sql = "Select sum(amount) amount from " + table + " where userid=@userid and amount is not null";

            if (userid == "")
            {
                sql = "Select sum(amount) amount from " + table + " where amount is not null";
            }

            SqlCommand command = new SqlCommand(sql);
            command.Parameters.AddWithValue("@userid", userid);
            double nBalance = gData.GetScalarDouble(command, "amount");
            return nBalance;
        }

        public static string GetNotes(string sPath)
        {
            string sNotesPath = sPath.Replace(".mp4", ".description");
            if (!System.IO.File.Exists(sNotesPath))
                return "";

            System.IO.StreamReader file = new System.IO.StreamReader(sNotesPath);
            string data = file.ReadToEnd();
            data = data.Replace("'", "");
            data = data.Replace("`", "");
            data = data.Replace("\"", "");
            if (data.Length > 7999)
                data = data.Substring(0, 7999);

            return data;
        }
    }


    public class MySQLData
    {
        private static string MySqlConn()
        {
            string connStr = "server=" + Config("mysqlhost") + ";user=" + Config("mysqluser") + ";database="
                + Config("mysqldatabase") + ";port=3306;password=" + Config("mysqlpassword");
            return connStr;
        }

        public static List<T> RunSelect<T>(string query)
        {
            dynamic m = new List<T>();
            try 
            {
                MySqlConnection conn = new MySqlConnection(MySqlConn());
                conn.Open();
                m = conn.Query<T>(query).AsList<T>();
                conn.Close();
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
            return m;
        }
        public static string GetScalarString(string sql, int ordinal)
        {
            try
            {
                MySqlDataReader dr = MySQLData.GetMySqlDataReader(sql);
                while (dr.Read())
                {
                    return dr[ordinal].ToString();
                }
            }
            catch (Exception)
            {

            }
            return "";
        }

        public static MySqlDataReader GetMySqlDataReader(string sql)
        {
            MySqlConnection conn = new MySqlConnection(MySqlConn());
            MySqlDataReader rdr = null;
            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                rdr = cmd.ExecuteReader();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return rdr;
        }

        public static bool ExecuteNonQuery(string sql)
        {
            MySqlConnection conn = new MySqlConnection(MySqlConn());
            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                conn.Close();
                return true;
            }
            catch (Exception ex)
            {
                Log("ExecuteNonQuery[mysql]::" + ex.Message);
                return false;
            }

        }
    }



    public class Data
    {
        public enum SecurityType
        {
            REQ_SA = 10,
            READ_ONLY = 0,
            REPLICATOR = 11,
            UNKNOWN = 99
        }
        
        private string RemoteHostName = "";
        private SecurityType _SecurityType = SecurityType.UNKNOWN;

        public string sSQLConn()
        {
            string sCS = "Database=" + Config("UnchainedDatabaseName") + ";MultipleActiveResultSets=true;Connection Timeout=7; ";

            if (RemoteHostName != "" && RemoteHostName != null)
            {
                sCS += "Server=" + RemoteHostName + ";";
            }
            else
            {
                sCS += "Server=" + Config("UnchainedDatabaseHost") + ";";
            }

            if (_SecurityType == SecurityType.REQ_SA)
            {
                sCS += "Uid=" + Config("UnchainedDatabaseUser")
                + ";pwd=" + Config("UnchainedDatabasePassword");
            }
            else
            {
                throw new Exception("Unknown Security Type");
            }
            return sCS;
        }
        public Data(SecurityType sa, string _RemoteHostName = "")
        {
            // Constructor goes here; since we use SQL Server connection pooling, dont create connection here, for best practices create connection at usage point and destroy connection after Using goes out of scope - see GetDataTable
            // This keeps the pool->databaseserver connection count < 10.  
            _SecurityType = sa;
            RemoteHostName = _RemoteHostName;
        }

        public SqlConnection GetSqlConn()
        {
            SqlConnection con = new SqlConnection(sSQLConn());
            return con;
        }

        public void ExecWithThrow(string sql, bool bLogErr, bool bLog = true)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(sSQLConn()))
                {
                    con.Open();
                    SqlCommand myCommand = new SqlCommand(sql, con);
                    myCommand.ExecuteNonQuery();
                }

            }
            catch (Exception ex)
            {
                if (bLogErr) Log("EXECWithThrow: " + sql + "," + ex.Message);
                throw (ex);
            }
        }


        public void ExecCmd(SqlCommand cmd, bool bLog = true, bool bLogError = true, bool bThrow = false)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(sSQLConn()))
                {
                    cmd.Connection = con;
                    con.Open();
                    cmd.ExecuteNonQuery();
                }

            }
            catch (Exception ex)
            {
                if (bThrow)
                    throw (ex);
                
                if (bLogError)
                    Log(" EXEC: " + cmd.CommandText + "," + ex.Message);
            }

        }
        public void Exec(string sql, bool bLog = true, bool bLogError = true)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(sSQLConn()))
                {
                    con.Open();
                    SqlCommand myCommand = new SqlCommand(sql, con);
                 
                    myCommand.ExecuteNonQuery();
                }

            }
            catch (Exception ex)
            {

                if (bLogError)
                    Log(" EXEC: " + sql + "," + ex.Message);
            }

        }
        public void ExecWithTimeout(string sql, double lTimeout)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(sSQLConn()))
                {
                    con.Open();

                    SqlCommand myCommand = new SqlCommand(sql, con);
                    myCommand.CommandTimeout = (int)lTimeout;
                    myCommand.ExecuteNonQuery();
                }

            }
            catch (Exception ex)
            {
                Log(" EXECWithTimeout: " + sql + "," + ex.Message);
            }

        }

        public DataTable GetDataTable(SqlCommand sqlCommand, bool bLog = true, bool bThrow = false)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(sSQLConn()))
                {
                    int nConnectionTimeout = con.ConnectionTimeout;
                    con.Open();
                    sqlCommand.Connection = con;
                    SqlDataAdapter a = new SqlDataAdapter(sqlCommand);
                   
                    DataTable t = new DataTable();
                    a.Fill(t);
                    return t;
                }
            }
            catch (Exception ex)
            {
                Log("GetDataTableViaSqlCommand:" + sqlCommand.CommandText + "," + ex.Message);
                if (bThrow) throw(ex);
            }
            DataTable dt = new DataTable();
            return dt;
        }


        public DataTable GetDataTable(string sql, bool bLog = true)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(sSQLConn()))
                {
                    int nConnectionTimeout = con.ConnectionTimeout;
                    con.Open();
                    SqlDataAdapter a = new SqlDataAdapter(sql, con);
                    DataTable t = new DataTable();
                    a.Fill(t);
                    return t;
                }
            }
            catch (Exception ex)
            {
                Log("GetDataTable:" + sql + "," + ex.Message);
            }
            DataTable dt = new DataTable();
            return dt;
        }


        public double GetScalarDouble(SqlCommand command, object vCol, bool bLog = true)
        {
            DataTable dt1 = GetDataTable(command, bLog);
            try
            {
                if (dt1.Rows.Count > 0)
                {
                    object oOut = null;
                    if (vCol.GetType().ToString() == "System.String")
                    {
                        oOut = dt1.Rows[0][vCol.ToString()];
                    }
                    else
                    {
                        oOut = dt1.Rows[0][Convert.ToInt32(vCol)];
                    }
                    double dOut = GetDouble(oOut.ToString());

                    return dOut;
                }
            }
            catch (Exception)
            {
            }
            return 0;
        }

        public double GetScalarAge(string sql, object vCol, bool bLog = true)
        {
            DataTable dt1 = GetDataTable(sql, bLog);
            try
            {
                if (dt1.Rows.Count > 0)
                {
                    object oOut = null;
                    if (vCol.GetType().ToString() == "System.String")
                    {
                        oOut = dt1.Rows[0][vCol.ToString()];
                    }
                    else
                    {
                        oOut = dt1.Rows[0][Convert.ToInt32(vCol)];
                    }
                    DateTime d1 = Convert.ToDateTime(oOut);
                    TimeSpan vAge = DateTime.Now - d1;
                    return vAge.TotalSeconds;

                }
            }
            catch (Exception)
            {
            }
            return 0;
        }


        public double GetScalarDouble(string sql, object vCol, bool bLog = true)
        {
            DataTable dt1 = GetDataTable(sql, bLog);
            try
            {
                if (dt1.Rows.Count > 0)
                {
                    object oOut = null;
                    if (vCol.GetType().ToString() == "System.String")
                    {
                        oOut = dt1.Rows[0][vCol.ToString()];
                    }
                    else
                    {
                        oOut = dt1.Rows[0][Convert.ToInt32(vCol)];
                    }
                    double dOut = GetDouble(oOut.ToString());

                    return dOut;
                }
            }
            catch (Exception)
            {
            }
            return 0;
        }


        public DataRow GetScalarRow(SqlCommand c)
        {
            DataRow dRow;

            DataTable dt1 = GetDataTable(c);
            try
            {
                if (dt1.Rows.Count > 0)
                {
                    dRow = dt1.Rows[0];
                    return dRow;
                }
            }
            catch (Exception)
            {
            }
            return null;
        }
        public DataRow GetScalarRow(string sql)
        {
            DataRow dRow;

            DataTable dt1 = GetDataTable(sql);
            try
            {
                if (dt1.Rows.Count > 0)
                {
                    dRow = dt1.Rows[0];
                    return dRow;
                }
            }
            catch (Exception)
            {
            }
            return null;
        }

        public string GetScalarString(SqlCommand sqlCommand, object vCol, bool bLog = true)
        {
            DataTable dt1 = GetDataTable(sqlCommand);
            try
            {
                if (dt1.Rows.Count > 0)
                {
                    object oOut = null;
                    if (vCol.GetType().ToString() == "System.String")
                    {
                        oOut = dt1.Rows[0][vCol.ToString()];
                    }
                    else
                    {
                        oOut = dt1.Rows[0][Convert.ToInt32(vCol)];
                    }
                    return oOut.ToString();
                }
            }
            catch (Exception)
            {
            }
            return "";
        }

        public string GetScalarString(string sql, object vCol, bool bLog = true)
        {
            DataTable dt1 = GetDataTable(sql);
            try
            {
                if (dt1.Rows.Count > 0)
                {
                    object oOut = null;
                    if (vCol.GetType().ToString() == "System.String")
                    {
                        oOut = dt1.Rows[0][vCol.ToString()];
                    }
                    else
                    {
                        oOut = dt1.Rows[0][Convert.ToInt32(vCol)];
                    }
                    return oOut.ToString();
                }
            }
            catch (Exception)
            {
            }
            return "";
        }

        public SqlDataReader GetDataReader(string sql)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(sSQLConn()))
                {
                    con.Open();
                    SqlDataReader myReader = default(SqlDataReader);
                    SqlCommand myCommand = new SqlCommand(sql, con);
                    myReader = myCommand.ExecuteReader();
                    return myReader;
                }
            }
            catch (Exception ex)
            {
                Log("GetDataReader:" + ex.Message + "," + sql);
            }
            SqlDataReader dr = default(SqlDataReader);
            return dr;
        }

        public string ReadFirstRow(string sql, object vCol, bool bLog = true)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(sSQLConn()))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {

                        SqlDataReader dr = cmd.ExecuteReader();
                        if (!dr.HasRows | dr.FieldCount == 0) return string.Empty;
                        while (dr.Read())
                        {
                            if (vCol is String)
                            {
                                return dr[(string)vCol].ToString();
                            }
                            else
                            {
                                return dr[(int)vCol].ToString();
                            }
                        }
                    }
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                Log("Readfirstrow: " + sql + ", " + ex.Message);
            }
            return "";
        }

    }
}
