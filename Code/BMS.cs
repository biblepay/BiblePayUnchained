using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

namespace Unchained
{
    public static class BMS
    {

        public static string ExecMVCCommand(string URL, int iTimeout = 30)
        {
            BiblePayClient wc = new BiblePayClient();
            try
            {
                wc.SetTimeout(iTimeout);
                string d = wc.FetchObject(URL).ToString();
                return d;
            }
            catch (Exception)
            {
                Console.WriteLine("Exec MVC Failed for " + URL);

                return "";
            }
        }

        public static byte[] StrToByteArray(string str)
        {
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            return encoding.GetBytes(str);
        }

        public static void SendBinaryFile(HttpResponse response, string filename, byte[] bytes)
        {
            response.Clear();
            response.Buffer = true;
            response.AddHeader("content-disposition", String.Format("attachment;filename={0}", filename));
            response.ContentType = "application/csv";
            response.BinaryWrite(bytes);
            response.End();
        }
        private static string Clean(string sKey, string sData, bool fRemoveColons = true)
        {
            sData = sData.Replace(sKey, "");
            sData = sData.Replace("\r\n", "");
            if (fRemoveColons)
                sData = sData.Replace(":", "");
            sData = sData.Replace(",", "");
            sData = sData.Replace("\"", "");
            return sData.Trim();
        }

        public static string PurifySQL(string value, double maxlength)
        {
            if (value == null)
                return "";
            if (value.IndexOf("'") > 0)
                value = "";
            if (value.IndexOf("--") > 0)
                value = "";
            if (value.IndexOf("/*") > 0)
                value = "";
            if (value.IndexOf("*/") > 0)
                value = "";
            if (value.ToLower().IndexOf("xp_") > 0)
                value = "";
            if (value.IndexOf(";") > 0)
                value = "";
            if (value.ToLower().IndexOf("drop ") > 0)
                value = "";
            if (value.Length > maxlength)
                value = "";
            return value;
        }
    }

    

    public class BiblePayClient : System.Net.WebClient
    {
        static bool ValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }

        private int DEFAULT_TIMEOUT = 30000;

        public object FetchObject(string URL)
        {
            object o = this.DownloadString(URL);
            return o;
        }

        public void SetTimeout(int iTimeout)
        {
            DEFAULT_TIMEOUT = iTimeout * 1000;
        }
        protected override System.Net.WebRequest GetWebRequest(Uri uri)
        {
            ServicePointManager.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback(ValidateCertificate);
            System.Net.WebRequest w = base.GetWebRequest(uri);
            w.Timeout = DEFAULT_TIMEOUT;
            return w;
        }
    }
}
