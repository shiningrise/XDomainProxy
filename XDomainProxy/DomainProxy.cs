using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace XDomainProxy
{
    public class DomainProxy: IHttpModule
    {
        public void Dispose()
        { }

        /// <summary>
        /// 验证HttpModule事件机制
        /// </summary>
        /// <param name="application"></param>
        public void Init(HttpApplication application)
        {
            application.BeginRequest += new EventHandler(application_BeginRequest);
            application.EndRequest += new EventHandler(application_EndRequest);
        }

        private void application_BeginRequest(object sender, EventArgs e)
        {
            HttpApplication application = (HttpApplication)sender;
            HttpContext context = application.Context;
            var request = context.Request;
            var response = context.Response;

            
            //    response.Filter = new HtmlStreamFilter(response.Filter, response.ContentEncoding, SetDomainProxy);

            //response.Write("application_BeginRequest<br/>");
            //request.InputStream.Position = 0;
            //var streamReader = new StreamReader(request.InputStream);
            //var text = streamReader.ReadToEnd();
            //response.Write(text.Replace("hello", "hi"));
        }

        string DomainProxyUrl = System.Configuration.ConfigurationManager.AppSettings["DomainProxy"].ToString();

        private void application_EndRequest(object sender, EventArgs e)
        {
            HttpApplication application = (HttpApplication)sender;
            HttpContext context = application.Context;
            var request = context.Request;
            var response = context.Response;

            if(request.FilePath.StartsWith("/DomainProxy"))
            {
                ProcessDomainProxy(context);
            }

            //response.End();
        }

        public string SetDomainProxy(string text)
        {
            text = text.Replace(DomainProxyUrl, "/DomainProxy");
            return text;
        }

        public void ProcessDomainProxy(HttpContext context)
        {
            var requestUrl = context.Request.Url.PathAndQuery;
            requestUrl = requestUrl.Replace("/DomainProxy", DomainProxyUrl);

            //实例化web访问类
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
            request.Method = context.Request.HttpMethod;
            request.ContentType = context.Request.ContentType;

            string postData = context.Request.Form.ToString();
            byte[] postdatabytes = Encoding.UTF8.GetBytes(postData);
            request.ContentLength = postdatabytes.Length;
            request.AllowAutoRedirect = false;
            //request.CookieContainer = loginCookie;
            request.KeepAlive = true;
            request.UserAgent = context.Request.UserAgent;

            if (context.Request.HttpMethod == "POST")
            {
                Stream stream;
                stream = request.GetRequestStream();
                stream.Write(postdatabytes, 0, postdatabytes.Length);
                stream.Close();
            }

            //接收响应
            var response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                string content = reader.ReadToEnd();
                context.Response.Write(content);
                context.Response.StatusCode = (int)response.StatusCode;
                context.Response.ContentType = response.ContentType;
            }
        }

    }
}