using System;
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
        private static readonly object Lock = new object();

        private static IDictionary<Guid, IWebsiteSecuritySettings> _cache;

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

                return SiteMap.Provider.FindSiteMapNodeFromKey(loginPage);
            }
        }

        static PermissionsFacade()
        {
            DataEvents<IWebsiteSecuritySettings>.OnStoreChanged += (sender, e) =>
            {
                lock (Lock)
                {
                    _cache = null;
                }
            };
        }

        public static IWebsiteSecuritySettings GetWebsiteSecuritySettings()
        {
            var cache = GetCache();
            var website = Guid.Parse(SiteMap.RootNode.Key);

            IWebsiteSecuritySettings settings;

            return cache.TryGetValue(website, out settings) ? settings : null;
        }

        public static Uri GetLoginUri()
        {
            var ctx = HttpContext.Current;
            var returnUrl = EnsureHttps(ctx.Request.Url).AbsolutePath;
            var loginPage = EnsureHttps(new Uri(ctx.Request.Url, LoginSiteMapNode.Url));

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
            return roles == null ? new string[0] : roles.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
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

        private static IDictionary<Guid, IWebsiteSecuritySettings> GetCache()
        {
            if (_cache == null)
            {
                lock (Lock)
                {
                    if (_cache == null)
                    {
                        using (var data = new DataConnection())
                        {
                            _cache = data.Get<IWebsiteSecuritySettings>().ToDictionary(s => s.WebsiteId);
                        }
                    }
                }
            }

            return _cache;
        }
    }
}