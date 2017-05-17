using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Security;

using Composite.Data;

using CompositeC1Contrib.Security.Data.Types;
using CompositeC1Contrib.Security.Web;

namespace CompositeC1Contrib.Security
{
    public static class PermissionsFacade
    {
        private static readonly ConcurrentDictionary<Guid, IWebsiteSecuritySettings> Cache = new ConcurrentDictionary<Guid, IWebsiteSecuritySettings>();

        public static SiteMapNode LoginSiteMapNode
        {
            get
            {
                var settings = GetWebsiteSecuritySettings();
                if (settings != null && settings.LoginPageId != null)
                {
                    return SiteMap.Provider.FindSiteMapNodeFromKey(settings.LoginPageId.Value.ToString());
                }

                var loginPage = FormsAuthentication.LoginUrl;
                if (loginPage.StartsWith("/"))
                {
                    loginPage = loginPage.Remove(0, 1);
                }

                if (Guid.TryParse(loginPage, out Guid _))
                {
                    return SiteMap.Provider.FindSiteMapNodeFromKey(loginPage);
                }

                return null;
            }
        }

        static PermissionsFacade()
        {
            DataEvents<IWebsiteSecuritySettings>.OnStoreChanged += (sender, e) =>
            {
                Cache.Clear();
            };
        }

        public static IWebsiteSecuritySettings GetWebsiteSecuritySettings()
        {
            var website = Guid.Parse(SiteMap.RootNode.Key);

            return Cache.GetOrAdd(website, g =>
            {
                using (var data = new DataConnection())
                {
                    return data.Get<IWebsiteSecuritySettings>().SingleOrDefault(_ => _.WebsiteId == g);
                }
            });
        }

        public static Uri GetLoginUri()
        {
            var loginNode = LoginSiteMapNode;
            if (loginNode == null)
            {
                return null;
            }

            var ctx = HttpContext.Current;
            var returnUrl = EnsureHttps(ctx.Request.Url).PathAndQuery;
            var loginPage = EnsureHttps(new Uri(ctx.Request.Url, loginNode.Url));

            return new Uri(loginPage + "?ReturnUrl=" + HttpUtility.UrlEncode(returnUrl));
        }

        public static bool HasAccess(SiteMapNode node)
        {
            var page = PageManager.GetPageById(new Guid(node.Key));

            return HasAccess(page);
        }

        public static bool HasAccess(IData data)
        {
            return data.GetSecurityEvaluator().HasAccess(data);
        }

        public static EvaluatedPermissions EvaluatePermissions(IDataPermissions permissions)
        {
            if (permissions == null)
            {
                return null;
            }

            var evaluatedPermissions = new EvaluatedPermissions
            {
                ExplicitAllowedRoles = Split(permissions.AllowedRoles).ToArray(),
                ExplicitDeniedRoled = Split(permissions.DeniedRoles).ToArray(),
            };

            return evaluatedPermissions;
        }

        public static IEnumerable<string> Split(string roles)
        {
            return roles?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries) ?? new string[0];
        }

        public static bool HasAccess(EvaluatedPermissions permissions)
        {
            if (permissions == null)
            {
                return true;
            }

            if (permissions.DeniedRoled.Length <= 0 && permissions.AllowedRoles.Length <= 0)
            {
                return true;
            }

            var principal = Thread.CurrentPrincipal;
            var isAuthenticated = principal.Identity.IsAuthenticated;
            var currentRole = isAuthenticated ? CompositeC1RoleProvider.AuthenticatedRole : CompositeC1RoleProvider.AnonymousdRole;

            if (permissions.DeniedRoled.Any(principal.IsInRole) || permissions.DeniedRoled.Contains(currentRole))
            {
                return false;
            }

            if (permissions.AllowedRoles.Any(principal.IsInRole) || permissions.AllowedRoles.Contains(currentRole))
            {
                return true;
            }

            return false;
        }

        public static Uri EnsureHttps(Uri uri)
        {
            if (!FormsAuthentication.RequireSSL)
            {
                return uri;
            }

            var uriBuilder = new UriBuilder(uri)
            {
                Scheme = "https",
                Port = 443
            };

            return uriBuilder.Uri;
        }
    }
}