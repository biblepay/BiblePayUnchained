using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BiblePayCommon.DataTableExtensions;

namespace Service
{
    public static class VideoServices
    {

        public static string run_cmd(string sFullWorkingDirectory, string sFileName, string args)
        {
            string result = " ";
            try
            {
                ProcessStartInfo start = new ProcessStartInfo();
                start.FileName = sFileName;
                start.Arguments = string.Format("{0}", args);
                start.UseShellExecute = false;
                start.CreateNoWindow = false;
                start.RedirectStandardOutput = true;
                start.WorkingDirectory = sFullWorkingDirectory;
                
                using (Process process = Process.Start(start))
                {
                    using (StreamReader reader = process.StandardOutput)
                    {
                        result = reader.ReadToEnd();
                    }
                }

                BiblePayCommon.Common.Log2("run_cmd result " + result);
                return result;
            }
            catch (Exception ex)
            {
                BiblePayCommon.Common.Log2("Run cmd failed " + ex.Message + " " + ex.StackTrace);
            }
            return result;
        }



        public static void SpawnVideoProcessorFromURLToMp4(string sURL, string sFullWorkingDirectory)
        {
            try
            {
                string vidArgs = sURL + " -w -f mp4 --verbose --write-description --no-check-certificate";
                string res = run_cmd(sFullWorkingDirectory, sFullWorkingDirectory + "\\youtube-dl.exe", vidArgs);
            }
            catch (Exception ex)
            {
                BiblePayCommon.Common.Log2("SVPFUM::" + ex.Message);
            }
        }

        public static string GetPathFromTube(string sURL)
        {
            string sFolderPath = "c:\\inetpub\\wwwroot\\videos\\";
            sURL += "&";
            string sTube = BiblePayCommon.Common.ExtractXML(sURL, "v=", "&").ToString();
            string[] files = System.IO.Directory.GetFiles(sFolderPath, "*.mp4");
            for (int i = 0; i < files.Length; i++)
            {
                string sPath = files[i];
                if (sPath.Contains(sTube))
                    return sPath;
            }
            return "";
        }
        public static BiblePayCommon.Common.User CoerceUser(bool fTestNet)
        {
            // This allows the server to perform certain actions as Administrator (such as saving a User record the first time)...
            // But note; this function does not give the server owner unlimited permissions to alter chain data
            // It only allows the server to Add to a chain, or EDIT its own authorized chain data.
            // A server cannot edit another federated servers chain data, but a server may add a new record or edit its own data.
            BiblePayCommon.Common.User u = new BiblePayCommon.Common.User();
            u.BiblePayAddress = GetFundingAddress(fTestNet);
            u.UserName = "Administrator";
            u.FirstName = "Administrator";
            u.LastName = "";
            u.id = fTestNet ? "fbf35a7907a1958bf3d443bd178f729deefb0c8d2fea066680a49638d422bf4e" : "c1f4d7c2fb3235e4ae4823659835559f0012a48370826f95ad90a3f51a4baa21";
            u.RSAKey = "239FF17611FF397FF6114FF4186FF466FF10200FF3265FF53079FF2572FF82490FF34369FF3338FF7712FF5814FF1284FF162FF4292FF21914FF342FF10098FF469FF130FF10538FF2420FF50032FF29490FF83115FF866FF4246FF5301FF749FF1584FF1439FF2228FF19793FF";

            u.SignatureTimestamp = (int)BiblePayCommon.Common.UnixTimestampUTC();
            u.Signature = BiblePayDLL.Sidechain.SignMessage(fTestNet, GetFundingKey(fTestNet), u.SignatureTimestamp.ToString());
            return u;
        }

        public static string GetFundingAddress(bool fTestNet)
        {
            string suffix = fTestNet ? "testnet" : "mainnet";
            string sKey = "FundingAddress_" + suffix;
            return BiblePayCommon.Encryption.CommonConfig(sKey);
        }

        public static string GetFundingKey(bool fTestNet)
        {
            string suffix = fTestNet ? "testnet" : "mainnet";
            string sKey = "FundingKey_" + suffix;
            return BiblePayCommon.Encryption.CommonConfig(sKey);
        }

        public static string InsertNewVideo(BiblePayCommon.Common.User u, bool fTestNet, string sPath, string sCategory, string sTitle, string sBody)
        {
            BiblePayCommon.Entity.object1 o = new BiblePayCommon.Entity.object1();
            o.FileName = sPath;
            o.Title = sTitle;
            o.Subject = sTitle;
            o.Body = sBody;
            o.Attachment = 0;
            o.Category = sCategory;
            BiblePayDLL.Sidechain.UploadIntoDSQL_Background(fTestNet, ref o, u);
            return o.URL;
        }

