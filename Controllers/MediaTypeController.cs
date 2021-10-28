using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace Unchained.Controllers
{
    public class MediaTypeController : ApiController
    {
        /*
        public string Get()
        {
            return "Web API";
        }
        */


        [HttpGet]
        public HttpResponseMessage Video(string id)
        {
            bool rangeMode = false;
            int startByte = 0;

            if (Request.Headers.Range != null)
                if (Request.Headers.Range.Ranges.Any())
                {
                    rangeMode = true;
                    var range = Request.Headers.Range.Ranges.First();
                    startByte = Convert.ToInt32(range.From ?? 0);
                    long? endRange = Request.Headers.Range.Ranges.First().To;
             
                }
            var filePath = "s:\\san\\Rapture2\\2.mp4";
            string mimeType = MimeMapping.GetMimeMapping(filePath);
            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite) { Position = startByte };

            if (rangeMode)
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.PartialContent)
                {
                    Content = new ByteRangeStreamContent(stream, Request.Headers.Range, MediaTypeHeaderValue.Parse(mimeType))
                };
                response.Headers.AcceptRanges.Add("bytes");
                return response;
            }
            else
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StreamContent(stream)
                };
                response.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(mimeType);
                return response;
            }
        }


        /*
        public HttpResponseMessage GetVideoContent()
        {
            var httpResponce = Request.CreateResponse();
            httpResponce.Content = new PushStreamContent((Action<Stream, HttpContent, TransportContext>)WriteContentToStream);
            return httpResponce;
        }
        */

        public async void WriteContentToStream(Stream outputStream, HttpContent content, TransportContext transportContext)
        {
            //path of file which we have to read//  
            var filePath = "s:\\san\\Rapture2\\1.mp4";
            //here set the size of buffer, you can set any size  
            int bufferSize = 512000;
            byte[] buffer = new byte[bufferSize];
            string mimeType = MimeMapping.GetMimeMapping(filePath);

            bool rangeMode = false;
            int startByte = 0;

            if (Request.Headers.Range != null)
                if (Request.Headers.Range.Ranges.Any())
                {
                    rangeMode = true;
                    var range = Request.Headers.Range.Ranges.First();
                    startByte = Convert.ToInt32(range.From ?? 0);
                }


            if (rangeMode)
            {
                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.PartialContent)
                    {
                        Content = new ByteRangeStreamContent(fileStream, Request.Headers.Range,
                        MediaTypeHeaderValue.Parse(mimeType))
                    };
                    response.Headers.AcceptRanges.Add("bytes");
                    //return response;
                }
            }
            else
            {
                //here we re using FileStream to read file from server//  
                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    int totalSize = (int)fileStream.Length;
                    /*here we are saying read bytes from file as long as total size of file 

                    is greater then 0*/
                    while (totalSize > 0)
                    {
                        int count = totalSize > bufferSize ? bufferSize : totalSize;
                        //here we are reading the buffer from orginal file  
                        int sizeOfReadedBuffer = fileStream.Read(buffer, 0, count);
                        //here we are writing the readed buffer to output//  
                        await outputStream.WriteAsync(buffer, 0, sizeOfReadedBuffer);
                        //and finally after writing to output stream decrementing it to total size of file.  
                        totalSize -= sizeOfReadedBuffer;
                    }
                }
            }
        }

    }
}
