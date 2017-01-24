using Owin;

using Composite.Data.DynamicTypes;

using CompositeC1Contrib.Security.Data.Types;

namespace CompositeC1Contrib.Security.Web
{
    public static class OwinExtensions
    {
        public static void UseCompositeC1ContribSecurity(this IAppBuilder app)
        {
            DynamicTypeManager.EnsureCreateStore(typeof(IMembershipUser));
            DynamicTypeManager.EnsureCreateStore(typeof(IDataPermissions));
            DynamicTypeManager.EnsureCreateStore(typeof(IWebsiteSecuritySettings));
        }
    }
}
