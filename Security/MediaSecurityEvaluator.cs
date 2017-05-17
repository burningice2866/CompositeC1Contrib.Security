using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Composite.Data;
using Composite.Data.Types;

using CompositeC1Contrib.Security.Data;
using CompositeC1Contrib.Security.Data.Types;

namespace CompositeC1Contrib.Security
{
    public class MediaSecurityEvaluator : SecurityEvaluator
    {
        private readonly ConcurrentDictionary<string, EvaluatedPermissions> _cache = new ConcurrentDictionary<string, EvaluatedPermissions>();

        public MediaSecurityEvaluator()
        {
            DataEvents<IDataPermissions>.OnAfterAdd += Flush;
            DataEvents<IDataPermissions>.OnAfterUpdate += Flush;
            DataEvents<IDataPermissions>.OnDeleted += Flush;
        }

        private void Flush(object sender, DataEventArgs e)
        {
            _cache.Clear();
        }

        public override EvaluatedPermissions GetEvaluatedPermissions(IData data)
        {
            if (data is IMediaFileFolder folder)
            {
                return GetEvaluatedPermissions(folder);
            }

            if (data is IMediaFile file)
            {
                return GetEvaluatedPermissions(file);
            }

            return base.GetEvaluatedPermissions(data);
        }

        private EvaluatedPermissions GetEvaluatedPermissions(IMediaFileFolder folder)
        {
            return _cache.GetOrAdd(folder.KeyPath, g =>
            {
                PermissionsCache.TryGetValue(new CompoundIDataPermissionsKey(folder), out IDataPermissions permission);

                return EvaluatePermissions(permission, p => EvaluateInheritedPermissions(GetParent(folder), p));
            });
        }

        private EvaluatedPermissions GetEvaluatedPermissions(IMediaFile file)
        {
            return _cache.GetOrAdd(file.KeyPath, g =>
            {
                PermissionsCache.TryGetValue(new CompoundIDataPermissionsKey(file), out IDataPermissions permission);

                return EvaluatePermissions(permission, p => EvaluateInheritedPermissions(GetParent(file), p));
            });
        }

        private static void EvaluateInheritedPermissions(IMediaFileFolder folder, EvaluatedPermissions evaluatedPermissions)
        {
            var allowedRoles = new List<string>();
            var deniedRolews = new List<string>();

            while (folder != null)
            {
                if (PermissionsCache.TryGetValue(new CompoundIDataPermissionsKey(folder), out IDataPermissions permissions))
                {
                    var ar = PermissionsFacade.Split(permissions.AllowedRoles);
                    var dr = PermissionsFacade.Split(permissions.DeniedRoles);

                    allowedRoles.AddRange(ar);
                    deniedRolews.AddRange(dr);

                    if (permissions.DisableInheritance)
                    {
                        break;
                    }
                }

                folder = GetParent(folder);
            }

            evaluatedPermissions.InheritedAllowedRules = allowedRoles.Distinct().ToArray();
            evaluatedPermissions.InheritedDenieddRules = deniedRolews.Distinct().ToArray();
        }

        private static IMediaFileFolder GetParent(IMediaFileFolder folder)
        {
            return GetParent(folder.StoreId, folder.Path);
        }

        private static IMediaFileFolder GetParent(IMediaFile file)
        {
            return GetParent(file.StoreId, Path.Combine(file.FolderPath, file.FileName));
        }

        private static IMediaFileFolder GetParent(string storeId, string path)
        {
            var parentPath = GetParentFolder(path);
            if (path.Equals("/", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            using (var data = new DataConnection())
            {
                return data.Get<IMediaFileFolder>().SingleOrDefault(f => f.StoreId == storeId && f.Path.Equals(parentPath, StringComparison.OrdinalIgnoreCase));
            }
        }

        private static string GetParentFolder(string path)
        {
            var split = path.Split('\\', '/');
            if (split.Length == 1)
            {
                return "/";
            }

            return String.Join("/", split.Take(split.Length - 1));
        }
    }
}