using System;

using Composite.Data;
using Composite.Data.Hierarchy;
using Composite.Data.Hierarchy.DataAncestorProviders;

namespace CompositeC1Contrib.Security.Extranet.Data.Types
{
    [AutoUpdateble]
    [KeyPropertyName("Name")]
    [DataScope(DataScopeIdentifier.PublicName)]
    [ImmutableTypeId("c7f0c301-b464-4000-a369-1dbdb9260e55")]
    [Title("Extranet role")]
    [LabelPropertyName("Name")]
    [DataAncestorProvider(typeof(NoAncestorDataAncestorProvider))]
    public interface IExtranetRole : IData
    {
        [StoreFieldType(PhysicalStoreFieldType.String, 255)]
        [ImmutableFieldId("f8157ae7-1ad7-44f0-996b-31affc41b525")]
        string Name { get; set; }
    }
}
