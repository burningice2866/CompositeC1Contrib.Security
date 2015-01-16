using System;
using System.Web;
using System.Web.Security;

using Composite.Core.WebClient;

namespace CompositeC1Contrib.Security.Web
{
    public class C1ConsoleHttpsEnforcer : IHttpModule
    {
        public void Init(HttpApplication app)
        {
            app.BeginRequest += app_BeginRequest;
        }

        private static void app_BeginRequest(object sender, EventArgs e)
        {
            var ctx = ((HttpApplication)sender).Context;
            var url = ctx.Request.Url;

            if (url.AbsolutePath.StartsWith(UrlUtils.AdminRootPath) && FormsAuthentication.RequireSSL && !ctx.Request.IsSecureConnection)
            {
                var uriBuilder = new UriBuilder(url)
                {
                    Scheme = "https",
                    Port = 443
                };

                url = uriBuilder.Uri;

                ctx.Response.Redirect(url.ToString());
            }
        }

        public void Dispose() { }
    }
}
