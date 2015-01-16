using Composite.Core.Application;
using Composite.Data.DynamicTypes;

using CompositeC1Contrib.Security.Data.Types;

namespace CompositeC1Contrib.Security
{
    [ApplicationStartup]
    public sealed class StartupHandler
    {
        public static void OnBeforeInitialize() { }

        public static void OnInitialized()
        {
            DynamicTypeManager.EnsureCreateStore(typeof(IMembershipUser));
            DynamicTypeManager.EnsureCreateStore(typeof(IPagePermissions));
            DynamicTypeManager.EnsureCreateStore(typeof(IMediaFilePermissions));
        }
    }
}
