using System;

using Composite.Data;
using Composite.Data.Hierarchy;
using Composite.Data.Hierarchy.DataAncestorProviders;

using CompositeC1Contrib.Security.Data.Types;

namespace CompositeC1Contrib.Security.Extranet.Data.Types
{
    [AutoUpdateble]
    [KeyPropertyName("Id")]
    [DataScope(DataScopeIdentifier.PublicName)]
    [ImmutableTypeId("d75f9f67-5c3b-449d-b88f-d6db3f80e682")]
    [Title("Extranet user")]
    [LabelPropertyName("FirstName")]
    [DataAncestorProvider(typeof(NoAncestorDataAncestorProvider))]
    public interface IExtranetUser : IData
    {
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [ImmutableFieldId("0625fad3-f3dc-440a-a37c-14598803bac3")]
        Guid Id { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [ImmutableFieldId("12854e7f-64fb-45ec-bb8b-a26560f73d4d")]
        [ForeignKey(typeof(IMembershipUser), "Id", AllowCascadeDeletes = true)]
        Guid MemberShipUserId { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.String, 255)]
        [ImmutableFieldId("c6eb12ab-b191-4978-8fa9-3a3cba60bb03")]
        string FirstName { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.String, 255)]
        [ImmutableFieldId("16a4d5c3-599e-492b-9f94-7276beec9ca3")]
        string LastName { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.LargeString)]
        [ImmutableFieldId("bd8ca839-c816-4eb1-a73a-becdbc956c3b")]
        string RoleNames { get; set; }
    }
}
