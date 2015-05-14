using System;

using Composite.Data;
using Composite.Data.Types;

namespace CompositeC1Contrib.Security.Data.Types
{
    [KeyPropertyName("Id")]
    [Title("Page permissions")]
    [LabelPropertyName("PageId")]
    [ImmutableTypeId("d9fc9f5a-1d23-42ea-8b62-a52df6f50ffd")]
    [Obsolete("Use IDataPermissions class")]
    public interface IPagePermissions : IPermissions
    {
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [ImmutableFieldId("00ba7e6a-009b-4e2e-bc8e-904c131bf6df")]
        [FunctionBasedNewInstanceDefaultFieldValue("<f:function name=\"Composite.Utils.Guid.NewGuid\" xmlns:f=\"http://www.composite.net/ns/function/1.0\" />")]
        Guid Id { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [ImmutableFieldId("C8B916ED-88F9-43C0-8B26-3DFCA1EA6CD9")]
        [ForeignKey(typeof(IPage), "Id", AllowCascadeDeletes = true)]
        Guid PageId { get; set; }
    }
}
