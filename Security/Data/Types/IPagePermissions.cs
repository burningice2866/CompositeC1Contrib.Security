using System;

using Composite.Data;
using Composite.Data.Types;

namespace CompositeC1Contrib.Security.Data.Types
{
    [Title("Page permissions")]
    [LabelPropertyName("PageId")]
    [ImmutableTypeId("d9fc9f5a-1d23-42ea-8b62-a52df6f50ffd")]
    public interface IPagePermissions : IDataPermissions
    {
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [ImmutableFieldId("C8B916ED-88F9-43C0-8B26-3DFCA1EA6CD9")]
        [ForeignKey(typeof(IPage), "Id", AllowCascadeDeletes = true)]
        Guid PageId { get; set; }
    }
}