        public static string CleanseHeading(string sMyHeading)
        {
            int iPos = 0;
            for (int i = 0; i < sMyHeading.Length; i++)
            {
                if (sMyHeading.Substring(i, 1) == "-")
                    iPos = i;
            }
            if (iPos > 1)
            {
                string sOut = sMyHeading.Substring(0, iPos - 1);
                return sOut;
            }
            return sMyHeading;
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

        public static string TrimEnd(string sTitle)
        {
            string[] vData = sTitle.Split(new string[] { "-" }, StringSplitOptions.None);
            if (vData.Length < 1)
                return sTitle;

            string sTrimming = vData[vData.Length - 1];
            string sNewData = sTitle.Replace("-" + sTrimming, "");
            return sNewData;

        }

        public static BiblePayCommon.Common.User RetrieveUser(bool fTestNet, string sFilter)
        {
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable3(fTestNet, "user1");
            dt = dt.FilterDataTable(sFilter);
            Object o = new BiblePayCommon.Common.User();
            if (dt.Rows.Count < 1)
                return (BiblePayCommon.Common.User)o;
            ExpandoObject oExpandoUser = BiblePayCommon.Common.CastDataTableRowToExpando(dt.Rows[0], "user1");
            BiblePayCommon.EntityCommon.CastExpandoToBiblePayObject(oExpandoUser, ref o);
            return (BiblePayCommon.Common.User)o;
        }

        public static void ConvertVideosFromRequestVideo(bool fTestNet)
        {
            try
            {
                string sVideoPath = BiblePayCommon.Encryption.CommonConfig("convertvideospath");
                if (sVideoPath == "")
                    return;

                DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable3(fTestNet, "VideoRequest", true);
                dt = dt.FilterDataTable("processTime=0");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    BiblePayCommon.Entity.VideoRequest vr = (BiblePayCommon.Entity.VideoRequest)BiblePayCommon.EntityCommon.TableRowToStronglyCastObject(dt, "VideoRequest", i);
                    string url = dt.Rows[i]["url"].ToString();
                    string sID = dt.Rows[i]["id"].ToString();
                    string sUserID = dt.Rows[i]["userid"].ToString();
                    string sParentID = vr.ParentID;

                    if (url.Contains("http"))
                    {
                        // Convert this particular youtube URL into an mp4
                        BiblePayCommon.Common.Log2("VideoServices::Processing " + url);
                        Console.WriteLine("Processing video " + url);
                        SpawnVideoProcessorFromURLToMp4(url, sVideoPath);

                        string sPath = GetPathFromTube(url);
                        // Convert the path to hash
                        string sNewFileName = Guid.NewGuid().ToString() + ".mp4";

                        if (System.IO.File.Exists(sPath))
                        {
                            BiblePayCommon.Common.Log2("VideoServices::Uploading " + sPath);

                            System.IO.FileInfo fi = new FileInfo(sPath);
                            string sTitle = BiblePayCommon.Common.Mid(fi.Name, 0, 200);
                            sTitle = TrimEnd(sTitle);

                            string sBody = BiblePayCommon.Common.Mid(GetNotes(sPath), 0, 4000);

                            if (sTitle != "")
                            {
                                string sCategory = "Religion";
                                BiblePayCommon.Common.User u = RetrieveUser(fTestNet, "id='" + sUserID + "'");
                                Console.WriteLine("Uploading video "+ sTitle + " from " + u.FullUserName() + " in domain " + u.domain);

                                vr.URL2 = InsertNewVideo(u, fTestNet, sPath, sCategory, sTitle, sBody);
                                vr.Processed = "FILLED";
                                vr.ProcessTime = (int)BiblePayCommon.Common.UnixTimestampUTC();
                                BiblePayDLL.Sidechain.InsertIntoDSQL(fTestNet, vr, CoerceUser(fTestNet));
                                // Delete the temporary file
                                //                                System.IO.File.Delete(sPath);

                            }
                        }
                        else
                        {
                            BiblePayCommon.Common.Log2("VideoServices::Unable to find file " + sPath);
                            vr.ProcessTime = (int)BiblePayCommon.Common.UnixTimestampUTC();
                            vr.Processed = "FAILED";
                            BiblePayDLL.Sidechain.InsertIntoDSQL(fTestNet, vr, CoerceUser(fTestNet));

                        }

                    }

                }

            }
            catch (Exception ex)
            {
                BiblePayCommon.Common.Log2("Error::232::" + ex.Message);
            }

        }

        }
    }
