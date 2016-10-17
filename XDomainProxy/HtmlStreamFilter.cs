using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace XDomainProxy
{
    public class HtmlStreamFilter : Stream
    {
        Stream responseStream;
        long position;
        StringBuilder responseHtml;
        #region = CurrentEncoding =  

        private Encoding _currentencoding;

        public Encoding CurrentEncoding
        {
            get { return _currentencoding; }
            set { _currentencoding = value; }
        }

        #endregion
        Func<string, string> _func;
        public HtmlStreamFilter(Stream inputStream, Encoding enc, Func<string, string> func
)
        {
            responseStream = inputStream;
            _currentencoding = enc;
            _func = func;
            responseHtml = new StringBuilder();
        }

        #region Filter overrides  
        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Close()
        {
            responseStream.Close();
        }

        public override void Flush()
        {
            responseStream.Flush();
        }

        public override long Length
        {
            get { return 0; }
        }

        public override long Position
        {
            get { return position; }
            set { position = value; }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return responseStream.Seek(offset, origin);
        }

        public override void SetLength(long length)
        {
            responseStream.SetLength(length);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return responseStream.Read(buffer, offset, count);
        }
        #endregion

        #region Dirty work  
        public override void Write(byte[] buffer, int offset, int count)
        {
            string strBuffer = CurrentEncoding.GetString(buffer, offset, count);
            #region =如果不是HTML文档，不作处理=  

            var bof = new Regex("<html", RegexOptions.IgnoreCase);
            if (!bof.IsMatch(strBuffer.ToString()))
            {
                responseStream.Write(buffer, offset, count);
                return;
            }
            #endregion

            //Regex eof = new Regex("</html>", RegexOptions.IgnoreCase);

            //if (!eof.IsMatch(strBuffer))
            //{
            //    responseHtml.Append(strBuffer);
            //}
            //else
            //{

            //}

            responseHtml.Append(strBuffer);
            string finalHtml = responseHtml.ToString();

            finalHtml = _func(finalHtml);

            // Transform the response and write it back out  

            byte[] data = CurrentEncoding.GetBytes(finalHtml);

            responseStream.Write(data, 0, data.Length);
        }
        #endregion

    }
}