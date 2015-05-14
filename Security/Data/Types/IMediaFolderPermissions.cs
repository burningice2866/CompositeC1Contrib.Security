using System;

using Composite.Data;
using Composite.Data.Types;

namespace CompositeC1Contrib.Security.Data.Types
{
    [KeyPropertyName("Id")]
    [Title("Mediafolder permissions")]
    [LabelPropertyName("KeyPath")]
    [ImmutableTypeId("4b7c3d57-232b-489f-b83d-321efe57596c")]
    [Obsolete("Use IDataPermissions class")]
    public interface IMediaFolderPermissions : IPermissions
    {
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [ImmutableFieldId("00ba7e6a-009b-4e2e-bc8e-904c131bf6df")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function name=\"Composite.Utils.Guid.NewGuid\" xmlns:f=\"http://www.composite.net/ns/function/1.0\" />")]
        Guid Id { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.String, 2048)]
        [ImmutableFieldId("9444d358-c6fc-42b7-af60-34c719c6879f")]
        [ForeignKey(typeof(IMediaFileFolder), "KeyPath", AllowCascadeDeletes = true)]
        String KeyPath { get; set; }
    }
}
