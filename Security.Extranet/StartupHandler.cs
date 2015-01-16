using Composite.Core.Application;
using Composite.Data.DynamicTypes;

using CompositeC1Contrib.Security.Extranet.Data.Types;

namespace CompositeC1Contrib.Security.Extranet
{
    [ApplicationStartup]
    public sealed class StartupHandler
    {
        public static void OnBeforeInitialize() { }

        public static void OnInitialized()
        {
            DynamicTypeManager.EnsureCreateStore(typeof(IExtranetRole));
            DynamicTypeManager.EnsureCreateStore(typeof(IExtranetUser));
        }
    }
}
