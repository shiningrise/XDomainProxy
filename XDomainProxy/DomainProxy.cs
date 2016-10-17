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
        public void Dispose() { }
        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(Application_BeginRequest);
            context.EndRequest += new EventHandler(Application_EndRequest);
        }
        public void Application_BeginRequest(object sender, EventArgs e)
        {
            HttpApplication application = (HttpApplication)sender;
            HttpContext context = application.Context;
            HttpResponse response = context.Response;
            string path = context.Request.Path;
            string file = System.IO.Path.GetFileName(path);
            ////重写后的URL地址 
            //Regex regex = new Regex("UserInfo(\\d+).aspx", RegexOptions.Compiled);
            //Match match = regex.Match(file);
            ////如果满足URL地址重写的条件 
            //if (match.Success)
            //{
            //    string userId = match.Groups[1].Value;
            //    string rewritePath = "UserInfo.aspx?UserId=" + userId;
            //    //将其按照UserInfo.aspx?UserId=123这样的形式重写，确保能正常执行 
            //    context.RewritePath(rewritePath);
            //}
        }
        public void Application_EndRequest(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            HttpContext context = application.Context;
            var request = context.Request;
            HttpResponse response = context.Response;
            response.StatusCode = 200;

            response.Write(request.Url.AbsolutePath);
            response.Write(request.RequestType);
            response.Write(request.Path);

            request.InputStream.Position = 0;
            StreamReader reader = new StreamReader(request.InputStream);
            string text = reader.ReadToEnd();
            response.Write(text);
            
        }
    }
}