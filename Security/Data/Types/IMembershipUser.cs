using System;

using Composite.Data;
using Composite.Data.Hierarchy;
using Composite.Data.Hierarchy.DataAncestorProviders;

namespace CompositeC1Contrib.Security.Data.Types
{
    [AutoUpdateble]
    [KeyPropertyName("Id")]
    [DataScope(DataScopeIdentifier.PublicName)]
    [ImmutableTypeId("8fc36e30-107c-4a15-805c-6b97baa674e5")]
    [Title("Membership user")]
    [LabelPropertyName("UserName")]
    [DataAncestorProvider(typeof(NoAncestorDataAncestorProvider))]
    public interface IMembershipUser : IData
    {
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [ImmutableFieldId("FDED9C51-FB7B-4954-AAB2-04AFAA046331")]
        Guid Id { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.String, 255)]
        [ImmutableFieldId("43B6FC71-46BA-4780-B8C7-83718E68524B")]
        string UserName { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.String, 255)]
        [ImmutableFieldId("851F157E-DDAF-4FC2-8E28-8CBD000E401B")]
        string Email { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.String, 255)]
        [ImmutableFieldId("7D606E9C-1CB1-4FCE-801A-0060794D8E84")]
        string Password { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.String, 255, IsNullable = true)]
        [ImmutableFieldId("44D46702-1CD8-459D-A315-D6B99497E5CF")]
        string PasswordQuestion { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.String, 255)]
        [ImmutableFieldId("FDE17AC3-A940-4EF5-B62D-DEAAA3D5D4B2")]
        string ProviderName { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.String, 255, IsNullable = true)]
        [ImmutableFieldId("AA61FF7D-E8FE-4F16-B6E3-488D9399EB6A")]
        string Comment { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.Boolean)]
        [ImmutableFieldId("7100562F-0FB2-41BD-A21B-9C468E6A3F87")]
        bool IsApproved { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.Boolean)]
        [ImmutableFieldId("C1EDAB0B-4366-4061-A98C-F4CB94A454B1")]
        bool IsLockedOut { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.DateTime)]
        [ImmutableFieldId("3D38C76B-1527-4982-806A-7960B30A37EA")]
        DateTime CreationDate { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.DateTime, IsNullable = true)]
        [ImmutableFieldId("C52B1B09-CC2A-4A77-8931-F8652AE10D1A")]
        DateTime? LastLoginDate { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.DateTime, IsNullable = true)]
        [ImmutableFieldId("42CCADFF-769E-4833-8487-4995FE14620C")]
        DateTime? LastPasswordChangedDate { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.DateTime, IsNullable = true)]
        [ImmutableFieldId("304F7E7E-2F6F-409E-9BE4-BD1A3C9EFCEA")]
        DateTime? LastActivityDate { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.DateTime, IsNullable = true)]
        [ImmutableFieldId("BF50FFC3-C80E-4366-9094-EC88C7184175")]
        DateTime? LastLockoutDate { get; set; }
    }
}
