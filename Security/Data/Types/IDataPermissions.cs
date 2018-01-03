using System;

using Composite.Data;

namespace CompositeC1Contrib.Security.Data.Types
{
    [Title("Data permissions")]
    [KeyPropertyName(0, nameof(DataTypeId))]
    [KeyPropertyName(1, nameof(DataId))]
    [ImmutableTypeId("006883b3-b56d-415d-9fb4-0ef901cd8f5f")]
    public interface IDataPermissions : IPermissions
    {
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [ImmutableFieldId("f883e41a-cc05-4f89-a0eb-cba3c8d7e9e9")]
        Guid DataTypeId { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.String, 512)]
        [ImmutableFieldId("65270a5e-33c5-4f7a-b6cb-fe63937d0be1")]
        string DataId { get; set; }
    }
}
