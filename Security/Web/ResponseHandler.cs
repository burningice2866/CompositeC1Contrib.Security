﻿using System;
using System.Web;
using System.Web.Security;

using Composite.Core.Routing.Pages;
using Composite.Core.WebClient.Renderings;
using Composite.Core.WebClient.Renderings.Plugins.RenderingResponseHandler;
using Composite.Data;
using Composite.Data.Types;

namespace CompositeC1Contrib.Security.Web
{
    public class ResponseHandler : IDataRenderingResponseHandler
    {
        public RenderingResponseHandlerResult GetDataResponseHandling(DataEntityToken requestedItemEntityToken)
        {
            var ctx = HttpContext.Current;
            var result = new RenderingResponseHandlerResult();

            var page = requestedItemEntityToken.Data as IPage;
            if (page != null)
            {
                HandlePageRequest(result, page, ctx);
            }

            var media = requestedItemEntityToken.Data as IMediaFile;
            if (media != null)
            {
                HandleMediaRequest(result, media);
            }

            if (ctx.User.Identity.IsAuthenticated)
            {
                result.PreventPublicCaching = true;
            }

            return result;
        }

        private static void HandlePageRequest(RenderingResponseHandlerResult result, IPage page, HttpContext ctx)
        {
            var loginNode = PermissionsFacade.LoginSiteMapNode;
            if (loginNode == null)
            {
                return;
            }

            var isLoginPage = loginNode.Key == page.Id.ToString();
            if (isLoginPage)
            {
                if (FormsAuthentication.RequireSSL && !ctx.Request.IsSecureConnection)
                {
                    EndResult(result, PermissionsFacade.EnsureHttps(ctx.Request.Url));
                }

                if (ctx.Request.QueryString["cmd"] == "logoff")
                {
                    FormsAuthentication.SignOut();
                    ctx.Session.Clear();

                    var uri = new Uri(ctx.Request.Url, "/");

                    EndResult(result, uri);
                }
            }

            if (result.EndRequest || PermissionsFacade.HasAccess(page))
            {
                return;
            }

            var loginUri = isLoginPage ? new Uri(ctx.Request.Url, "/") : PermissionsFacade.GetLoginUri();

            EndResult(result, loginUri);
        }

        private static void HandleMediaRequest(RenderingResponseHandlerResult result, IMediaFile media)
        {
            if (PermissionsFacade.HasAccess(media))
            {
                return;
            }

            EndResult(result, null);
        }

        private static void EndResult(RenderingResponseHandlerResult result, Uri uri)
        {
            result.EndRequest = true;
            result.PreventPublicCaching = true;

            if (uri != null)
            {
                result.RedirectRequesterTo = uri;
            }

            Prevent404OnPathInfoRequest();
        }

        private static void Prevent404OnPathInfoRequest()
        {
            C1PageRoute.RegisterPathInfoUsage();
        }
    }
}
