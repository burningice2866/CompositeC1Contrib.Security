using System;

using Composite.Data;
using Composite.Data.Hierarchy;
using Composite.Data.Hierarchy.DataAncestorProviders;
using Composite.Data.Types;

namespace CompositeC1Contrib.Security.Data.Types
{
    [AutoUpdateble]
    [KeyPropertyName(nameof(WebsiteId))]
    [DataScope(DataScopeIdentifier.PublicName)]
    [ImmutableTypeId("cb7fce18-88cc-4104-82ca-48bf0b4d06b9")]
    [Title("Websitesite security settings")]
    [LabelPropertyName(nameof(WebsiteId))]
    [DataAncestorProvider(typeof(NoAncestorDataAncestorProvider))]
    public interface IWebsiteSecuritySettings : IData
    {
        [StoreFieldType(PhysicalStoreFieldType.Guid)]
        [ImmutableFieldId("dae7276f-8b2b-4057-b73d-6611efa6e902")]
        [ForeignKey(typeof(IPage), nameof(IPage.Id), AllowCascadeDeletes = true)]
        Guid WebsiteId { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.Guid, IsNullable = true)]
        [ImmutableFieldId("ccba2148-3810-4843-98b0-c30af52a30e1")]
        [ForeignKey(typeof(IPage), nameof(IPage.Id), AllowCascadeDeletes = false)]
        Guid? LoginPageId { get; set; }

        [StoreFieldType(PhysicalStoreFieldType.Guid, IsNullable = true)]
        [ImmutableFieldId("84b6919b-2781-4dab-8c74-2db90283f447")]
        [ForeignKey(typeof(IPage), nameof(IPage.Id), AllowCascadeDeletes = false)]
        Guid? ForgotPasswordPageId { get; set; }
    }
}
