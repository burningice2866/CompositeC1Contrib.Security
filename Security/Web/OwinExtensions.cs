using System;
using System.Linq;

using Owin;

using Composite.Data;
using Composite.Data.DynamicTypes;
using Composite.Data.Transactions;
using Composite.Data.Types;


using CompositeC1Contrib.Security.Data.Types;

namespace CompositeC1Contrib.Security.Web
{
    public static class OwinExtensions
    {
        public static void UseCompositeC1ContribSecurity(this IAppBuilder app)
        {
            DynamicTypeManager.EnsureCreateStore(typeof(IMembershipUser));
            DynamicTypeManager.EnsureCreateStore(typeof(IDataPermissions));

            MigrateOldData();
        }

        private static void MigrateOldData()
        {
            var types = new[] { typeof(IPagePermissions), typeof(IMediaFolderPermissions), typeof(IMediaFilePermissions) };

            foreach (var type in types)
            {
                DataTypeDescriptor descriptor;
                if (!DynamicTypeManager.TryGetDataTypeDescriptor(type, out descriptor))
                {
                    continue;
                }

                using (var transaction = TransactionsFacade.CreateNewScope())
                {
                    using (var data = new DataConnection())
                    {
                        var permissions = DataFacade.GetData(type).OfType<IPermissions>().ToList();
                        foreach (var permission in permissions)
                        {
                            var newPermissions = data.CreateNew<IDataPermissions>();

                            newPermissions.DataTypeId = GetImmutableTypeId(permission);
                            newPermissions.DataId = GetDataId(permission);
                            newPermissions.AllowedRoles = permission.AllowedRoles;
                            newPermissions.DeniedRoles = permission.DeniedRoles;
                            newPermissions.DisableInheritance = permission.DisableInheritance;

                            data.Add(newPermissions);
                            data.Delete(permission);
                        }
                    }

                    DynamicTypeManager.DropStore(descriptor);

                    transaction.Complete();
                }
            }
        }

        private static Guid GetImmutableTypeId(IData data)
        {
            var folder = data as IMediaFolderPermissions;
            if (folder != null)
            {
                return typeof(IMediaFileFolder).GetImmutableTypeId();
            }

            var file = data as IMediaFilePermissions;
            if (file != null)
            {
                return typeof(IMediaFile).GetImmutableTypeId();
            }

            var page = data as IPagePermissions;
            if (page != null)
            {
                return typeof(IPage).GetImmutableTypeId();
            }

            return data.GetImmutableTypeId();
        }

        private static string GetDataId(IData data)
        {
            var folder = data as IMediaFolderPermissions;
            if (folder != null)
            {
                return folder.KeyPath;
            }

            var file = data as IMediaFilePermissions;
            if (file != null)
            {
                return file.KeyPath;
            }

            var page = data as IPagePermissions;
            if (page != null)
            {
                return page.PageId.ToString();
            }

            return data.GetUniqueKey().ToString();
        }
    }
}
