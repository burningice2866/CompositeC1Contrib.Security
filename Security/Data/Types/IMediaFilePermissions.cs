using System;

using Composite.Data;
using Composite.Data.Types;

namespace CompositeC1Contrib.Security.Data.Types
{
    [Title("Media permissions")]
    [LabelPropertyName("KeyPath")]
    [ImmutableTypeId("9ad61e43-cd24-4c9b-8de0-14f61b1ba204")]
    public interface IMediaFilePermissions : IDataPermissions
    {
        [StoreFieldType(PhysicalStoreFieldType.String, 2048)]
        [ImmutableFieldId("8bfacd40-c727-408f-ae5a-20e4db03136a")]
        [ForeignKey(typeof(IMediaFile), "KeyPath", AllowCascadeDeletes = true)]
        String KeyPath { get; set; }
    }
}
