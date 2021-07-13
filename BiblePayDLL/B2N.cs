using B2Net;
using B2Net.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static BiblePayDLL.IB2N;
using static BiblePayDLL.Shared;

namespace BiblePayDLL
{
    public static class IB2N
    {
        public struct b2fileinfo
        {
            public string FilePath;
            public string FileName;
        };

        public static double PERCENT_COMPLETED = 0;

        public static void Upload(string sPath, string FileName)
        {
            b2fileinfo b = new b2fileinfo();
            b.FilePath = sPath;
            b.FileName = FileName;
            B2N n = new B2N();
            Thread t = new Thread(new ParameterizedThreadStart(n.UploadB2N));
            t.Start(b);
        }
            
    }


    class B2N
    {
        // Harvest Todo: obfuscate the blaze keys
        private B2Options Options { get; set; }
        private string applicationKeyId = "0002809827987530000000001";
        private string applicationKey = "K000SXNmuPMdakvrdg2+ECf+tuIgBuE";
        private string _BucketName = "bbp100";
        private B2Bucket _ActiveBucket = null;

        //public System.Net.Http.HttpClient httpClient = null;
        private B2Client Client = null;
        
        private void InitializeB2N()
        {
            try
            {
                Options = new B2Options()
                {
                    KeyId = applicationKeyId,
                    ApplicationKey = applicationKey
                };

                Client = new B2Client(Options);
                //var result = Client.Authorize().Result;
                //bool fAuth = string.IsNullOrEmpty(result.AuthorizationToken);

                var buckets = Client.Buckets.GetList().Result;
                foreach (B2Bucket b2Bucket in buckets)
                {
                    if (b2Bucket.BucketName == _BucketName)
                    {
                        _ActiveBucket = b2Bucket;
                    }
                }
            }
            catch (Exception ex)
            {
                IO.Log(ex.Message);
            }
        }
        
        public long GetFileSize(string sPath)
        {
            FileStream fileStream = File.OpenRead(sPath);
            long fileSize = fileStream.Length;
            fileStream.Close();
            return fileSize;
        }
        public void UploadB2N(object b2)
        {
            b2fileinfo b2i = (b2fileinfo)b2;
            long nSize = GetFileSize(Path.Combine(b2i.FilePath, b2i.FileName));
            long minPartSize = 1024 * (5 * 1024);
            if (nSize < minPartSize * 3)
            {
                UploadB2NSmall(b2i.FilePath, b2i.FileName);
            }
            else
            {
                UploadB2NLarge(b2i.FilePath, b2i.FileName);
            }
        }

        public void UploadB2NSmall(string sFilePath, string sFileName)
        {
            InitializeB2N();
            string sFullPath = Path.Combine(sFilePath, sFileName);
            var fileData = File.ReadAllBytes(sFullPath);
            string hash = Utilities.GetSHA1Hash(fileData);
            var file = Client.Files.Upload(fileData, sFileName, _ActiveBucket.BucketId).Result;
            string sURL = "https://f000.backblazeb2.com/file/bbp100/" + file.FileName;
            bool fSuccess = hash == file.ContentSHA1;
        }


        public bool UploadB2NLarge(string sFilePath, string fileName)
        {
            InitializeB2N();
            long minPartSize = 1024 * (5 * 1024);
            FileStream fileStream = File.OpenRead(Path.Combine(sFilePath, fileName));
            byte[] c = null;
            List<byte[]> parts = new List<byte[]>();
            var shas = new List<string>();
            long fileSize = fileStream.Length;
            long totalBytesParted = 0;

            while (totalBytesParted < fileSize)
            {
                var partSize = minPartSize;
                // If last part is less than min part size, get that length
                if (fileSize - totalBytesParted < minPartSize)
                {
                    partSize = fileSize - totalBytesParted;
                }
                c = new byte[partSize];
                fileStream.Seek(totalBytesParted, SeekOrigin.Begin);
                fileStream.Read(c, 0, c.Length);
                parts.Add(c);
                totalBytesParted += partSize;
            }

            foreach (var part in parts)
            {
                string hash = Utilities.GetSHA1Hash(part);
                shas.Add(hash);
            }

            B2File start = null;
            B2File finish = null;
            try
            {
                start = Client.LargeFiles.StartLargeFile(fileName, "", _ActiveBucket.BucketId).Result;

                for (int i = 0; i < parts.Count; i++)
                {
                    var uploadUrl = Client.LargeFiles.GetUploadPartUrl(start.FileId).Result;
                    var part = Client.LargeFiles.UploadPart(parts[i], i + 1, uploadUrl).Result;
                    IB2N.PERCENT_COMPLETED = i / (parts.Count + .01);

                }

                finish = Client.LargeFiles.FinishLargeFile(start.FileId, shas.ToArray()).Result;
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            string sURL = "https://f000.backblazeb2.com/file/bbp100/" + start.FileName;
            bool fSuccess = start.FileId == finish.FileId;
            return fSuccess;
        }

    }
}
