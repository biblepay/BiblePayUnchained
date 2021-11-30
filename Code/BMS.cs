using BiblePayCommon;
using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Unchained
{
    public static class BMS
    {


        public static string GetCookie(string sKey)
        {
            try
            {
                HttpCookie _pool = HttpContext.Current.Request.Cookies["c_" + sKey];
                if (_pool != null)
                {
                    string sOut = (_pool.Value ?? string.Empty).ToString();
                    string sDeciphered = BiblePayCommon.Encryption.Base65Decode(sOut);
                    return sDeciphered;
                }
            }
            catch (Exception)
            {

            }
            return "";
        }

        public static void StoreCookie(string sKey, string sValue)
        {
            try
            {
                // if its the cookie consent cookie its ok to store it because that is changing the actual preference.
                // cookieconsent=9 when they blocked cookies, 1 when they allow cookies, 0 when they never decided yet
                double nCookieConsent = BiblePayCommon.Common.GetDouble(BMS.GetCookie("cookieconsent"));
                if (nCookieConsent == 9 && sKey != "cookieconsent")
                {
                    // Blocked
                    return;
                }

                string sEnc = BiblePayCommon.Encryption.Base65Encode(sValue);
                HttpCookie _pool = new HttpCookie("c_" + sKey);
                _pool[sKey] = sEnc;
                _pool.Expires = DateTime.Now.AddDays(7);
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.Response.Cookies.Add(_pool);
                }
                HttpContext.Current.Response.Cookies["c_" + sKey].Value = sEnc;
            }
            catch (Exception ex)
            {
                string sError = ex.Message;
                
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
     
        // We don't use ansi 92 sql on this server, just saving this in case we need to make a call to another server.
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

        public static bool SendSMSCode(string sToPhone1, int nCode)
        {
            try
            {
                var authToken = Common.Config("twilioauthtoken");
                var accountSid = Common.Config("twilioaccountsid");
                Twilio.TwilioClient.Init(accountSid, authToken);
                var client = new TwilioRestClient(accountSid, authToken);
                string sFrom = Common.Config("twiliofromphonenumber");
                if (!sToPhone1.Contains("+"))
                {
                    sToPhone1 = "+1" + sToPhone1;
                }
                var message = MessageResource.Create(
                    to: new PhoneNumber(sToPhone1),
                    from: new PhoneNumber(sFrom),
                    body: "Hello from BiblePay!  Your pin is " + nCode.ToString(), client: client);
                return true;
            }
            catch(Exception ex)
            {
                Common.Log("Error while sending SMS: " + ex.Message);
                return false;
            }

        }
    }
}
