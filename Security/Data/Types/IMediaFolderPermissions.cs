using System;

using Composite.Data;
using Composite.Data.Types;

namespace CompositeC1Contrib.Security.Data.Types
{
    [Title("Mediafolder permissions")]
    [LabelPropertyName("KeyPath")]
    [ImmutableTypeId("4b7c3d57-232b-489f-b83d-321efe57596c")]
    public interface IMediaFolderPermissions : IDataPermissions
    {
        [StoreFieldType(PhysicalStoreFieldType.String, 2048)]
        [ImmutableFieldId("9444d358-c6fc-42b7-af60-34c719c6879f")]
        [ForeignKey(typeof(IMediaFileFolder), "KeyPath", AllowCascadeDeletes = true)]
        String KeyPath { get; set; }
    }
}
