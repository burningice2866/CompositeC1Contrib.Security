using System;

using Composite.Data;
using Composite.Data.Types;

namespace CompositeC1Contrib.Security.Data.Types
{
    [KeyPropertyName("Id")]
    [Title("Media permissions")]
    [LabelPropertyName("KeyPath")]
    [ImmutableTypeId("9ad61e43-cd24-4c9b-8de0-14f61b1ba204")]
    [Obsolete("Use IDataPermissions class")]
    public interface IMediaFilePermissions : IPermissions
    {
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [ImmutableFieldId("00ba7e6a-009b-4e2e-bc8e-904c131bf6df")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function name=\"Composite.Utils.Guid.NewGuid\" xmlns:f=\"http://www.composite.net/ns/function/1.0\" />")]
        Guid Id { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.String, 2048)]
        [ImmutableFieldId("8bfacd40-c727-408f-ae5a-20e4db03136a")]
        [ForeignKey(typeof(IMediaFile), "KeyPath", AllowCascadeDeletes = true)]
        String KeyPath { get; set; }
    }
}
