﻿using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

using Composite.Data;
using Composite.Data.Hierarchy;
using Composite.Data.Hierarchy.DataAncestorProviders;

namespace CompositeC1Contrib.Security.Data.Types
{
    [AutoUpdateble]
    [DataScope(DataScopeIdentifier.PublicName)]
    [DataAncestorProvider(typeof(NoAncestorDataAncestorProvider))]
    public interface IPermissions : IData
    {
        [NotNullValidator]
        [StoreFieldType(PhysicalStoreFieldType.LargeString)]
        [ImmutableFieldId("89e29af2-82a4-43e6-8262-d8d99c92921d")]
        string AllowedRoles { get; set; }

        [NotNullValidator]
        [StoreFieldType(PhysicalStoreFieldType.LargeString)]
        [ImmutableFieldId("39e057f7-a933-41cf-a3ab-bbadfd29ca53")]
        string DeniedRoles { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.Boolean)]
        [ImmutableFieldId("01cc8f10-773a-4d37-9961-e29d43d32df3")]
        bool DisableInheritance { get; set; }
    }
}
