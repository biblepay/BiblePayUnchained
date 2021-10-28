using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MimeKit;
using OpenPop;
using OpenPop.Mime;
using OpenPop.Mime.Header;

namespace Service
{
    class Service
    {
        // The Service loop performs functions that are not possible to run from the ASP.NET (IIS) Application
        // This includes:  Converting youtube video URLs to mp4

        // Not only are these conversions long running, but more specifically need to be run as Administrator
        // The IIS App Pool User does not have enough privilege to run external tools with arguments, such as youtube-dl.exe, etc.


        private static byte[] BytesFromString(string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }

        public static BiblePayCommon.Common.User uT;
        public static BiblePayCommon.Common.User uP;

        public static void ServiceLoop()
        {
            int i = 0;

            uT = VideoServices.CoerceUser(true);
            uP = VideoServices.CoerceUser(false);
            BiblePayCommon.Encryption.SetPrivateKey(uT, uP);
            BiblePayCommon.Entity.user1 userTest = new BiblePayCommon.Entity.user1();
            BiblePayCommon.Entity.user1 userProd = new BiblePayCommon.Entity.user1();

            Console.WriteLine("Starting Service loop v1.1");

            while (true)
            {
                i++;

                VideoServices.ConvertVideosFromRequestVideo(false);
                
                System.Threading.Thread.Sleep(60000 * 1);
         
            }
        }
    }
}
